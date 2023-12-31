# IT.Xml.C14N
[![NuGet version (IT.Xml.C14N)](https://img.shields.io/nuget/v/IT.Xml.C14N.svg)](https://www.nuget.org/packages/IT.Xml.C14N)
[![NuGet pre version (IT.Xml.C14N)](https://img.shields.io/nuget/vpre/IT.Xml.C14N.svg)](https://www.nuget.org/packages/IT.Xml.C14N)
[![GitHub Actions](https://github.com/pairbit/IT.Xml.C14N/workflows/Build/badge.svg)](https://github.com/pairbit/IT.Xml.C14N/actions)
[![Releases](https://img.shields.io/github/release/pairbit/IT.Xml.C14N.svg)](https://github.com/pairbit/IT.Xml.C14N/releases)

Implementation of C14N XML Transform

## How to transform

```csharp
var xml = "<Doc b='2' a='1'><Field>Multi\r\nline text</Field></Doc>";
var xmlC14N = "<Doc a=\"1\" b=\"2\"><Field>Multi\nline text</Field></Doc>";
var hashC14N = Convert.FromBase64String("/VfhzVfGVK9EQibaw14T+h+BuduE02JYxobW1T+0fRo=");

using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

var c14N = new XmlDsigExcC14NTransform();
c14N.LoadInput(stream);
var transformedStream = (Stream)c14N.GetOutput();

var transformedBytes = new byte[transformedStream.Length];
transformedStream.Read(transformedBytes, 0, transformedBytes.Length);

using var hashAlg = SHA256.Create();
var hash = hashAlg.ComputeHash(transformedBytes);
Assert.That(hash.SequenceEqual(hashC14N));

var transformedXml = Encoding.UTF8.GetString(transformedBytes);
Assert.That(transformedXml, Is.EqualTo(xmlC14N));
```

## How to calculate hash

```csharp
var xml = "<Doc b='2' a='1'><Field>Multi\r\nline text</Field></Doc>";
var hashC14N = Convert.FromBase64String("/VfhzVfGVK9EQibaw14T+h+BuduE02JYxobW1T+0fRo=");

using var hashAlg = SHA256.Create();
using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

var c14N = new XmlDsigExcC14NTransform();
c14N.LoadInput(stream);
var hash = c14N.GetDigestedOutput(hashAlg);

Assert.That(hash.SequenceEqual(hashC14N));
```
