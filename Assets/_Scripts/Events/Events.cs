using System;
using System.Collections.Generic;
using UnityEngine;

public enum Events
{
    OnPrologueEnd,
    OnLevel1End
}

public struct OnPrologueEnd : IEvent { }

public struct OnLevel1End : IEvent { }

public static class EventFactory
{
    public static IEvent CreateEvent(Events eventType)
    {
        switch (eventType)
        {
            case Events.OnPrologueEnd:
                return new OnPrologueEnd();
            case Events.OnLevel1End:
                return new OnLevel1End();
            default:
                return null;
        }
    }
    
    public static void Raise(Events eventType)
    {
        Debug.Log($"Event raised: {eventType.GetType().Name}");
        EventBusUtil.RaiseEvent(CreateEvent(eventType));
    }
}