
/*
* Copyright(C) 2010-2013 OPSoft Inc
* 
* File Name	: CmsEventRegister.cs
* Author	: Newmin (new.min@msn.com)
* Create	: 2013/05/21 19:59:54
* Description	:
*
*/

using Ops.Cms.Conf;
using Ops.Cms.Domain.Interface._old;

namespace Spc
{
    using Ops.Cms;
    using Ops.Cms.CacheService;
    using Ops.Cms.Resource;
    using Spc.BLL;
    using StructureMap;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

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
                x.For<IComment>().Singleton().Use<CommentBLL>();
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
            Cms.RegSites(SiteCacheManager.GetAllSites().ToArray());

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
