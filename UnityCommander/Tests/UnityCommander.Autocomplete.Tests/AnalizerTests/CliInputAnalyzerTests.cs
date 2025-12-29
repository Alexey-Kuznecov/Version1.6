using FluentAssertions;
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;
using UnityCommander.Testing.Infrastructure;

namespace UnityCommander.Autocomplete.Tests.AnalizerTests
{
    public class CliInputAnalyzerTests
    {
        private readonly ICliInputAnalyzer _analyzer;

        public CliInputAnalyzerTests()
        {
            // arrange
            var env = TestCliEnvironment.CreateGit();
            _analyzer = env.Resolve<ICliInputAnalyzer>();
        }

        [Fact]
        public void Analyze_Unclosed_Quote_Should_Report_Error()
        {
            var input = "(\"commit test extra\"";

            var result = _analyzer.Analyze(input, input.Length);

            result.Error.Should().NotBeNull();
            result.IsComplete.Should().BeFalse();
        }

        [Theory]
        [InlineData("git", CompletionKind.Variant)]
        [InlineData("git commit", CompletionKind.PositionalArgument)]
        [InlineData("git commit test", CompletionKind.Flag)]
        [InlineData("git commit test -m", CompletionKind.FlagValue)]
        [InlineData("git commit test -m message", CompletionKind.Flag)]
        public void Git_Command_Analyze_Should_Return_Expected_Next_State(
            string input,
            CompletionKind expectedNext)
        {
            var result = _analyzer.Analyze(input, input.Length);

            result.ExpectedNext.Should().Be(expectedNext);
        }

        [Theory]
        // ─── Начало ввода команды ─────────────────
        [InlineData("plugin", CompletionKind.Variant)]
        //[InlineData("plugin l", CompletionKind.Variant)]           // должен предложить load, list
        //[InlineData("plugin lo", CompletionKind.Variant)]          // должен предложить load
        //[InlineData("plugin unload", CompletionKind.PositionalArgument)]
        //[InlineData("plugin reload", CompletionKind.PositionalArgument)]
        //[InlineData("plugin info", CompletionKind.PositionalArgument)]

        //// ─── Позиционные аргументы ─────────────────
        //[InlineData("plugin load ", CompletionKind.PositionalArgument)]          // ждем путь
        //[InlineData("plugin load C:\\Plugins\\MyPlugin.dll", CompletionKind.Flag)]
        //[InlineData("plugin unload MyPlugin", CompletionKind.Flag)]
        //[InlineData("plugin reload MyPlugin", CompletionKind.Flag)]
        //[InlineData("plugin info MyPlugin", CompletionKind.Flag)]

        //// ─── Флаги ─────────────────
        //[InlineData("plugin load C:\\Plugins\\MyPlugin.dll --force", CompletionKind.Flag)]
        //[InlineData("plugin load C:\\Plugins\\MyPlugin.dll -f", CompletionKind.Flag)]
        //[InlineData("plugin load C:\\Plugins\\MyPlugin.dll --dependencies", CompletionKind.Flag)]
        //[InlineData("plugin unload --all", CompletionKind.Flag)]
        //[InlineData("plugin reload --all", CompletionKind.Flag)]
        //[InlineData("plugin list --verbose", CompletionKind.Flag)]
        //[InlineData("plugin info MyPlugin --all", CompletionKind.Flag)]

        // ─── Значения флагов (если бы были флаги с значениями) ─────────────────
        //[InlineData("plugin load C:\\Plugins\\MyPlugin.dll --some-flag ", CompletionKind.FlagValue)] // пример
        public void Plugin_Command_Analyze_Should_Return_Expected_Next_State(
            string input,
            CompletionKind expectedNext)
        {
            var result = _analyzer.Analyze(input, input.Length);

            result.ExpectedNext.Should().Be(expectedNext);
        }

        [Fact]
        public void Analyze_Snapshot()
        {
            var input = "git commit -m \"Fix bug\" --amend";

            var result = _analyzer.Analyze(input, input.Length);

            //result.Should().MatchSnapshot();
        }

        [Theory]
        [MemberData(nameof(ParseCases))]
        public void Analyze_Should_Parse_Command_Correctly(
            string input,
            ExpectedParseResult expected)
        {
            // act
            var result = _analyzer.Analyze(input, input.Length);

            // assert
            result.Command?.Name.Should().Be(expected.CommandName);

            result.Flags
                .Select(f => f.Descriptor.Name)
                .Should()
                .BeEquivalentTo(expected.Flags);

            result.Flags
                .Select(f => (f.Descriptor.Name, f.Value))
                .Should()
                .BeEquivalentTo(expected.FlagValues);

            result.PositionalArguments
                .Select(p => p.Value)
                .Should()
                .BeEquivalentTo(expected.PositionalValues);

            result.IsComplete.Should().Be(expected.IsComplete);
            (result.Error != null).Should().Be(expected.HasError);
        }

        public static IEnumerable<object[]> ParseCases()
        {
            yield return new object[]
            {
                "git commit -m",
                new ExpectedParseResult(
                    CommandName: "commit",
                    Flags: new[] { "message" },
                    FlagValues: Array.Empty<(string, string?)>(),
                    PositionalValues: Array.Empty<string>(),
                    IsComplete: false,
                    HasError: false
                )
            };

            yield return new object[]
            {
                "git commit -m \"Fix bug\" --amend",
                new ExpectedParseResult(
                    CommandName: "commit",
                    Flags: new[] { "message", "amend" },
                    FlagValues: new[]
                    {
                        ("message", "Fix bug"),
                        ("amend", null)
                    },
                    PositionalValues: Array.Empty<string>(),
                    IsComplete: true,
                    HasError: false
                )
                    };

                    yield return new object[]
                    {
                "",
                new ExpectedParseResult(
                    CommandName: null,
                    Flags: Array.Empty<string>(),
                    FlagValues: Array.Empty<(string, string?)>(),
                    PositionalValues: Array.Empty<string>(),
                    IsComplete: false,
                    HasError: false
                )
            };
        }
    }
}
