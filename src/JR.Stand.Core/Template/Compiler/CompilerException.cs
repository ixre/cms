using System;
using System.CodeDom.Compiler;
using System.Text;

namespace JR.DevFw.Template.Compiler
{
    public class CompilerException : Exception
    {
        private string _Code;
        private CompilerErrorCollection _Errors;

        public string Code
        {
            get { return _Code; }
        }

        public CompilerErrorCollection Errors
        {
            get { return _Errors; }
        }

        public CompilerException(string code, CompilerErrorCollection errors)
        {
            _Code = code;
            _Errors = errors;
        }

        public override string ToString()
        {
            StringBuilder message = new StringBuilder();

            foreach (CompilerError err in _Errors)
            {
                message.AppendFormat("({0}): error {1}: {2}", err.Line, err.ErrorNumber, err.ErrorText);
            }

            return message.ToString();
        }
    }
}