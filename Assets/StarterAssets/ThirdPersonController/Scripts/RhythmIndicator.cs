using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RhythmIndicator : MonoBehaviour
{
    private Image indicator;
    private Vector3 originalScale;
    public float pulseScale = 1.3f; // Size increase when pulsing
    public float pulseDuration = 0.1f; // Time it takes to pulse

    void Start()
    {
        indicator = GetComponent<Image>();
        originalScale = transform.localScale;

        // Subscribe to the metronome event
        Metronome.OnBeat += PulseIndicator;
    }

    void PulseIndicator()
    {
        StartCoroutine(PulseEffect());
    }

    IEnumerator PulseEffect()
    {
        transform.localScale = originalScale * pulseScale;
        yield return new WaitForSeconds(pulseDuration);
        transform.localScale = originalScale;
    }

    private void OnDestroy()
    {
        Metronome.OnBeat -= PulseIndicator; // Unsubscribe when destroyed
    }
}