/*--------------------------------------
 * Name   :   DataMappingException
 * Author   :   Sonven
 * Create   :   2009-12-04 10:18
 * LastModify   
 * Note
 *-------------------------------------*/

using System;

namespace JR.Stand.Core.Data.Orm.Mapping
{
    public class DataMappingException : ApplicationException
    {
        private string message;
        private object source;
        private string sourceId;

        public DataMappingException(string message)
        {
            this.message = message;
        }

        public DataMappingException(string source, string message) : this(message)
        {
            this.sourceId = source;
        }

        public DataMappingException(object source, string message) : this(message)
        {
            this.source = source;
        }

        public override string Message
        {
            get { return message; }
        }

        public override string Source
        {
            get { return sourceId; }
            set { base.Source = value; }
        }
    }
}