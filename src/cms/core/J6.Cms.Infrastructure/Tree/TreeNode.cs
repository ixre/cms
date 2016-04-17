using System.Collections.Generic;

namespace J6.Cms.Infrastructure.Tree
{
    /// <summary>
    /// 树节点
    /// </summary>
    public struct TreeNode
    {
        private string _value;
        private string _text;
        private string _uri;
        private string _icon;

        private IList<TreeNode> _childs;
        private bool _open;
        public TreeNode(string text,string value,string uri,bool open,string icon)
        {
            this._value = value;
            this._text = text;
            this._uri = uri;
            this._open = open;
            this._icon = icon;
            this._childs = new List<TreeNode>();
        }
        public string value { get { return this._value; } }
        public string text { get { return this._text; } }
        public string uri { get { return this._uri; } }
        public string icon { get { return this._icon; } }
        public bool open { get { return this._open; } }
        public IList<TreeNode> childs { get { return this._childs; } }
    }
}
