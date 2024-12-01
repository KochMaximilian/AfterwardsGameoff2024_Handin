using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityUtils;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class WordMerger 
{
    private readonly WordManager _wordManager;
    
    public WordMerger(WordManager wordManager)
    {
        _wordManager = wordManager;
    }
    
    public async UniTask<Word> Merge(Word word1, Word word2)
    {
        if(!word1.WordNode.WordData.AllowCombining || !word2.WordNode.WordData.AllowCombining) return null;
        
        Combination wordDataCombination = _wordManager.CombinationTable.GetCombination(word1.WordNode.WordData, word2.WordNode.WordData);
        
        WordData outputWord;
        if (wordDataCombination == null)
        {
            // If there is no combination, try to combine the words literally
            if(!TryCombine(word1.WordNode.WordData, word2.WordNode.WordData, out outputWord)) 
                return null;
        }
        else outputWord = wordDataCombination.OutputWord;
        
        WordNode[] nodes = {word1.WordNode, word2.WordNode};
        int[] depths = nodes.Select(node => node.Depth).ToArray();
        Vector3 position = word1.transform.position;
        
        await UniTask.WhenAll(_wordManager.DestroyAsync(word2), _wordManager.DestroyAsync(word1));
        WordVfxManger.Instance.PlayParticles(WordCombinationType.Merge, position, depths);
        WordSounds.PlayMergeSound(true);
        
        return await _wordManager.CreateWordAsync(new WordNode(outputWord, nodes[0], nodes[1]), position);
    }
    
    public async UniTask<bool> TrySplit(Word word)
    {
        if (word.WordNode.Depth == 0)
        {
            return false;
        }
        
        WordNode parentWord1 = word.WordNode.ParentWords[0];
        WordNode parentWord2 = word.WordNode.ParentWords[1];
        Vector3 position = word.transform.position;
        int[] depths = {parentWord1.Depth, parentWord2.Depth};  
        
        await _wordManager.DestroyAsync(word);
        WordVfxManger.Instance.PlayParticles(WordCombinationType.Split, position, depths);
        WordSounds.PlaySplitSound(true);

        Vector3 randomOffset = new Vector3(50 * Random.Range(0.5f, 1f) * Helpers.GetRandomSign(), 50 * Random.Range(0.5f, 1f) * Helpers.GetRandomSign());
        Vector3 finalPos = word.transform.parent.TransformPoint(randomOffset + word.transform.localPosition);
        _ = _wordManager.CreateWordAsync(parentWord1, position,  finalPos);
        
        finalPos = word.transform.parent.TransformPoint(word.transform.localPosition - randomOffset);
        _ = _wordManager.CreateWordAsync(parentWord2, position,  finalPos);
        
        return true;
    }
    
    private bool TryCombine(WordData word1, WordData word2, out WordData outputWord)
    {
        if(!word1.AllowCombining || !word2.AllowCombining || !word1.AllowLiteralCombining || !word2.AllowLiteralCombining) 
        {
            outputWord = null;
            return false;
        }
        
        outputWord = ScriptableObject.CreateInstance<WordData>();
        outputWord.Text = word1.Text + word2.Text;
        outputWord.AllowCombining = true;
        outputWord.AllowLiteralCombining = true;
        return true;
    }
}