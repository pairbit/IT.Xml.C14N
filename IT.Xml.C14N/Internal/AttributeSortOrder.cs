using System;
using System.Collections;
using System.Xml;

namespace IT.Xml.C14N.Internal;

// This class does lexicographic sorting by NamespaceURI first and then by LocalName.
internal sealed class AttributeSortOrder : IComparer
{
    internal AttributeSortOrder() { }

    public int Compare(object a, object b)
    {
        XmlNode nodeA = a as XmlNode;
        XmlNode nodeB = b as XmlNode;
        if (nodeA == null || nodeB == null) throw new ArgumentException();
        int namespaceCompare = string.CompareOrdinal(nodeA.NamespaceURI, nodeB.NamespaceURI);
        if (namespaceCompare != 0) return namespaceCompare;
        return string.CompareOrdinal(nodeA.LocalName, nodeB.LocalName);
    }
}