using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IT.Xml.C14N.Tests;

public class ReadmeTest
{
    private string _xml;
    private byte[] _xmlBytes;

    private string _xmlC14N;
    private byte[] _hashC14N;

    [SetUp]
    public void Setup()
    {
        _xml = "<Doc b='2' a='1'><Field>Multi\r\nline text</Field></Doc>";
        _xmlBytes = Encoding.UTF8.GetBytes(_xml);
        _xmlC14N = "<Doc a=\"1\" b=\"2\"><Field>Multi\nline text</Field></Doc>";
        //_hashC14N = Convert.FromBase64String("tI92wp7PvAzJtPeXXJmT/BWpB4Cvnrw+28GfC3m4AWw=");
        _hashC14N = Convert.FromBase64String("/VfhzVfGVK9EQibaw14T+h+BuduE02JYxobW1T+0fRo=");
    }

    [Test]
    public void HowToTransformTest()
    {
        using var stream = new MemoryStream(_xmlBytes);

        var c14N = new ExcCanonicalXml(stream);

        var sb = new StringBuilder();
        c14N.Write(sb);

        var transformedString = sb.ToString();
        Assert.That(transformedString, Is.EqualTo(_xmlC14N));

        var transformedBytes = Encoding.UTF8.GetBytes(transformedString);

        //using var hashAlg = new Gost_R3411_2012_256_HashAlgorithm();
        using var hashAlg = SHA256.Create();
        var hash = hashAlg.ComputeHash(transformedBytes);
        Assert.That(hash.SequenceEqual(_hashC14N));
    }

    [Test]
    public void HowToCalcTest()
    {
        //using var hashAlg = new Gost_R3411_2012_256_HashAlgorithm();
        using var hashAlg = SHA256.Create();
        using var stream = new MemoryStream(_xmlBytes);

        var c14N = new ExcCanonicalXml(stream);

        c14N.WriteHash(hashAlg);

        hashAlg.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

        Assert.That(hashAlg.Hash.AsSpan().SequenceEqual(_hashC14N));
    }
}