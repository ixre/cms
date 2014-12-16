using Spc.Models;

namespace Ops.Cms.Domain.Interface._old
{
    public interface IUser
    {
        string ConvertPermissionArrayToString(Spc.Models.Operation[] operations);
        Spc.Models.Operation[] ConvertToPermissionArray(string permissions);
        bool CreateNewOperation(string name, string path);
        void CreateUser(Spc.Models.User user);
        void DeleteOperation(int id);
        int DeleteUser(string username);
        string EncodePassword(string password);
        Spc.Models.User[] GetAllUser();
        Spc.Models.Operation GetOperation(System.Func<Spc.Models.Operation, bool> func);
        Spc.Models.Operation GetOperation(int id);
        System.Collections.Generic.IList<Spc.Models.Operation> GetOperationList();
        System.Collections.Generic.IEnumerable<Spc.Models.Operation> GetOperationList(System.Func<Spc.Models.Operation, bool> func);
        System.Data.DataTable GetPagedAvailableOperationList(bool available, int pageSize, int currentPageIndex, out int recordCount, out int pageCount);
        System.Data.DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount, out int pageCount);
        Spc.Models.User GetUser(System.Func<Spc.Models.User, bool> func);
        Spc.Models.User GetUser(string username);
        Spc.Models.User GetUser(string username, string password);
        Spc.Models.UserGroup GetUserGroup(UserGroups group);
        Spc.Models.UserGroup[] GetUserGroups();
        Spc.Models.User[] GetUsers(System.Func<Spc.Models.User, bool> func);
        bool ModifyUserPassword(string username, string oldPassword, string newPassword);
        void RenameUserGroup(UserGroups group, string groupName);
        void RenewOperations();
        void ResetUserPassword(string username, string password);
        void UpdateOperation(int id, string name, string path, bool available);
        void UpdateUser(string username, int siteid, string name, UserGroups group, bool available);
        void UpdateUserGroupPermissions(UserGroups group, Spc.Models.Operation[] permissions);
        void UpdateUserLastLoginDate(string username);
        bool UserIsExist(string username);
    }
}
