namespace component.caching
{
    public interface ICacheManager
    {
        T Get<T>(string applicationKey, string key);
        bool Add<T>(string applicationKey, string key, T value);
        bool Flush(string applicationKey);
    }
}