using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class BinarySaving<T> : ISavingSystem<T> where T : class
{
    public bool SaveData(T data, string fileName = "gameData")
    {
        string persistentPath = Application.persistentDataPath + $"/{fileName}.save";
        using FileStream fileStream = new FileStream(persistentPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, data);
        return true;
    }
        
    private void SaveData(T data, string fileName, string path)
    {
        string persistentPath = path + $"/{fileName}.save";
        using FileStream fileStream = new FileStream(persistentPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, data);
    }
        
    public T LoadData(string fileName = "gameData") 
    {
        string persistentPath = Application.persistentDataPath + $"/{fileName}.save";
        if (File.Exists(persistentPath))
        {
            using FileStream fileStream = new FileStream(persistentPath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            fileStream.Position = 0;
            T data = (T)binaryFormatter.Deserialize(fileStream);
            return data;
        }

        Debug.Log("File can't be found");
        return null;
    }
        
    private T LoadData(string fileName, string path)
    {
        string persistentPath = path + $"/{fileName}.save";
        if (File.Exists(persistentPath))
        {
            using FileStream fileStream = new FileStream(persistentPath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            fileStream.Position = 0;
            T data = (T)binaryFormatter.Deserialize(fileStream);
            return data;
        }

        Debug.Log("File can't be found");
        return null;
    }

    public void DeleteData(string fileName = "gameData")
    {
        string persistentPath = Application.persistentDataPath + $"/{fileName}.save";
        if (File.Exists(persistentPath))
        {
            File.Delete(persistentPath);
        }
    }
        
    private void DeleteData(string fileName, string path)
    {
        string persistentPath = path + $"/{fileName}.save";
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
        string persistentPath = Application.persistentDataPath + $"/{saveName}.save";
        return File.Exists(persistentPath);
    }
}