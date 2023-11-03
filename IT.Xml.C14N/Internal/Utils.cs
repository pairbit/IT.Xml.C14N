using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace IT.Xml.C14N.Internal;

internal static class Utils
{
    // The maximum number of characters in an XML document (0 means no limit).
    internal const int MaxCharactersInDocument = 0;

    // The entity expansion limit. This is used to prevent entity expansion denial of service attacks.
    internal const long MaxCharactersFromEntities = (long)1e7;

    // A helper function that determines if a namespace node is a committed attribute
    internal static bool IsCommittedNamespace(XmlElement element, string prefix, string value)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));

        string name = prefix.Length > 0 ? "xmlns:" + prefix : "xmlns";
        if (element.HasAttribute(name) && element.GetAttribute(name) == value) return true;
        return false;
    }

    internal static string GetAttribute(XmlElement element, string localName, string namespaceURI)
    {
        string s = element.HasAttribute(localName) ? element.GetAttribute(localName) : null;
        if (s == null && element.HasAttribute(localName, namespaceURI))
            s = element.GetAttribute(localName, namespaceURI);
        return s;
    }

    internal static bool HasAttribute(XmlElement element, string localName, string namespaceURI)
    {
        return element.HasAttribute(localName) || element.HasAttribute(localName, namespaceURI);
    }

    internal static bool VerifyAttributes(XmlElement element, string expectedAttrName)
    {
        return VerifyAttributes(element, expectedAttrName == null ? null : new string[] { expectedAttrName });
    }

    internal static bool VerifyAttributes(XmlElement element, string[] expectedAttrNames)
    {
        foreach (XmlAttribute attr in element.Attributes)
        {
            // There are a few Xml Special Attributes that are always allowed on any node. Make sure we allow those here.
            bool attrIsAllowed = attr.Name == "xmlns" || attr.Name.StartsWith("xmlns:") || attr.Name == "xml:space" || attr.Name == "xml:lang" || attr.Name == "xml:base";
            int expectedInd = 0;
            while (!attrIsAllowed && expectedAttrNames != null && expectedInd < expectedAttrNames.Length)
            {
                attrIsAllowed = attr.Name == expectedAttrNames[expectedInd];
                expectedInd++;
            }
            if (!attrIsAllowed)
                return false;
        }
        return true;
    }

    internal static bool IsNamespaceNode(XmlNode n)
    {
        return n.NodeType == XmlNodeType.Attribute && (n.Prefix.Equals("xmlns") || n.Prefix.Length == 0 && n.LocalName.Equals("xmlns"));
    }

    internal static bool IsXmlNamespaceNode(XmlNode n)
    {
        return n.NodeType == XmlNodeType.Attribute && n.Prefix.Equals("xml");
    }

    // We consider xml:space style attributes as default namespace nodes since they obey the same propagation rules
    internal static bool IsDefaultNamespaceNode(XmlNode n)
    {
        bool b1 = n.NodeType == XmlNodeType.Attribute && n.Prefix.Length == 0 && n.LocalName.Equals("xmlns");
        bool b2 = IsXmlNamespaceNode(n);
        return b1 || b2;
    }

    internal static bool IsEmptyDefaultNamespaceNode(XmlNode n)
    {
        return IsDefaultNamespaceNode(n) && n.Value.Length == 0;
    }

    internal static string GetNamespacePrefix(XmlAttribute a)
    {
        Debug.Assert(IsNamespaceNode(a) || IsXmlNamespaceNode(a));
        return a.Prefix.Length == 0 ? string.Empty : a.LocalName;
    }

    internal static bool HasNamespacePrefix(XmlAttribute a, string nsPrefix)
    {
        return GetNamespacePrefix(a).Equals(nsPrefix);
    }

    internal static bool IsNonRedundantNamespaceDecl(XmlAttribute a, XmlAttribute nearestAncestorWithSamePrefix)
    {
        if (nearestAncestorWithSamePrefix == null)
            return !IsEmptyDefaultNamespaceNode(a);
        else
            return !nearestAncestorWithSamePrefix.Value.Equals(a.Value);
    }

    internal static bool IsXmlPrefixDefinitionNode(XmlAttribute a)
    {
        return false;
        //            return a.Prefix.Equals("xmlns") && a.LocalName.Equals("xml") && a.Value.Equals(NamespaceUrlForXmlPrefix);
    }

    internal static void SBReplaceCharWithString(StringBuilder sb, char oldChar, string newString)
    {
        int i = 0;
        int newStringLength = newString.Length;
        while (i < sb.Length)
        {
            if (sb[i] == oldChar)
            {
                sb.Remove(i, 1);
                sb.Insert(i, newString);
                i += newStringLength;
            }
            else i++;
        }
    }

    internal static XmlReader PreProcessStreamInput(Stream inputStream, XmlResolver xmlResolver, string baseUri)
    {
        XmlReaderSettings settings = GetSecureXmlReaderSettings(xmlResolver);
        XmlReader reader = XmlReader.Create(inputStream, settings, baseUri);
        return reader;
    }

    internal static XmlReaderSettings GetSecureXmlReaderSettings(XmlResolver xmlResolver)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.XmlResolver = xmlResolver;
        settings.DtdProcessing = DtdProcessing.Parse;
        settings.MaxCharactersFromEntities = MaxCharactersFromEntities;
        settings.MaxCharactersInDocument = MaxCharactersInDocument;
        return settings;
    }

    internal static bool NodeInList(XmlNode node, XmlNodeList nodeList)
    {
        foreach (XmlNode nodeElem in nodeList)
        {
            if (nodeElem == node) return true;
        }
        return false;
    }

    internal static Hashtable TokenizePrefixListString(string s)
    {
        Hashtable set = new Hashtable();
        if (s != null)
        {
            string[] prefixes = s.Split(null);
            foreach (string prefix in prefixes)
            {
                if (prefix.Equals("#default"))
                {
                    set.Add(string.Empty, true);
                }
                else if (prefix.Length > 0)
                {
                    set.Add(prefix, true);
                }
            }
        }
        return set;
    }

    internal static string EscapeWhitespaceData(string data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(data);
        SBReplaceCharWithString(sb, (char)13, "&#xD;");
        return sb.ToString();
    }

    internal static string EscapeTextData(string data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(data);
        sb.Replace("&", "&amp;");
        sb.Replace("<", "&lt;");
        sb.Replace(">", "&gt;");
        SBReplaceCharWithString(sb, (char)13, "&#xD;");
        return sb.ToString();
    }

    internal static string EscapeCData(string data)
    {
        return EscapeTextData(data);
    }

    internal static string EscapeAttributeValue(string value)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(value);
        sb.Replace("&", "&amp;");
        sb.Replace("<", "&lt;");
        sb.Replace("\"", "&quot;");
        SBReplaceCharWithString(sb, (char)9, "&#x9;");
        SBReplaceCharWithString(sb, (char)10, "&#xA;");
        SBReplaceCharWithString(sb, (char)13, "&#xD;");
        return sb.ToString();
    }

    internal static XmlDocument GetOwnerDocument(XmlNodeList nodeList)
    {
        foreach (XmlNode node in nodeList)
        {
            if (node.OwnerDocument != null)
                return node.OwnerDocument;
        }
        return null;
    }
}