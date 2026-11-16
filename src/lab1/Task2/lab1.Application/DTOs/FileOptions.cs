using System.Globalization;
using System.Text;

namespace lab1.Application.DTOs;

public class FileOptions
{
    public string EncodingName { get; set; } = "utf-8";
    public string Delimiter { get; set; } = ",";
    public bool HasHeader { get; set; } = true;
    public string Culture { get; set; } = "en-US";
    public string Sheet { get; set; } = "AirQuality";

    public Encoding GetEncoding() => Encoding.GetEncoding(EncodingName,
        EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
    public CultureInfo GetCulture() => CultureInfo.GetCultureInfo(Culture);
    public void Validate()
    {
        _ = GetEncoding();
        _ = GetCulture();
        if (Delimiter.Length != 1 || Delimiter is "\r" or "\n" or "\"")
            throw new ArgumentException("Роздільник має бути одним символом: кома, крапка з комою або табуляція.");
        if (string.IsNullOrWhiteSpace(Sheet) || Sheet.Length > 31 || Sheet.IndexOfAny(['[', ']', ':', '*', '?', '/', '\\']) >= 0)
            throw new ArgumentException("Некоректна назва аркуша XLSX.");
    }
}
