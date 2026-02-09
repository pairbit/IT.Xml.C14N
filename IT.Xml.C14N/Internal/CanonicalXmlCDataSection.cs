using System.Xml;
using System.Text;
using System.Security.Cryptography;

namespace IT.Xml.C14N.Internal;

// the class that provides node subset state and canonicalization function to XmlCDataSection
internal sealed class CanonicalXmlCDataSection : XmlCDataSection, ICanonicalizableNode
{
    private bool _isInNodeSet;
    public CanonicalXmlCDataSection(string? data, XmlDocument doc, bool defaultNodeSetInclusionState) : base(data, doc)
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
            strBuilder.Append(Utils.EscapeCData(Data));
    }

    public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
    {
        if (IsInNodeSet)
        {
            hash.Append(Encoding.UTF8.GetBytes(Utils.EscapeCData(Data)));
        }
    }

    public void WriteHash(IIncrementalHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
    {
        if (IsInNodeSet)
        {
            hash.Append(Encoding.UTF8.GetBytes(Utils.EscapeCData(Data)));
        }
    }
}