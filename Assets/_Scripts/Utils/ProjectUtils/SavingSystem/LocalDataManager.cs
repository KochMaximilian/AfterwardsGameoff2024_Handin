using System;
using System.Threading.Tasks;
using UnityEngine;

public class LocalDataManager<T> : IAsyncDataManager<T> where T : class, new()
{
    public event Action<T> OnDataChanged;
    public event Action<T> OnDataLoaded;
    
    private LocalGameTimeHandler _gameTimeHandler;
    private ISavingSystem<T> _savingSystem;
    private T _data;
    
    
    public LocalDataManager(SaveType saveType)
    {
        _savingSystem = saveType switch
        {
            SaveType.Binary => new BinarySaving<T>(),
            SaveType.Json => new JsonSaving<T>()
        };    
        
        _gameTimeHandler = (LocalGameTimeHandler) GetGameTimeHandler(TimeHandlerType.OnlyLocal);
    }
    
    public IGameTimeHandler GetGameTimeHandler(TimeHandlerType type)
    {
        if(_gameTimeHandler != null) return _gameTimeHandler;
        
        switch (type)
        {
            case TimeHandlerType.OnlyLocal:
                _gameTimeHandler = new LocalGameTimeHandler();
                break;
            case TimeHandlerType.OnlyServer:
                //Can throw exception if NTP time is not available
                _gameTimeHandler = new NTPGameTimeHandler();
                break;
            case TimeHandlerType.FirstServerThenLocal:
                try
                {
                    _gameTimeHandler = new NTPGameTimeHandler();
                }
                catch (Exception e)
                {
                    //Could not get NTP time, using local time
                    _gameTimeHandler = new LocalGameTimeHandler();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return _gameTimeHandler;
    }

    public bool SaveData(T data, string path = "gameData")
    {
        bool result = true;
        try
        {
            _gameTimeHandler.OnDataChanged();
            result = _savingSystem.SaveData(data, path);
        
            OnDataChanged?.Invoke(data);
            _data = data;
        }
        catch (Exception e)
        {
            Debug.LogError("Could not save data! Error: " + e.Message);
            throw;
        }
        
        return result;
    }
    
    public async Task SaveDataAsync(T data)
    {
        try
        {
            await _gameTimeHandler.OnDataChangedAsync();
            await _savingSystem.SaveDataAsync(data);
        
            OnDataChanged?.Invoke(data);
            _data = data;
        }
        catch (Exception e)
        {
            Debug.LogError("Could not save data! Error: " + e.Message);
            throw;
        }
    }

    public bool LoadData(string path = "gameData")
    {
        T data = _savingSystem.LoadData(path);

        if(data == null)
        {
            OnDataLoaded?.Invoke(new());
            return false;
        }
        
        _data = data;
        OnDataLoaded?.Invoke(data);
        return true;
    }
    
    public async Task LoadDataAsync()
    {
        T data = await _savingSystem.LoadDataAsync();
        
        if(data == null)
        {
            OnDataLoaded?.Invoke(new());
            return;
        }
        
        _data = data;
        OnDataLoaded?.Invoke(data);
    }

    public Task<T> GetDataAsync()
    {
        return Task.FromResult(_data);
    }

    public T GetData()
    {
        return _data;
    }

    public void DeleteData(string saveName)
    {
        _savingSystem.DeleteData(saveName);
    }

    public bool HasSave(string saveName)
    {
        return _savingSystem.HasSave(saveName);
    }
}