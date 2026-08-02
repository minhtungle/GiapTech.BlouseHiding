using GiapTech.BlouseHiding.Application.Common.Interfaces;
using Microsoft.Net.Http.Headers;

namespace GiapTech.BlouseHiding.Web.Services;

public class CurrentLocale : ICurrentLocale
{
    // Khớp ADR-0006 — 6 ngôn ngữ, vi mặc định.
    private static readonly string[] SupportedLocales = ["vi", "en", "ja", "zh", "ko", "es"];

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentLocale(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Value
    {
        get
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();
            if (string.IsNullOrWhiteSpace(header))
            {
                return "vi";
            }

            var requested = StringWithQualityHeaderValue.ParseList([header])
                .OrderByDescending(v => v.Quality ?? 1.0)
                .Select(v => v.Value.Value)
                .Where(v => v is not null)
                .Select(v => v!.Split('-')[0].ToLowerInvariant());

            foreach (var candidate in requested)
            {
                if (SupportedLocales.Contains(candidate))
                {
                    return candidate;
                }
            }

            return "vi";
        }
    }
}
