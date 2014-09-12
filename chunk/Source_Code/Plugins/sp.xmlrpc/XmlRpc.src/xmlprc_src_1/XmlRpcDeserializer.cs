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
  using System.Diagnostics;
  using System.Collections.Generic;

  public class XmlRpcDeserializer
  {
    public XmlRpcNonStandard NonStandard
    {
      get { return m_nonStandard; }
      set { m_nonStandard = value; }
    }
    XmlRpcNonStandard m_nonStandard = XmlRpcNonStandard.None;

    // private properties
    protected bool AllowInvalidHTTPContent
    {
      get { return (m_nonStandard & XmlRpcNonStandard.AllowInvalidHTTPContent) != 0; }
    }

    protected bool AllowNonStandardDateTime
    {
      get { return (m_nonStandard & XmlRpcNonStandard.AllowNonStandardDateTime) != 0; }
    }

    protected bool AllowStringFaultCode
    {
      get { return (m_nonStandard & XmlRpcNonStandard.AllowStringFaultCode) != 0; }
    }

    protected bool IgnoreDuplicateMembers
    {
      get { return (m_nonStandard & XmlRpcNonStandard.IgnoreDuplicateMembers) != 0; }
    }

    protected bool MapEmptyDateTimeToMinValue
    {
      get { return (m_nonStandard & XmlRpcNonStandard.MapEmptyDateTimeToMinValue) != 0; }
    }

    protected bool MapZerosDateTimeToMinValue
    {
      get { return (m_nonStandard & XmlRpcNonStandard.MapZerosDateTimeToMinValue) != 0; }
    }

    public Object MapValueNode(
      IEnumerator<Node> iter,
      Type valType,
      MappingStack mappingStack,
      MappingAction mappingAction)
    {
      var valueNode = iter.Current as ValueNode;
      // if suppplied type is System.Object then ignore it because
      // if doesn't provide any useful information (parsing methods
      // expect null in this case)
      if (valType != null && valType.BaseType == null)
        valType = null;
      object ret = "";

      if (valueNode is StringValue && valueNode.ImplicitValue)
        CheckImplictString(valType, mappingStack);

      Type mappedType;

      object retObj = null;
      if (iter.Current is ArrayValue)
        retObj = MapArray(iter, valType, mappingStack, mappingAction,
          out mappedType);
      else if (iter.Current is StructValue)
      {
        // if we don't know the expected struct type then we must
        // map the XML-RPC struct as an instance of XmlRpcStruct
        if (valType != null && valType != typeof(XmlRpcStruct)
          && !valType.IsSubclassOf(typeof(XmlRpcStruct)))
        {
          retObj = MapStruct(iter, valType, mappingStack, mappingAction,
            out mappedType);
        }
        else
        {
          if (valType == null || valType == typeof(object))
            valType = typeof(XmlRpcStruct);
          // TODO: do we need to validate type here?
          retObj = MapHashtable(iter, valType, mappingStack, mappingAction,
            out mappedType);
        }
      }
      else if (iter.Current is Base64Value)
        retObj = MapBase64(valueNode.Value, valType, mappingStack, mappingAction,
          out mappedType);
      else if (iter.Current is IntValue)
      {
        retObj = MapInt(valueNode.Value, valType, mappingStack, mappingAction,
          out mappedType);
      }
      else if (iter.Current is LongValue)
      {
        retObj = MapLong(valueNode.Value, valType, mappingStack, mappingAction,
          out mappedType);
      }
      else if (iter.Current is StringValue)
      {
        retObj = MapString(valueNode.Value, valType, mappingStack, mappingAction,
          out mappedType);
      }
      else if (iter.Current is BooleanValue)
      {
        retObj = MapBoolean(valueNode.Value, valType, mappingStack, mappingAction,
          out mappedType);
      }
      else if (iter.Current is DoubleValue)
      {
        retObj = MapDouble(valueNode.Value, valType, mappingStack, mappingAction,
          out mappedType);
      }
      else if (iter.Current is DateTimeValue)
      {
        retObj = MapDateTime(valueNode.Value, valType, mappingStack, mappingAction,
          out mappedType);
      }
      else if (iter.Current is NilValue)
      {
        retObj = MapNilValue(valueNode.Value, valType, mappingStack, mappingAction,
          out mappedType);
      }

      return retObj;
    }

    private object MapDateTime(string value, Type valType, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      CheckExpectedType(valType, typeof(DateTime), mappingStack);
      mappedType = typeof(DateTime);
      return OnStack("dateTime", mappingStack, delegate()
      {
        if (value == "" && MapEmptyDateTimeToMinValue)
          return DateTime.MinValue;
        DateTime retVal;
        if (!DateTime8601.TryParseDateTime8601(value, out retVal))
        {
          if (MapZerosDateTimeToMinValue && value.StartsWith("0000")
            && (value == "00000000T00:00:00" || value == "0000-00-00T00:00:00Z"
            || value == "00000000T00:00:00Z" || value == "0000-00-00T00:00:00"))
            retVal = DateTime.MinValue;
          else
            throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
              + " contains invalid dateTime value "
              + StackDump(mappingStack));
        }
        return retVal;
      });
    }

    private object MapDouble(string value, Type valType, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      CheckExpectedType(valType, typeof(double), mappingStack);
      mappedType = typeof(double);
      return OnStack("double", mappingStack, delegate()
      {
        try
        {
          double ret = Double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
          return ret;
        }
        catch (Exception)
        {
          throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
            + " contains invalid double value " + StackDump(mappingStack));
        }
      });
    }

    private object MapBoolean(string value, Type valType, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      CheckExpectedType(valType, typeof(bool), mappingStack);
      mappedType = typeof(bool);
      return OnStack("boolean", mappingStack, delegate()
      {
        if (value == "1")
          return true;
        else if (value == "0")
          return false;
        else
          throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
            + " contains invalid boolean value "
            + StackDump(mappingStack));
      });
    }

    private object MapString(string value, Type valType, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      CheckExpectedType(valType, typeof(string), mappingStack);
      if (valType != null && valType.IsEnum)
        return MapStringToEnum(value, valType, "i8", mappingStack, mappingAction, out mappedType);
      mappedType = typeof(string);
      return OnStack("string", mappingStack, delegate()
      { return value; });
    }

    private object MapStringToEnum(string value, Type enumType, string xmlRpcType,
      MappingStack mappingStack, MappingAction mappingAction, out Type mappedType)
    {
      mappedType = enumType;
      return OnStack(xmlRpcType, mappingStack, delegate()
      {
        try
        {
          object ret = Enum.Parse(enumType, value, true);
          return ret;
        }
        catch (XmlRpcInvalidEnumValue)
        {
          throw;
        }
        catch (Exception ex)
        {
          throw new XmlRpcInvalidEnumValue(mappingStack.MappingType
            + " contains invalid or out of range " + xmlRpcType + " value mapped to enum "
            + StackDump(mappingStack));
        }
      });
    }

    private object MapLong(string value, Type valType, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      CheckExpectedType(valType, typeof(long), mappingStack);
      if (valType != null && valType.IsEnum)
        return MapNumberToEnum(value, valType, "i8", mappingStack, mappingAction, out mappedType);
      mappedType = typeof(long);
      return OnStack("i8", mappingStack, delegate()
      {
        long ret;
        if (!Int64.TryParse(value, out ret))
          throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
            + " contains invalid i8 value " + StackDump(mappingStack));
        return ret;
      });
    }

    private object MapInt(string value, Type valType, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      CheckExpectedType(valType, typeof(int), mappingStack);
      if (valType != null && valType.IsEnum)
        return MapNumberToEnum(value, valType, "int", mappingStack, mappingAction, out mappedType);
      mappedType = typeof(int);
      return OnStack("integer", mappingStack, delegate()
      {
        int ret;
        if (!Int32.TryParse(value, out ret))
          throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
            + " contains invalid int value " + StackDump(mappingStack));
        return ret;
      });
    }

    private object MapNumberToEnum(string value, Type enumType, string xmlRpcType,
      MappingStack mappingStack, MappingAction mappingAction, out Type mappedType)
    {
      mappedType = enumType;
      return OnStack(xmlRpcType, mappingStack, delegate()
      {
        try
        {
          long lnum = long.Parse(value);
          Type underlyingType = Enum.GetUnderlyingType(enumType);
          object enumNumberValue = Convert.ChangeType(lnum, underlyingType, null);
          if (!Enum.IsDefined(enumType, enumNumberValue))
            throw new XmlRpcInvalidEnumValue(mappingStack.MappingType
              + " contains " + xmlRpcType + "mapped to undefined enum value " 
              + StackDump(mappingStack));
          object ret = Enum.ToObject(enumType, enumNumberValue);
          return ret;
        }
        catch (XmlRpcInvalidEnumValue)
        {
          throw;
        }
        catch (Exception ex)
        {
          throw new XmlRpcInvalidEnumValue(mappingStack.MappingType
            + " contains invalid or out of range " + xmlRpcType + " value mapped to enum "
            + StackDump(mappingStack));
        }
      });
    }

    private object MapBase64(string value, Type valType, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      CheckExpectedType(valType, typeof(byte[]), mappingStack);
      mappedType = typeof(int);
      return OnStack("base64", mappingStack, delegate()
      {
        if (value == "")
          return new byte[0];
        else
        {
          try
          {
            byte[] ret = Convert.FromBase64String(value);
            return ret;
          }
          catch (Exception)
          {
            throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
              + " contains invalid base64 value "
              + StackDump(mappingStack));
          }
        }
      });
    }

    private object MapNilValue(string p, Type type, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      if (type == null 
        || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        || (!type.IsPrimitive || !type.IsValueType)
        || type == typeof(object))
      {
        mappedType = type;
        return null;
      }
      else
      {
        throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
          + " contains <nil> value which cannot be mapped to type " 
          + (type != null && type != typeof(object) ? type.Name : "object")
          + " "
          + StackDump(mappingStack));
      }
    }

    protected object MapHashtable(IEnumerator<Node> iter, Type valType, MappingStack mappingStack, 
      MappingAction mappingAction, out Type mappedType)
    {
      mappedType = null;
      XmlRpcStruct retObj = new XmlRpcStruct();
      mappingStack.Push("struct mapped to XmlRpcStruct");
      try
      {
        while (iter.MoveNext() && iter.Current is StructMember)
        {
          string rpcName = (iter.Current as StructMember).Value;
          if (retObj.ContainsKey(rpcName)
            && !IgnoreDuplicateMembers)
            throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
              + " contains struct value with duplicate member "
              + rpcName
              + " " + StackDump(mappingStack));
          iter.MoveNext();

          object value = OnStack(String.Format("member {0}", rpcName),
            mappingStack, delegate()
            {
              return MapValueNode(iter, null, mappingStack, mappingAction);
            });
          if (!retObj.ContainsKey(rpcName))
            retObj[rpcName] = value;
        }
      }
      finally
      {
        mappingStack.Pop();
      }
      return retObj;
    }

    private object MapStruct(IEnumerator<Node> iter, Type valueType, MappingStack mappingStack,
      MappingAction mappingAction, out Type mappedType)
    {
      mappedType = null;

      if (valueType.IsPrimitive)
      {
        throw new XmlRpcTypeMismatchException(mappingStack.MappingType
          + " contains struct value where "
          + XmlRpcTypeInfo.GetXmlRpcTypeString(valueType)
          + " expected " + StackDump(mappingStack));
      }
      if (valueType.IsGenericType
        && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        valueType = valueType.GetGenericArguments()[0];
      }
      object retObj;
      try
      {
        retObj = Activator.CreateInstance(valueType);
      }
      catch (Exception)
      {
        throw new XmlRpcTypeMismatchException(mappingStack.MappingType
          + " contains struct value where "
          + XmlRpcTypeInfo.GetXmlRpcTypeString(valueType)
          + " expected (as type " + valueType.Name + ") "
          + StackDump(mappingStack));
      }
      // Note: mapping action on a struct is only applied locally - it 
      // does not override the global mapping action when members of the 
      // struct are mapped
      MappingAction localAction = mappingAction;
      if (valueType != null)
      {
        mappingStack.Push("struct mapped to type " + valueType.Name);
        localAction = StructMappingAction(valueType, mappingAction);
      }
      else
      {
        mappingStack.Push("struct");
      }
      // create map of field names and remove each name from it as 
      // processed so we can determine which fields are missing
      var names = new List<string>();
      CreateFieldNamesMap(valueType, names);
      int fieldCount = 0;
      List<string> rpcNames = new List<string>();
      try
      {
        while (iter.MoveNext())
        {
          if (!(iter.Current is StructMember))
            break;
          string rpcName = (iter.Current as StructMember).Value;
          if (rpcNames.Contains(rpcName))
          {
            if (!IgnoreDuplicateMembers)
              throw new XmlRpcInvalidXmlRpcException(mappingStack.MappingType
                + " contains struct value with duplicate member "
                + rpcName
                + " " + StackDump(mappingStack));
            else
              continue;
          }
          else
            rpcNames.Add(rpcName);

          string name = GetStructName(valueType, rpcName) ?? rpcName;
          MemberInfo mi = valueType.GetField(name);
          if (mi == null) mi = valueType.GetProperty(name);
          if (mi == null)
          {
            iter.MoveNext();  // move to value
            if (iter.Current is ComplexValueNode)
            {
              int depth = iter.Current.Depth;
              while (!(iter.Current is EndComplexValueNode && iter.Current.Depth == depth))
                iter.MoveNext();
            }
            continue;
          }
          if (names.Contains(name))
            names.Remove(name);
          else
          {
            if (Attribute.IsDefined(mi, typeof(NonSerializedAttribute)))
            {
              mappingStack.Push(String.Format("member {0}", name));
              throw new XmlRpcNonSerializedMember("Cannot map XML-RPC struct "
              + "member onto member marked as [NonSerialized]: "
              + " " + StackDump(mappingStack));
            }
          }
          Type memberType = mi.MemberType == MemberTypes.Field
          ? (mi as FieldInfo).FieldType : (mi as PropertyInfo).PropertyType;
          string mappingMsg = valueType == null
              ? String.Format("member {0}", name)
              : String.Format("member {0} mapped to type {1}", name, memberType.Name);

          iter.MoveNext();
          object valObj = OnStack(mappingMsg, mappingStack, delegate()
          {
            return MapValueNode(iter, memberType, mappingStack, mappingAction);
          });

          if (mi.MemberType == MemberTypes.Field)
            (mi as FieldInfo).SetValue(retObj, valObj);
          else
            (mi as PropertyInfo).SetValue(retObj, valObj, null);
          fieldCount++;
        }

        if (localAction == MappingAction.Error && names.Count > 0)
          ReportMissingMembers(valueType, names, mappingStack);
        return retObj;
      }
      finally
      {
        mappingStack.Pop();
      }
    }

    private object MapArray(IEnumerator<Node> iter, Type valType, 
      MappingStack mappingStack, MappingAction mappingAction,
      out Type mappedType)
    {
      mappedType = null;
      // required type must be an array
      if (valType != null
        && !(valType.IsArray == true
            || valType == typeof(Array)
            || valType == typeof(object)))
      {
        throw new XmlRpcTypeMismatchException(mappingStack.MappingType
          + " contains array value where "
          + XmlRpcTypeInfo.GetXmlRpcTypeString(valType)
          + " expected " + StackDump(mappingStack));
      }
      if (valType != null)
      {
        XmlRpcType xmlRpcType = XmlRpcTypeInfo.GetXmlRpcType(valType);
        if (xmlRpcType == XmlRpcType.tMultiDimArray)
        {
          mappingStack.Push("array mapped to type " + valType.Name);
          Object ret = MapMultiDimArray(iter, valType, mappingStack,
            mappingAction);
          return ret;
        }
        mappingStack.Push("array mapped to type " + valType.Name);
      }
      else
        mappingStack.Push("array");

      var values = new List<object>();
      Type elemType = DetermineArrayItemType(valType);

      bool bGotType = false;
      Type useType = null;

      while (iter.MoveNext() && iter.Current is ValueNode)
      {
        mappingStack.Push(String.Format("element {0}", values.Count));
        object value = MapValueNode(iter, elemType, mappingStack, mappingAction);
        values.Add(value);
        mappingStack.Pop();
      }

      foreach (object value in values)
      {
        if (value == null)
          continue;
        if (bGotType == false)
        {
          useType = value.GetType();
          bGotType = true;
        }
        else
        {
          if (useType != value.GetType())
            useType = null;
        }
      }

      Object[] args = new Object[1];
      args[0] = values.Count;
      Object retObj = null;
      if (valType != null
        && valType != typeof(Array)
        && valType != typeof(object))
      {
        retObj = CreateArrayInstance(valType, args);
      }
      else
      {
        if (useType == null)
          retObj = CreateArrayInstance(typeof(object[]), args);
        else
          retObj = Array.CreateInstance(useType, (int)args[0]); ;
      }
      for (int j = 0; j < values.Count; j++)
      {
        ((Array)retObj).SetValue(values[j], j);
      }

      mappingStack.Pop();

      return retObj;
    }

    private static Type DetermineArrayItemType(Type valType)
    {
      Type elemType = null;
      if (valType != null
        && valType != typeof(Array)
        && valType != typeof(object))
      {
        elemType = valType.GetElementType();
      }
      else
      {
        elemType = typeof(object);
      }
      return elemType;
    }


    private void CheckImplictString(Type valType, MappingStack mappingStack)
    {
      if (valType != null && valType != typeof(string) && !valType.IsEnum)
      {
        throw new XmlRpcTypeMismatchException(mappingStack.MappingType
          + " contains implicit string value where "
          + XmlRpcTypeInfo.GetXmlRpcTypeString(valType)
          + " expected " + StackDump(mappingStack));
      }
    }

    Object MapMultiDimArray(IEnumerator<Node> iter, Type ValueType,
      MappingStack mappingStack, MappingAction mappingAction)
    {
      // parse the type name to get element type and array rank
#if (!COMPACT_FRAMEWORK)
      Type elemType = ValueType.GetElementType();
      int rank = ValueType.GetArrayRank();
#else
      string[] checkMultiDim = Regex.Split(ValueType.FullName, 
        "\\[,[,]*\\]$");
      Type elemType = Type.GetType(checkMultiDim[0]);
      string commas = ValueType.FullName.Substring(checkMultiDim[0].Length+1, 
        ValueType.FullName.Length-checkMultiDim[0].Length-2);
      int rank = commas.Length+1;
#endif
      // elements will be stored sequentially as nested arrays are mapped
      var elements = new List<object>();
      // create array to store length of each dimension - initialize to 
      // all zeroes so that when parsing we can determine if an array for 
      // that dimension has been mapped already
      int[] dimLengths = new int[rank];
      dimLengths.Initialize();
      MapMultiDimElements(iter, rank, 0, elemType, elements, dimLengths,
        mappingStack, mappingAction);
      // build arguments to define array dimensions and create the array
      Object[] args = new Object[dimLengths.Length];
      for (int argi = 0; argi < dimLengths.Length; argi++)
      {
        args[argi] = dimLengths[argi];
      }
      Array ret = (Array)CreateArrayInstance(ValueType, args);
      // copy elements into new multi-dim array
      //!! make more efficient
      int length = ret.Length;
      for (int e = 0; e < length; e++)
      {
        int[] indices = new int[dimLengths.Length];
        int div = 1;
        for (int f = (indices.Length - 1); f >= 0; f--)
        {
          indices[f] = (e / div) % dimLengths[f];
          div *= dimLengths[f];
        }
        ret.SetValue(elements[e], indices);
      }
      return ret;
    }

    void MapMultiDimElements(IEnumerator<Node> iter, int Rank, int CurRank,
      Type elemType, List<object> elements, int[] dimLengths,
      MappingStack mappingStack, MappingAction mappingAction)
    {
      //XmlNode dataNode = SelectSingleNode(node, "data");
      //XmlNode[] childNodes = SelectNodes(dataNode, "value");
      //int nodeCount = childNodes.Length;
      ////!! check that multi dim array is not jagged
      //if (dimLengths[CurRank] != 0 && nodeCount != dimLengths[CurRank])
      //{
      //  throw new XmlRpcNonRegularArrayException(
      //    "Multi-dimensional array must not be jagged.");
      //}
      //dimLengths[CurRank] = nodeCount;  // in case first array at this rank
      int nodeCount = 0;
      if (CurRank < (Rank - 1))
      {
        while (iter.MoveNext() && iter.Current is ArrayValue)
        {
          nodeCount++;
          MapMultiDimElements(iter, Rank, CurRank + 1, elemType,
            elements, dimLengths, mappingStack, mappingAction);
        }
      }
      else
      {
        while (iter.MoveNext() && iter.Current is ValueNode)
        {
          nodeCount++;
          object value = MapValueNode(iter, elemType, mappingStack, mappingAction);
          elements.Add(value);
        }
      }
      dimLengths[CurRank] = nodeCount;
    }

    public Object ParseValueElement(
      XmlReader rdr,
      Type valType,
      MappingStack mappingStack,
      MappingAction mappingAction)
    {
      var iter = new XmlRpcParser().ParseValue(rdr).GetEnumerator();
      iter.MoveNext();

      return MapValueNode(iter, valType, mappingStack, mappingAction);
    }

    private static void CreateFieldNamesMap(Type valueType, List<string> names)
    {
      foreach (FieldInfo fi in valueType.GetFields())
      {
        if (Attribute.IsDefined(fi, typeof(NonSerializedAttribute)))
          continue;
        names.Add(fi.Name);
      }
      foreach (PropertyInfo pi in valueType.GetProperties())
      {
        if (Attribute.IsDefined(pi, typeof(NonSerializedAttribute)))
          continue;
        names.Add(pi.Name);
      }
    }

    private void CheckExpectedType(Type expectedType, Type actualType, MappingStack mappingStack)
    {
      if (expectedType != null && expectedType.IsEnum)
      {
        Type[] i4Types = new Type[] { typeof(byte), typeof(sbyte), typeof(short), 
          typeof(ushort), typeof(int) };
        Type[] i8Types = new Type[] { typeof(uint), typeof(long) };
        Type underlyingType = Enum.GetUnderlyingType(expectedType);
        if (Array.IndexOf(i4Types, underlyingType) >= 0)
          expectedType = typeof(Int32);
        else if (Array.IndexOf(i8Types, underlyingType) >= 0)
          expectedType = typeof(long);
        else
          throw new XmlRpcInvalidEnumValue(mappingStack.MappingType +
          " contains "
          + XmlRpcTypeInfo.GetXmlRpcTypeString(actualType)
          + " which cannot be mapped to  "
          + XmlRpcTypeInfo.GetXmlRpcTypeString(expectedType)
          + " " + StackDump(mappingStack));
      }
      // TODO: throw exception for invalid enum type
      if (expectedType != null && expectedType != typeof(Object)
        && expectedType != actualType
        && (actualType.IsValueType
          && expectedType != typeof(Nullable<>).MakeGenericType(actualType)))
      {
        throw new XmlRpcTypeMismatchException(mappingStack.MappingType +
          " contains "
          + XmlRpcTypeInfo.GetXmlRpcTypeString(actualType)
          + " value where "
          + XmlRpcTypeInfo.GetXmlRpcTypeString(expectedType)
          + " expected " + StackDump(mappingStack));
      }
    }

    delegate T Func<T>();

    private T OnStack<T>(string p, MappingStack mappingStack, Func<T> func)
    {
      mappingStack.Push(p);
      try
      {
        return func();
      }
      finally
      {
        mappingStack.Pop();
      }
    }

    void ReportMissingMembers(
      Type valueType,
      List<string> names,
      MappingStack mappingStack)
    {
      StringBuilder sb = new StringBuilder();
      int errorCount = 0;
      string sep = "";
      foreach (string s in names)
      {
        MappingAction memberAction = MemberMappingAction(valueType, s,
          MappingAction.Error);
        if (memberAction == MappingAction.Error)
        {
          sb.Append(sep);
          sb.Append(s);
          sep = " ";
          errorCount++;
        }
      }
      if (errorCount > 0)
      {
        string plural = "";
        if (errorCount > 1)
          plural = "s";
        throw new XmlRpcTypeMismatchException(mappingStack.MappingType
          + " contains struct value with missing non-optional member"
          + plural + ": " + sb.ToString() + " " + StackDump(mappingStack));
      }
    }

    string GetStructName(Type ValueType, string XmlRpcName)
    {
      // given a member name in an XML-RPC struct, check to see whether
      // a field has been associated with this XML-RPC member name, return
      // the field name if it has else return null
      if (ValueType == null)
        return null;
      foreach (FieldInfo fi in ValueType.GetFields())
      {
        Attribute attr = Attribute.GetCustomAttribute(fi,
          typeof(XmlRpcMemberAttribute));
        if (attr != null
          && attr is XmlRpcMemberAttribute
          && ((XmlRpcMemberAttribute)attr).Member == XmlRpcName)
        {
          string ret = fi.Name;
          return ret;
        }
      }
      foreach (PropertyInfo pi in ValueType.GetProperties())
      {
        Attribute attr = Attribute.GetCustomAttribute(pi,
          typeof(XmlRpcMemberAttribute));
        if (attr != null
          && attr is XmlRpcMemberAttribute
          && ((XmlRpcMemberAttribute)attr).Member == XmlRpcName)
        {
          string ret = pi.Name;
          return ret;
        }
      }
      return null;
    }

    MappingAction StructMappingAction(
      Type type,
      MappingAction currentAction)
    {
      // if struct member has mapping action attribute, override the current
      // mapping action else just return the current action
      if (type == null)
        return currentAction;
      Attribute attr = Attribute.GetCustomAttribute(type,
        typeof(XmlRpcMissingMappingAttribute));
      if (attr != null)
        return ((XmlRpcMissingMappingAttribute)attr).Action;
      return currentAction;
    }

    MappingAction MemberMappingAction(
      Type type,
      string memberName,
      MappingAction currentAction)
    {
      // if struct member has mapping action attribute, override the current
      // mapping action else just return the current action
      if (type == null)
        return currentAction;
      Attribute attr = null;
      FieldInfo fi = type.GetField(memberName);
      if (fi != null)
        attr = Attribute.GetCustomAttribute(fi,
          typeof(XmlRpcMissingMappingAttribute));
      else
      {
        PropertyInfo pi = type.GetProperty(memberName);
        attr = Attribute.GetCustomAttribute(pi,
          typeof(XmlRpcMissingMappingAttribute));
      }
      if (attr != null)
        return ((XmlRpcMissingMappingAttribute)attr).Action;
      return currentAction;
    }

    string StackDump(MappingStack mappingStack)
    {
      StringBuilder sb = new StringBuilder();
      foreach (string elem in mappingStack)
      {
        sb.Insert(0, elem);
        sb.Insert(0, " : ");
      }
      sb.Insert(0, mappingStack.MappingType);
      sb.Insert(0, "[");
      sb.Append("]");
      return sb.ToString();
    }

    // TODO: following to return Array?
    object CreateArrayInstance(Type type, object[] args)
    {
#if (!COMPACT_FRAMEWORK)
      return Activator.CreateInstance(type, args);
#else
    Object Arr = Array.CreateInstance(type.GetElementType(), (int)args[0]);
    return Arr;
#endif
    }

    bool IsStructParamsMethod(MethodInfo mi)
    {
      if (mi == null)
        return false;
      bool ret = false;
      Attribute attr = Attribute.GetCustomAttribute(mi,
        typeof(XmlRpcMethodAttribute));
      if (attr != null)
      {
        XmlRpcMethodAttribute mattr = (XmlRpcMethodAttribute)attr;
        ret = mattr.StructParams;
      }
      return ret;
    }
  }
}


