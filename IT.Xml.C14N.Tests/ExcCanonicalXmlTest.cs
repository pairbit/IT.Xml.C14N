using System;
using System.IO;
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

        var sb = new StringBuilder();
        foreach (var encodingInfo in encodingInfos)
        {
            var codePage = encodingInfo.CodePage;
            var encoding = encodingInfo.GetEncoding();
            var bytes = encoding.GetBytes(str);

            try
            {
                sb.Clear();
                Write(sb, new MemoryStream(bytes), encoding);

                Assert.That(sb.ToString(), Is.EqualTo(strC14N));
            }
            catch
            {
                Console.WriteLine(codePage);
                throw;
            }
        }
    }

    private static void Write(StringBuilder sb, Stream stream, Encoding encoding)
    {
        var context = new XmlParserContext(null, null, null, default, enc: encoding);

        var xml = new ExcCanonicalXml(stream, false, null, XmlResolverHelper.GetThrowingResolver(), context);

        xml.Write(sb);
    }
}