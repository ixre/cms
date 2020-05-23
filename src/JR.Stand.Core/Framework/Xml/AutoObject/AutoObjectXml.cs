using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace JR.Stand.Core.Framework.Xml.AutoObject
{
    /// <summary>
    /// 对象属性
    /// </summary>
    public struct XmlObjectProperty
    {
        private string key;
        private string name;
        private string descript;

        /// <summary>
        /// 属性键
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// 属性对应的名称
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        public string Descript
        {
            get { return descript; }
        }

        public XmlObjectProperty(string key, string name)
        {
            this.key = key;
            this.name = name;
            this.descript = "";
        }

        public XmlObjectProperty(string key, string name, string descript)
        {
            this.key = key;
            this.name = name;
            this.descript = descript;
        }
    }

    /// <summary>
    /// XmlObject特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class XmlObjectAttribute : Attribute
    {
        public string Name { get; set; }
        public string Descript { get; set; }

        public XmlObjectAttribute(string name, string description)
        {
            this.Name = name;
            this.Descript = description;
        }
    }


    /// <summary>
    /// XmlObject
    /// </summary>
    public class XmlObject
    {
        private string key;
        private string name;
        private string descript;

        /// <summary>
        /// 属性键
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// 属性对应的名称
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 描述/值
        /// </summary>
        public string Descript
        {
            get { return descript; }
        }

        public XmlObject(string key, string name, string descript)
        {
            this.key = key;
            this.name = name;
            this.descript = descript;
        }

        public XmlObjectProperty[] Properties { get; set; }

        /// <summary>
        /// 转换为Json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{'key':'")
                .Append(this.key.Replace("'", "\\'"))
                .Append("','name':'")
                .Append((this.name ?? "").Replace("'", "\\'"))
                .Append("','descript':'")
                .Append((this.descript ?? "").Replace("'", "\\'"))
                .Append("','properties':");

            //添加属性
            if (this.Properties == null || this.Properties.Length == 0)
            {
                sb.Append("[]");
            }
            else
            {
                int i = 0;

                sb.Append("[");

                foreach (XmlObjectProperty pro in this.Properties)
                {
                    if (i++ != 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append("{'key':'")
                        .Append(pro.Key.Replace("'", "\\'"))
                        .Append("','name':'")
                        .Append((pro.Name ?? "").Replace("'", "\\'"))
                        .Append("','descript':'")
                        .Append((pro.Descript ?? "").Replace("'", "\\'"))
                        .Append("'}");
                }

                sb.Append("]");
            }

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// 将对象列表转换为json
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static string ToJson(IEnumerable<XmlObject> objects)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;

            sb.Append("[");

            foreach (XmlObject obj in objects)
            {
                if (i++ != 0)
                {
                    sb.Append(",");
                }
                sb.Append(obj.ToJson());
            }

            sb.Append("]");

            return sb.ToString();
        }
    }


    /// <summary>
    /// XmlObject特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class XmlObjectPropertyAttribute : Attribute
    {
        public string Name { get; set; }
        public string Descript { get; set; }

        public XmlObjectPropertyAttribute(string name)
        {
            this.Name = name;
        }

        public XmlObjectPropertyAttribute(string name, string descript)
        {
            this.Name = name;
            this.Descript = descript;
        }
    }

    public class AutoObjectXml
    {
        private string filePath;
        private XmlDocument xd;
        private XmlNode rootNode;

        public AutoObjectXml(string filePath)
        {
            this.filePath = filePath;

            if (!File.Exists(this.filePath))
            {
                File.Create(filePath).Dispose();
                const string initData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<objects>\r\n</objects>";

                byte[] data = Encoding.UTF8.GetBytes(initData);
                FileStream fs = new FileStream(this.filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
            }

            using (TextReader tr = new StreamReader(this.filePath))
            {
                xd = new XmlDocument();
                //this.xmlContent = tr.ReadToEnd();
                xd.LoadXml(tr.ReadToEnd());

                XmlNode xnode = xd.SelectSingleNode("//objects");
                if (xnode == null)
                {
                    throw new NotSupportedException("XML文件中不包含objects节点");
                }

                this.rootNode = xnode;
                tr.Dispose();
            }
        }


        /// <summary>
        /// 获取Objects根节点
        /// </summary>
        /// <returns></returns>
        private XmlNode __GetObjectsNode()
        {
            XmlNode xnode = xd.SelectSingleNode("//objects");
            if (xnode == null)
            {
                throw new NotSupportedException("XML文件中不包含objects节点");
            }
            return xnode;
        }

        /// <summary>
        /// 插入对象到objects节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="describe"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public bool InsertObjectNode(string key, string name, string descript, params XmlObjectProperty[] properties)
        {
            if (rootNode.SelectSingleNode(String.Format("object[@key='{0}']", key)) != null) return false; //已经存在属性
            XmlNode tempNode;
            XmlNode node = xd.CreateElement("object");
            XmlAttribute attr = xd.CreateAttribute("key");

            //add Key
            attr.Value = key;
            node.Attributes.Append(attr);

            //add Name
            attr = xd.CreateAttribute("name");
            attr.Value = name;
            node.Attributes.Append(attr);

            //add Descript
            tempNode = xd.CreateElement("describe");
            XmlCDataSection cdd = xd.CreateCDataSection(descript);
            tempNode.AppendChild(cdd);
            node.AppendChild(tempNode);

            XmlNode attrNode = xd.CreateElement("properties");

            foreach (XmlObjectProperty obj in properties)
            {
                //add Property
                tempNode = xd.CreateElement("property");
                attr = xd.CreateAttribute("key");
                attr.Value = obj.Key;
                tempNode.Attributes.Append(attr);

                attr = xd.CreateAttribute("name");
                attr.Value = obj.Name;
                tempNode.Attributes.Append(attr);

                if (!String.IsNullOrEmpty(obj.Descript))
                {
                    cdd = xd.CreateCDataSection(obj.Descript);
                    tempNode.AppendChild(cdd);
                }

                attrNode.AppendChild(tempNode);
            }
            node.AppendChild(attrNode);

            //add object
            rootNode.AppendChild(node);


            return true;
        }

        /// <summary>
        /// 从dll读取并插入，对象需指定XmlObject特性，属性可选XmlObjectProperty特性
        /// </summary>
        /// <param name="dllPath"></param>
        /// <param name="checkNameSpace"></param>
        public void InsertFromDLL(string dllPath, bool includeNoProperyAttribute, string checkNameSpace)
        {
            if (!File.Exists(dllPath)) throw new FileNotFoundException();
            Assembly ass = Assembly.LoadFile(dllPath);
            bool ckNs = checkNameSpace != null; //是否从指定的命名空间加载
            Type[] types = ass.GetTypes();
            foreach (Type t in types)
            {
                if (ckNs && !t.Namespace.StartsWith(checkNameSpace))
                {
                    continue;
                }

                this.InsertFromType(t, includeNoProperyAttribute);
            }
        }

        /// <summary>
        /// 从dll读取并插入，对象需指定XmlObject特性，属性必须XmlObjectProperty特性
        /// </summary>
        /// <param name="dllPath"></param>
        /// <param name="checkNameSpace"></param>
        public void InsertFromDLL(string dllPath, string checkNameSpace)
        {
            this.InsertFromDLL(dllPath, false, checkNameSpace);
        }

        /// <summary>
        /// 从dll读取并插入，对象需指定XmlObject特性，属性必须XmlObjectProperty特性
        /// </summary>
        /// <param name="dllPath"></param>
        /// <param name="checkNameSpace"></param>
        public void InsertFromDLL(string dllPath, bool includeNoProperyAttribute)
        {
            this.InsertFromDLL(dllPath, includeNoProperyAttribute, null);
        }


        private delegate void MemberXmlObjectHandler(MemberInfo member);

        public void InsertFromType(Type type, bool includeNoProperyAttribute)
        {
            Type t = type;
            //加载类型
            object[] objs = t.GetCustomAttributes(typeof (XmlObjectAttribute), false);
            if (objs.Length == 0) return;

            XmlObjectAttribute xa = (XmlObjectAttribute) objs[0];
            XmlObjectPropertyAttribute xpa = null;
            IList<XmlObjectProperty> list = new List<XmlObjectProperty>();
            object[] objAtt;

            MemberXmlObjectHandler handler = (p) =>
            {
                objAtt = p.GetCustomAttributes(typeof (XmlObjectPropertyAttribute), true);
                xpa = objAtt.Length == 0 ? null : (XmlObjectPropertyAttribute) objAtt[0];

                if (xpa != null || includeNoProperyAttribute)
                {
                    list.Add(new XmlObjectProperty(
                        p.Name,
                        xpa == null || xpa.Name == null ? p.Name : xpa.Name,
                        xpa == null || xpa.Name == null ? "" : xpa.Descript)
                        );
                }
            };


            foreach (PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                handler(p);
            }

            foreach (MethodInfo p in t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                handler(p);
            }

            this.InsertObjectNode(t.Name, xa.Name ?? t.Name, xa.Descript ?? "", list.ToArray());
        }

        /// <summary>
        /// 删除指定前缀的对象
        /// </summary>
        /// <param name="prefix"></param>
        public void RemoveObjects(string prefix)
        {
            XmlNodeList list = this.rootNode.SelectNodes("descendant::object");
            foreach (XmlNode x in list)
            {
                if (x.Attributes["key"].Value.StartsWith(prefix))
                {
                    this.rootNode.RemoveChild(x);
                }
            }
        }

        /// <summary>
        /// 删除所有对象
        /// </summary>
        public void RemoveAllObjects()
        {
            XmlNodeList list = this.rootNode.SelectNodes("descendant::object");
            foreach (XmlNode x in list)
            {
                this.rootNode.RemoveChild(x);
            }
        }

        public XmlObject GetObject(string objectKey)
        {
            XmlNode node = this.rootNode.SelectSingleNode(String.Format("object[@key='{0}']", objectKey));
            if (node == null) return null;

            IList<XmlObjectProperty> prolist = new List<XmlObjectProperty>();
            XmlObject obj = new XmlObject(node.Attributes["key"].Value, node.Attributes["name"].Value,
                node.SelectSingleNode("describe").InnerText);
            XmlNodeList proNodes = node.SelectNodes("properties/property");
            foreach (XmlNode n in proNodes)
            {
                prolist.Add(new XmlObjectProperty(
                    n.Attributes["key"].Value,
                    n.Attributes["name"].Value,
                    n.InnerText));
            }

            obj.Properties = prolist.ToArray();

            return obj;
        }

        /// <summary>
        /// 获取所有对象
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XmlObject> GetObjects()
        {
            IList<XmlObjectProperty> prolist = new List<XmlObjectProperty>();

            XmlNodeList nodes = this.rootNode.SelectNodes("object");
            XmlObject obj;
            XmlNodeList proNodes;
            foreach (XmlNode node in nodes)
            {
                obj = new XmlObject(node.Attributes["key"].Value, node.Attributes["name"].Value,
                    node.SelectSingleNode("describe").InnerText);
                proNodes = node.SelectNodes("properties/property");

                prolist.Clear();
                foreach (XmlNode n in proNodes)
                {
                    prolist.Add(new XmlObjectProperty(
                        n.Attributes["key"].Value,
                        n.Attributes["name"].Value,
                        n.InnerText));
                }
                obj.Properties = prolist.ToArray();

                yield return obj;
            }
        }

        /// <summary>
        /// 保存到文件中
        /// </summary>
        public void Flush()
        {
            /*
            using (TextWriter tr = new StreamWriter(this.filePath))
            {
                tr.Write(xmlContent);
                tr.Flush();
                tr.Dispose();
            }*/

            xd.Save(this.filePath);
        }
    }
}