using System.Collections;
using System.Linq;
using UnityEngine;
using UnityUtils;

public class OptionsSelector : MonoBehaviour
{
    [SerializeField] private Transform optionsParent;
    [SerializeField] private GameObject optionPrefab;

    private RectTransform _rectTransform;
    private float _height;
    
    private bool _isShowing;

    private void Start()
    {
        _height = optionPrefab.GetComponent<RectTransform>().rect.height;
        _rectTransform = GetComponent<RectTransform>();
        
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        transform.LookAt(Helpers.Camera.transform);
    }
    
    public IEnumerator ShowOptions(DialogueOption[] options, Transform optionsTransform)
    {
        if(options == null || options.Length == 0) yield break;
        _isShowing = true;
        gameObject.SetActive(true);
        transform.SetParent(optionsTransform);
        transform.localPosition = Vector3.zero;
        
        foreach (Transform child in optionsParent)
        {
            Destroy(child.gameObject);
        }

        Option[] optionComponents = new Option[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            GameObject option = Instantiate(optionPrefab, optionsParent);
            optionComponents[i] = option.GetComponent<Option>();
            optionComponents[i].Initialize(options[i]);
        }
        
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, options.Length * _height);

        while (optionComponents.All(x => !x.IsSelected) && _isShowing)
        {
            yield return null;
        }
        
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        _isShowing = false;
        gameObject.SetActive(false);
    }
}