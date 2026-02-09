using System;

namespace IT.Xml.C14N;

public interface IIncrementalHashAlgorithm
{
    void Append(byte data);

    void Append(ReadOnlySpan<byte> data);
}