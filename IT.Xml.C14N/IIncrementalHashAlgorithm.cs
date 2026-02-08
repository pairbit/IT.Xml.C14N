using System;

namespace IT.Xml.C14N;

public interface IIncrementalHashAlgorithm
{
    void Append(ReadOnlySpan<byte> span);
}