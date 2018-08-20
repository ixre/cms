using System;


namespace T2.Cms.Infrastructure
{
    public class Error
    {
        private string message;

        public Error(string message)
        {
            this.message = message;
        }
        public String Message
        {
            get
            {
                return this.message;
            }
        }
    }
}
