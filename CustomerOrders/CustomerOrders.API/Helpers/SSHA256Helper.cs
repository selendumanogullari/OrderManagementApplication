using System.Text;
using System.Security.Cryptography;

namespace CustomerOrders.API.Helpers
{
    public static class SSHA256Helper
    {
        // Şifreyi hash'lemek için
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Şifreyi byte dizisine dönüştürüp hash'liyoruz
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Hash'lenmiş byte dizisini Base64 string'e çeviriyoruz
                return Convert.ToBase64String(bytes);
            }
        }

        // Hash'lenmiş şifreyi kontrol etmek için
        public static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            // Girilen şifreyi hash'liyoruz
            string hashedEnteredPassword = HashPassword(enteredPassword);

            // Hash'lenmiş şifreyi kontrol ediyoruz
            return hashedEnteredPassword == storedHashedPassword;
        }
    }
}