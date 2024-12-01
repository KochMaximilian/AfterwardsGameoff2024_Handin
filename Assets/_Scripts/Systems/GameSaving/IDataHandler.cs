public interface IDataHandler<T>
{
    T GetData();
    void LoadData(T data);
    
    long GetHash();
}