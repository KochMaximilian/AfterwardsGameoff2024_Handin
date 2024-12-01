using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityUtils;

public class DialogueManager : PersistentSingleton<DialogueManager>
{
    public static event Action<NPCDialogue, int> OnSentenceStart;
    public static event Action<NPCDialogue, int> OnSentenceEnd;
    
    
    [Header("References")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Animator animator;
    [SerializeField] private OptionsSelector optionsSelector;
        
    [Header("Index References")]
    [SerializeField] private GameObject index;
    [SerializeField] private GameObject wordCollectParticlesPrefab;
    [SerializeField] private Transform particlesParent;
    
    [Header("Settings")]
    [SerializeField] private float timeBetweenLetters = 0.02f;
    [SerializeField] private float fastForwardMultiplier = 3f;
    [SerializeField] private float dialogMaxDistance = 10f;

    [Header("Index Settings")]
    [SerializeField] private float indexAppearTime = 0.5f;
    [SerializeField] private float indexPerCharTime = 0.05f;
    [SerializeField] private float whenIndexSpacing = 60f;
    [SerializeField] private float indexYOffset = 0.25f;
    [SerializeField] private float penalizationTime = 0.2f;
    [SerializeField] private float collectAnimationTime = 0.1f;
    
    private bool _isDialogueActive;
    private IndexController _indexController;
    private List<Sentence> _sentences;
    private NPCDialogue _currentDialogue;
    private int _currentSentenceIndex;
    private int _charsSinceLastSound;
    private int _charsPerSound;
    
    private Transform _conversationOrigin;
    private Transform _optionsTransform;
    
    private Transform _playerTransform;
    private Action _onBreakCallback;
    private Action _onEndCallback;
    
    public bool IsDialogueActive => _isDialogueActive;
    public bool IsTalking { get; private set; }

    private void Start()
    {
        _isDialogueActive = false;
        _sentences = new List<Sentence>();
        _indexController = 
            new IndexController
            (
                dialogueText,
                index,
                wordCollectParticlesPrefab,
                particlesParent,
                whenIndexSpacing,
                indexAppearTime,
                indexPerCharTime, 
                indexYOffset,
                collectAnimationTime,
                penalizationTime
            );
        
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        CheckDialogueDistance();
    }

    public void StartDialogue(DialogueInteractable interactable, bool hideIfTooFar, Action onBreak = null, Action onEnd = null)
    {
        if(_isDialogueActive) return;
        
        _optionsTransform = interactable.OptionsPosition;
        _conversationOrigin = hideIfTooFar? interactable.transform : _playerTransform;
        _onBreakCallback = onBreak;
        _onEndCallback = onEnd;
        
        if(interactable.Dialogue.Dialogue.Length == 0)
        {
            if(interactable.Dialogue.Options.Length > 0)
            {
                StartCoroutine(optionsSelector.ShowOptions(interactable.Dialogue.Options, _optionsTransform));
            }
            
            return;
        }
        
        StartDialogue(interactable.Dialogue);
    }
  

    private void StartDialogue(NPCDialogue dialogue)
    {
        if(_isDialogueActive || dialogue.Dialogue.Length == 0) return;
        
        nameText.text = dialogue.actor.actorName;
        _currentDialogue = dialogue;
        _isDialogueActive = true;
        
        _charsPerSound = dialogue.actor.charactersPerSound;
        _charsSinceLastSound = _charsPerSound;
        animator.SetBool("IsOpen", true);
        
        _sentences.Clear();
        foreach (string sentence in dialogue.Dialogue)
        {
            _sentences.Add(SentenceProcessor.ProcessSentence(sentence, dialogue.keyWords));
        }
        
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.ConversationStart);

        _currentSentenceIndex = 0;
        StartCoroutine(AppearDialogue(_sentences[_currentSentenceIndex]));
    }
    
