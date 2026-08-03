namespace GiapTech.BlouseHiding.Application.Catalog;

// 5 locale lưu ở bảng *_translations — không có "vi" vì đã có ở cột Name gốc (xem ADR-0006).
public static class CatalogLocales
{
    public static readonly string[] Supported = ["en", "ja", "zh", "ko", "es"];

    public static bool IsSupported(string locale) => Supported.Contains(locale);
}
