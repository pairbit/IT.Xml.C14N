# IT.Xml.C14N
[![NuGet version (IT.Xml.C14N)](https://img.shields.io/nuget/v/IT.Xml.C14N.svg)](https://www.nuget.org/packages/IT.Xml.C14N)
[![NuGet pre version (IT.Xml.C14N)](https://img.shields.io/nuget/vpre/IT.Xml.C14N.svg)](https://www.nuget.org/packages/IT.Xml.C14N)

Implementation of C14N XML Transform

## How to transform

```csharp
var xml = "<Doc b='2' a='1'><Field>Multi\r\nline text</Field></Doc>";
var xmlC14N = "<Doc a=\"1\" b=\"2\"><Field>Multi\nline text</Field></Doc>";
var hashC14N = Convert.FromBase64String("tI92wp7PvAzJtPeXXJmT/BWpB4Cvnrw+28GfC3m4AWw=");

using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

var c14N = new XmlDsigExcC14NTransform();
c14N.LoadInput(stream);
var transformedStream = (Stream)c14N.GetOutput();

var transformedBytes = new byte[transformedStream.Length];
transformedStream.Read(transformedBytes, 0, transformedBytes.Length);

using var hashAlg = new Gost_R3411_2012_256_HashAlgorithm();
var hash = hashAlg.ComputeHash(transformedBytes);
Assert.That(hash.SequenceEqual(hashC14N));

var transformedXml = Encoding.UTF8.GetString(transformedBytes);
Assert.That(transformedXml, Is.EqualTo(xmlC14N));
```

## How to calculate hash

```csharp
var xml = "<Doc b='2' a='1'><Field>Multi\r\nline text</Field></Doc>";
var hashC14N = Convert.FromBase64String("tI92wp7PvAzJtPeXXJmT/BWpB4Cvnrw+28GfC3m4AWw=");

using var hashAlg = new Gost_R3411_2012_256_HashAlgorithm();
using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

var c14N = new XmlDsigExcC14NTransform();
c14N.LoadInput(stream);
var hash = c14N.GetDigestedOutput(hashAlg);

Assert.That(hash.SequenceEqual(hashC14N));
```