using System.Security.Cryptography;
using System.Text;

namespace IT.Xml.C14N.Internal;

// the interface to be implemented by all subclasses of XmlNode
// that have to provide node subsetting and canonicalization features.
internal interface ICanonicalizableNode
{
    bool IsInNodeSet
    {
        get;
        set;
    }

    //void WriteUtf8(System.Buffers.IBufferWriter<byte> writer, DocPosition docPos, AncestralNamespaceContextManager anc);
    void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc);
    void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc);
    void WriteHash(IIncrementalHashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc);
}