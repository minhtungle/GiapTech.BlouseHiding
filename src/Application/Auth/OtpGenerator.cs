using System.Security.Cryptography;
using System.Text;

namespace GiapTech.BlouseHiding.Application.Auth;

public static class OtpGenerator
{
    public static string GenerateCode() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

    public static string Hash(string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
        return Convert.ToHexString(bytes);
    }
}
