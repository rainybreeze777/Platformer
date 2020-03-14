using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DuckingUEvent : UnityEvent<bool> {}
[System.Serializable]
public class JumpUEvent : UnityEvent {}
[System.Serializable]
public class JumpReleaseUEvent : UnityEvent {}
[System.Serializable]
public class AboutToDieUEvent : UnityEvent {}
[System.Serializable]
public class DeadUEvent : UnityEvent {}
[System.Serializable]
public class PlayerSpawnedUEvent : UnityEvent {}