using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtils;

public class GameStateCapturer : Singleton<GameStateCapturer>
{
    private LocalDataManager<GameStateSnapshot> _localDataManager;
    
    private void Awake()
    {
        _localDataManager = new LocalDataManager<GameStateSnapshot>(SaveType.Binary);
    }
    
    public bool HasSave(string saveName)
    {
        return _localDataManager.HasSave(saveName);
    }
    
    public void CaptureGameState(string saveName)
    {
        GameStateSnapshot gameStateSnapshot = new GameStateSnapshot();
        
        List<WordSnapshot> wordPictures = CaptureWordPictures();
        gameStateSnapshot.WordSnapshots = wordPictures;
        
        List<SentenceSnapshot> collectedSentences = CaptureCollectedSentences();
        gameStateSnapshot.CollectedSentences = collectedSentences;

        gameStateSnapshot.currentLevel =  GameManager.Instance.LevelManager.CurrentLevelIndex;
        gameStateSnapshot.PlayerPosition = GameObject.FindWithTag("Player").transform.position;
        gameStateSnapshot.PlayerRotation = CameraController.Instance.transform.rotation.eulerAngles;
        gameStateSnapshot.time = Timer.Instance.CurrentTime;

        IEnumerable<IDataHandler<InteractableObjectSnapshot>> interactableObjects =
            FindObjectsByType<InteractableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Select(interactableObject => interactableObject.GetComponent<IDataHandler<InteractableObjectSnapshot>>());
        
        gameStateSnapshot.interactableObjects = interactableObjects.Select(interactableObject => interactableObject.GetData()).ToList();

        _localDataManager.SaveData(gameStateSnapshot, saveName);
    }
    
    public bool LoadGameState(string saveName)
    {
        _localDataManager.OnDataLoaded -= OnDataLoaded;
        _localDataManager.OnDataLoaded += OnDataLoaded;
        return _localDataManager.LoadData(saveName);
    }
    
    public void DeleteGameState(string saveName)
    {
        _localDataManager.DeleteData(saveName);
    }
    
    private void OnDataLoaded(GameStateSnapshot gameStateSnapshot)
    {
        if(gameStateSnapshot == null || gameStateSnapshot.WordSnapshots == null) return;
        
        GameManager.Instance.LevelManager.TransitionToLevel(gameStateSnapshot.currentLevel,
            () =>
            {
                WordManager wordManager = WordManager.Instance;
        
                wordManager.ClearWords();
                foreach (var wordPicture in gameStateSnapshot.WordSnapshots)
                {
                    _ = wordManager.CreateWordAsync(wordPicture.GetWordNode(), wordPicture.Position);
                }
        
                List<Sentence> collectedSentences = new List<Sentence>();
                foreach (var sentenceSnapshot in gameStateSnapshot.CollectedSentences)
                {
                    collectedSentences.Add(sentenceSnapshot.GetSentence());
                }
                SentenceProcessor.SetCachedSentences(collectedSentences);
        
                GameObject.FindWithTag("Player").transform.position = gameStateSnapshot.PlayerPosition;
                CameraController.Instance.SetRotation(gameStateSnapshot.PlayerRotation);
        
                Timer.Instance.CurrentTime = gameStateSnapshot.time;
                Timer.Instance.IsRunning = GameManager.Instance.LevelManager.CurrentLevel.IsTimerActive;
                
                IEnumerable<IDataHandler<InteractableObjectSnapshot>> interactableObjects =
                    FindObjectsByType<InteractableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                        .Select(interactableObject => interactableObject.GetComponent<IDataHandler<InteractableObjectSnapshot>>());

                foreach (var interactableObject in interactableObjects)
                {
                    InteractableObjectSnapshot interactableObjectSnapshot = gameStateSnapshot.interactableObjects
                        .FirstOrDefault(snapshot => snapshot.hash == interactableObject.GetHash());
                    if (interactableObjectSnapshot != null && interactableObjectSnapshot.dialoguePath != null)
                    {
                        interactableObject.LoadData(interactableObjectSnapshot);
                    }
                }
            });
    }

    private List<WordSnapshot> CaptureWordPictures()
    {
        List<WordSnapshot> wordPictures = new List<WordSnapshot>();
        var words = WordManager.Instance.Words;

        foreach (var word in words)
        {
            WordSnapshot wordSnapshot = new WordSnapshot();
            wordSnapshot.wordNodeSnapshot = CaptureWordNodePicture(word.WordNode);
            wordSnapshot.Position = word.transform.position;
            wordPictures.Add(wordSnapshot);
        }
        
        return wordPictures;
    }
    
    private List<SentenceSnapshot> CaptureCollectedSentences()
    {
        List<SentenceSnapshot> sentenceSnapshots = new List<SentenceSnapshot>();
        var sentences = SentenceProcessor.CachedSentencesList;

        foreach (var sentence in sentences)
        {
            SentenceSnapshot sentenceSnapshot = new SentenceSnapshot(sentence);
            sentenceSnapshots.Add(sentenceSnapshot);
        }
        
        return sentenceSnapshots;
    }
    
    private WordNodeSnapshot CaptureWordNodePicture(WordNode wordNode)
    {
        WordNodeSnapshot wordNodeSnapshot = new WordNodeSnapshot();
        wordNodeSnapshot.WordData = new WordDataSnapshot(wordNode.WordData);
        
        if (wordNode.ParentWords.Count > 0 && wordNode.ParentWords[0] != null)
        {
            wordNodeSnapshot.ParentWords[0] = CaptureWordNodePicture(wordNode.ParentWords[0]);
        }
        
        if (wordNode.ParentWords.Count > 1 && wordNode.ParentWords[1] != null)
        {
            wordNodeSnapshot.ParentWords[1] = CaptureWordNodePicture(wordNode.ParentWords[1]);
        }

        return wordNodeSnapshot;
    }
}
