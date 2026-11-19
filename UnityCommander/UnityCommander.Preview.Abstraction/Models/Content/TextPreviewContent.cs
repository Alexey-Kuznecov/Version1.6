
namespace UnityCommander.Preview.Abstraction.Models.Content;

public sealed class TextPreviewContent : PreviewContent
{
    public string Text { get; }

    public string? Language { get; }

    public TextPreviewContent(string text, string? language = null)
        : base(PreviewKind.Text)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Language = language;
    }
}
