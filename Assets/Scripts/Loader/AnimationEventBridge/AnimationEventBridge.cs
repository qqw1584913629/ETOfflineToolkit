using UnityEngine;
using System;
using System.Collections.Generic;

namespace MH 
{
    public class AnimationEventBridge : MonoBehaviour 
    {
        public Dictionary<string, Action> eventCallbacks = new Dictionary<string, Action>();
        
        // Unity动画事件会调用这个方法
        public void HandleAnimationEvent(string eventId)
        {
            if (eventCallbacks.TryGetValue(eventId, out var callback))
            {
                callback?.Invoke();
            }
        }

        public void RegisterEvent(string eventId, Action callback)
        {
            eventCallbacks[eventId] = callback;
        }

        public void ClearEvents()
        {
            eventCallbacks.Clear();
        }
    }
} 