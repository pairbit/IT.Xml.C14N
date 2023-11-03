using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace IT.Xml.C14N;

using Internal;

public abstract class Transform
{
    private string _algorithm;
    private string _baseUri;
    internal XmlResolver _xmlResolver;
    private bool _bResolverSet;

    internal string BaseURI
    {
        get { return _baseUri; }
        set { _baseUri = value; }
    }

    protected Transform() { }

    public string Algorithm
    {
        get { return _algorithm; }
        set { _algorithm = value; }
    }

    public XmlResolver Resolver
    {
        internal get
        {
            return _xmlResolver;
        }
        // This property only has a public setter. The rationale for this is that we don't have a good value
        // to return when it has not been explicitely set, as we are using XmlSecureResolver by default
        set
        {
            _xmlResolver = value;
            _bResolverSet = true;
        }
    }

    internal bool ResolverSet
    {
        get { return _bResolverSet; }
    }

    public abstract Type[] InputTypes
    {
        get;
    }

    public abstract Type[] OutputTypes
    {
        get;
    }

    internal bool AcceptsType(Type inputType)
    {
        if (InputTypes != null)
        {
            for (int i = 0; i < InputTypes.Length; i++)
            {
                if (inputType == InputTypes[i] || inputType.IsSubclassOf(InputTypes[i]))
                    return true;
            }
        }
        return false;
    }

    //
    // public methods
    //

    public XmlElement GetXml()
    {
        XmlDocument document = new XmlDocument();
        document.PreserveWhitespace = true;
        return GetXml(document);
    }

    internal XmlElement GetXml(XmlDocument document)
    {
        return GetXml(document, "Transform");
    }

    internal XmlElement GetXml(XmlDocument document, string name)
    {
        XmlElement transformElement = document.CreateElement(name, SignedXml.XmlDsigNamespaceUrl);
        if (!string.IsNullOrEmpty(Algorithm))
            transformElement.SetAttribute("Algorithm", Algorithm);
        XmlNodeList children = GetInnerXml();
        if (children != null)
        {
            foreach (XmlNode node in children)
            {
                transformElement.AppendChild(document.ImportNode(node, true));
            }
        }
        return transformElement;
    }

    public abstract void LoadInnerXml(XmlNodeList nodeList);

    protected abstract XmlNodeList GetInnerXml();

    public abstract void LoadInput(object obj);

    public abstract object GetOutput();

    public abstract object GetOutput(Type type);

    public virtual byte[] GetDigestedOutput(HashAlgorithm hash)
    {
        return hash.ComputeHash((Stream)GetOutput(typeof(Stream)));
    }
}