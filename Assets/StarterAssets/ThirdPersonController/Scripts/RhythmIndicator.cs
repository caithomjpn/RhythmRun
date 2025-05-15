using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class RhythmIndicator : MonoBehaviour
{
    [Header("Visual Settings")]
    public TextMeshProUGUI beatNumberText;
    public float pulseScale = 1.3f;
    public float pulseDuration = 0.1f;
    public Color pulseColor = Color.yellow;

    private Vector3 originalScale;
    private Color originalColor;

    private void Start()
    {
        beatNumberText.text = "Connected!";
        originalScale = transform.localScale;

        if (beatNumberText != null)
        {
            originalColor = beatNumberText.color;
        }

        Metronome.OnBeat += ShowBeatNumber;
    }

    private void ShowBeatNumber()
    {

        if (beatNumberText != null)
        {
            beatNumberText.text = Metronome.CurrentBeat.ToString();
            beatNumberText.color = Metronome.IsActionBeat ? pulseColor : originalColor;
        }
    }

    private IEnumerator PulseEffect()
    {
        transform.localScale = originalScale * pulseScale;
        yield return new WaitForSeconds(pulseDuration);
        transform.localScale = originalScale;
    }

    private void OnDestroy()
    {
        Metronome.OnBeat -= ShowBeatNumber;
    }


}
