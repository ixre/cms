/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/19
 * Time: 14:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace JR.Cms.Core
{
    /// <summary>
    /// Description of CmsException.
    /// </summary>
    [Serializable]
    public class CmsException : Exception
    {
        private Exception _baseException;
        private string _message;

        public CmsException()
        {
        }

        public CmsException(string message, Exception innerException) : base(message)
        {
            _baseException = innerException;
        }

        public CmsException(string message)
        {
            _message = message;
        }

        protected virtual string GetMessage(string message)
        {
            return message;
        }

        public override string Message => GetMessage(Message ?? base.Message);

        public override Exception GetBaseException()
        {
            return _baseException ?? base.GetBaseException();
        }

        public override string HelpLink
        {
            get => base.HelpLink;
            set => base.HelpLink = value;
        }
    }

    public class CmsApplicationException : CmsException
    {


        public CmsApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CmsApplicationException(string message) : base(message)
        {
        }

        protected override string GetMessage(string message)
        {
            return base.GetMessage("应用程序异常!信息：" + message);
        }
    }
}