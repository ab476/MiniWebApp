using System.Text;

namespace Validators.Generators;

public class CSharpCodeBuilder
{
    private readonly StringBuilder _sb = new();
    private int _indentLevel = 0;

    // Uses 4 spaces per indentation level
    private string Indent => new(' ', _indentLevel * 4);

    public CSharpCodeBuilder AppendLine(string line = "")
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            _sb.AppendLine();
        }
        else
        {
            _sb.AppendLine($"{Indent}{line}");
        }
        return this;
    }
    public CSharpCodeBuilder AppendLines<T>(IEnumerable<T> items, Func<T, string> formatter)
    {
        foreach (var item in items)
        {
            // Apply the format and append it using the existing indentation logic
            AppendLine(formatter(item));
        }
        return this;
    }
    public CSharpCodeBuilder BeginBlock(string? declaration = null)
    {
        if (!string.IsNullOrWhiteSpace(declaration))
        {
            AppendLine(declaration!);
        }
        AppendLine("{");
        _indentLevel++;
        return this;
    }

    public CSharpCodeBuilder EndBlock(string suffix = "")
    {
        _indentLevel--;
        AppendLine($"}}{suffix}");
        return this;
    }

    public string Build() => _sb.ToString();
}