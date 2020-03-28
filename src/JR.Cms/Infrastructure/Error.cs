using System;


namespace JR.Cms.Infrastructure
{
    public class Error
    {
        private string message;

        public Error(string message)
        {
            this.message = message;
        }

        public string Message => message;
    }
}