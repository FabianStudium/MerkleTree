namespace CryptoLib.Utilities
{
    /// <summary>
    /// Provides utility functions for cryptographic operations.
    /// </summary>
    public static class CryptoUtilities
    {
        public static byte[] HashByteArray(byte[] data)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }
    }
}