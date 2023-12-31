using System.Xml;
using System.Text;
using System.Security.Cryptography;

namespace IT.Xml.C14N.Internal;

// the class that provides node subset state and canonicalization function to XmlEntityReference
internal sealed class CanonicalXmlEntityReference : XmlEntityReference, ICanonicalizableNode
{
    private bool _isInNodeSet;

    public CanonicalXmlEntityReference(string name, XmlDocument doc, bool defaultNodeSetInclusionState)
        : base(name, doc)
    {
        _isInNodeSet = defaultNodeSetInclusionState;
    }

    public bool IsInNodeSet
    {
        get { return _isInNodeSet; }
        set { _isInNodeSet = value; }
    }

    public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
    {
        if (IsInNodeSet)
            CanonicalizationDispatcher.WriteGenericNode(this, strBuilder, docPos, anc);
    }

    public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
    {
        if (IsInNodeSet)
            CanonicalizationDispatcher.WriteHashGenericNode(this, hash, docPos, anc);
    }
}