/*
 * Copyright 2010 OPS,All rights reseved!
 * name     : ArchiveHelper
 * author   : newmin
 * date     : 2010/12/06
 */
namespace Ops.Cms
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Text.RegularExpressions;
    using Ops.Data;
    using Ops.Cms.Models;
    using Ops.Cms.IDAL;
    using DAL;

    [Obsolete]
    public class UriHelper
    {
        private static readonly IArchiveDAL dal = new ArchiveDAL();
        private static Regex archiveRegex = new Regex("{\\d+}");
        private static readonly Regex uriRegex = new Regex("{suffix}");


        private const string TEMP_URI_SUFFIX = ".html";

        public string Convert(string uri)
        {
            if (uriRegex.IsMatch(uri))
            {
                return uriRegex.Replace(uri,TEMP_URI_SUFFIX);
            }
            return uri;
        }

        /// <summary>
        /// 文档的链接地址
        /// </summary>
        /// <returns></returns>
        public string GetArchiveUri(Archive archive)
        {
            return Convert(archiveRegex.Replace(TEMP_URI_SUFFIX , String.IsNullOrEmpty(archive.Alias) ? archive.ID.ToString() : archive.Alias));
        }
        public string GetArchiveUri(string alias, int id)
        {
            return Convert(archiveRegex.Replace(TEMP_URI_SUFFIX , String.IsNullOrEmpty(alias) ? id.ToString() : alias));
        }
        public string GetArchiveUri(object alias, object id)
        {
            return Convert(archiveRegex.Replace(TEMP_URI_SUFFIX , alias == null || alias.ToString() == "" ? id.ToString() : alias.ToString()));
        }
    }
}