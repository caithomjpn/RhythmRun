using System;
using System.Collections;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public static bool GameHasStarted { get; private set; }
    public static bool BeatWindowOpen { get; private set; }
    public static bool IsActionBeat { get; private set; }
    public static int GlobalBeatCount { get; private set; }
    public static int CurrentBeat { get; private set; }

    public static event Action OnBeat;

    [Header("Metronome Settings")]
    public float bpm = 115f;
    public AudioSource audioSource;

    private float beatInterval;
    private int beatCount = 0;
    private int startupBeatCounter = 0;

    private void Awake()
    {
        GameHasStarted = false;
        BeatWindowOpen = false;
        IsActionBeat = false;
        GlobalBeatCount = 0;
        CurrentBeat = 0;
    }

    private void Start()
    {
        beatInterval = 60f / bpm;

        if (audioSource != null)
        {
            audioSource.Play();  // Revert to instant playback
        }

        InvokeRepeating(nameof(TriggerBeat), 0f, beatInterval);
    }

    private void TriggerBeat()
    {
        BeatWindowOpen = true;

        beatCount++;
        if (beatCount > 4) beatCount = 1;

        GlobalBeatCount++;
        CurrentBeat = beatCount;

        IsActionBeat = (GlobalBeatCount % 4 == 2 || GlobalBeatCount % 4 == 0);

        Debug.Log($"BEAT: {CurrentBeat} | Global: {GlobalBeatCount} | ActionBeat: {IsActionBeat} | GameHasStarted: {GameHasStarted}");

        if (!GameHasStarted)
        {
            startupBeatCounter++;
            if (startupBeatCounter >= 4)
            {
                GameHasStarted = true;
                Debug.Log("GameHasStarted = true");
            }
        }

        OnBeat?.Invoke();
        StartCoroutine(CloseBeatWindow());
    }

    private IEnumerator CloseBeatWindow()
    {
        yield return new WaitForSeconds(0.3f);
        BeatWindowOpen = false;
    }
}
