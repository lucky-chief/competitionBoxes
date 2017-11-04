using UnityEngine;
using System.Collections;

public class Boot : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        SetupSingletons();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetupSingletons()
    {
        GameController gameCtrl = Singleton.GetInstance("GameController") as GameController;
        TimeMgr timeMgr = Singleton.GetInstance("TimeMgr") as TimeMgr;
        timeMgr.StartTimer();
        timeMgr.AddTimeEvent(TimeMgr.TimeEvent.NewTimeEvent("AA", gameCtrl.StartGame, 1));
    }

}
