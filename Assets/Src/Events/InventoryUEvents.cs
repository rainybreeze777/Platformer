using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ObtainItemUEvent : UnityEvent<InvtItem> {}
[System.Serializable]
public class SpendItemUEvent : UnityEvent<InvtItem> {}