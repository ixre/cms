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

using System;
using System.Collections.Generic;
using System.Text;

namespace CookComputing.XmlRpc
{
  public class XmlRpcFormatSettings
  {
    public int Indentation
    {
      get { return m_indentation; }
      set { m_indentation = value; }
    }
    int m_indentation = 2;

    public bool UseEmptyElementTags
    {
      get { return m_bUseEmptyElementTag; }
      set { m_bUseEmptyElementTag = value; }
    }
    bool m_bUseEmptyElementTag = true;

    public bool UseEmptyParamsTag
    {
      get { return m_bUseEmptyParamsTag; }
      set { m_bUseEmptyParamsTag = value; }
    }
    bool m_bUseEmptyParamsTag = true;

    public bool UseIndentation
    {
      get { return m_bUseIndentation; }
      set { m_bUseIndentation = value; }
    }
    bool m_bUseIndentation = true;

    public bool UseIntTag
    {
      get { return m_useIntTag; }
      set { m_useIntTag = value; }
    }
    bool m_useIntTag;

    public bool UseStringTag
    {
      get { return m_useStringTag; }
      set { m_useStringTag = value; }
    }
    bool m_useStringTag = true;

    public Encoding XmlEncoding
    {
      get { return m_encoding; }
      set { m_encoding = value; }
    }
    Encoding m_encoding = null;

    public bool OmitXmlDeclaration { get; set; }
  }
}
