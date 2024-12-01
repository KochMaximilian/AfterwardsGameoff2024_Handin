using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityUtils;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WordInputManager))]
public class WordManager : Singleton<WordManager>
{
    #region Fields

    [Header("References")]
    [SerializeField] private GameObject wordPrefab;
    [SerializeField] private Transform wordContainer;
    
    [Header("Data")]
    [SerializeField] private CombinationTable combinationTable;
    [SerializeField] private int poolSize = 10;
    
    private ObjectPool<Word> _wordPool;
    private WordMerger _wordMerger;
    private List<Word> _words;
    private WordInputManager _wordInputManager;

    #endregion

    #region Properties

    public CombinationTable CombinationTable => combinationTable;
    public Transform WordContainer => wordContainer;
    public IReadOnlyList<Word> Words => _words;
    public WordInputManager WordInputManager => _wordInputManager;

    #endregion
    
    #region Initialization

    protected override void Awake()
    {
        base.Awake();

        // Set the pivot of the word container to the bottom left corner to match position calculations
        combinationTable = Instantiate(combinationTable);
        _wordMerger = new WordMerger(this);
        _wordInputManager = GetComponent<WordInputManager>();

        _words = new List<Word>();
        InitializePool();
    }
    
    private void InitializePool()
    {
        _wordPool = new ObjectPool<Word>(
            () =>
            {
                var word = Instantiate(wordPrefab, wordContainer).GetComponent<Word>();
                return word;
            }, 
            x => x.gameObject.SetActive(true), 
            x => x.gameObject.SetActive(false), 
            x => Destroy(x.gameObject));
        
        List<Word> words = new List<Word>();
        for(int i = 0; i < poolSize; i++)
        {
            words.Add(_wordPool.Get());
        }
        foreach (var word in words)
        {
            _wordPool.Release(word);
        }
    }

    public void ClearWords()
    {
        foreach (var word in _words)
        {
            Destroy(word.gameObject);
        }   
        _words.Clear();
        _wordPool.Clear();
        _wordPool.Dispose();
        
        InitializePool();
    }
    
    #endregion

    #region Merge Methods
    public async UniTask<Word> Merge(Word word1, Word word2)
    { 
        if(word1.Visuals.DuringAnimation || word2.Visuals.DuringAnimation) return null;

        return await _wordMerger.Merge(word1, word2);
    }
    
    public async UniTask<bool> TrySplit(Word word)
    {
        if(word.Visuals.DuringAnimation) return false;
        
        return await _wordMerger.TrySplit(word);
    }
    #endregion
    
    #region Word Creation
    public async UniTask<Word> CreateWordAsync(WordNode wordNode)
    {
        //Create the word
        Word word = _wordPool.Get();
        word.Initialize(wordNode);
        _words.Add(word);
        
        // Check if the word is out of bounds and move it in
        var mover = WordMovementManager.Instance;
        Vector3 position = mover.GetRandomPositionInBounds(word.Rect);
        mover.InstantMoveTo(word.GetComponent<RectTransform>(), position);
        
        await word.Visuals.AppearAsync();
        return word;
    }
    
    public async UniTask<Word> CreateWordAsync(WordNode wordNode, Vector3 position)
    {
        //Create the word
        Word word = _wordPool.Get();
        word.Initialize(wordNode);
        _words.Add(word);

        // Check if the word is out of bounds and move it in
        var mover = WordMovementManager.Instance;
        mover.InstantMoveTo(word.GetComponent<RectTransform>(), position);
        
        await word.Visuals.AppearAsync();
        return word;
    }
    
    public async UniTask<Word> CreateWordAsync(WordNode wordNode, Vector3 position, Vector3 finalPosition)
    {
        //Create the word
        Word word = _wordPool.Get();
        word.Initialize(wordNode);
        _words.Add(word);

        //Explosion movement effect
        finalPosition = WordMovementManager.Instance.GetClosestInBoundsPoint(word.Rect, finalPosition);
        await word.Visuals.AppearAsync(position, finalPosition);
        return word;
    }

    public async UniTask DestroyAsync(Word word)
    {
        if(word.Visuals.DuringAnimation) return;
        
        _words.Remove(word);
        await word.Visuals.DisappearAsync();
        
        _wordPool.Release(word);
    }
    #endregion
}