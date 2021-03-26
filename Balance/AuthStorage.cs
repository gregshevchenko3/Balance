using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Balance
{
    public class AuthStorage
    {
        /**
         *          ДИТЯЧИЙ ЗАХИСТ БД
         *    БД не зможе прочитати посторонній, оскільки БД - зашифрована. Ключ від якої -
         *  знає (зможе прочитати кожен із своїх, використовуючи свій логін і свій пароль)
         *    
         *  АЛЕ:
         *    Відсутній захист від "своїх". Із-за відсутності реалізації ЕЦП, а також 
         *  журналювання дій користувачів - неможливо вістежити. Хто, коли, і які саме 
         *  зміни внесе.
         * 
         * Паролі зберігаються в файлі passwd.json у вигляді:
         * {
         *      "users": [
         *          "AES256 encrypted data for user1",
         *          "AES256 encrypted data for user2",
         *          "AES256 encrypted data for user3",
         *      ]
         * }
         * 
         * Далі:
         * - Функція шифрування буде виглядати так:
         *                  AES_CFB_Encrypt(data, key, iv)
         * - Функція де-шифрування буде виглядати так:
         *                  AES_CFB_Decrypt(encrypted_data, key, iv)
         * - Функція кешування буде виглядати так:
         *                        SHA256(data)
         * - Функція, що повертає молодшу половину аргументу, виглядає так:
         *                          Low(data)
         * 
         * Аутентифікація:
         * Користувач вводить ім'я і пароль. 
         * Програма обчислює ключ (котрий ніде більше не зберігається):
         *                key = SHA256(SHA256(username:password));
         * вектор ініціалізації 
         *                iv = Low(SHA256(SHA256(username):SHA256(password)));
         *                
         * Після цього, намагається розшифровувати записи з масиву users. В розшифрованому 
         * вигляді - буде отримано об'єкт
         * 
         *      {
         *          "username": "...",
         *          "sqlite": "...",
         *          "digest": "..."
         *      }
         * Де:
         *      
         *      username - ім'я користувача. Повинно збігатися з тим, що ввів користувач.
         *      sqlite - Ключ, яким зашифрована БД.
         *      digest - SHA256(<значення поля username>:<значення поля sqlite>:<значення поля random_seed>)
         */
        [JsonPropertyName("users")]
        public List<string> Users { get; set; }
        
        public AuthStorage()
        {
            Users = new List<string>();
        }
        public void appendUser(string name, string password, string key)
        {
            User us = new User(name.Trim().ToLower(), key);
            string enc_json = us.serializeAndEncrypt(password);
            foreach (string user_enc_record in Users)
            {
                if (user_enc_record == enc_json) throw new Exception("User already registered");
            }
            Users.Add(enc_json);
        }
        public bool check_quality(string password)
        {
            return true;
        }
        public byte[] getDBKey(string longin, string password)
        {
            User us = null; 
            foreach (string hex in Users)
            {
                us = User.decryptAndDerialize(hex, longin, password);
                if (us != null) break;
            }
            if (us == null) return null;
            return Enumerable.Range(0, us.Key.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(us.Key.Substring(x, 2), 16)).ToArray();
        }
        public static AuthStorage deserialize(string filename)
        {
            string json = File.ReadAllText(filename);
            return JsonSerializer.Deserialize<AuthStorage>(json);
        }
        public void serialize(string filename)
        {
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(filename, json);

        }
    }
}
