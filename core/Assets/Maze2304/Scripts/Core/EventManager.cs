using System;
using System.Collections.Generic;

namespace Core
{
    public class EventManager
    {
        static private Dictionary<Type, Action<EventKind>> eventListeners = new Dictionary<Type, Action<EventKind>>();
        static private Dictionary<Delegate, Action<EventKind>> eventLookUpTable = new Dictionary<Delegate, Action<EventKind>>();
        
        static public void AddListener<T>(Action<T> listener) where T : EventKind
        {
            if (!eventLookUpTable.ContainsKey(listener))
            {
                Action<EventKind> newAction =  (e) => listener((T) e);
                eventLookUpTable[listener] = newAction;

                var type = typeof(T);
                if (eventListeners.TryGetValue(type, out Action<EventKind> actions))
                {
                    eventListeners[type] = actions += newAction;
                }
                else
                {
                    eventListeners[type] = newAction;
                }
            }
        }

        static public void RemoveListener<T>(Action<T> listener) where T : EventKind
        {
            if (eventLookUpTable.TryGetValue(listener, out var target))
            {
                var type = typeof(T);
                if (eventListeners.TryGetValue(type, out var actions))
                {
                    actions -= target;
                    if (actions == null)
                    {
                        eventListeners.Remove(type);
                    }
                    else
                    {
                        eventListeners[type] = actions;
                    }
                }

                eventLookUpTable.Remove(listener);
            }
        }

        static public void Broadcast(EventKind evt)
        {
            if (eventListeners.TryGetValue(evt.GetType(), out var actions))
            {
                actions.Invoke(evt);
            }
        }
    }
}
