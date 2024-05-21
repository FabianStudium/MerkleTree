using CryptoLib.Utilities;

namespace CryptoLib.LamportSignature
{
    public class LamportKeyPair
    {
        public byte[][][] PrivateKeys { get; private set; }
        public byte[][][] PublicKeys { get; private set; }

        public LamportKeyPair()
        {
            // Allocate space for 256 x 2 private and public keys, each an array of 32 bytes
            PrivateKeys = new byte[256][][];
            PublicKeys = new byte[256][][];

            for (int i = 0; i < 256; i++)
            {
                PrivateKeys[i] = new byte[2][];
                PublicKeys[i] = new byte[2][];
                for (int j = 0; j < 2; j++)
                {
                    PrivateKeys[i][j] = new byte[32]; // Each key is an array of 32 bytes
                    PublicKeys[i][j] = new byte[32];  // Same for the public keys
                }
            }

            GenerateKeys();
        }

        private void GenerateKeys()
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                for (int i = 0; i < 256; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        rng.GetBytes(PrivateKeys[i][j]); // Fill private key with random bytes
                        PublicKeys[i][j] = CryptoUtilities.HashByteArray(PrivateKeys[i][j]);  // Generate public key by hashing the private key
                    }
                }

                // rng.GetBytes(PrivateKeys[3][0]); // Test if verification is correct
            }
        }

        private void GenerateKeysDebug()
        {
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int b = 0; b < 32; b++) // Fill private key with random bytes
                    {
                        PrivateKeys[i][j][b] = (byte)j;
                    }

                    PublicKeys[i][j] = CryptoUtilities.HashByteArray(PrivateKeys[i][j]);  // Generate public key by hashing the private key
                }
            }
        }
    }
}