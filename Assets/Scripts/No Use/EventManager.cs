using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public enum EVENT_CODE
    {
        UNIT_SELECT,
        CLICK_TILE,
    }

    private static Dictionary<EVENT_CODE, UnityAction<GameObject>> eventDict = 
        new Dictionary<EVENT_CODE, UnityAction<GameObject>>();

    public static void Subscribe(EVENT_CODE eventCode, UnityAction<GameObject> listener)
    {
        if (eventDict.TryGetValue(eventCode, out var thisEvent)) {
            thisEvent += listener;

            eventDict[eventCode] = thisEvent;
        } else
        {
            eventDict[eventCode] = listener;
        }
    }

    public static void Unsubscribe(EVENT_CODE eventCode, UnityAction<GameObject> listener)
    {
        if (eventDict.TryGetValue(eventCode, out var thisEvent))
        {
            thisEvent -= listener;
            eventDict[eventCode] = thisEvent;
        }
    }

    public static void RunEvent(EVENT_CODE eventCode, GameObject param)
    {
        if (eventDict.TryGetValue(eventCode, out var thisEvent))
        {
            thisEvent?.Invoke(param);
        }
    }
}