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
        // Анализатор вернул CompletionKind.None ошибка системы или нет хотя по мне так 
        // завершонная строка команды далжна выглядеть так git commit test -m message -amend
        // Скороее всего message посцитал как флаг или как значения флага -m 
        // здесь надо выдовать ошибку и вообще опозиционный аргумент это message а test ошибка которую система не обработала по факту
        public void Analyze_Should_Return_Expected_Next_State(
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
