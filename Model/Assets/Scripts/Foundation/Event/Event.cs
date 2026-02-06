using System;
using System.Collections.Generic;
using Foundation.CommonModel;
using UnityEngine;

namespace Foundation.Event
{
    public class EventDelegate
    {
        public readonly Type eventType;
        public readonly Delegate eventDelegate;

        public EventDelegate(Type inEventType, Delegate inEventDelegate)
        {
            eventType = inEventType;
            eventDelegate = inEventDelegate;
        }
    }
    
    public class Event : Singleton<Event> 
    {
        private readonly Dictionary<Type, List<Delegate>> _eventTable = new Dictionary<Type, List<Delegate>>(8);
        public void OnInit()
        {
            
        }

        public void OnDestroy()
        {
            foreach (var pair in _eventTable)
            {
                pair.Value.Clear();
            }

            _eventTable.Clear();
        }
        
        
        public EventDelegate Subscribe<T>(Action<T> handler)
        {
            if (handler == null)
            {
                Debug.Log(" [ Subscribe ] " + typeof(T).Name + " handler null " );
                return null;
            }

            Debug.Log(" [ Subscribe ] " + typeof(T).Name + " | " + handler.Method.Name);
            Type key = typeof(T);
            List<Delegate> handlers;
            if (!_eventTable.TryGetValue(key, out handlers))
            {
                handlers = new List<Delegate>(4);
                _eventTable.Add(key, handlers);
            }

            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }

            EventDelegate listener = new EventDelegate(key, handler);
            return listener;
        }

        public void UnSubscribe(EventDelegate e)
        {
            if (e == null)
            {
                return;
            }

            Type key = e.eventType;
            List<Delegate> handlers;
            if (_eventTable.TryGetValue(key, out handlers))
            {
                if (handlers.Contains(e.eventDelegate))
                {
                    handlers.Remove(e.eventDelegate); 
                } 
            }
        }

        public void SendEvent<T>(T eventData = default)
        {
            Type key = typeof(T);
            List<Delegate> handlers;
            if (!_eventTable.TryGetValue(key, out handlers))
            {
                return;
            }

            int count = handlers.Count;
            for (int i = 0; i < count; i++)
            {
                var action = handlers[i] as Action<T>;
                if (action != null)
                {
                    action(eventData);
                }
            }
        }
        
    }
}