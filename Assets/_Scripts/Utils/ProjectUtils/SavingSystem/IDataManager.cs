using System;
using System.Threading.Tasks;

public interface IDataManager<T>
{
    public event Action<T> OnDataChanged;
    public event Action<T> OnDataLoaded; 
    
    public IGameTimeHandler GetGameTimeHandler(TimeHandlerType timeHandlerType);
    
    public void SaveData(T data);
    public void LoadData();
    public T GetData();
}

public interface IAsyncDataManager<T>
{
    public Task SaveDataAsync(T data);
    public Task LoadDataAsync();
    public Task<T> GetDataAsync();
}

public enum TimeHandlerType
{
    OnlyLocal,
    OnlyServer,
    FirstServerThenLocal
}

public enum SaveType
{
    Binary,
    Json
}