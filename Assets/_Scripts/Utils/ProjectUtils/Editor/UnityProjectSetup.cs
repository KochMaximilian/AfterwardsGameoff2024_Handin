using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

public class UnityProjectSetup 
{
    [MenuItem("Tools/Setup Project and Scene", priority = 0)]
    public static void SetupProjectAndScene()
    {
        SetupUnityProject();
        SetupScene();
    }
    
    [MenuItem("Tools/Setup Unity Project")]
    public static void SetupUnityProject()
    {
        Debug.Log("Setting up Unity Project");
        AddFolders();
    }
    
    [MenuItem("Tools/Setup Scene")]
    public static void SetupScene()
    {
        Debug.Log("Setting up Scene");
        AddGameObjects();
    }
    
    private static void AddGameObjects()
    {
        CreateGameObject("======== MANAGERS ========");
        CreateGameObject("=========== UI ===========");
        CreateGameObject("====== ENVIRONMENT ======", new List<string>()
        {
            "======== LIGHTS ========",
            "======== VOLUME ========",
            "========= MAP =========="
        });
        CreateGameObject("========= PLAYER =========");
    }

    private static void CreateGameObject(string gameObjectName, List<string> children = null)
    {
        GameObject gameObject = GameObject.Find(gameObjectName);
        if(gameObject == null)
        {
            gameObject = new GameObject(gameObjectName);
            Debug.Log("Created GameObject: " + gameObject.name);
        }
        else Debug.Log("GameObject already exists: " + gameObjectName);
        
        if(children != null)
        {
            foreach (var child in children)
            {
                if(gameObject.transform.Find(child) != null)
                {
                    Debug.Log("Child GameObject already exists: " + child);
                    continue;
                }
                
                GameObject childObject = new GameObject(child);
                childObject.transform.parent = gameObject.transform;
                Debug.Log("Created Child GameObject: " + child);
            }
        }
    }

    private static void AddFolders()
    {
        Dictionary<string, List<string>> folders = new Dictionary<string, List<string>>();
        
        folders.Add("_Scripts", new List<string>() {"Editor", "Managers", "Systems", "UI", "Utils"});
        folders.Add("Art", new List<string>(){"Materials", "Models", "Textures", "Sprites"});
        folders.Add("Audio", new List<string>(){"Music", "SFX"});
        folders.Add("Resources", new List<string>(){"Prefabs", "ScriptableObjects"});
        folders.Add("Scenes", new List<string>());
        
        CreateFolders(folders);
    }
    
    private static void CreateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("Created Folder: " + path);
        }
    }
    private static void CreateFolders(Dictionary<string, List<string>> folders)
    {
        string basePath = Application.dataPath;
        
        foreach (var folder in folders)
        {
            string path = Path.Combine(basePath, folder.Key);
            CreateFolder(path);

            foreach (var subFolder in folder.Value)
            {
                string subPath = Path.Combine(path, subFolder);
                CreateFolder(subPath);
            }
        }
    }
    

}
