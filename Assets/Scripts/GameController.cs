﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public Map map { get; private set; }
    public GridEmitter selfEmitter { get; private set; }
    public GridEmitter enermyEmitter { get; private set; }

    private SwipeRecognizer swipRg;

    void OnSwipe(SwipeGesture gesture)
    {
        Debug.Log("gesture" + gesture.Direction);
        if(gesture.Direction == FingerGestures.SwipeDirection.Up || gesture.Direction == FingerGestures.SwipeDirection.UpperDiagonals)
        {
            selfEmitter.Shot();
        }
        else if(gesture.Direction == FingerGestures.SwipeDirection.Left)
        {
            selfEmitter.Step(-1);
        }
        else if (gesture.Direction == FingerGestures.SwipeDirection.Right)
        {
            selfEmitter.Step(1);
        }
    }

    public void StartGame(float deltaTime)
    {
        map = new Map();
        map.col = 14;
        map.row = 16;
        map.Init(GameObject.Find("Canvas/Map").transform);

        selfEmitter = new GridEmitter();
        selfEmitter.Init(true);

        enermyEmitter = new GridEmitter();
        enermyEmitter.Init(false);

        TimeMgr timeMgr = Singleton.GetInstance("TimeMgr") as TimeMgr;
        TimeMgr.TimeEvent timeEvt = TimeMgr.TimeEvent.NewTimeEvent("SelfEmitter", SelfSpawnGrids, 0, 3);
        timeMgr.AddTimeEvent(timeEvt);

        timeEvt = TimeMgr.TimeEvent.NewTimeEvent("EnermyEmitter", EnermySpawnGrids, 0, 3);
        timeMgr.AddTimeEvent(timeEvt);

        GameObject.Find("Canvas/bg").GetComponent<SwipeRecognizer>().OnGesture += OnSwipe;
    }

    void SelfSpawnGrids(float deltaTime)
    {
        selfEmitter.SpawnGrids();
    }

    void EnermySpawnGrids(float deltaTime)
    {
        enermyEmitter.SpawnGrids();
    }

    void Start()
    {
        

        //swipRg = GameObject.Find("bg").GetComponent<SwipeRecognizer>();

        //swipRg.OnGesture += OnSwipe;
    }

    private void OnDestroy()
    {
       // swipRg.OnGesture -= OnSwipe;
    }

    void Update()
    {

    }

}
