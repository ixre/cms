using System;
using System.IO;

namespace JR.Stand.Core.Framework
{
    public class Logger
    {
        private static readonly Logger _logger = new Logger("");
        public static Logger GetDefault() => _logger;
        private readonly TextWriter _writer = null;
        private readonly string _typeName;

        private Logger(String typeName)
        {
            this._typeName = typeName;
        }
        public static Logger Factory(Type type)
        {
            return new Logger(type.FullName);
        }

        /// <summary>
        /// 输出警告信息
        /// </summary>
        /// <param name="text"></param>
        public void Warning(string text)
        {
            this.Print(this._writer??Console.Out,"[ Warning]: ",text);
        }

        /// <summary>
        /// 输出错误信息
        /// </summary>
        /// <param name="text"></param>
        public void Error(string text)
        {
            this.Print(this._writer??Console.Error,"[ Error]: ",text);
        }
        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="text"></param>
        public void Info(string text)
        {
            this.Print(this._writer??Console.Out,"[ Info]: ",text);
        }
        /// <summary>
        /// 输出DEBUG信息
        /// </summary>
        /// <param name="text"></param>
        public void Debug(string text)
        {
            this.Print(this._writer??Console.Out,"[ Debug]: ",text);
        }
        private void Print(TextWriter writer,String prefix, String text)=>(writer ?? Console.Out).WriteAsync(
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {this._typeName} - {prefix}{text}\n");
    }
}