using System.Collections.Generic;

namespace JR.Cms.Infrastructure.Tree
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

        public TreeNode(string text, string value, string uri, bool open, string icon)
        {
            _value = value;
            _text = text;
            _uri = uri;
            _open = open;
            _icon = icon;
            _childs = new List<TreeNode>();
        }

        public string value => _value;
        public string text => _text;
        public string uri => _uri;
        public string icon => _icon;
        public bool open => _open;
        public IList<TreeNode> childs => _childs;
    }
}