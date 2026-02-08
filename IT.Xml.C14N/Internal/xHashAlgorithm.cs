using System.Security.Cryptography;

namespace IT.Xml.C14N.Internal;

internal static class xHashAlgorithm
{
    public static void Append(this HashAlgorithm hash, byte[] data)
    {
        hash.TransformBlock(data, 0, data.Length, null, 0);
    }
}