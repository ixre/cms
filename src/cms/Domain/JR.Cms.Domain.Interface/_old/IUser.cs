using JR.Cms.Domain.Interface.Models;

namespace JR.Cms.Domain.Interface._old
{
    public interface IUser
    {
        string ConvertPermissionArrayToString(Models.Operation[] operations);
        Models.Operation[] ConvertToPermissionArray(string permissions);
        bool CreateNewOperation(string name, string path);
        void DeleteOperation(int id);
        string EncodePassword(string password);
        Models.User[] GetAllUser();
        Models.Operation GetOperation(System.Func<Operation, bool> func);
        Models.Operation GetOperation(int id);
        System.Collections.Generic.IList<Operation> GetOperationList();
        System.Collections.Generic.IEnumerable<Operation> GetOperationList(System.Func<Operation, bool> func);
        System.Data.DataTable GetPagedAvailableOperationList(bool available, int pageSize, int currentPageIndex, out int recordCount, out int pageCount);
        System.Data.DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount, out int pageCount);
        Models.User GetUser(System.Func<Models.User, bool> func);
        Models.User GetUser(string username);
        Models.UserGroup GetUserGroup(UserGroups group);
        Models.UserGroup[] GetUserGroups();
        Models.User[] GetUsers(System.Func<Models.User, bool> func);
        void RenameUserGroup(UserGroups group, string groupName);
        void RenewOperations();
       // void ResetUserPassword(string username, string password);
        void UpdateOperation(int id, string name, string path, bool available);
        //void UpdateUser(string username, int siteid, string name, UserGroups group, bool available);
        void UpdateUserGroupPermissions(UserGroups group, Models.Operation[] permissions);
        void UpdateUserLastLoginDate(string username);
        bool UserIsExist(string username);
    }
}
