using Ops.Cms.DataTransfer;
using Ops.Cms.Domain.Interface.Common;
using Ops.Cms.Domain.Interface.Content;
using Ops.Cms.Domain.Interface.Content.Archive;
using Ops.Cms.ServiceContract;
using System;
using System.Collections.Generic;

namespace Ops.Cms.Service
{
    public class ContentService : IContentServiceContract
    {
        private IContentRepository _contentRep;

       public ContentService(
           IContentRepository contentRep)
        {
            this._contentRep = contentRep;
        }

        public IBaseContent GetContent(int siteId,string typeIndent, int contentId)
        {
            return this._contentRep.GetContent(siteId).GetContent(typeIndent, contentId);
        }

        public int SaveRelatedLink(int siteId,string typeIndent, int contentId, LinkDto link)
        {

            IBaseContent content = this.GetContent(siteId,typeIndent, contentId);

            if(link.LinkId>0)
            {
                ILink _link = content.LinkManager.GetLinkById(link.LinkId);
                _link.LinkTitle = link.LinkTitle;
                _link.LinkName = link.LinkName;
                _link.LinkUri = link.LinkUri;
                _link.Enabled = link.Enabled;

            }else{
                content.LinkManager.Add(link.LinkId, link.LinkName, link.LinkTitle, link.LinkUri, link.Enabled);
            }

            content.LinkManager.SaveLinks();
            return link.LinkId;
        }

        public void RemoveRelatedLink(int siteId, string typeIndent, int contentId, int relatedLinkId)
        {
            IBaseContent content = this.GetContent(siteId,typeIndent, contentId);

            if (relatedLinkId > 0)
            {
                ILink link = content.LinkManager.GetLinkById(relatedLinkId);
                content.LinkManager.Remove(link);
            }
            else
            {
                throw new Exception("relatedLinkId无效");
            }

            content.LinkManager.SaveLinks();

        }

        public IEnumerable<LinkDto> GetRelatedLinks(int siteId, string typeIndent, int contentId)
        {
            IBaseContent content = this.GetContent(siteId,typeIndent, contentId);
            IList<ILink> linkList = content.LinkManager.GetRelatedLinks();
            foreach (ILink link in linkList)
            {
                yield return this.ConvertToLinkDto(link);
            }
        }

        private LinkDto ConvertToLinkDto(ILink link)
        {
            return new LinkDto
            {
                LinkId = link.LinkId,
                Enabled = link.Enabled,
                LinkUri = link.LinkUri,
                LinkName = link.LinkName,
                LinkTitle = link.LinkTitle
            };
        }
    }
}
