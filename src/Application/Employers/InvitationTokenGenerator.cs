using System.Security.Cryptography;
using System.Text;

namespace GiapTech.BlouseHiding.Application.Employers;

public static class InvitationTokenGenerator
{
    public static string GenerateToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    public static string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
