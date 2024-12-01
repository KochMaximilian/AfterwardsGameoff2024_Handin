public class BaseState : IState
{
    protected BaseState()
    {
    }
    
    public virtual void Enter() {}

    public virtual void Update() {}

    public virtual void FixedUpdate() {}

    public virtual void Exit() {}
}