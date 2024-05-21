using System;
using System.Security.Cryptography;
using System.Text;

namespace Lamport_old
{
    public class Lamport_old
    {
        public int KeySize { get; private set; } // Number of pairs
        public int HashSize { get; private set; } // Hash size in bytes, using SHA256

        public Lamport_old()
        {
            KeySize = 256; // 256 pairs of keys
            HashSize = 32; // SHA256 hash size in bytes
        }

        public void GenerateAndDisplayKeys()
        {
            var (privateKeys, publicKeys) = GenerateKeys();

            Console.WriteLine("Private Key:");

            foreach (var pair in privateKeys)
            {
                Console.WriteLine($"({ConvertToHex(pair.Item1)}, {ConvertToHex(pair.Item2)})");
            }

            Console.WriteLine("\nPublic Key:");

            foreach (var pair in publicKeys)
            {
                Console.WriteLine($"({ConvertToHex(pair.Item1)}, {ConvertToHex(pair.Item2)})");
            }
        }

        private ((byte[], byte[])[] privateKey, (byte[], byte[])[] publicKey) GenerateKeys()
        {
            var privateKeys = new (byte[], byte[])[KeySize];
            var publicKeys = new (byte[], byte[])[KeySize];

            using (var rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < KeySize; i++)
                {
                    byte[] privateKeyPart1 = new byte[HashSize];
                    byte[] privateKeyPart2 = new byte[HashSize];
                    rng.GetBytes(privateKeyPart1);
                    rng.GetBytes(privateKeyPart2);
                    privateKeys[i] = (privateKeyPart1, privateKeyPart2);

                    publicKeys[i] = (Hash(privateKeyPart1), Hash(privateKeyPart2));
                }
            }

            return (privateKeys, publicKeys);
        }

        private byte[] Hash(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }

        private string ConvertToHex(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);

            foreach (byte b in data)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}