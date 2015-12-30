using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class GameTimer : MonoBehaviour {

    //Is the timer on?
    bool on=false;
    //Should the timer start when it's loaded?
    public bool StartOnLoad=false;
    public bool DestroyOnFinished = false;

    //The time the timer will take before going off
    public float TimerTime;

    //Total elapsed time
    float _elapsedTime;

	// Use this for initialization
	void Start () {
        if (StartOnLoad)
            on=true;
	}
	

    //Start the timer
    public void StartTimer() {
        on=true;
    }

    //Stop the timer
    public void StopTimer() {
        on=false;
    }

    //Reset the timer
    public void ResetTimer() {
        _elapsedTime=0;
    }

    public UnityEvent onTimerOver = new UnityEvent();

	// Update is called once per frame
	void Update () {
        if (!on)
            return;

        _elapsedTime+=Time.deltaTime;

        if (_elapsedTime>=TimerTime)
            TimerDone();
	}

    void TimerDone() {
        StopTimer();
        ResetTimer();
        onTimerOver.Invoke();
        if (DestroyOnFinished)
            Destroy(gameObject);
    }
}
