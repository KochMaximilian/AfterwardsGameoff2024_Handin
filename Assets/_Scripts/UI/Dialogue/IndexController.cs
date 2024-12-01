using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityUtils;

public class IndexController
{
    private readonly TMP_Text _dialogueText;
    private readonly GameObject _index;
    private readonly float _whenIndexSpacing;
    private readonly float _indexAppearTime;
    private readonly float _indexPerCharTime;
    private readonly float _indexYOffset;
    private readonly float _collectAnimationTime;
    private readonly float _penalizationTime;
    
    private float _penalizedUtil;
    private readonly ObjectPool<ParticleSystem> _wordCollectParticlesPool;
    
    public IndexController(TMP_Text dialogueText, GameObject index, GameObject wordCollectParticlesPrefab, Transform particleParent, float whenIndexSpacing, float indexAppearTime, float indexPerCharTime, float indexYOffset, float collectAnimationTime, float penalizationTime)
    {
        _dialogueText = dialogueText;
        _index = index;
        _whenIndexSpacing = whenIndexSpacing;
        _indexAppearTime = indexAppearTime;
        _indexPerCharTime = indexPerCharTime;
        _indexYOffset = indexYOffset;
        _collectAnimationTime = collectAnimationTime;
        _penalizationTime = penalizationTime;
        
        _wordCollectParticlesPool = new ObjectPool<ParticleSystem>(
            () => Object.Instantiate(wordCollectParticlesPrefab, particleParent).GetComponent<ParticleSystem>(),
            x => x.gameObject.SetActive(true),
            x => x.gameObject.SetActive(false),
            x => Object.Destroy(x.gameObject));
    }
    
    public IEnumerator StartCollectingIndex(Sentence sentence)
    {
        float initialSpacing = _dialogueText.lineSpacing;
        yield return AnimateLineSpacing(initialSpacing, _whenIndexSpacing, _indexAppearTime);

        int lineCount = _dialogueText.textInfo.lineCount;
        Vector3[] startPositions, endPositions;
        int[] charCounts;
        PrepareLineData(out startPositions, out endPositions, out charCounts, lineCount);

        _index.SetActive(true);

        // Forward collection
        yield return ProcessLines(sentence, startPositions, endPositions, charCounts, true);

        // Backward collection if necessary
        if (sentence.HasWordsToCollect)
        {
            yield return ProcessLines(sentence, startPositions, endPositions, charCounts, false);
        }

        _index.SetActive(false);
        yield return AnimateLineSpacing(_whenIndexSpacing, initialSpacing, _indexAppearTime);
    }

    private IEnumerator AnimateLineSpacing(float from, float to, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            _dialogueText.lineSpacing = Mathf.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _dialogueText.lineSpacing = to;
    }

    private void PrepareLineData(out Vector3[] startPositions, out Vector3[] endPositions, out int[] charCounts, int lineCount)
    {
        startPositions = new Vector3[lineCount];
        endPositions = new Vector3[lineCount];
        charCounts = new int[lineCount];

        for (int i = 0; i < lineCount; i++)
        {
            TMP_LineInfo lineInfo = _dialogueText.textInfo.lineInfo[i];
            int startChar = GetFirstVisibleCharacter(lineInfo);
            int endChar = GetLastVisibleCharacter(lineInfo);

            startPositions[i] = _dialogueText.GetWordCenter(startChar, startChar) + Vector3.up * _indexYOffset;
            endPositions[i] = _dialogueText.GetWordCenter(endChar, endChar) + Vector3.up * _indexYOffset;
            charCounts[i] = endChar - startChar + 1;
        }
    }

    private int GetFirstVisibleCharacter(TMP_LineInfo lineInfo)
    {
        int index = lineInfo.firstCharacterIndex;
        while (!_dialogueText.textInfo.characterInfo[index].isVisible)
        {
            index++;
        }
        return index;
    }

    private int GetLastVisibleCharacter(TMP_LineInfo lineInfo)
    {
        int index = lineInfo.lastCharacterIndex;
        while (!_dialogueText.textInfo.characterInfo[index].isVisible)
        {
            index--;
        }
        return index;
    }

