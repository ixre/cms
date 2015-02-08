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

  public enum XmlRpcType
  {
    tInvalid,
    tInt32,
    tInt64,
    tBoolean,
    tString,
    tDouble,
    tDateTime,
    tBase64,
    tStruct,
    tHashtable,
    tArray,
    tMultiDimArray,
    tVoid,
    tNil
  }

  public class XmlRpcTypeInfo
  {
    static bool IsVisibleXmlRpcMethod(MethodInfo mi)
    {
      bool ret = false;
      Attribute attr = Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr != null)
      {
        XmlRpcMethodAttribute mattr = (XmlRpcMethodAttribute)attr;
        ret = !(mattr.Hidden || mattr.IntrospectionMethod == true);
      }
      return ret;
    }

    public static string GetXmlRpcMethodName(MethodInfo mi)
    {
      XmlRpcMethodAttribute attr = (XmlRpcMethodAttribute)
        Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr != null
        && attr.Method != null
        && attr.Method != "")
      {
        return attr.Method;
      }
      else
      {
        return mi.Name;
      }
    }

    public static XmlRpcType GetXmlRpcType(object o)
    {
      return o == null ? XmlRpcType.tNil : GetXmlRpcType(o.GetType());
    }

    public static XmlRpcType GetXmlRpcType(Type t)
    {
      return GetXmlRpcType(t, new Stack<Type>());
    }

    private static XmlRpcType GetXmlRpcType(Type t, Stack<Type> typeStack)
    {
      XmlRpcType ret;
      if (t == typeof(Int32))
        ret = XmlRpcType.tInt32;
      else if (t == typeof(Int64))
        ret = XmlRpcType.tInt64;
      else if (t == typeof(Boolean))
        ret = XmlRpcType.tBoolean;
      else if (t == typeof(String))
        ret = XmlRpcType.tString;
      else if (t == typeof(Double))
        ret = XmlRpcType.tDouble;
      else if (t == typeof(DateTime))
        ret = XmlRpcType.tDateTime;
      else if (t == typeof(byte[]))
        ret = XmlRpcType.tBase64;
      else if (t == typeof(XmlRpcStruct))
      {
        ret = XmlRpcType.tHashtable;
      }
      else if (t == typeof(Array))
        ret = XmlRpcType.tArray;
      else if (t.IsArray)
      {
#if (!COMPACT_FRAMEWORK)
        Type elemType = t.GetElementType();
        if (elemType != typeof(Object)
          && GetXmlRpcType(elemType, typeStack) == XmlRpcType.tInvalid)
        {
          ret = XmlRpcType.tInvalid;
        }
        else
        {
          if (t.GetArrayRank() == 1)  // single dim array
            ret = XmlRpcType.tArray;
          else
            ret = XmlRpcType.tMultiDimArray;
        }
#else
        //!! check types of array elements if not Object[]
        Type elemType = null;
        string[] checkSingleDim = Regex.Split(t.FullName, "\\[\\]$");
        if (checkSingleDim.Length > 1)  // single dim array
        {
          elemType = Type.GetType(checkSingleDim[0]);
          ret = XmlRpcType.tArray;
        }
        else
        {
          string[] checkMultiDim = Regex.Split(t.FullName, "\\[,[,]*\\]$");
          if (checkMultiDim.Length > 1)
          {
            elemType = Type.GetType(checkMultiDim[0]);
            ret = XmlRpcType.tMultiDimArray;
          }
          else
            ret = XmlRpcType.tInvalid;
        }
        if (elemType != null)
        {
          if (elemType != typeof(Object) 
            && GetXmlRpcType(elemType, typeStack) == XmlRpcType.tInvalid)
          {
            ret = XmlRpcType.tInvalid;
          }
        }
#endif

      }
      else if (t == typeof(int?))
        ret = XmlRpcType.tInt32;
      else if (t == typeof(long?))
        ret = XmlRpcType.tInt64;
      else if (t == typeof(Boolean?))
        ret = XmlRpcType.tBoolean;
      else if (t == typeof(Double?))
        ret = XmlRpcType.tDouble;
      else if (t == typeof(DateTime?))
        ret = XmlRpcType.tDateTime;
      else if (t == typeof(void))
      {
        ret = XmlRpcType.tVoid;
      }
      else if ((t.IsValueType && !t.IsPrimitive && !t.IsEnum)
        || t.IsClass)
      {
        // if type is struct or class its only valid for XML-RPC mapping if all 
        // its members have a valid mapping or are of type object which
        // maps to any XML-RPC type
        MemberInfo[] mis = t.GetMembers();
        foreach (MemberInfo mi in mis)
        {
          if (Attribute.IsDefined(mi, typeof(NonSerializedAttribute)))
            continue;
          if (mi.MemberType == MemberTypes.Field)
          {
            FieldInfo fi = (FieldInfo)mi;
            if (typeStack.Contains(fi.FieldType))
              continue;
            try
            {
              typeStack.Push(fi.FieldType);
              if ((fi.FieldType != typeof(Object)
                && GetXmlRpcType(fi.FieldType, typeStack) == XmlRpcType.tInvalid))
              {
                return XmlRpcType.tInvalid;
              }
            }
            finally
            {
              typeStack.Pop();
            }
          }
          else if (mi.MemberType == MemberTypes.Property)
          {
            PropertyInfo pi = (PropertyInfo)mi;
            if (pi.GetIndexParameters().Length > 0)
              return XmlRpcType.tInvalid;
            if (typeStack.Contains(pi.PropertyType))
              continue;
            try
            {
              typeStack.Push(pi.PropertyType);
              if ((pi.PropertyType != typeof(Object)
                && GetXmlRpcType(pi.PropertyType, typeStack) == XmlRpcType.tInvalid))
              {
                return XmlRpcType.tInvalid;
              }
            }
            finally
            {
              typeStack.Pop();
            }
          }
        }
        ret = XmlRpcType.tStruct;
      }
      else if (t.IsEnum)
      {
        Type enumBaseType = Enum.GetUnderlyingType(t);
        if (enumBaseType == typeof(int) || enumBaseType == typeof(byte)
          || enumBaseType == typeof(sbyte) || enumBaseType == typeof(short)
          || enumBaseType == typeof(ushort))
          ret = XmlRpcType.tInt32;
        else if (enumBaseType == typeof(long) || enumBaseType == typeof(UInt32))
          ret = XmlRpcType.tInt64;
        else
          ret = XmlRpcType.tInvalid;
      }
      else
        ret = XmlRpcType.tInvalid;
      return ret;
    }

    static public string GetXmlRpcTypeString(Type t)
    {
      XmlRpcType rpcType = GetXmlRpcType(t);
      return GetXmlRpcTypeString(rpcType);
    }

    static public string GetXmlRpcTypeString(XmlRpcType t)
    {
      string ret = null;
      if (t == XmlRpcType.tInt32)
        ret = "integer";
      else if (t == XmlRpcType.tInt64)
        ret = "i8";
      else if (t == XmlRpcType.tBoolean)
        ret = "boolean";
      else if (t == XmlRpcType.tString)
        ret = "string";
      else if (t == XmlRpcType.tDouble)
        ret = "double";
      else if (t == XmlRpcType.tDateTime)
        ret = "dateTime";
      else if (t == XmlRpcType.tBase64)
        ret = "base64";
      else if (t == XmlRpcType.tStruct)
        ret = "struct";
      else if (t == XmlRpcType.tHashtable)
        ret = "struct";
      else if (t == XmlRpcType.tArray)
        ret = "array";
      else if (t == XmlRpcType.tMultiDimArray)
        ret = "array";
      else if (t == XmlRpcType.tVoid)
        ret = "void";
      else
        ret = null;
      return ret;
    }

    public static string GetUrlFromAttribute(Type type)
    {
      XmlRpcUrlAttribute urlAttr = Attribute.GetCustomAttribute(type, typeof(XmlRpcUrlAttribute))
        as XmlRpcUrlAttribute;
      return urlAttr != null ? urlAttr.Uri : null;
    }

    public static string GetRpcMethodName(MethodInfo mi)
    {
      string rpcMethod;
      string MethodName = mi.Name;
      Attribute attr = Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcBeginAttribute));
      if (attr != null)
      {
        rpcMethod = ((XmlRpcBeginAttribute)attr).Method;
        if (rpcMethod == "")
        {
          if (!MethodName.StartsWith("Begin") || MethodName.Length <= 5)
            throw new Exception(String.Format(
              "method {0} has invalid signature for begin method",
              MethodName));
          rpcMethod = MethodName.Substring(5);
        }
        return rpcMethod;
      }
      // if no XmlRpcBegin attribute, must have XmlRpcMethod attribute   
      attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute));
      if (attr == null)
      {
        throw new Exception("missing method attribute");
      }
      XmlRpcMethodAttribute xrmAttr = attr as XmlRpcMethodAttribute;
      rpcMethod = xrmAttr.Method;
      if (rpcMethod == "")
      {
        rpcMethod = mi.Name;
      }
      return rpcMethod;
    }
  }
}