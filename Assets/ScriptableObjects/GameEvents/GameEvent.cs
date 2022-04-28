using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Essentially a scriptable object wrapper for a UnityEvent so that you can distribute it in the editor
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameEvent", order = 1), System.Serializable]
public class GameEvent : ScriptableObject
{
    private UnityEvent internalEvent = new UnityEvent();
    public void Raise(){ internalEvent.Invoke(); }
    public void RegisterListener(UnityAction listener){ internalEvent.AddListener(listener); }
    public void UnregisterListener(UnityAction listener){ internalEvent.RemoveListener(listener); }
}