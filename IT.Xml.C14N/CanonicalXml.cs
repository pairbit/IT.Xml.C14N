using IT.Xml.C14N.Internal;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace IT.Xml.C14N;

public sealed class CanonicalXml
{
    private readonly CanonicalXmlDocument _c14nDoc;
    private readonly C14NAncestralNamespaceContextManager _ancMgr;

    public CanonicalXml(Stream stream, XmlParserContext? context = null, XmlResolver? resolver = null, bool includeComments = false)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (resolver == null) resolver = XmlResolverHelper.GetThrowingResolver();

        var settings = Utils.GetSecureXmlReaderSettings(resolver);

        _c14nDoc = new CanonicalXmlDocument(true, includeComments);
        _c14nDoc.XmlResolver = resolver;
        _c14nDoc.Load(XmlReader.Create(stream, settings, context));
        _ancMgr = new C14NAncestralNamespaceContextManager();
    }

    public CanonicalXml(Stream stream, string? baseUri, XmlResolver? resolver = null, bool includeComments = false)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (resolver == null) resolver = XmlResolverHelper.GetThrowingResolver();

        var settings = Utils.GetSecureXmlReaderSettings(resolver);

        _c14nDoc = new CanonicalXmlDocument(true, includeComments);
        _c14nDoc.XmlResolver = resolver;
        _c14nDoc.Load(XmlReader.Create(stream, settings, baseUri));
        _ancMgr = new C14NAncestralNamespaceContextManager();
    }

    public CanonicalXml(XmlDocument document, XmlResolver? resolver = null, bool includeComments = false)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        if (resolver == null)
            resolver = XmlResolverHelper.GetThrowingResolver();

        _c14nDoc = new CanonicalXmlDocument(true, includeComments);
        _c14nDoc.XmlResolver = resolver;
        _c14nDoc.Load(new XmlNodeReader(document));
        _ancMgr = new C14NAncestralNamespaceContextManager();
    }

    public CanonicalXml(XmlNodeList nodeList, XmlResolver? resolver = null, bool includeComments = false)
    {
        if (nodeList == null)
            throw new ArgumentNullException(nameof(nodeList));

        XmlDocument doc = Utils.GetOwnerDocument(nodeList);
        if (doc == null)
            throw new ArgumentException(nameof(nodeList));

        if (resolver == null)
            resolver = XmlResolverHelper.GetThrowingResolver();

        _c14nDoc = new CanonicalXmlDocument(false, includeComments);
        _c14nDoc.XmlResolver = resolver;
        _c14nDoc.Load(new XmlNodeReader(doc));
        _ancMgr = new C14NAncestralNamespaceContextManager();

        MarkInclusionStateForNodes(nodeList, doc, _c14nDoc);
    }

    public void Write(StringBuilder sb)
    {
        _c14nDoc.Write(sb, DocPosition.BeforeRootElement, _ancMgr);
    }

    public void WriteHash(HashAlgorithm hash)
    {
        _c14nDoc.WriteHash(hash, DocPosition.BeforeRootElement, _ancMgr);
    }

    public void AppendHash(IIncrementalHashAlgorithm hash)
    {
        _c14nDoc.WriteHash(hash, DocPosition.BeforeRootElement, _ancMgr);
    }

    private static void MarkInclusionStateForNodes(XmlNodeList nodeList, XmlDocument inputRoot, XmlDocument root)
    {
        CanonicalXmlNodeList elementList = new CanonicalXmlNodeList();
        CanonicalXmlNodeList elementListCanonical = new CanonicalXmlNodeList();
        elementList.Add(inputRoot);
        elementListCanonical.Add(root);
        int index = 0;

        do
        {
            XmlNode currentNode = (XmlNode)elementList[index]!;
            XmlNode currentNodeCanonical = (XmlNode)elementListCanonical[index]!;
            XmlNodeList childNodes = currentNode.ChildNodes;
            XmlNodeList childNodesCanonical = currentNodeCanonical.ChildNodes;
            for (int i = 0; i < childNodes.Count; i++)
            {
                elementList.Add(childNodes[i]);
                elementListCanonical.Add(childNodesCanonical[i]);

                if (Utils.NodeInList(childNodes[i], nodeList))
                {
                    MarkNodeAsIncluded(childNodesCanonical[i]!);
                }

                XmlAttributeCollection attribNodes = childNodes[i]!.Attributes;
                if (attribNodes != null)
                {
                    for (int j = 0; j < attribNodes.Count; j++)
                    {
                        if (Utils.NodeInList(attribNodes[j], nodeList))
                        {
                            MarkNodeAsIncluded(childNodesCanonical[i]!.Attributes!.Item(j)!);
                        }
                    }
                }
            }
            index++;
        } while (index < elementList.Count);
    }

    private static void MarkNodeAsIncluded(XmlNode node)
    {
        if (node is ICanonicalizableNode cnode)
            cnode.IsInNodeSet = true;
    }
}