using System;
using System.Collections.Generic;
using OpenEoB.Events;
using UnityEngine;
using Object = System.Object;

namespace OpenEoB
{
    public class EventManager : MonoBehaviour
    {
        private struct EventAction
        {
            public Action Action;
            public Object Owner;

            public EventAction(Object owner, Action action)
            {
                Action = action;
                Owner = owner;
            }
        }

        public static EventManager Instance;

        private Dictionary<EventId, List<EventAction>> _actions;

        private void Awake()
        {
            if (Instance != null)
            {
                throw new Exception("An EventManager already exists");
            }

            Instance = this;

            _actions = new Dictionary<EventId, List<EventAction>>();
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void RegisterAction(EventId eventId, Object owner, Action action)
        {
            if (!_actions.ContainsKey(eventId))
            {
                _actions[eventId] = new List<EventAction>();
            }

            _actions[eventId].Add(new EventAction(owner, action));
        }

        public void TriggerEvent(EventId eventId)
        {
            UnityEngine.Debug.Log("Event triggered: eventId");
            if (_actions.ContainsKey(eventId))
            {
                foreach (var eventAction in _actions[eventId])
                {
                    eventAction.Action();
                }
            }
        }

        public void UnregisterActionsForOwner(Object owner)
        {
            foreach (var eventActions in _actions.Values)
            {
                var index = 0;
                while (index < eventActions.Count)
                {
                    var commandAction = eventActions[index];
                    if (commandAction.Owner == owner)
                    {
                        eventActions.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }
    }
}
