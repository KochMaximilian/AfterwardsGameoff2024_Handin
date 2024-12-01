using UnityEngine;

public class WordProvider : MonoBehaviour, IWordProvider
{
    [SerializeField] private WordToProvide[] words;
    
    private void Start()
    {
        foreach (WordToProvide word in words)
        {
            if (word.provideOnStart)
            {
                word.Provide();
            }
        }
    }
    
    public void ProvideWords()
    {
        foreach (WordToProvide word in words)
        {
            word.Provide();
        }
    }
    
    public bool TryProvide(string word, int amount, out int providedAmount)
    {
        foreach (WordToProvide wordToProvide in words)
        {
            if (wordToProvide.wordData.Text == word)
            {
                providedAmount = wordToProvide.Provide(amount);
                return providedAmount > 0;
            }
        }
        
        providedAmount = 0;
        return false;
    }
}