using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityUtils;

public class LevelTransitioner : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Image fadeImage;

    [Header("Settings")] [SerializeField] private float fadeDuration = 1f;

    private bool _duringTransition;
    private float _time;
    
    public bool DuringTransition => _duringTransition;

    public void TransitionToLevel(SceneEnum scene, Action callback)
    {
        if (_duringTransition)
        {
            return;
        }

        StartCoroutine(TransitionToLevelCoroutine(scene, callback));
    }

    private IEnumerator TransitionToLevelCoroutine(SceneEnum scene, Action callback)
    {
        _duringTransition = true;
        _time = 0;

        fadeImage.DOFade(0, 0);
        fadeImage.DOFade(1, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)scene);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f || _time < fadeDuration)
        {
            _time += Time.deltaTime;
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
        
        bool completed = false;
        asyncOperation.completed += _ => completed = true;
        
        yield return new WaitUntil(() => completed);
        yield return WaitFor.Seconds(fadeDuration);

        fadeImage.DOFade(0, fadeDuration);
        _duringTransition = false;
        InitializeAllObjects();
        callback?.Invoke();
    }
    
   

    private void InitializeAllObjects()
    {
        var initializers = FindObjectsOfType<MonoBehaviour>().OfType<ISceneInitializer>();
        foreach (var initializer in initializers)
        {
            initializer.OnStartScene();
        }
    }

    public void TransitionToLevelImmediate(SceneEnum levelScene, Action onLevelChanged)
    {
        SceneManager.LoadScene((int)levelScene);
        InitializeAllObjects();
        fadeImage.DOFade(1, 0);
        fadeImage.DOFade(0, fadeDuration);
        onLevelChanged?.Invoke();
    }
}