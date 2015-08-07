
/*
* Copyright(C) 2010-2013 K3F.NET
* 
* File Name	: CmsEventRegister.cs
* publisher_id	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using System.Linq;
using J6.Cms.BLL;
using J6.Cms.CacheService;
using J6.Cms.Conf;
using J6.Cms.Domain.Interface._old;
using J6.Cms.Resource;
using StructureMap;

namespace J6.Cms
{
    public class CmsEventRegister
    {

        /// <summary>
        /// CMS初始化
        /// </summary>
        public static void Init()
        {

            //设置依赖反转
            ObjectFactory.Configure(x =>
            {

                //x.For<IArchiveModel>().Singleton().Use<ArchiveBLL>();
                // x.For<ICategoryModel>().Singleton().Use<CategoryBLL>();
                x.For<IComment>().Singleton().Use<CommentBll>();
                // x.For<ILink>().Singleton().Use<LinkBLL>();
                x.For<Imember>().Singleton().Use<MemberBll>();
                x.For<Imessage>().Singleton().Use<MessageBLL>();
                x.For<Imodule>().Singleton().Use<ModuleBLL>();
                //x.For<ISite>().Singleton().Use<SiteBLL>();
                // x.For<ITemplateBind>().Singleton().Use<TemplateBindBLL>();
                x.For<IUser>().Singleton().Use<UserBll>();
                x.For<ITable>().Singleton().Use<TableBll>();

            });

            //读取站点
            if (Cms.Installed)
            {
                Cms.RegSites(SiteCacheManager.GetAllSites().ToArray());
            }

            //内嵌资源释放
            SiteResourceInit.Init();

            //设置可写权限
            Cms.Utility.SetDirCanWrite(CmsVariables.RESOURCE_PATH);
            Cms.Utility.SetDirCanWrite("templates/");
            Cms.Utility.SetDirCanWrite(CmsVariables.FRAMEWORK_PATH);
            Cms.Utility.SetDirCanWrite(CmsVariables.PLUGIN_PATH);
            Cms.Utility.SetDirCanWrite(CmsVariables.TEMP_PATH + "update");
            Cms.Utility.SetDirHidden("config");
            Cms.Utility.SetDirHidden("bin");

         

        }
    }
}
