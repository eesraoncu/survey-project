using System.Security.Cryptography;
using System.Text;

namespace SurveyApp.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        Console.WriteLine($"=== PASSWORD VERIFICATION ===");
        Console.WriteLine($"Input password length: {password?.Length ?? 0}");
        Console.WriteLine($"Stored hash length: {hashedPassword?.Length ?? 0}");
        
        // Frontend'den hash'lenmiş şifre geliyorsa direkt karşılaştır
        if (password?.Length == 44) // Base64 SHA-256 hash uzunluğu
        {
            Console.WriteLine($"Frontend hash detected, comparing directly");
            Console.WriteLine($"Hash match: {password == hashedPassword}");
            return password == hashedPassword;
        }
        
        // Eğer plain text geliyorsa (eski yöntem) hash'le
        var hashedInput = HashPassword(password);
        Console.WriteLine($"Plain text detected, hashing first");
        Console.WriteLine($"Generated hash length: {hashedInput?.Length ?? 0}");
        Console.WriteLine($"Hash match: {hashedInput == hashedPassword}");
        
        return hashedInput == hashedPassword;
    }
}
