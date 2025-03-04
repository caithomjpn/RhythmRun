using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Metronome : MonoBehaviour
{
    public float bpm = 120f;
    private float beatInterval;
    private float nextBeatTime;
    public static event Action OnBeat;


    // Start is called before the first frame update
    void Start()
    {
        beatInterval = 60f / bpm;
        nextBeatTime = Time.time + beatInterval;

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextBeatTime)
        {
            OnBeat?.Invoke();
            nextBeatTime += beatInterval;
        }
    }
}
