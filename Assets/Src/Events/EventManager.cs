using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {
  private Dictionary<Type, UnityEventBase> m_UEventsDict;
  public DuckingUEvent m_DuckingUEvent;
  public JumpUEvent m_JumpUEvent;
  public JumpReleaseUEvent m_JumpReleaseUEvent;

  void Awake() {
    m_DuckingUEvent = new DuckingUEvent();
    m_JumpUEvent = new JumpUEvent();
    m_JumpReleaseUEvent = new JumpReleaseUEvent();
    m_UEventsDict = new Dictionary<Type, UnityEventBase>();
    m_UEventsDict.Add(typeof(DuckingUEvent), m_DuckingUEvent);
    m_UEventsDict.Add(typeof(JumpUEvent), m_JumpUEvent);
    m_UEventsDict.Add(typeof(JumpReleaseUEvent), m_JumpReleaseUEvent);
    m_UEventsDict.Add(typeof(ObtainItemUEvent), new ObtainItemUEvent());
    m_UEventsDict.Add(typeof(SpendItemUEvent), new SpendItemUEvent());
  }

  public bool Invoke<UE0>()
    where UE0 : UnityEvent
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE0), out ue);
    Debug.Assert(res, "Event " + typeof(UE0) + " is not found! Did you forget to add it to m_UEventsDict in EventManager Awake() ?");
    if (res) {
      ((UE0) ue).Invoke();
    }
    return res;
  }

  public bool Invoke<UE1, T0>(T0 p)
    where UE1 : UnityEvent<T0>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE1), out ue);
    Debug.Assert(res, "Event " + typeof(UE1) + " is not found! Did you forget to add it to m_UEventsDict in EventManager Awake() ?");
    if (res)
    {
      ((UE1)ue).Invoke(p);
    }
    return res;
  }

  public bool Invoke<UE2, T0, T1>(T0 p1, T1 p2)
    where UE2 : UnityEvent<T0, T1>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE2), out ue);
    Debug.Assert(res, "Event " + typeof(UE2) + " is not found! Did you forget to add it to m_UEventsDict in EventManager Awake() ?");
    if (res)
    {
      ((UE2)ue).Invoke(p1, p2);
    }
    return res;
  }

  public bool Invoke<UE3, T0, T1, T2>(T0 p1, T1 p2, T2 p3)
    where UE3 : UnityEvent<T0, T1, T2>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE3), out ue);
    Debug.Assert(res, "Event " + typeof(UE3) + " is not found! Did you forget to add it to m_UEventsDict in EventManager Awake() ?");
    if (res)
    {
      ((UE3)ue).Invoke(p1, p2, p3);
    }
    return res;
  }

  public bool Invoke<UE4, T0, T1, T2, T3>(T0 p1, T1 p2, T2 p3, T3 p4)
    where UE4 : UnityEvent<T0, T1, T2, T3>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE4), out ue);
    Debug.Assert(res, "Event " + typeof(UE4) + " is not found! Did you forget to add it to m_UEventsDict in EventManager Awake() ?");
    if (res)
    {
      ((UE4)ue).Invoke(p1, p2, p3, p4);
    }
    return res;
  }

  public bool AddListener<UE0>(UnityAction action) 
    where UE0 : UnityEvent
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE0), out ue);
    if (res)
    {
      ((UE0)ue).AddListener(action);
    }
    return res;
  }

  public bool AddListener<UE1, T0>(UnityAction<T0> action)
    where UE1 : UnityEvent<T0>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE1), out ue);
    if (res)
    {
      ((UE1)ue).AddListener(action);
    }
    return res;
  }

  public bool AddListener<UE2, T0, T1>(UnityAction<T0, T1> action)
  where UE2 : UnityEvent<T0, T1>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE2), out ue);
    if (res)
    {
      ((UE2)ue).AddListener(action);
    }
    return res;
  }

  public bool AddListener<UE3, T0, T1, T2>(UnityAction<T0, T1, T2> action)
    where UE3 : UnityEvent<T0, T1, T2>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE3), out ue);
    if (res)
    {
      ((UE3)ue).AddListener(action);
    }
    return res;
  }

  public bool AddListener<UE4, T0, T1, T2, T3>(UnityAction<T0, T1, T2, T3> action)
    where UE4 : UnityEvent<T0, T1, T2, T3>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE4), out ue);
    if (res)
    {
      ((UE4)ue).AddListener(action);
    }
    return res;
  }

  public bool RemoveListener<UE0>(UnityAction action) 
    where UE0 : UnityEvent
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE0), out ue);
    if (res)
    {
      ((UE0)ue).RemoveListener(action);
    }
    return res;
  }

  public bool RemoveListener<UE1, T0>(UnityAction<T0> action)
    where UE1 : UnityEvent<T0>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE1), out ue);
    if (res)
    {
      ((UE1)ue).RemoveListener(action);
    }
    return res;
  }

  public bool RemoveListener<UE2, T0, T1>(UnityAction<T0, T1> action)
  where UE2 : UnityEvent<T0, T1>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE2), out ue);
    if (res)
    {
      ((UE2)ue).RemoveListener(action);
    }
    return res;
  }

  public bool RemoveListener<UE3, T0, T1, T2>(UnityAction<T0, T1, T2> action)
    where UE3 : UnityEvent<T0, T1, T2>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE3), out ue);
    if (res)
    {
      ((UE3)ue).RemoveListener(action);
    }
    return res;
  }

  public bool RemoveListener<UE4, T0, T1, T2, T3>(UnityAction<T0, T1, T2, T3> action)
    where UE4 : UnityEvent<T0, T1, T2, T3>
  {
    UnityEventBase ue;
    bool res = m_UEventsDict.TryGetValue(typeof(UE4), out ue);
    if (res)
    {
      ((UE4)ue).RemoveListener(action);
    }
    return res;
  }
}