    private IEnumerator ProcessLines(Sentence sentence, Vector3[] startPositions, Vector3[] endPositions, int[] charCounts, bool isForward)
    {
        int lineCount = startPositions.Length;
        int offset = isForward ? 0 : GetTotalCharCount(charCounts);

        int startIndex = isForward ? 0 : lineCount - 1;
        int endIndex = isForward ? lineCount : -1;
        int step = isForward ? 1 : -1;

        for (int i = startIndex; i != endIndex; i += step)
        {
            yield return ProcessLine(sentence, startPositions[i], endPositions[i], charCounts[i], offset, isForward);
            offset += isForward ? charCounts[i] : -charCounts[i];
            if (!sentence.HasWordsToCollect) break;
        }
    }

    private IEnumerator ProcessLine(Sentence sentence, Vector3 start, Vector3 end, int charCount, int offset, bool isForward)
    {
        if (!isForward)
        {
            (start, end) = (end, start);
        }
        
        _penalizedUtil = 0;
        float elapsedTime = 0f;
        float timePerChar = charCount * _indexPerCharTime;

        _index.transform.position = start;
        float xPosition;
        float yPosition = _index.transform.position.y;

        while (elapsedTime < timePerChar)
        {
            xPosition = Mathf.Lerp(start.x, end.x, elapsedTime / timePerChar);
            yPosition = Mathf.Lerp(yPosition, end.y, Time.deltaTime / _collectAnimationTime);
            _index.transform.position = new Vector3(xPosition, yPosition, start.z);
            elapsedTime += Time.deltaTime;
            
            //Ignore input if penalized
            if (elapsedTime < _penalizedUtil)
            {
                yield return null;
                continue;
            }
            
            if (InputManager.Instance.SpaceBarInput.WasPressedThisFrame())
            {
                int charIndex = isForward
                    ? offset + (int)(elapsedTime / _indexPerCharTime)
                    : offset - (int)(elapsedTime / _indexPerCharTime);

                yield return HandleWordCollection(sentence, elapsedTime, charIndex, _index.transform.position);
                if (!sentence.HasWordsToCollect) break;
            }
            yield return null;
        }
    }

    private IEnumerator HandleWordCollection(Sentence sentence, float elapsedTime, int charIndex, Vector3 currentPosition)
    {
        float animationTime = 0f;

        Vector3 initialPosition = currentPosition;
        Vector3 targetPosition = initialPosition - Vector3.up * _indexYOffset;

        while (animationTime < _collectAnimationTime)
        {
            _index.transform.position = Vector3.Lerp(initialPosition, targetPosition, animationTime / _collectAnimationTime);
            animationTime += Time.deltaTime;
            yield return null;
        }

        if (sentence.TryCollectWord(charIndex, out var keyWord) )
        {
            _dialogueText.text = sentence.Text;
            Vector3 wordPosition = _dialogueText.GetWordCenter(keyWord.noTagsIndex, keyWord.noTagsIndex + keyWord.text.Length / 2);
            CoroutineController.Start(PlayParticles(wordPosition));
            AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.IndexHit);
        }
        else
        {
            PenalizeInput(elapsedTime);
            AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.IndexMiss);
        }
    }
    
    private IEnumerator PlayParticles(Vector3 position)
    {
        var particles = _wordCollectParticlesPool.Get();
        var main = particles.main;
        
        particles.transform.position = position;
        particles.Play();

        yield return new WaitForSeconds(main.startLifetime.constantMax);
        _wordCollectParticlesPool.Release(particles);
    }

    private void PenalizeInput(float elapsedTime)
    {
        Image indexImage = _index.GetComponent<Image>();
        Color originalColor = indexImage.color;
        indexImage.DOFade(0, _penalizationTime / 5f)
            .SetLoops(5, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => indexImage.color = originalColor);
        
        _penalizedUtil = elapsedTime + _penalizationTime;
    }

    private int GetTotalCharCount(int[] charCounts)
    {
        int total = 0;
        foreach (int count in charCounts)
        {
            total += count;
        }
        return total;
    }
}