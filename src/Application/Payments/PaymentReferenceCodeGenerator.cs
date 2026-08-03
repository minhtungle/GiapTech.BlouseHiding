using System.Security.Cryptography;

namespace GiapTech.BlouseHiding.Application.Payments;

public static class PaymentReferenceCodeGenerator
{
    // Bỏ ký tự dễ nhầm (0/O, 1/I) vì NTD phải gõ tay vào nội dung chuyển khoản ngân hàng.
    private const string Alphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

    public static string Generate()
    {
        var chars = new char[6];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return $"PAY-{new string(chars)}";
    }
}
