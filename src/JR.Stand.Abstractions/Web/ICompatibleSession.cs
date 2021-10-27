










namespace JR.Stand.Abstracts.Web
{
    public interface ICompatibleSession
    {
        void SetInt32(string key, in int value);
        int GetInt32(string key);
        void SetString(string key, string value);
        string GetString(string key);
        T GetObjectFromJson<T>(string key);
        void SetObjectAsJson(string key, object o);
        void Remove(string key);
    }
}