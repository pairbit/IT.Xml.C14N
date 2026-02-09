using IT.Xml.C14N.Internal;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace IT.Xml.C14N;

public sealed class ExcCanonicalXml
{
    private readonly CanonicalXmlDocument _c14nDoc;
    private readonly ExcAncestralNamespaceContextManager _ancMgr;

    public ExcCanonicalXml(Stream inputStream, bool includeComments = false, string? inclusiveNamespacesPrefixList = null, XmlResolver? resolver = null, XmlParserContext? inputContext = null)
    {
        if (inputStream == null)
            throw new ArgumentNullException(nameof(inputStream));

        if (resolver == null)
            resolver = XmlResolverHelper.GetThrowingResolver();
        
        var settings = Utils.GetSecureXmlReaderSettings(resolver);

        _c14nDoc = new CanonicalXmlDocument(true, includeComments);
        _c14nDoc.XmlResolver = resolver;
        _c14nDoc.Load(XmlReader.Create(inputStream, settings, inputContext));
        _ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
    }

    public ExcCanonicalXml(Stream inputStream, bool includeComments, string inclusiveNamespacesPrefixList, XmlResolver resolver, string strBaseUri)
    {
        if (inputStream == null)
            throw new ArgumentNullException(nameof(inputStream));

        if (resolver == null)
            resolver = XmlResolverHelper.GetThrowingResolver();

        var settings = Utils.GetSecureXmlReaderSettings(resolver);

        _c14nDoc = new CanonicalXmlDocument(true, includeComments);
        _c14nDoc.XmlResolver = resolver;
        _c14nDoc.Load(XmlReader.Create(inputStream, settings, strBaseUri));
        _ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
    }

    public ExcCanonicalXml(XmlDocument document, bool includeComments = false, string? inclusiveNamespacesPrefixList = null, XmlResolver? resolver = null)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        if (resolver == null)
            resolver = XmlResolverHelper.GetThrowingResolver();

        _c14nDoc = new CanonicalXmlDocument(true, includeComments);
        _c14nDoc.XmlResolver = resolver;
        _c14nDoc.Load(new XmlNodeReader(document));
        _ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);
    }

    public ExcCanonicalXml(XmlNodeList nodeList, bool includeComments = false, string? inclusiveNamespacesPrefixList = null, XmlResolver? resolver = null)
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
        _ancMgr = new ExcAncestralNamespaceContextManager(inclusiveNamespacesPrefixList);

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
            XmlNode currentNode = elementList[index];
            XmlNode currentNodeCanonical = elementListCanonical[index];
            XmlNodeList childNodes = currentNode.ChildNodes;
            XmlNodeList childNodesCanonical = currentNodeCanonical.ChildNodes;
            for (int i = 0; i < childNodes.Count; i++)
            {
                elementList.Add(childNodes[i]);
                elementListCanonical.Add(childNodesCanonical[i]);

                if (Utils.NodeInList(childNodes[i], nodeList))
                {
                    MarkNodeAsIncluded(childNodesCanonical[i]);
                }

                XmlAttributeCollection attribNodes = childNodes[i].Attributes;
                if (attribNodes != null)
                {
                    for (int j = 0; j < attribNodes.Count; j++)
                    {
                        if (Utils.NodeInList(attribNodes[j], nodeList))
                        {
                            MarkNodeAsIncluded(childNodesCanonical[i].Attributes.Item(j));
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