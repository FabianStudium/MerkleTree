using System.Text;
using CryptoLib.LamportSignature;
using CryptoLib.Utilities;

public class Lamport
{
    public LamportKeyPair KeyPair;

    public Lamport()
    {
        KeyPair = new LamportKeyPair(); // generates private and public key pairs; byte[256][2][32]
    }

    public byte[][] SignMessage(string message)
    {
        byte[][] signature = new byte[256][];  // 256-byte array to store the signature components

        // Hash the message to ensure it's always 256 bits (32 bytes)
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] messageHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
            //var messageBits = ConversionUtilities.ConvertMessage(message);

            for (int i = 0; i < messageHash.Length * 8; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = 7 - (i % 8);
                bool bit = (messageHash[byteIndex] & (1 << bitIndex)) != 0;
                signature[i] = bit ? KeyPair.PrivateKeys[i][1] : KeyPair.PrivateKeys[i][0];
            }

            //for (int i = 0; i < messageHash.Length*8; i++)
            //{
            //    signature[i] = messageBits[i] ? KeyPair.PrivateKeys[i][1] : KeyPair.PrivateKeys[i][0];
            //}
        }
        // Source: Copilot
        // for (int i = 0; i < message.Length; i++)
        // {
        //     int byteIndex = i / 8;
        //     int bitIndex = i % 8;
        //     messageBits[i] = (message[byteIndex] & (1 << (7 - bitIndex))) != 0;  // Extract bit at position i
        // }

        return signature;
    }

    public bool MessageIsVerified(byte[][] signature, string message)
    {
        byte[][] verification = new byte[256][];
        //var messageBits = ConversionUtilities.ConvertMessage(message);

        // Hash the message to ensure it's always 256 bits (32 bytes)
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] messageHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));

            for (int i = 0; i < messageHash.Length * 8; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = 7 - (i % 8);
                bool bit = (messageHash[byteIndex] & (1 << bitIndex)) != 0;
                verification[i] = CryptoUtilities.HashByteArray(signature[i]);

                if ((bit && !verification[i].SequenceEqual(KeyPair.PublicKeys[i][1])) ||
                    (!bit && !verification[i].SequenceEqual(KeyPair.PublicKeys[i][0])))
                {
                    Console.WriteLine("Detected wrong key on position: " + i);
                    return false;
                }
            }

            //for (int i = 0; i < signature.Length; i++)
            //{
            //    if (signature[i] is null) break;
            //    verification[i] = CryptoUtilities.HashByteArray(signature[i]);  // Generate public key by hashing the private key
            //}

            //for (int i = 0; i < verification.Length; i++)
            //{
            //    if (verification[i] is null) break;

            //    if (messageBits[i] is false) // bit equals 0
            //    {
            //        if (verification[i].SequenceEqual(KeyPair.PublicKeys[i][0]) is false)
            //        {
            //            Console.WriteLine($"Detected wrong key on position: {i}");
            //            return false;
            //        }
            //    }
            //    else // bit equals 1
            //    {
            //        if (verification[i].SequenceEqual(KeyPair.PublicKeys[i][1]) is false)
            //        {
            //            Console.WriteLine($"Detected wrong key on position: {i}");
            //            return false;
            //        }
            //    }
            //}
        }
        return true;
    }
}

/*
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class Lamport
{
    public byte[][][] privateKeys { get; private set; } // 3D jagged array for private keys
    public byte[][][] publicKeys { get; private set; }  // 3D jagged array for public keys

    public Lamport()
    {
        // Allocate space for 256 x 2 private and public keys, each an array of 32 bytes
        privateKeys = new byte[256][][];
        publicKeys = new byte[256][][];

        for (int i = 0; i < 256; i++)
        {
            privateKeys[i] = new byte[2][];
            publicKeys[i] = new byte[2][];
            for (int j = 0; j < 2; j++)
            {
                privateKeys[i][j] = new byte[32]; // Each key is an array of 32 bytes
                publicKeys[i][j] = new byte[32];  // Same for the public keys
            }
        }

        GenerateKeys();
        //GenerateKeysDebug();
    }

    private void GenerateKeys()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    rng.GetBytes(privateKeys[i][j]); // Fill private key with random bytes
                    publicKeys[i][j] = Hash(privateKeys[i][j]);  // Generate public key by hashing the private key
                }
            }

            rng.GetBytes(privateKeys[3][0]); // Test if verification is correct
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
                    privateKeys[i][j][b] = (byte)j;
                }

                publicKeys[i][j] = Hash(privateKeys[i][j]);  // Generate public key by hashing the private key
            }
        }
    }

    public byte[][] SignMessage(string message)
    {
        byte[][] signature = new byte[256][];  // 256-byte array to store the signature components

        var messageBits = ConvertMessage(message);
        

        for (int i = 0; i < messageBits.Length; i++)
        {
            signature[i] = messageBits[i] ? privateKeys[i][1] : privateKeys[i][0];
        }
        //for (int i = 0; i < 256; i++)
        //{
        //    int bitIndex = i / 8;
        //    int bit = (message[bitIndex] >> (7 - (i % 8))) & 1;  // Extract bit at position i
        //    signature[i] = privateKeys[i][bit];  // Select the correct key based on the message bit
        //}

        return signature;
    }

    public bool MessageIsVerified(byte[][] signature, string message)
    {
        byte[][] verification = new byte[256][];
        var messageBits = ConvertMessage(message);

        for (int i = 0; i < signature.Length; i++)
        {
            if (signature[i] is null) break;
            verification[i] = Hash(signature[i]);  // Generate public key by hashing the private key
        }

        for (int i = 0; i < verification.Length; i++)
        {
            if (verification[i] is null) break;

            if (messageBits[i] is false) // bit equals 0
            {
                if (verification[i].SequenceEqual(publicKeys[i][0]) is false)
                {
                    Console.WriteLine("Detected wrong key on position: " + i);
                    return false;
                }
            }
            else // bit equals 1
            {
                if (verification[i].SequenceEqual(publicKeys[i][1]) is false)
                {
                    Console.WriteLine("Detected wrong key on position: " + i);
                    return false;
                }
            }
        }

        return true;
    }

    public byte[] Hash(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(data);
        }
    }

    static BitArray ConvertMessage(string message)
    {
        if (Encoding.ASCII.GetByteCount(message) > 32)
        {
            Console.WriteLine("Message is too long. It should be up to 32 characters (256 bits).");
            throw new Exception();
        }

        // Convert string to bit array
        return StringToBitArray(message);
    }

    static BitArray StringToBitArray(string message)
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
*/