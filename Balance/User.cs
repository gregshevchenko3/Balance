using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Balance
{
    public class User
    {
        /** Ім'я користувача */
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /** Пароль до Sqlite-файлу */
        [JsonPropertyName("sqlite")]
        public string Key { get; set; }
        /** Контрольна сума імені користувача та паролю до БД*/
        [JsonPropertyName("digest")]
        public string Digest { get; set; }

        public User(string name, string key)
        {
            Name = name;
            Key = key;
            Digest = hash($"{name}:{key}");
        }
        /** Серіалізація в json та шифрування користувача */
        public string serializeAndEncrypt(string password)
        {
            string jsonString = JsonSerializer.Serialize(this);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = CalcKey(Name, password);
                aesAlg.IV = CalcIV(Name, password);
                aesAlg.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(jsonString);
                        }
                        return ByteArrayToString(msEncrypt.ToArray());
                    }
                }
            }
        }
        /** Розшифровування json-об'єту та десеріалфзація користувача */
        public static User decryptAndDerialize(string hex, string username, string password)
        {
            byte[] input = Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
            string plainJson = "";
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = CalcKey(username, password);
                aesAlg.IV = CalcIV(username, password);
                aesAlg.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                try
                {
                    using (MemoryStream msDecrypt = new MemoryStream(input))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            
                                plainJson = srDecrypt.ReadToEnd();
                        }
                    }
                }
                }
                catch (Exception exc)
                {
                    return null;
                }
            }
            User user = JsonSerializer.Deserialize<User>(plainJson);
            if (user.Name != username) return null;
            if (hash($"{user.Name}:{user.Key}") != user.Digest) return null;
            return user;
        }
        /** Серіалізація в строку масива байтів */
        private static string ByteArrayToString(byte[] data)
        {
            var sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
        /** Хеш sha256 */
        private static string hash(string data)
        {
            using (SHA256 sHA256 = SHA256.Create())
            {
                return ByteArrayToString(sHA256.ComputeHash(Encoding.ASCII.GetBytes(data)));
            }

        }
        /** Обчислення вектору ініціалізації для AES */
        private static byte[] CalcIV(string username, string password)
        {
            byte[] sha, ret = new byte[128 / 8];
            using (SHA256 sHA256 = SHA256.Create())
            {
                byte[] usrname = sHA256.ComputeHash(Encoding.ASCII.GetBytes(username));
                byte[] passwd = sHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                byte[] res = new byte[512 / 8 + 1];
                for (int i = 0; i < 256 / 8; i++)
                {
                    res[i] = usrname[i];
                    res[i + 256 / 8 + 1] = passwd[i];
                }
                res[256 / 8] = Convert.ToByte(':');
                sha = sHA256.ComputeHash(res);
            }
            for (int i = 0; i < 128 / 8; i++) ret[i] = sha[i];
            return ret;
        }
        /** Обчислення ключа шифрування для AES */
        private static byte[] CalcKey(string username, string password)
        {
            using (SHA256 sHA256 = SHA256.Create())
            {
                return sHA256.ComputeHash(
                    sHA256.ComputeHash(
                        Encoding.ASCII.GetBytes(
                            $"{username}:{password}"
                            )
                        )
                    );
            }
        }
    }
}
