using System;
using OPS.Web;

namespace OPSoft.WebControlCenter
{
    public class ControlHandler:ExecuteHandler
    {
        public ControlHandler()
        {
            handlerType = typeof(Handler.Check);
        }
    }
}