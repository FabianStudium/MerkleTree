using System.Text;

namespace MerkleTree
{
    public class MerkleTreeDemo
    {
        static void Main()
        {
            int leafCount = 8;
            var mss = new MerkleSignatureScheme(leafCount);

            string message = "H";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            (byte[][] signature, byte[][] authPath) = mss.Sign(messageBytes, 0);

            bool isValid = mss.Verify(messageBytes, signature, authPath);

            Console.WriteLine($"Signature is valid: {isValid}");
        }
    }
}