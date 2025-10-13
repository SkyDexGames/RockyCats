using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class AES256
{
    private static byte[] key;
    private static byte[] iv;

    // Inicializa una clave e IV aleatorios (solo debe hacerlo el MasterClient)
    public static void GenerateNewKey()
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256; // AES-256
            aes.GenerateKey();
            aes.GenerateIV();

            key = aes.Key;
            iv = aes.IV;
        }
    }

    // Permite configurar una clave e IV recibidas de Photon
    public static void SetKey(string base64Key, string base64IV)
    {
        key = Convert.FromBase64String(base64Key);
        iv = Convert.FromBase64String(base64IV);
    }

    public static string GetKeyBase64() => Convert.ToBase64String(key);
    public static string GetIVBase64() => Convert.ToBase64String(iv);

    public static string Encrypt(string plainText)
    {
        if (key == null || iv == null)
            throw new InvalidOperationException("AES key or IV not initialized.");

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                return Convert.ToBase64String(encrypted);
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        if (key == null || iv == null)
            throw new InvalidOperationException("AES key or IV not initialized.");

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var decryptor = aes.CreateDecryptor())
            {
                byte[] bytes = Convert.FromBase64String(cipherText);
                byte[] decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }
}
