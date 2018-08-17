using System.Globalization;
using System;
using System.Collections.Generic;
using J6.Cms.Dal;
using J6.Cms.Domain.Interface.Common;
using J6.Cms.Domain.Interface.Content;

namespace J6.Cms.ServiceRepository
{
    public class LinkRepository : ILinkRepository
    {
        private LinkDal _linkDal = new LinkDal();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkManager"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        private void ReadLinks(IContentLinkManager linkManager, string contentType, int contentId)
        {
            this._linkDal.ReadLinksOfContent(contentType, contentId, rd =>
            {
                while (rd.Read())
                {
                    linkManager.Add(
                        int.Parse(rd["id"].ToString()),
                       int.Parse(rd["related_indent"].ToString()),
                       int.Parse(rd["related_content_id"].ToString()),
                        rd["enabled"].ToString() == "1" || rd["enabled"].ToString() == "True"
                        );
                }
            });
        }
        private void SaveLinks(string typeIndent, int relatedId, IList<IContentLink> list)
        {
            this._linkDal.SaveLinksOfContent(typeIndent, relatedId, list);
        }


        public void ReadLinksOfContent(IContentLinkManager linkManager, int contentModelIndent, int contentId)
        {
            this.ReadLinks(linkManager, contentModelIndent.ToString(), contentId);

        }

        public void SaveLinksOfContent(int contentModelIndent, int contentId, IList<IContentLink> list)
        {
            SaveLinks(contentModelIndent.ToString(), contentId, list);
        }

        public void RemoveRelatedLinks(string typeIndent, int relatedId, IList<int> list)
        {
            String ids = "";

            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0)
                {
                    ids += ",";
                }
                ids += list[i].ToString(CultureInfo.InvariantCulture);
            }

            this._linkDal.RemoveRelatedLinks(typeIndent, relatedId, ids);
        }
    }
}
