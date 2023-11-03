namespace IT.Xml.C14N.Internal;

// the current rendering position in document
internal enum DocPosition
{
    BeforeRootElement,
    InRootElement,
    AfterRootElement
}