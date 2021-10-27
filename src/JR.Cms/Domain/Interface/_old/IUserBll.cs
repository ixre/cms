using JR.Cms.Domain.Interface.Models;

namespace JR.Cms.Domain.Interface._old
{
    public interface IUserBll
    {
        string ConvertPermissionArrayToString(Operation[] operations);
        Operation[] ConvertToPermissionArray(string permissions);
        bool CreateNewOperation(string name, string path);
        void DeleteOperation(int id);
        string EncodePassword(string password);
        Models.User[] GetAllUser();
        Operation GetOperation(System.Func<Operation, bool> func);
        Operation GetOperation(int id);
        System.Collections.Generic.IList<Operation> GetOperationList();
        System.Collections.Generic.IEnumerable<Operation> GetOperationList(System.Func<Operation, bool> func);

        System.Data.DataTable GetPagedAvailableOperationList(bool available, int pageSize, int currentPageIndex,
            out int recordCount, out int pageCount);

        System.Data.DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount,
            out int pageCount);

        Models.User GetUser(System.Func<Models.User, bool> func);
        Models.User GetUser(string username);
        UserGroup GetUserGroup(UserGroups group);
        UserGroup[] GetUserGroups();
        Models.User[] GetUsers(System.Func<Models.User, bool> func);
        void RenameUserGroup(UserGroups group, string groupName);

        void RenewOperations();

        // void ResetUserPassword(string username, string password);
        void UpdateOperation(int id, string name, string path, bool available);

        //void UpdateUser(string username, int siteid, string name, UserGroups group, bool available);
        void UpdateUserGroupPermissions(UserGroups group, Operation[] permissions);
        void UpdateUserLastLoginDate(string username);
        bool UserIsExist(string username);
    }
}