using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class JsonSaving<T> : ISavingSystem<T> where T : class
{
    public bool SaveData(T data, string fileName = "gameData")
    {
        string persistentPath = Application.persistentDataPath + $"/{fileName}.json";
        string jsonData = JsonUtility.ToJson(data);

        using StreamWriter streamWriter = new StreamWriter(persistentPath);
        streamWriter.Write(jsonData);
        streamWriter.Close();
        return true;
    }
        
    private void SaveData(T data, string fileName, string path)
    {
        string persistentPath = path + $"/{fileName}.json";
        string jsonData = JsonUtility.ToJson(data);

        using StreamWriter streamWriter = new StreamWriter(persistentPath);
        streamWriter.Write(jsonData);
        streamWriter.Close();
    }

    public T LoadData(string fileName = "gameData")
    {
        string persistentPath = Application.persistentDataPath + $"/{fileName}.json";
        if (File.Exists(persistentPath))
        {
            using StreamReader streamReader = new StreamReader(persistentPath);
            string jsonData = streamReader.ReadToEnd();
            streamReader.Close();

            T gameData = JsonUtility.FromJson<T>(jsonData);
            return gameData;
        }
            
        Debug.Log("File can't be found");
        return null;
    }

    private T LoadData(string fileName, string path)
    {
        string persistentPath = path + $"/{fileName}.json";
        if (File.Exists(persistentPath))
        {
            using StreamReader streamReader = new StreamReader(persistentPath);
            string jsonData = streamReader.ReadToEnd();
            streamReader.Close();

            T gameData = JsonUtility.FromJson<T>(jsonData);
            return gameData;
        }
            
        Debug.Log("File can't be found");
        return null;
    }
        
    public void DeleteData(string fileName = "gameData")
    {
        string persistentPath = Application.persistentDataPath + $"/{fileName}.json";
        if (File.Exists(persistentPath))
        {
            File.Delete(persistentPath);
        }
    }
        
    private void DeleteData(string fileName, string path)
    {
        string persistentPath = path + $"/{fileName}.json";
        if (File.Exists(persistentPath))
        {
            File.Delete(persistentPath);
        }
    }
        
    public async Task SaveDataAsync(T data, string fileName = "gameData")
    {
        string persistentPath = Application.persistentDataPath;
        await Task.Run(() => SaveData(data, fileName, persistentPath));
    }
        
    public async Task<T> LoadDataAsync(string fileName = "gameData")
    {
        string persistentPath = Application.persistentDataPath;
        return await Task.Run(() => LoadData(fileName, persistentPath));
    }
        
    public async Task DeleteDataAsync(string fileName = "gameData")
    {
        string persistentPath = Application.persistentDataPath;
        await Task.Run(() => DeleteData(fileName, persistentPath));
    }

    public bool HasSave(string saveName)
    {
        string persistentPath = Application.persistentDataPath + $"/{saveName}.json";
        return File.Exists(persistentPath);
    }
}