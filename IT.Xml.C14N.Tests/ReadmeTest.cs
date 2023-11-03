using IT.Hashing.Gost.Native;
using System;
using System.IO;
using System.Linq;
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
        _hashC14N = Convert.FromBase64String("tI92wp7PvAzJtPeXXJmT/BWpB4Cvnrw+28GfC3m4AWw=");
    }

    [Test]
    public void HowToTransformTest()
    {
        using var stream = new MemoryStream(_xmlBytes);

        var c14N = new XmlDsigExcC14NTransform();
        c14N.LoadInput(stream);
        var transformedStream = (Stream)c14N.GetOutput();

        var transformedBytes = new byte[transformedStream.Length];
        transformedStream.Read(transformedBytes, 0, transformedBytes.Length);

        using var hashAlg = new Gost_R3411_2012_256_HashAlgorithm();
        var hash = hashAlg.ComputeHash(transformedBytes);
        Assert.That(hash.SequenceEqual(_hashC14N));

        var transformedXml = Encoding.UTF8.GetString(transformedBytes);
        Assert.That(transformedXml, Is.EqualTo(_xmlC14N));
    }

    [Test]
    public void HowToCalcTest()
    {
        using var hashAlg = new Gost_R3411_2012_256_HashAlgorithm();
        using var stream = new MemoryStream(_xmlBytes);

        var c14N = new XmlDsigExcC14NTransform();
        c14N.LoadInput(stream);
        var hash = c14N.GetDigestedOutput(hashAlg);

        Assert.That(hash.SequenceEqual(_hashC14N));
    }
}