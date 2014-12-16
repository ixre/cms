namespace Ops.Cms.Domain.Interface._old
{
    public interface Imember
    {
        void Delete(int id, bool deleteInfo);
        bool DetectNickNameAvailable(string nickname);
        bool DetectUsernameAvailable(string username);
        string GenericMemberToken(string username);
        Spc.Models.Member GetMember(int memberID);
        Spc.Models.Member GetMember(string username);
        void GetMemberDetails(int id, out Spc.Models.Member member, out Spc.Models.MemberDetails details);
        void GetMemberDetails(string username, out Spc.Models.Member member, out Spc.Models.MemberDetails details);
        System.Data.DataTable GetPagedMembers(int pageSize, ref int currentPageIndex, out int recordCount, out int pageCount);
        int Regsiter(string username, string password, string nickname, string email, string ip, bool needActive);
        void Update(Spc.Models.Member member);
        Spc.Models.Member VerifyMember(string username, string password);
        bool VerifyToken(string token, string username);
    }
}
