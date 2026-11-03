using System.Globalization;
using System.Text;

namespace Bingora.Infrastructure.Helpers;

public static class StringHelper
{
    public static string NormalizeSearchTerm(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized.Where(c =>
                     CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            sb.Append(c);
        return sb.ToString().ToLowerInvariant();
    }
}