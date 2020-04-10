/*
 * XMLDocument
 * 
 * Copyright 2010 OPS,All rights reserved!
 * date :   2010/11/22
 */

using System;
using System.Xml;

namespace JR.Stand.Core.Framework.Xml
{
    [Obsolete]
    public class XmlDocHelper
    {
        public static object ReadAttributeValue(string xmlPath, string xpath)
        {
            XmlDocument x = new XmlDocument();
            x.Load(xmlPath);
            XmlNode xn = x.SelectSingleNode(xpath);
            return xn.Value;
        }
    }
}