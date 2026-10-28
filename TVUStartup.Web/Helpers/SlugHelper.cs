using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TVUStartup.Web.Helpers;

/// <summary>Tạo đường dẫn thân thiện (slug) từ chuỗi tiếng Việt: "Nông sản Sạch" → "nong-san-sach"</summary>
public static partial class SlugHelper
{
    public static string ToSlug(string text)
    {
        // Tách dấu khỏi ký tự rồi bỏ phần dấu; riêng "đ" phải đổi thủ công
        var normalized = text.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        var slug = NonAlphaNumeric().Replace(sb.ToString(), "-").Trim('-');
        return slug.Length > 150 ? slug[..150].Trim('-') : slug;
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphaNumeric();
}
