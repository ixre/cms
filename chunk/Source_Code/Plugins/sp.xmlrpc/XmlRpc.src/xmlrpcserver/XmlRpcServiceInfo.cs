/* 
XML-RPC.NET library
Copyright (c) 2001-2006, Charles Cook <charlescook@cookcomputing.com>

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
  using System.Collections.Generic;
  using System.Reflection;
  using System.Text.RegularExpressions;

  public class XmlRpcServiceInfo
  {
    public static XmlRpcServiceInfo CreateServiceInfo(Type type)
    {
      XmlRpcServiceInfo svcInfo = new XmlRpcServiceInfo();
      // extract service info
      XmlRpcServiceAttribute svcAttr = (XmlRpcServiceAttribute)
        Attribute.GetCustomAttribute(type, typeof(XmlRpcServiceAttribute));
      if (svcAttr != null && svcAttr.Description != "")
        svcInfo.doc = svcAttr.Description;
      if (svcAttr != null && svcAttr.Name != "")
        svcInfo.Name = svcAttr.Name;
      else
        svcInfo.Name = type.Name;
      // extract method info
      var methods = new Dictionary<string, XmlRpcMethodInfo>();

      foreach (Type itf in type.GetInterfaces())
      {
        XmlRpcServiceAttribute itfAttr = (XmlRpcServiceAttribute)
          Attribute.GetCustomAttribute(itf, typeof(XmlRpcServiceAttribute));
        if (itfAttr != null)
          svcInfo.doc = itfAttr.Description;
#if (!COMPACT_FRAMEWORK)
        InterfaceMapping imap = type.GetInterfaceMap(itf);
        foreach (MethodInfo mi in imap.InterfaceMethods)
        {
          ExtractMethodInfo(methods, mi, itf);
        }
#else
        foreach (MethodInfo mi in itf.GetMethods())
        {
          ExtractMethodInfo(methods, mi, itf);
        }
#endif
      }

      foreach (MethodInfo mi in type.GetMethods())
      {
        var mthds = new List<MethodInfo>();
        mthds.Add(mi);
        MethodInfo curMi = mi;
        while (true)
        {
          MethodInfo baseMi = curMi.GetBaseDefinition();
          if (baseMi.DeclaringType == curMi.DeclaringType)
            break;
          mthds.Insert(0, baseMi);
          curMi = baseMi;
        }
        foreach (MethodInfo mthd in mthds)
        {
          ExtractMethodInfo(methods, mthd, type);
        }
      }
      svcInfo.methodInfos = new XmlRpcMethodInfo[methods.Count];
      methods.Values.CopyTo(svcInfo.methodInfos, 0);
      Array.Sort(svcInfo.methodInfos);
      return svcInfo;
    }

    public MethodInfo GetMethodInfo(string xmlRpcMethodName)
    {
      foreach (XmlRpcMethodInfo xmi in methodInfos)
      {
        if (xmlRpcMethodName == xmi.XmlRpcName)
        {
          return xmi.MethodInfo;
        }
      }
      return null;
    }

    static void ExtractMethodInfo(Dictionary<string, XmlRpcMethodInfo> methods, 
      MethodInfo mi, Type type)
    {
      XmlRpcMethodAttribute attr = (XmlRpcMethodAttribute)
        Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr == null)
        return;
      XmlRpcMethodInfo mthdInfo = new XmlRpcMethodInfo();
      mthdInfo.MethodInfo = mi;
      mthdInfo.XmlRpcName = XmlRpcTypeInfo.GetXmlRpcMethodName(mi);
      mthdInfo.MiName = mi.Name;
      mthdInfo.Doc = attr.Description;
      mthdInfo.IsHidden = attr.IntrospectionMethod | attr.Hidden;
      // extract parameters information
      var parmList = new List<XmlRpcParameterInfo>();
      ParameterInfo[] parms = mi.GetParameters();
      foreach (ParameterInfo parm in parms)
      {
        XmlRpcParameterInfo parmInfo = new XmlRpcParameterInfo();
        parmInfo.Name = parm.Name;
        parmInfo.Type = parm.ParameterType;
        parmInfo.XmlRpcType = XmlRpcTypeInfo.GetXmlRpcTypeString(parm.ParameterType);
        // retrieve optional attributed info
        parmInfo.Doc = "";
        XmlRpcParameterAttribute pattr = (XmlRpcParameterAttribute)
          Attribute.GetCustomAttribute(parm,
          typeof(XmlRpcParameterAttribute));
        if (pattr != null)
        {
          parmInfo.Doc = pattr.Description;
          parmInfo.XmlRpcName = pattr.Name;
        }
        parmInfo.IsParams = Attribute.IsDefined(parm,
          typeof(ParamArrayAttribute));
        parmList.Add(parmInfo);
      }
      mthdInfo.Parameters = parmList.ToArray();
      // extract return type information
      mthdInfo.ReturnType = mi.ReturnType;
      mthdInfo.ReturnXmlRpcType = XmlRpcTypeInfo.GetXmlRpcTypeString(mi.ReturnType);
      object[] orattrs = mi.ReturnTypeCustomAttributes.GetCustomAttributes(
        typeof(XmlRpcReturnValueAttribute), false);
      if (orattrs.Length > 0)
      {
        mthdInfo.ReturnDoc = ((XmlRpcReturnValueAttribute)orattrs[0]).Description;
      }

      if (methods.ContainsKey(mthdInfo.XmlRpcName))
      {
        throw new XmlRpcDupXmlRpcMethodNames(String.Format("Method "
          + "{0} in type {1} has duplicate XmlRpc method name {2}",
          mi.Name, type.Name, mthdInfo.XmlRpcName));
      }
      else
        methods.Add(mthdInfo.XmlRpcName, mthdInfo);
    }

    public string GetMethodName(string XmlRpcMethodName)
    {
      foreach (XmlRpcMethodInfo methodInfo in methodInfos)
      {
        if (methodInfo.XmlRpcName == XmlRpcMethodName)
          return methodInfo.MiName;
      }
      return null;
    }

    public String Doc
    {
      get { return doc; }
      set { doc = value; }
    }

    public String Name
    {
      get { return name; }
      set { name = value; }
    }

    public XmlRpcMethodInfo[] Methods
    {
      get { return methodInfos; }
    }

    public XmlRpcMethodInfo GetMethod(
      String methodName)
    {
      foreach (XmlRpcMethodInfo mthdInfo in methodInfos)
      {
        if (mthdInfo.XmlRpcName == methodName)
          return mthdInfo;
      }
      return null;
    }

    private XmlRpcServiceInfo()
    {
    }

    XmlRpcMethodInfo[] methodInfos;
    String doc;
    string name;
  }
}