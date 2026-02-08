using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace IT.Xml.C14N.Tests;

internal class ExcCanonicalXmlTest
{
    [Test]
    public void Test()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var encodingInfos = Encoding.GetEncodings();

        Console.WriteLine($"encodingInfos count: {encodingInfos.Length}");

        var str = "<Doc   a  =  '1'  ><!--comment-->Text</Doc >";
        var strC14N = "<Doc a=\"1\">Text</Doc>";
        var sha256 = "fmcmAi1fbHYhWMToN0wsnUdMfN37CC4SmH8l63to6ZU=";

        using var hashAlg = SHA256.Create();
        var hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(strC14N));
        Assert.That(Convert.ToBase64String(hash), Is.EqualTo(sha256));

        var sb = new StringBuilder();
        foreach (var encodingInfo in encodingInfos)
        {
            var encoding = encodingInfo.GetEncoding();
            var bytes = encoding.GetBytes(str);

            try
            {
                sb.Clear();
                hashAlg.Initialize();

                Write(sb, hashAlg, new MemoryStream(bytes), encoding);

                Assert.That(sb.ToString(), Is.EqualTo(strC14N));
                Assert.That(hashAlg.Hash.AsSpan().SequenceEqual(hash), Is.True);
            }
            catch
            {
                Console.WriteLine(encodingInfo.CodePage);
                throw;
            }
        }
    }

    private static void Write(StringBuilder sb, HashAlgorithm hashAlg, Stream stream, Encoding encoding)
    {
        var context = new XmlParserContext(null, null, null, default, enc: encoding);

        var xml = new ExcCanonicalXml(stream, false, null, XmlResolverHelper.GetThrowingResolver(), context);

        xml.Write(sb);
        xml.WriteHash(hashAlg);

        hashAlg.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
    }

    class GOST256 : IT.Hashing.Gost.Gost3411_2012_256Digest, IIncrementalHashAlgorithm
    {
        public void Append(byte data)
        {
            Update(data);
        }

        public void Append(ReadOnlySpan<byte> data)
        {
            BlockUpdate(data);
        }
    }
}