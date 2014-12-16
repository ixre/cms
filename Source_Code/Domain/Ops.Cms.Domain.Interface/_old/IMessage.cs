namespace Ops.Cms.Domain.Interface._old
{
    public interface Imessage
    {
        bool Delete(int receiveUid, int id);
        Spc.Models.Message GetMessage(int id);
        System.Collections.Generic.IEnumerable<Spc.Models.Message> GetPagedMessage(int uid, Spc.Models.MessageType type, int pageSize, ref int currentPageIndex, out int recordCount, out int pageCount);
        void SendMessage(int sendUid, int receiveUid, string subject, string content);
        bool SetRead(int receiveUid, int id);
        bool SetRecycle(int receiveUid, int id);
    }
}
