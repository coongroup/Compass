using System.ComponentModel;
using System;

namespace OmssaLib
{
    [Flags]
    public enum OutputFileType
    {
        [Description("-oc")]
        Csv = 0x0,
        [Description("-ox")]
        Xml = 0x1,
        [Description("-op")]
        PepXml = 0x2,
        [Description("-obz2")]
        XmlBzip2 = 0x4,
        [Description("-o")]
        Asn1Text = 0x8,
        [Description("-ob")]
        Asn1Binary = 0x16
    }
}
