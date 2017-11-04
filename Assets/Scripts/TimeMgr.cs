using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimeMgr : MonoBehaviour
{
    public class TimeEvent
    {
        public string eventName;
        public float interval;
        public float secondsLater;
        public Action<float> callback;
        public float setupTime;
        public int executeType;

        public static TimeEvent NewTimeEvent(string eventName, Action<float> callback, float secondsLater, float interval = 0,int executeType = 0)
        {
            TimeEvent evt = new TimeEvent();
            evt.eventName = eventName;
            evt.callback = callback;
            evt.secondsLater = secondsLater;
            evt.interval = interval;
            evt.executeType = executeType;
            return evt;
        }
    }

    public static TimeMgr Instance = null;

    private Dictionary<string, TimeEvent> timeEventMap = new Dictionary<string, TimeEvent>();
    private float logicTimeAccumulated = 0F;
    private bool timming;

    public void StartTimer()
    {
        timming = true;
    }

    public void StopTimer()
    {
        timming = false;
    }

    public void AddFrameTimeEvent(string timeEvtName,Action<float> callback)
    {
        TimeEvent evt = TimeEvent.NewTimeEvent(timeEvtName, callback, 0, 0, 1);
        AddTimeEvent(evt);
    }

    public void AddTimeEvent(TimeEvent timeEvt)
    {
        if (timeEventMap.ContainsKey(timeEvt.eventName))
        {
            Debug.Log("覆盖已有的时间事件！name： " + timeEvt.eventName);
        }
        timeEvt.setupTime = logicTimeAccumulated;
        timeEventMap[timeEvt.eventName] = timeEvt;
    }

    public void RemoveTimeEvent(string timeEventName)
    {
        if(timeEventMap.ContainsKey(timeEventName))
        {
            timeEventMap.Remove(timeEventName);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (timming)
        {
            logicTimeAccumulated += Time.fixedDeltaTime;
            ExecuteTimeEvt(Time.fixedDeltaTime);
        }
    }

    private void ExecuteTimeEvt(float deltaTime)
    {
        List<string> keys = new List<string>(timeEventMap.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            TimeEvent evt = timeEventMap[keys[i]];
            if(evt.executeType == 1)
            {
                evt.callback.Invoke(deltaTime);
            }
            else
            {
                if (evt.secondsLater != 0 && Mathf.Abs(evt.secondsLater + evt.setupTime - logicTimeAccumulated) < 0.001f)
                {
                    evt.callback.Invoke(deltaTime);
                    timeEventMap.Remove(keys[i]);
                }
                else if(Mathf.Abs(logicTimeAccumulated - evt.setupTime - evt.interval) < 0.001f)
                {
                    evt.setupTime = logicTimeAccumulated;
                    evt.callback.Invoke(deltaTime);
                }
            }
        }
    }
}
