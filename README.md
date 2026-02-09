# IT.Xml.C14N
[![NuGet version (IT.Xml.C14N)](https://img.shields.io/nuget/v/IT.Xml.C14N.svg)](https://www.nuget.org/packages/IT.Xml.C14N)
[![NuGet pre version (IT.Xml.C14N)](https://img.shields.io/nuget/vpre/IT.Xml.C14N.svg)](https://www.nuget.org/packages/IT.Xml.C14N)
[![GitHub Actions](https://github.com/pairbit/IT.Xml.C14N/workflows/Build/badge.svg)](https://github.com/pairbit/IT.Xml.C14N/actions)
[![Releases](https://img.shields.io/github/release/pairbit/IT.Xml.C14N.svg)](https://github.com/pairbit/IT.Xml.C14N/releases)

XML Canonicalization

## How to transform

```csharp
var xml = "<Doc b='2' a='1'><Field>Multi\r\nline text</Field></Doc>";
var xmlC14N = "<Doc a=\"1\" b=\"2\"><Field>Multi\nline text</Field></Doc>";
var hashC14N = Convert.FromBase64String("/VfhzVfGVK9EQibaw14T+h+BuduE02JYxobW1T+0fRo=");

using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

var c14N = new ExcCanonicalXml(stream);

var sb = new StringBuilder();
c14N.Write(sb);

var transformedString = sb.ToString();
Assert.That(transformedString, Is.EqualTo(xmlC14N));

var transformedBytes = Encoding.UTF8.GetBytes(transformedString);

using var hashAlg = SHA256.Create();
var hash = hashAlg.ComputeHash(transformedBytes);
Assert.That(hash.SequenceEqual(hashC14N));
```

## How to calculate hash

```csharp
var xml = "<Doc b='2' a='1'><Field>Multi\r\nline text</Field></Doc>";
var hashC14N = Convert.FromBase64String("/VfhzVfGVK9EQibaw14T+h+BuduE02JYxobW1T+0fRo=");

using var hashAlg = SHA256.Create();
using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

var c14N = new ExcCanonicalXml(stream);

c14N.WriteHash(hashAlg);

hashAlg.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

Assert.That(hashAlg.Hash.AsSpan().SequenceEqual(hashC14N));
```