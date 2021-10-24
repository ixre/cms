using JR.Cms.Domain.Interface._old;
using JR.Cms.Infrastructure.Ioc;

namespace JR.Cms.Library.DataAccess.BLL
{
    public class CmsLogic
    {
        private static T GetInstance<T>()
        {
            return Ioc.GetInstance<T>();
        }

        public static IComment Comment => GetInstance<IComment>();
        public static Imember Member => GetInstance<Imember>();
        public static Imessage Message => GetInstance<Imessage>();
        public static Imodule Module => GetInstance<Imodule>();


        public static ITable Table => GetInstance<ITable>();
        public static IUserBll UserBll => GetInstance<IUserBll>();
    }
}