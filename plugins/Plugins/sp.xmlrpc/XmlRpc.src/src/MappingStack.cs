using System;
using System.Collections.Generic;
using System.Text;

namespace CookComputing.XmlRpc
{
    public class MappingStack : Stack<string>
    {
      public MappingStack(string parseType)
      {
        m_parseType = parseType;
      }


      public string MappingType
      {
        get { return m_parseType; }
      }

      public string m_parseType = "";
    }
  }
