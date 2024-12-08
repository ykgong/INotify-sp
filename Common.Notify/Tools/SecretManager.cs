using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Notify.Tools
{
    public class SecretManager
    {
        private readonly string _secretsPath;

        public SecretManager(string secretsPath)
        {
            _secretsPath = secretsPath;
        }

        /// <summary>
        /// 将秘密写入到指定的文件中。
        /// </summary>
        /// <param name="secretName">秘密的名称（文件名，不带扩展名）。</param>
        /// <param name="secretValue">秘密的值。</param>
        public void StoreSecret(string secretValue)
        {
            var _secretsPathDir = Path.GetDirectoryName(_secretsPath);
            // 确保 .secrets 目录存在
            Directory.CreateDirectory(_secretsPathDir);             

            // 将秘密写入文件
            File.WriteAllText(_secretsPath, secretValue, Encoding.UTF8);
            Console.WriteLine($"Secret '{_secretsPath}' has been stored.");
        }
    }
}
