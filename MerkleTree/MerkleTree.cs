namespace MerkleTree
{
    public class MerkleTree
    {
        private byte[][] leaves;
        private byte[][] tree;

        public MerkleTree(byte[][] leaves)
        {
            this.leaves = leaves;
            this.tree = BuildMerkleTree(leaves);
        }

        private byte[][] BuildMerkleTree(byte[][] leaves)
        {
            int levels = (int)Math.Ceiling(Math.Log2(leaves.Length)); // fancy, for demonstration purposes, could be hardcoded
            int nodes = (1 << (levels + 1)) - 1;
            byte[][] tree = new byte[nodes][];

            Array.Copy(leaves, 0, tree, nodes / 2, leaves.Length);

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                for (int i = nodes / 2 - 1; i >= 0; i--)
                {
                    if (tree[2 * i + 1] == null || tree[2 * i + 2] == null)
                    {
                        tree[i] = null;
                    }
                    else
                    {
                        tree[i] = sha256.ComputeHash(Combine(tree[2 * i + 1], tree[2 * i + 2]));
                        Console.WriteLine($"Node {i}: {BitConverter.ToString(tree[i])}");
                    }
                }
            }

            return tree;
        }

        private byte[] Combine(byte[] left, byte[] right)
        {
            byte[] combined = new byte[left.Length + right.Length];
            Array.Copy(left, 0, combined, 0, left.Length);
            Array.Copy(right, 0, combined, left.Length, right.Length);
            return combined;
        }

        public byte[] Root => tree[0];

        public byte[][] GetAuthPath(int index)
        {
            int levels = (int)Math.Ceiling(Math.Log2(leaves.Length));
            int nodes = (1 << (levels + 1)) - 1;

            index += nodes / 2;
            byte[][] path = new byte[levels][];

            for (int i = 0; i < levels; i++)
            {
                int sibling = (index % 2 == 0) ? index - 1 : index + 1;
                path[i] = tree[sibling];
                index = (index - 1) / 2;
            }

            return path;
        }
    }

}