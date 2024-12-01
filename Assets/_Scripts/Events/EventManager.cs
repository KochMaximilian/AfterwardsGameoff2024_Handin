using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onPrologueEnd;
    [SerializeField] private UnityEvent onLevel1End;
    
    private EventBinding<OnPrologueEnd> _prologueEndBinding;
    private EventBinding<OnLevel1End> _level1EndBinding;

    private void Awake()
    {
        _prologueEndBinding = new EventBinding<OnPrologueEnd>(onPrologueEnd.Invoke);
        _level1EndBinding = new EventBinding<OnLevel1End>(onLevel1End.Invoke);
        
        EventBus<OnPrologueEnd>.Register(_prologueEndBinding);
        EventBus<OnLevel1End>.Register(_level1EndBinding);
    }
    
    private void OnDestroy()
    {
        EventBus<OnPrologueEnd>.Deregister(_prologueEndBinding);
        EventBus<OnLevel1End>.Deregister(_level1EndBinding);
    }
    
    public void GoToNextLevel()
    {
        GameManager.Instance.LevelManager.TransitionToNextLevel();
    }
}