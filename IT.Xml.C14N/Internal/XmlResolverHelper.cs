#nullable enable
using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace IT.Xml.C14N.Internal;

internal static class XmlResolverHelper
{
    internal static XmlResolver GetThrowingResolver()
    {
#if NET7_0_OR_GREATER
        return XmlResolver.ThrowingResolver;
#else
        return XmlThrowingResolver.s_singleton;
#endif
    }

#if !NET7_0_OR_GREATER
        // An XmlResolver that forbids all external entity resolution.
        // (Copied from XmlResolver.ThrowingResolver.cs.)
        private sealed class XmlThrowingResolver : XmlResolver
        {
            internal static readonly XmlThrowingResolver s_singleton = new();

            // Private constructor ensures existing only one instance of XmlThrowingResolver
            private XmlThrowingResolver() { }

            public override ICredentials Credentials
            {
                set { /* Do nothing */ }
            }

            public override object GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
            {
                throw new XmlException("Cryptography_Xml_EntityResolutionNotSupported");
            }

            public override Task<object> GetEntityAsync(Uri absoluteUri, string? role, Type? ofObjectToReturn)
            {
                throw new XmlException("Cryptography_Xml_EntityResolutionNotSupported");
            }
        }
#endif
}