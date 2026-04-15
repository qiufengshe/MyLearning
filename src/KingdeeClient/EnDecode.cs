using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace KingdeeClient;

public static class EnDecode
{
    public static string Encode(object data)
    {
        string s = "KingdeeK";
        string s2 = "KingdeeK";
        try
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            byte[] bytes2 = Encoding.ASCII.GetBytes(s2);
            byte[] inArray = null;
            int length = 0;
            using (DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider())
            {
                _ = dESCryptoServiceProvider.KeySize;
                using MemoryStream memoryStream = new MemoryStream();
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(bytes, bytes2), CryptoStreamMode.Write);
                using StreamWriter streamWriter = new StreamWriter(cryptoStream);
                streamWriter.Write(data);
                streamWriter.Flush();
                cryptoStream.FlushFinalBlock();
                streamWriter.Flush();
                inArray = memoryStream.GetBuffer();
                length = (int)memoryStream.Length;
            }
            return Convert.ToBase64String(inArray, 0, length);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static string Encode1(object data)
    {
        string s = "KingdeeK";
        string s2 = "KingdeeK";
        try
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            byte[] bytes2 = Encoding.ASCII.GetBytes(s2);
            byte[] inArray = null;
            int length = 0;
            using (var des = DES.Create())
            {
                using MemoryStream memoryStream = new();
                using CryptoStream cryptoStream = new(memoryStream, des.CreateEncryptor(bytes, bytes2), CryptoStreamMode.Write);
                using StreamWriter streamWriter = new(cryptoStream);
                streamWriter.Write(data);
                streamWriter.Flush();
                cryptoStream.FlushFinalBlock();
                streamWriter.Flush();
                inArray = memoryStream.GetBuffer();
                length = (int)memoryStream.Length;
            }
            return Convert.ToBase64String(inArray, 0, length);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static string HmacSHA256(string message, string secret, Encoding encoding, bool isHex = false)
    {
        secret = secret ?? "";
        byte[] bytes = encoding.GetBytes(secret);
        byte[] bytes2 = encoding.GetBytes(message);
        using HMACSHA256 hMACSHA = new HMACSHA256(bytes);
        byte[] array = hMACSHA.ComputeHash(bytes2);
        if (isHex)
        {
            string s = ByteToHexStr(array).ToLower();
            return Convert.ToBase64String(encoding.GetBytes(s));
        }
        return Convert.ToBase64String(array);
    }

    public static string ByteToHexStr(byte[] bytes)
    {
        string text = "";
        if (bytes != null)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                text += bytes[i].ToString("X2");
            }
        }
        return text;
    }

    public static string ByteToHexStr1(byte[] bytes)
    {
        StringBuilder builder = new(bytes.Length);
        if (bytes != null)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("X2"));
            }
        }
        return builder.ToString();
    }

    internal static string EncryptAppSecret(string appSecret)
    {
        if (Regex.IsMatch(appSecret, "^([0-9a-zA-Z]{32})$"))
        {
            return Convert.ToBase64String(XOREncode(Convert.FromBase64String(appSecret)));
        }
        return ROT13Encode(appSecret);
    }

    internal static string DecryptAppSecret(string appSecret)
    {
        if (appSecret.Length == 32)
        {
            return Convert.ToBase64String(XOREncode(Convert.FromBase64String(appSecret)));
        }
        return ROT13Encode(appSecret);
    }

    private static byte[] XOREncode(byte[] input)
    {
        string s = "0054f397c6234378b09ca7d3e5debce7";
        byte[] array = new byte[input.Length];
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        for (int i = 0; i < input.Length; i++)
        {
            array[i] = BitConverter.GetBytes(input[i] ^ bytes[i])[0];
        }
        return array;
    }

    private static string ROT13Encode(string InputText)
    {
        string text = "";
        for (int i = 0; i < InputText.Length; i++)
        {
            int num = Convert.ToChar(InputText.Substring(i, 1));
            if (num >= 97 && num <= 109)
            {
                num += 13;
            }
            else if (num >= 110 && num <= 122)
            {
                num -= 13;
            }
            else if (num >= 65 && num <= 77)
            {
                num += 13;
            }
            else if (num >= 78 && num <= 90)
            {
                num -= 13;
            }
            text += (char)num;
        }
        return text;
    }

    private static string ROT13Encode1(string inputText)
    {
        StringBuilder builder = new(inputText.Length);
        for (int i = 0; i < inputText.Length; i++)
        {
            int num = inputText[i];
            if (num >= 97 && num <= 109)
            {
                num += 13;
            }
            else if (num >= 110 && num <= 122)
            {
                num -= 13;
            }
            else if (num >= 65 && num <= 77)
            {
                num += 13;
            }
            else if (num >= 78 && num <= 90)
            {
                num -= 13;
            }
            builder.Append((char)num);
        }
        return builder.ToString();
    }

    public static string UrlEncodeWithUpperCode(string str, Encoding encoding)
    {
        StringBuilder stringBuilder = new StringBuilder(str.Length);
        for (int i = 0; i < str.Length; i++)
        {
            char value = str[i];
            if (HttpUtility.UrlEncode(value.ToString()).Length > 1)
            {
                stringBuilder.Append(HttpUtility.UrlEncode(value.ToString(), encoding).ToUpper());
            }
            else
            {
                stringBuilder.Append(value);
            }
        }
        return stringBuilder.ToString();
    }
}

