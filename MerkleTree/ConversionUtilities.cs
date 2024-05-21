using System.Collections;
using System.Text;

namespace CryptoLib.Utilities
{
    public static class ConversionUtilities
    {
        public static byte[] HashByteArray(byte[] data)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }

        public static BitArray ConvertMessage(string message)
        {
            if (Encoding.ASCII.GetByteCount(message) > 32)
            {
                Console.WriteLine("Message is too long. It should be up to 32 characters (256 bits).");
                throw new Exception();
            }

            // Convert string to bit array
            return StringToBitArray(message);
        }

        public static BitArray StringToBitArray(string message)
        {
            // Convert string to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(message);

            // Create a bit array based on bytes
            BitArray bitArray = new BitArray(bytes);

            // Correct the bit order within each byte
            BitArray correctOrderBits = new BitArray(bitArray.Length);

            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    correctOrderBits[i * 8 + j] = bitArray[i * 8 + (7 - j)];
                }
            }

            return correctOrderBits;
        }
    }
}