    private IEnumerator AppearDialogue(Sentence sentence)
    {
        OnSentenceStart?.Invoke(_currentDialogue, _currentSentenceIndex);
        
        dialogueText.text = "";
        string sentenceText = sentence.Text;
        float containerWidth = dialogueText.rectTransform.rect.width;
        IsTalking = true;

        for (var i = 0; i < sentenceText.Length; i++)
        {
            if(_charsSinceLastSound >= _charsPerSound)
            {
                AudioManager.Instance.PlaySoundAtPosition(_currentDialogue.actor.voice, _conversationOrigin.position);
                _charsSinceLastSound = 0;
            }
            else _charsSinceLastSound++;
            
            // Check if the current character starts a rich text tag and if so, add the whole tag to the dialogue text
            SkipTag(sentenceText, ref i);
            if(i >= sentenceText.Length) break;
            
            dialogueText.text +=  sentenceText[i];
            
            if(sentenceText[i] == ' ')
            {
                string testLine = dialogueText.text + sentenceText.GetNextWord(i + 1);
                Vector2 preferredValues = dialogueText.GetPreferredValues(testLine);

                if (preferredValues.x > containerWidth) dialogueText.text += "\n";
            }   
            
            float time = timeBetweenLetters;
            tutorialText.text = "Hold [SPACE] to fast forward...";
            if (InputManager.Instance.SpaceBarInput.IsPressed())
            {
                tutorialText.text = "";
                time /= fastForwardMultiplier;
            }

            yield return WaitFor.Seconds(time);
        }
        
        IsTalking = false;

        // Wait for the player to read the sentence
        tutorialText.text = "Press [SPACE] to continue...";
        while (!InputManager.Instance.SpaceBarInput.WasPressedThisFrame())
        {
            yield return null;
        }

        if (sentence.HasWordsToCollect)
        {
            tutorialText.text = "Press [SPACE] when the index is over a word to collect it...";
            yield return _indexController.StartCollectingIndex(sentence);
        }

        _charsSinceLastSound = _charsPerSound;
        
        //If there are no more sentences, display the options
        if (_currentSentenceIndex >= _sentences.Count-1 && _currentDialogue.Options.Length > 0)
        {
            yield return optionsSelector.ShowOptions(_currentDialogue.Options, _optionsTransform);
        }
        else DisplayNextSentence();
    }

    private void SkipTag(string sentenceText, ref int i)
    {
        while (i < sentenceText.Length && sentenceText[i] == '<' && i + 1 < sentenceText.Length)
        {
            var tagContent = "<"; 
            i++;

            while (i < sentenceText.Length && sentenceText[i] != '>')
            {
                tagContent += sentenceText[i++];
            }

            if (i < sentenceText.Length && sentenceText[i] == '>')
            {
                tagContent += '>'; 
                i++;
            }

            dialogueText.text += tagContent;
        }
    }

    private void DisplayNextSentence()
    {
        OnSentenceEnd?.Invoke(_currentDialogue, _currentSentenceIndex);
        
        _currentSentenceIndex++;
        if (_currentSentenceIndex >= _sentences.Count)
        {
            if(_currentDialogue.onEndDialogue != null)
            {
                _isDialogueActive = false;
                
                StartDialogue(_currentDialogue.onEndDialogue);
                return;
            }
            
            _onEndCallback?.Invoke();
            _onEndCallback = null;
            
            StopDialogue();
            return;
        }
        
        StartCoroutine(AppearDialogue(_sentences[_currentSentenceIndex]));
    }

    public void SelectOption(DialogueOption option)
    {
        if(option.dialogue != null)
        {
            _isDialogueActive = false;
            StartDialogue(option.dialogue);
        }
        else
        {
            DisplayNextSentence();
        }
    }
    
    private void CheckDialogueDistance()
    {
        if(_conversationOrigin == null) return;
        if(Vector3.Distance(_conversationOrigin.position, _playerTransform.position) > dialogMaxDistance)
        {
            StopDialogue();
        }
    }
    
    public void StopDialogue()
    {
        StopAllCoroutines();
        
        _onBreakCallback?.Invoke();
        _onBreakCallback = null;
        
        if (_currentSentenceIndex < _sentences.Count)
            OnSentenceEnd?.Invoke(_currentDialogue, _currentSentenceIndex);
        
        index.SetActive(false);
        dialogueText.lineSpacing = 0;
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.ConversationEnd);

        _isDialogueActive = false;
        animator.SetBool("IsOpen", false);
        optionsSelector.transform.SetParent(transform);
        optionsSelector.Hide();
    }
}