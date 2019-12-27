using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DuckingUEvent : UnityEvent<bool> {}
[System.Serializable]
public class JumpUEvent : UnityEvent {}
[System.Serializable]

public class JumpReleaseUEvent : UnityEvent {}