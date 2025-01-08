using System.Security.Cryptography;
using System.Text;

namespace OnlineMsgServer.Core
{
    class RsaService
    {
        //用于服务端加密解密
        private static readonly RSA _Rsa = RSA.Create();
        private static readonly object _RsaLock = new();

        //用于客户端加密
        private static readonly RSA _PublicRsa = RSA.Create();
        private static readonly object _PublicRsaLock = new();

        /// <summary>
        /// 用客户端公钥加密
        /// </summary>
        public static string EncryptForClient(string pkey, string msg)
        {
            lock (_PublicRsaLock)
            {
                _PublicRsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pkey), out _);
                // byte[] encrypt = _PublicRsa.Encrypt(Encoding.UTF8.GetBytes(msg), RSAEncryptionPadding.OaepSHA256);
                // return Convert.ToBase64String(encrypt);
                return RsaEncrypt(_PublicRsa, msg);
            }
        }

        /// <summary>
        /// 导入指定私钥，如果不存在则创建并保存
        /// </summary>
        public static void LoadRsaPkey()
        {
            lock (_RsaLock)
            {
                string pkeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".pkey");
                if (File.Exists(pkeyPath))
                {
                    DateTime lastModifyTime = File.GetLastWriteTime(pkeyPath);
                    bool isOneYearApart = Math.Abs((DateTime.Now - lastModifyTime).TotalDays) >= 365;
                    if (isOneYearApart)
                    {
                        File.WriteAllText(pkeyPath, Convert.ToBase64String(_Rsa.ExportPkcs8PrivateKey()));
                    }
                    else
                    {
                        string pkey = File.ReadAllText(pkeyPath);
                        _Rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(pkey), out _);
                    }

                }
                else
                {
                    File.WriteAllText(pkeyPath, Convert.ToBase64String(_Rsa.ExportPkcs8PrivateKey()));
                }

            }
        }

        /// <summary>
        /// 以base64格式导出公钥字符串
        /// </summary>
        /// <returns>公钥字符串，base64格式</returns>
        public static string GetRsaPublickKey()
        {
            lock (_RsaLock)
            {
                return Convert.ToBase64String(_Rsa.ExportSubjectPublicKeyInfo());
            }
        }

        /// <summary>
        /// 服务端解密 base64编码
        /// </summary>
        /// <param name="secret">密文</param>
        /// <returns> 原文字符串</returns>
        public static string Decrypt(string secret)
        {
            lock (_RsaLock)
            {
                byte[] secretBytes = Convert.FromBase64String(secret);
                int size = secretBytes.Length;
                int blockSize = 256;
                int blockCount = size / blockSize;
                List<byte> decryptList = [];
                for (int i = 0; i < blockCount; i++)
                {
                    byte[] block = new byte[blockSize];
                    Array.Copy(secretBytes, i * blockSize, block, 0, blockSize);
                    byte[] decryptBlock = _Rsa.Decrypt(block, RSAEncryptionPadding.OaepSHA256);
                    decryptList.AddRange(decryptBlock);
                }

                // byte[] decrypt = _Rsa.Decrypt(Convert.FromBase64String(base64),
                //                     RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString([.. decryptList]);
            }
        }

        /// <summary>
        /// 服务端加密 base64编码
        /// </summary>
        /// <param name="src">原文字符串</param>
        /// <returns>密文</returns>
        public static string Encrypt(string src)
        {
            lock (_RsaLock)
            {
                return RsaEncrypt(_Rsa, src);
            }
        }

        private static string RsaEncrypt(RSA rsa, string src)
        {
            byte[] srcBytes = Encoding.UTF8.GetBytes(src);
            int size = srcBytes.Length;
            int blockSize = 190;
            int blockCount = size / blockSize + 1;
            List<byte> encryptList = [];
            for (int i = 0; i < blockCount; i++)
            {
                int len = Math.Min(blockSize, size - i * blockSize);
                byte[] block = new byte[len];
                Array.Copy(srcBytes, i * blockSize, block, 0, len);
                byte[] encryptBlock = rsa.Encrypt(block, RSAEncryptionPadding.OaepSHA256);
                encryptList.AddRange(encryptBlock);
            }
            return Convert.ToBase64String([.. encryptList]);
        }
    }
}