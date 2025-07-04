using IT.Hashing.Gost.Native;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace IT.Xml.C14N.Tests;

public class TransformTest
{
    private byte[] _dataBytes;
    private string _dataString;
    private byte[] _hash;

    [SetUp]
    public void Setup()
    {
        //<Doc><Field>��������\r\n���� ���������</Field></Doc>
        _dataBytes = Convert.FromBase64String("PERvYz48RmllbGQ+0JfQvdCw0YfQtdC90LjQtQ0K0L/QvtC70Y8g0LTQvtC60YPQvNC10L3RgtCwPC9GaWVsZD48L0RvYz4=");
        _dataString = Encoding.UTF8.GetString(_dataBytes);
        //_hash = Convert.FromBase64String("oQwkI5hh95o6owmW29XJRFFI7Ne5k4HDe5wHcHpJmCM=");
        _hash = Convert.FromBase64String("4oWG0jf8D+nVsaER1B4CqdKN5ucFUQ4+NVW3WpY/78o=");
    }

    [Test]
    public void XmlDocument_Test()
    {
        //using var hashAlg = new Gost_R3411_2012_256_HashAlgorithm();
        using var hashAlg = SHA256.Create();
        var data = _dataString;

        if (data.Contains("\r\n"))
        {
            data = data.Replace("\r\n", "\n");
        }

        var doc = new XmlDocument();
        doc.PreserveWhitespace = true;
        doc.LoadXml(data);

        //'\r' -> &#xD;
        var hash1 = IT_TransformC14(doc, hashAlg);
        var hash2 = Sys_TransformC14(doc, hashAlg);

        Assert.That(hash1.SequenceEqual(hash2), Is.True);

        Assert.That(_hash.SequenceEqual(hash1), Is.True);
    }

    [Test]
    public void Stream_Test()
    {
        //using var hashAlg = new Gost_R3411_2012_256_HashAlgorithm();
        using var hashAlg = SHA256.Create();

        using var stream = new MemoryStream(_dataBytes);

        var hash1 = IT_TransformC14(stream, hashAlg);

        stream.Position = 0;
        var hash2 = Sys_TransformC14(stream, hashAlg);

        Assert.That(hash1.SequenceEqual(hash2), Is.True);

        Assert.That(_hash.SequenceEqual(hash1), Is.True);
    }

    private static byte[] IT_TransformC14(object input, HashAlgorithm hashAlg)
    {
        var c14NTransform = new XmlDsigExcC14NTransform();

        c14NTransform.LoadInput(input);

        return c14NTransform.GetDigestedOutput(hashAlg);
    }

    private static byte[] Sys_TransformC14(object input, HashAlgorithm hashAlg)
    {
        var c14NTransform = new System.Security.Cryptography.Xml.XmlDsigExcC14NTransform();

        c14NTransform.LoadInput(input);

        return c14NTransform.GetDigestedOutput(hashAlg);
    }
}