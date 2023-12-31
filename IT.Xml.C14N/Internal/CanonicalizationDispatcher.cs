using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace IT.Xml.C14N.Internal;

// the central dispatcher for canonicalization writes. not all node classes
// implement ICanonicalizableNode; so a manual dispatch is sometimes necessary.
internal static class CanonicalizationDispatcher
{
    public static void Write(XmlNode node, StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
    {
        if (node is ICanonicalizableNode)
        {
            ((ICanonicalizableNode)node).Write(strBuilder, docPos, anc);
        }
        else
        {
            WriteGenericNode(node, strBuilder, docPos, anc);
        }
    }

    public static void WriteGenericNode(XmlNode node, StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        XmlNodeList childNodes = node.ChildNodes;
        foreach (XmlNode childNode in childNodes)
        {
            Write(childNode, strBuilder, docPos, anc);
        }
    }

    public static void WriteHash(XmlNode node, HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
    {
        if (node is ICanonicalizableNode)
        {
            ((ICanonicalizableNode)node).WriteHash(hash, docPos, anc);
        }
        else
        {
            WriteHashGenericNode(node, hash, docPos, anc);
        }
    }

    public static void WriteHashGenericNode(XmlNode node, HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        XmlNodeList childNodes = node.ChildNodes;
        foreach (XmlNode childNode in childNodes)
        {
            WriteHash(childNode, hash, docPos, anc);
        }
    }
}