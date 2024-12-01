using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState CurrentState => _currentState.State;
    
    private StateNode _currentState;
    private readonly Dictionary<Type, StateNode> _nodes = new();
    private readonly HashSet<ITransition> _anyTransition = new();

    public void Update()
    {
        ITransition transition = GetTransition();
        if (transition != null && transition.To != _currentState.State)
            ChangeState(transition.To);
        
        _currentState.State?.Update();
    }

    private ITransition GetTransition()
    {
        foreach (var transition in _anyTransition)
            if (transition.Condition.Evaluate())
                return transition;

        foreach (var transition in _currentState.Transitions)
            if (transition.Condition.Evaluate())
                return transition;

        return null;
    }

    public void FixedUpdate()
    {
        _currentState.State?.FixedUpdate();
    }

    public void SetState(IState state)
    {
        _currentState = GetOrAddNode(state);
        _currentState.State?.Enter();
    }

    private void ChangeState(IState state)
    {
        _currentState.State?.Exit();
        _currentState = GetOrAddNode(state);
        _currentState.State?.Enter();
    }
    
    public void At(IState from, IState to, IPredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }
    
    public void Any(IState to, IPredicate condition)
    {
        _anyTransition.Add(new Transition(to, condition));
    }

    private StateNode GetOrAddNode(IState state)
    {
        if (!_nodes.ContainsKey(state.GetType()))
            _nodes.Add(state.GetType(), new StateNode(state));

        return _nodes[state.GetType()];
    }


    class StateNode
    {
        public IState  State { get;}
        public HashSet<ITransition> Transitions { get; }

        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<ITransition>();
        }
        
        public void AddTransition(IState to, IPredicate condition)
        {
            Transitions.Add(new Transition(to, condition));
        }
    }

}