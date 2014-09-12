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

namespace CookComputing.XmlRpc
{
  using System;
  using System.Collections;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Text.RegularExpressions;
    using Ops.Framework;

  public class XmlRpcServerProtocol : SystemMethodsBase
  {

    public Stream Invoke(Stream requestStream)
    {
      try
      {
        var serializer = new XmlRpcResponseSerializer();
        var deserializer = new XmlRpcRequestDeserializer();
        Type type = this.GetType();
        XmlRpcServiceAttribute serviceAttr = (XmlRpcServiceAttribute)
          Attribute.GetCustomAttribute(this.GetType(),
          typeof(XmlRpcServiceAttribute));

        if (serviceAttr != null)
        {
            if (serviceAttr.XmlEncoding != null)
            {
                serializer.XmlEncoding = Encoding.GetEncoding(serviceAttr.XmlEncoding) ?? Config.DefaultEncoding;
            }
            serializer.UseEmptyParamsTag = serviceAttr.UseEmptyElementTags;
            serializer.UseIntTag = serviceAttr.UseIntTag;
            serializer.UseStringTag = serviceAttr.UseStringTag;
            serializer.UseIndentation = serviceAttr.UseIndentation;
            serializer.Indentation = serviceAttr.Indentation;
        }

        XmlRpcRequest xmlRpcReq = deserializer.DeserializeRequest(requestStream, this.GetType());
        XmlRpcResponse xmlRpcResp = Invoke(xmlRpcReq);

        Stream responseStream = new MemoryStream();
        serializer.SerializeResponse(responseStream, xmlRpcResp);
        responseStream.Seek(0, SeekOrigin.Begin);
        return responseStream;
      }
      catch (Exception ex)
      {
        XmlRpcFaultException fex;
        if (ex is XmlRpcException)
          fex = new XmlRpcFaultException(0, ((XmlRpcException)ex).Message);
        else if (ex is XmlRpcFaultException)
          fex = (XmlRpcFaultException)ex;
        else 
          fex = new XmlRpcFaultException(0, ex.Message);
        XmlRpcSerializer serializer = new XmlRpcSerializer();
        Stream responseStream = new MemoryStream();
        serializer.SerializeFaultResponse(responseStream, fex);
        responseStream.Seek(0, SeekOrigin.Begin);
        return responseStream;      
      }
    }

    public XmlRpcResponse Invoke(XmlRpcRequest request)
    {
      MethodInfo mi = null;
      if (request.mi != null)
      {
        mi = request.mi;
      }
      else
      {
        mi = this.GetType().GetMethod(request.method);
      }
      // exceptions thrown during an MethodInfo.Invoke call are
      // package as inner of 
      Object reto;
      try
      {
        reto = mi.Invoke(this, request.args);
      }
      catch(Exception ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        throw ex;
      }
      // methods which have void return type always return integer 0
      // because XML-RPC doesn't support no return type (could use nil
      // but want to maintain backwards compatibility in this area)
      if (mi != null && mi.ReturnType == typeof(void))
        reto = 0;
      XmlRpcResponse response = new XmlRpcResponse(reto);
      return response;
    }

    bool IsVisibleXmlRpcMethod(MethodInfo mi)
    {
      bool ret = false;
      Attribute attr = Attribute.GetCustomAttribute(mi, 
        typeof(XmlRpcMethodAttribute));
      if (attr != null)
      {
        XmlRpcMethodAttribute mattr = (XmlRpcMethodAttribute)attr;
        ret = !mattr.Hidden;
      }
      return ret;
    }
  }
}
