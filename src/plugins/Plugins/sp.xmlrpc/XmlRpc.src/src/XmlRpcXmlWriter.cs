using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace CookComputing.XmlRpc
{
  public static class XmlRpcXmlWriter
  {
      public static XmlWriter Create(Stream stm, XmlRpcFormatSettings settings)
      {
          if (settings.XmlEncoding == null) settings.XmlEncoding = Config.DefaultEncoding;
          var stmWriter = new EncodingStreamWriter(stm, settings.XmlEncoding);
          XmlWriter xtw = XmlWriter.Create(stmWriter, ConfigureXmlFormat(settings));
          return xtw;
      }

    private static XmlWriterSettings ConfigureXmlFormat(XmlRpcFormatSettings settings)
    {
      if (settings.UseIndentation)
      {
        return new XmlWriterSettings
        {
          Indent = true,
          IndentChars = new string(' ', settings.Indentation),
          Encoding = settings.XmlEncoding ?? Config.DefaultEncoding,
          NewLineHandling = NewLineHandling.None,
          OmitXmlDeclaration = settings.OmitXmlDeclaration,
        };
      }
      else
      {
        return new XmlWriterSettings
        {
          Indent = false,
          Encoding = settings.XmlEncoding ?? Config.DefaultEncoding,
          OmitXmlDeclaration = settings.OmitXmlDeclaration,
        };
      }
    }

    private class EncodingStreamWriter : StreamWriter
    {
      Encoding _encoding;

      public EncodingStreamWriter(Stream stm, Encoding encoding)
        : base(stm)
      {
        _encoding = encoding;
      }

      public override Encoding Encoding
      {
        get { return _encoding; }
      }
    }
  }
}
