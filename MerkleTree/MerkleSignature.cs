using System.Text;
using CryptoLib.Utilities;

namespace MerkleTree
{
    public class MerkleSignatureScheme
    {
        private Lamport[] lamportKeys;
        private MerkleTree merkleTree;

        public MerkleSignatureScheme(int leafCount)
        {
            lamportKeys = new Lamport[leafCount];
            byte[][] leaves = new byte[leafCount][];

            for (int i = 0; i < leafCount; i++)
            {
                lamportKeys[i] = new Lamport();
                leaves[i] = CombinePublicKeys(lamportKeys[i].KeyPair.PublicKeys);
                //using (var sha256 = System.Security.Cryptography.SHA256.Create())
                //{
                //    leaves[i] = sha256.ComputeHash(CombinePublicKeys(lamportKeys[i].KeyPair.PublicKeys));
                //}
                Console.WriteLine($"Leaf {i}: {BitConverter.ToString(leaves[i])}");
            }

            merkleTree = new MerkleTree(leaves);

            Console.WriteLine($"Merkle Root: {BitConverter.ToString(merkleTree.Root)}");
        }

        private byte[] CombinePublicKeys(byte[][][] publicKeys)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                foreach (var keyPair in publicKeys)
                {
                    foreach (var key in keyPair)
                    {
                        ms.Write(key, 0, key.Length);
                    }
                }

                return CryptoUtilities.HashByteArray(ms.ToArray());
            }
        }

        public byte[] PublicKey => merkleTree.Root;

        public (byte[][] signature, byte[][] authPath) Sign(byte[] message, int index)
        {
            var otsSignature = lamportKeys[index].SignMessage(Encoding.UTF8.GetString(message));
            var authPath = merkleTree.GetAuthPath(index);

            Console.WriteLine($"Signing message: {Encoding.UTF8.GetString(message)}");
            Console.WriteLine($"Signature for leaf {index}: {BitConverter.ToString(otsSignature.SelectMany(b => b).ToArray())}");
            Console.WriteLine($"Auth path for leaf {index}: {string.Join(", ", authPath.Select(p => BitConverter.ToString(p)))}");

            return (otsSignature, authPath);
        }

        public bool Verify(byte[] message, byte[][] signature, byte[][] authPath)
        {
            Console.WriteLine($"Verifying message: {Encoding.UTF8.GetString(message)}");
            Console.WriteLine($"Signature: {BitConverter.ToString(signature.SelectMany(b => b).ToArray())}");
            Console.WriteLine($"Auth path: {string.Join(", ", authPath.Select(p => BitConverter.ToString(p)))}");

            bool isVerified = lamportKeys[0].MessageIsVerified(signature, Encoding.UTF8.GetString(message));

            if (!isVerified)
            {
                Console.WriteLine("Lamport signature verification failed.");
                return false;
            }

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] combined = Combine(signature);

                for (int i = 0; i < authPath.Length; i++)
                {
                    if (authPath[i] != null)
                    {
                        combined = sha256.ComputeHash(Combine(combined, authPath[i]));
                        Console.WriteLine($"Combined hash at level {i}: {BitConverter.ToString(combined)}");
                    }
                }

                bool isRootEqual = combined.SequenceEqual(merkleTree.Root);

                if (!isRootEqual)
                {
                    Console.WriteLine("Merkle root verification failed.");
                }

                return isRootEqual;
            }
        }

        private byte[] Combine(byte[][] arrays)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                foreach (var array in arrays)
                {
                    if (array != null)
                    {
                        ms.Write(array, 0, array.Length);
                    }
                    else
                    {
                        Console.WriteLine("Null array found in Combine method.");
                    }
                }

                return CryptoUtilities.HashByteArray(ms.ToArray());
                //return ms.ToArray();
            }
        }

        private byte[] Combine(byte[] left, byte[] right)
        {
            byte[] combined = new byte[left.Length + right.Length];
            Array.Copy(left, 0, combined, 0, left.Length);
            Array.Copy(right, 0, combined, left.Length, right.Length);
            return combined;
        }
    }
}