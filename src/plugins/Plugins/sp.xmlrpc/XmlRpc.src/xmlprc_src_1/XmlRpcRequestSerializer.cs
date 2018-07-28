/* 
XML-RPC.NET library
Copyright (c) 2001-2011, Charles Cook <charlescook@cookcomputing.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

// TODO: overriding default mapping action in a struct should not affect nested structs

namespace CookComputing.XmlRpc
{
  using System;
  using System.Collections;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Xml;
  using System.Collections.Generic;

  public class XmlRpcRequestSerializer : XmlRpcSerializer
  {
    public XmlRpcRequestSerializer() { }
    public XmlRpcRequestSerializer(XmlRpcFormatSettings settings) : base(settings) { }

    public void SerializeRequest(Stream stm, XmlRpcRequest request)
    {
      XmlWriter xtw = XmlRpcXmlWriter.Create(stm, base.XmlRpcFormatSettings);
      xtw.WriteStartDocument();
      xtw.WriteStartElement("", "methodCall", "");
      {
        var mappingActions = new MappingActions();
        mappingActions = GetTypeMappings(request.mi, mappingActions);
        mappingActions = GetMappingActions(request.mi, mappingActions);
        WriteFullElementString(xtw, "methodName", request.method);
        if (request.args.Length > 0 || UseEmptyParamsTag)
        {
          xtw.WriteStartElement("params");
          try
          {
            if (!IsStructParamsMethod(request.mi))
              SerializeParams(xtw, request, mappingActions);
            else
              SerializeStructParams(xtw, request, mappingActions);
          }
          catch (XmlRpcUnsupportedTypeException ex)
          {
            throw new XmlRpcUnsupportedTypeException(ex.UnsupportedType,
              String.Format("A parameter is of, or contains an instance of, "
              + "type {0} which cannot be mapped to an XML-RPC type",
              ex.UnsupportedType));
          }
          WriteFullEndElement(xtw);
        }
      }
      WriteFullEndElement(xtw);
      xtw.Flush();
    }

    void SerializeParams(XmlWriter xtw, XmlRpcRequest request,
      MappingActions mappingActions)
    {
      ParameterInfo[] pis = null;
      if (request.mi != null)
      {
        pis = request.mi.GetParameters();
      }
      for (int i = 0; i < request.args.Length; i++)
      {
        var paramMappingActions = pis == null ? mappingActions
          : GetMappingActions(pis[i], mappingActions);
        if (pis != null)
        {
          if (i >= pis.Length)
            throw new XmlRpcInvalidParametersException("Number of request "
              + "parameters greater than number of proxy method parameters.");
          if (i == pis.Length - 1
            && Attribute.IsDefined(pis[i], typeof(ParamArrayAttribute)))
          {
            Array ary = (Array)request.args[i];
            foreach (object o in ary)
            {
              //if (o == null)
              //  throw new XmlRpcNullParameterException(
              //    "Null parameter in params array");
              xtw.WriteStartElement("", "param", "");
              Serialize(xtw, o, paramMappingActions);
              WriteFullEndElement(xtw);
            }
            break;
          }
        }
        //if (request.args[i] == null)
        //{
        //  throw new XmlRpcNullParameterException(String.Format(
        //    "Null method parameter #{0}", i + 1));
        //}
        xtw.WriteStartElement("", "param", "");
        Serialize(xtw, request.args[i], paramMappingActions);
        WriteFullEndElement(xtw);
      }
    }

    void SerializeStructParams(XmlWriter xtw, XmlRpcRequest request,
      MappingActions mappingActions)
    {
      ParameterInfo[] pis = request.mi.GetParameters();
      if (request.args.Length > pis.Length)
        throw new XmlRpcInvalidParametersException("Number of request "
          + "parameters greater than number of proxy method parameters.");
      if (Attribute.IsDefined(pis[request.args.Length - 1],
        typeof(ParamArrayAttribute)))
      {
        throw new XmlRpcInvalidParametersException("params parameter cannot "
          + "be used with StructParams.");
      }
      xtw.WriteStartElement("", "param", "");
      xtw.WriteStartElement("", "value", "");
      xtw.WriteStartElement("", "struct", "");
      for (int i = 0; i < request.args.Length; i++)
      {
        if (request.args[i] == null)
        {
          throw new XmlRpcNullParameterException(String.Format(
            "Null method parameter #{0}", i + 1));
        }
        xtw.WriteStartElement("", "member", "");
        WriteFullElementString(xtw, "name", pis[i].Name);
        Serialize(xtw, request.args[i], mappingActions);
        WriteFullEndElement(xtw);
      }
      WriteFullEndElement(xtw);
      WriteFullEndElement(xtw);
      WriteFullEndElement(xtw);
    }
  }
}
