using JR.Cms.Domain.Interface._old;
using JR.Cms.Infrastructure.Ioc;

namespace JR.Cms.BLL
{
    public class CmsLogic
    {
        private static T GetInstance<T>()
        {
            return Ioc.GetInstance<T>();
        }
        public static IComment Comment { get { return GetInstance<IComment>(); } }
        public static Imember Member { get { return GetInstance<Imember>(); } }
        public static Imessage Message { get { return GetInstance<Imessage>(); } }
        public static Imodule Module { get { return GetInstance<Imodule>(); } }


        public static ITable Table { get { return GetInstance<ITable>(); } }
        public static IUserBll UserBll { get { return GetInstance<IUserBll>(); } }

    }
}
