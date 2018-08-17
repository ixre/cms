/* 
XML-RPC.NET library
Copyright (c) 2001-2009, Charles Cook <charlescook@cookcomputing.com>

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CookComputing.XmlRpc
{
  public class XmlRpcStruct : IDictionary, ICollection, IEnumerable 
#if (!SILVERLIGHT)
    , ISerializable, IDeserializationCallback, ICloneable
#endif
  {
    private List<string> _keys = new List<string>();
    private List<object> _values = new List<object>();
    private Dictionary<string, object> _base = new Dictionary<string, object>();
    private object _syncRoot = new object();

    public void Add(object key, object value)
    {
      if (!(key is string))
      {
        throw new ArgumentException("XmlRpcStruct key must be a string.");
      }
      //if (XmlRpcServiceInfo.GetXmlRpcType(value.GetType())
      //    == XmlRpcType.tInvalid)
      //{
      //  throw new ArgumentException(String.Format(
      //    "Type {0} cannot be mapped to an XML-RPC type", value.GetType()));
      //}
      _base.Add(key as string, value);
      _keys.Add(key as string);
      _values.Add(value);
    }

    public void Clear()
    {
      _base.Clear();
      _keys.Clear();
      _values.Clear();
    }

    public bool Contains(object key)
    {
      return _base.ContainsKey(key as string);
    }

    public bool ContainsKey(object key)
    {
      return _base.ContainsKey(key as string);
    }

    public bool ContainsValue(object value)
    {
      return _base.ContainsValue(value as string);
    }

    public IDictionaryEnumerator GetEnumerator()
    {
      return new XmlRpcStruct.Enumerator(_keys, _values);
    }

    public bool IsFixedSize
    {
      get { return false; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public ICollection Keys
    {
      get { return _keys; }
    }

    public void Remove(object key)
    {
      _base.Remove(key as string);
      int idx = _keys.IndexOf(key as string);
      if (idx >= 0)
      {
        _keys.RemoveAt(idx);
        _values.RemoveAt(idx);
      }
    }

    public ICollection Values
    {
      get { return _values; }
    }

    public object this[object key]
    {
      get
      {
        return _base[key as string];
      }
      set
      {
        if (!(key is string))
        {
          throw new ArgumentException("XmlRpcStruct key must be a string.");
        }
        //if (XmlRpcServiceInfo.GetXmlRpcType(value.GetType())
        //    == XmlRpcType.tInvalid)
        //{
        //  throw new ArgumentException(String.Format(
        //    "Type {0} cannot be mapped to an XML-RPC type", value.GetType()));
        //}
        _base[key as string] = value;
        _keys.Add(key as string);
        _values.Add(value);
      }
    }

    public void CopyTo(Array array, int index)
    {
      throw new NotImplementedException(); // TODO: implement
    }

    public int Count
    {
      get { return _base.Count; }
    }

    public bool IsSynchronized
    {
      get { return false; }
    }

    public object SyncRoot
    {
      get { return _syncRoot; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return new XmlRpcStruct.Enumerator(_keys, _values);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      throw new NotImplementedException(); // TODO: implement
    }

    int ICollection.Count
    {
      get { return _base.Count; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    object ICollection.SyncRoot
    {
      get { return _syncRoot; }
    }

#if (!SILVERLIGHT)
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException(); // TODO: implement
    }

    public void OnDeserialization(object sender)
    {
      throw new NotImplementedException(); // TODO: implement
    }
#endif

    public object Clone()
    {
      throw new NotImplementedException(); // TODO: implement
    }

    private class Enumerator : IDictionaryEnumerator
    {
      private List<string> _keys;
      private List<object> _values;
      private int _index;

      public Enumerator(List<string> keys, List<object> values)
      {
        _keys = keys;
        _values = values;
        _index = -1;
      }

      public void Reset()
      {
        _index = -1;
      }

      public object Current
      {
        get
        {
          CheckIndex();
          return new DictionaryEntry(_keys[_index], _values[_index]);
        }
      }

      public bool MoveNext()
      {
        _index++;
        if (_index >= _keys.Count)
          return false;
        else
          return true;
      }

      public DictionaryEntry Entry
      {
        get
        {
          CheckIndex();
          return new DictionaryEntry(_keys[_index], _values[_index]);
        }
      }

      public object Key
      {
        get
        {
          CheckIndex();
          return _keys[_index];
        }
      }

      public object Value
      {
        get
        {
          CheckIndex();
          return _values[_index];
        }
      }

      private void CheckIndex()
      {
        if (_index < 0 || _index >= _keys.Count)
          throw new InvalidOperationException(
            "Enumeration has either not started or has already finished.");
      }
    }
  }

  public class XmlRpcStructs : Dictionary<object, object>
  {
    //private ArrayList _keys = new ArrayList();
    //private ArrayList _values = new ArrayList();

    //public override void Add(object key, object value)
    //{
    //  if (!(key is string))
    //  {
    //    throw new ArgumentException("XmlRpcStruct key must be a string.");
    //  }
    //  if (XmlRpcServiceInfo.GetXmlRpcType(value.GetType())
    //      == XmlRpcType.tInvalid)
    //  {
    //    throw new ArgumentException(String.Format(
    //      "Type {0} cannot be mapped to an XML-RPC type", value.GetType()));
    //  }
    //  base.Add(key, value);
    //  _keys.Add(key);
    //  _values.Add(value);
    //}

    //public override object this[object key]
    //{
    //  get
    //  {
    //    return base[key];
    //  }
    //  set
    //  {
    //    if (!(key is string))
    //    {
    //      throw new ArgumentException("XmlRpcStruct key must be a string.");
    //    }
    //    if (XmlRpcServiceInfo.GetXmlRpcType(value.GetType())
    //        == XmlRpcType.tInvalid)
    //    {
    //      throw new ArgumentException(String.Format(
    //        "Type {0} cannot be mapped to an XML-RPC type", value.GetType()));
    //    }
    //    base[key] = value;
    //    _keys.Add(key);
    //    _values.Add(value);
    //  }
    //}

    //public override bool Equals(Object obj)
    //{
    //  if (obj.GetType() != typeof(XmlRpcStruct))
    //    return false;
    //  XmlRpcStruct xmlRpcStruct = (XmlRpcStruct)obj;
    //  if (this.Keys.Count != xmlRpcStruct.Count)
    //    return false;
    //  foreach (String key in this.Keys)
    //  {
    //    if (!xmlRpcStruct.ContainsKey(key))
    //      return false;
    //    if (!this[key].Equals(xmlRpcStruct[key]))
    //      return false;
    //  }
    //  return true;
    //}

    //public override int GetHashCode()
    //{
    //  int hash = 0;
    //  foreach (object obj in Values)
    //  {
    //    hash ^= obj.GetHashCode();
    //  }
    //  return hash;
    //}

    //public override void Clear()
    //{
    //  base.Clear();
    //  _keys.Clear();
    //  _values.Clear();
    //}

    //public new IDictionaryEnumerator GetEnumerator()
    //{
    //  return new XmlRpcStruct.Enumerator(_keys, _values);
    //}

    //public override ICollection Keys
    //{
    //  get
    //  {
    //    return _keys;
    //  }
    //}

    //public override void Remove(object key)
    //{
    //  base.Remove(key);
    //  int idx = _keys.IndexOf(key);
    //  if (idx >= 0)
    //  {
    //    _keys.RemoveAt(idx);
    //    _values.RemoveAt(idx);
    //  }
    //}

    //public override ICollection Values
    //{
    //  get
    //  {
    //    return _values;
    //  }
    //}

    //private class Enumerator : IDictionaryEnumerator
    //{
    //  private ArrayList _keys;
    //  private ArrayList _values;
    //  private int _index;

    //  public Enumerator(ArrayList keys, ArrayList values)
    //  {
    //    _keys = keys;
    //    _values = values;
    //    _index = -1;
    //  }

    //  public void Reset()
    //  {
    //    _index = -1;
    //  }

    //  public object Current
    //  {
    //    get
    //    {
    //      CheckIndex();
    //      return new DictionaryEntry(_keys[_index], _values[_index]);
    //    }
    //  }

    //  public bool MoveNext()
    //  {
    //    _index++;
    //    if (_index >= _keys.Count)
    //      return false;
    //    else
    //      return true;
    //  }

    //  public DictionaryEntry Entry
    //  {
    //    get
    //    {
    //      CheckIndex();
    //      return new DictionaryEntry(_keys[_index], _values[_index]);
    //    }
    //  }

    //  public object Key
    //  {
    //    get
    //    {
    //      CheckIndex();
    //      return _keys[_index];
    //    }
    //  }

    //  public object Value
    //  {
    //    get
    //    {
    //      CheckIndex();
    //      return _values[_index];
    //    }
    //  }

    //  private void CheckIndex()
    //  {
    //    if (_index < 0 || _index >= _keys.Count)
    //      throw new InvalidOperationException(
    //        "Enumeration has either not started or has already finished.");
    //  }
    //}
  }
}