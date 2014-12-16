/*
 * Created by SharpDevelop.
 * User: newmin
 * Date: 2014/1/19
 * Time: 14:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Runtime.Serialization;

namespace Ops.Cms.Core
{
	/// <summary>
	/// Description of CmsException.
	/// </summary>
    [Serializable]
	public class CmsException:Exception
	{
		private Exception _baseException;
		private string _message;

        public CmsException()
        {
        }

		public CmsException(string message,Exception innerException):base(message)
		{
			this._baseException=innerException;
		}
		
		public CmsException(string message)
		{
			this._message=message;
		}
		
		protected virtual string GetMessage(string message)
		{
			return message;
		}
		
		public override string Message {
			get {
				return this.GetMessage(this.Message??base.Message);
			}
		}
		
		public override Exception GetBaseException()
		{
			return this._baseException??base.GetBaseException();
		}
		public override string HelpLink {
			get { return base.HelpLink; }
			set { base.HelpLink = value; }
		} 
	}
	
	public class CmsApplicationException:CmsException
	{

        protected CmsApplicationException(SerializationInfo s, StreamingContext stream)
        {

      }


		public CmsApplicationException(string message,Exception innerException):base(message,innerException)
		{
		
		}
		
		public CmsApplicationException(string message):base(message)
		{
			
		}
		
		protected override string GetMessage(string message)
		{
			return base.GetMessage("应用程序异常!信息："+message);
		}
	}
}
