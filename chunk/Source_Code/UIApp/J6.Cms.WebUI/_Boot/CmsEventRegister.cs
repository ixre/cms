
/*
* Copyright(C) 2010-2013 S1N1.COM
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
                x.For<Imember>().Singleton().Use<MemberBLL>();
                x.For<Imessage>().Singleton().Use<MessageBLL>();
                x.For<Imodule>().Singleton().Use<ModuleBLL>();
                //x.For<ISite>().Singleton().Use<SiteBLL>();
                // x.For<ITemplateBind>().Singleton().Use<TemplateBindBLL>();
                x.For<IUser>().Singleton().Use<UserBLL>();
                x.For<ITable>().Singleton().Use<TableBLL>();

            });

            //读取站点
            if (j6.Installed)
            {
                j6.RegSites(SiteCacheManager.GetAllSites().ToArray());
            }

            //内嵌资源释放
            SiteResourceInit.Init();

            //设置可写权限
            j6.Utility.SetDirCanWrite(CmsVariables.RESOURCE_PATH);
            j6.Utility.SetDirCanWrite("templates/");
            j6.Utility.SetDirCanWrite(CmsVariables.FRAMEWORK_PATH);
            j6.Utility.SetDirCanWrite(CmsVariables.PLUGIN_PATH);
            j6.Utility.SetDirCanWrite(CmsVariables.TEMP_PATH + "update");
            j6.Utility.SetDirHidden("config");
            j6.Utility.SetDirHidden("bin");

         

        }
    }
}
