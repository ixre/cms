
namespace AtNet.Cms.Domain.Interface._old
{
    public interface IComment
    {
        void DeleteArchiveReviews(string archiveId);
        void DeleteComment(int commentId);
        void DeleteMemberComments(int memberId);
        System.Data.DataTable GetArchiveComments(string archiveId, bool desc);
        int GetArchiveCommentsCount(string archiveId);
        void InsertComment(string archiveId, int memberId, string ip, string content);
        bool SubmitReviews(string archiveId, int memberId, bool agree);
    }
}
