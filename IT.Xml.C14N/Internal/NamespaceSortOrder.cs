using System;
using System.Collections;
using System.Xml;

namespace IT.Xml.C14N.Internal;

internal sealed class NamespaceSortOrder : IComparer
{
    internal NamespaceSortOrder() { }

    public int Compare(object a, object b)
    {
        XmlNode nodeA = a as XmlNode;
        XmlNode nodeB = b as XmlNode;
        if (nodeA == null || nodeB == null) throw new ArgumentException();
        bool nodeAdefault = Utils.IsDefaultNamespaceNode(nodeA);
        bool nodeBdefault = Utils.IsDefaultNamespaceNode(nodeB);
        if (nodeAdefault && nodeBdefault) return 0;
        if (nodeAdefault) return -1;
        if (nodeBdefault) return 1;
        return string.CompareOrdinal(nodeA.LocalName, nodeB.LocalName);
    }
}