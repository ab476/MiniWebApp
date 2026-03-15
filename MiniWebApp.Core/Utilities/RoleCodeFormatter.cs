using System.Text;

namespace MiniWebApp.Core.Utilities;

public static class RoleCodeFormatter
{
    /// <summary>
    /// Converts a standard name into a normalized ROLE_CODE format.
    /// </summary>
    public static string Format(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(name.Length + 5);

        for (int i = 0; i < name.Length; i++)
        {
            char currentChar = name[i];

            if (currentChar == ' ' || currentChar == '-')
            {
                sb.Append('_');
                continue;
            }

            if (char.IsUpper(currentChar) && i > 0 && char.IsLower(name[i - 1]))
            {
                sb.Append('_');
            }

            sb.Append(char.ToUpperInvariant(currentChar));
        }

        return sb.ToString();
    }
}