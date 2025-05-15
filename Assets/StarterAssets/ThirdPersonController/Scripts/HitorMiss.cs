using UnityEngine;
using UnityEngine.UI;

public class HitorMiss : MonoBehaviour
{
    public Image indicatorImage;
    public Color hitColor = Color.green;
    public Color missColor = Color.red;
    public float flashDuration = 0.3f;

    private Color originalColor;

    private void Start()
    {
        if (indicatorImage != null)
        {
            originalColor = indicatorImage.color;
        }
    }

    public void ShowHit()
    {
        Debug.Log(" HIT called");
        StopAllCoroutines();
        StartCoroutine(FlashColor(hitColor));
    }

    public void ShowMiss()
    {
        Debug.Log("MISS called");
        StopAllCoroutines();
        StartCoroutine(FlashColor(missColor));
    }


    private System.Collections.IEnumerator FlashColor(Color color)
    {
        if (indicatorImage != null)
        {
            indicatorImage.color = color;
            yield return new WaitForSeconds(flashDuration);
            indicatorImage.color = originalColor;
        }
    }
}
