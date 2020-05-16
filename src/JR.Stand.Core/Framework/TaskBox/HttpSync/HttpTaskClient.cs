using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;

namespace JR.Stand.Core.Framework.TaskBox.HttpSync
{
    /// <summary>
    /// Http任务客户端
    /// </summary>
    public class HttpTaskClient : ITaskExecuteClient
    {
        private string server;
        private object token;

        public HttpTaskClient(string serverUrl, string token)
        {
            this.server = serverUrl;
            this.token = token;
        }

        public string ClientName
        {
            get { return "HttpTaskClient"; }
        }

        public void Execute(ITask task)
        {
            foreach (Hashtable data in task.Datas)
            {
                this.Post(task, data);
            }
        }

        private void Post(ITask task, Hashtable data)
        {
            if (data.ContainsKey("auth_token"))
                data.Add("auth_token", this.token);

            string url = this.server
                         + "?auth_token=" + this.token
                         + "&action=" + task.TaskName;

            try
            {
                HttpWebResponse rsp = HttpSimpleRequest.CreatePostHttpResponse(
                    url,
                    data,
                    null,
                    Encoding.UTF8,
                    null
                    );

                StreamReader sr = new StreamReader(rsp.GetResponseStream());
                string result = sr.ReadToEnd();

                sr.BaseStream.Dispose();
                sr.Dispose();

                task.SetState(this, TaskState.Ok, TaskMessageParser.ConvertToSyncMessage(result));
            }
            catch (Exception exc)
            {
                task.SetState(this, TaskState.Error, new TaskMessage
                {
                    Result = false,
                    Message = exc.Message
                });
            }
        }

        public bool TestConnect()
        {
            try
            {
                string url = this.server
                             + "?auth_token=" + this.token
                             + "&action=connect";

                HttpWebResponse rsp = HttpSimpleRequest.CreatePostHttpResponse(
                    url,
                    new Hashtable(),
                    null,
                    Encoding.UTF8,
                    null
                    );
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }
    }
}