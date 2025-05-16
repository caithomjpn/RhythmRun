using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StageSelector : MonoBehaviour
{
    public TextMeshProUGUI[] stageTexts; // assign in Inspector
    private int currentIndex = 0;

    private void Start()
    {
        UpdateHighlight();
    }

    private void Update()
    {
        if (stageTexts == null || stageTexts.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % stageTexts.Length;
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = (currentIndex - 1 + stageTexts.Length) % stageTexts.Length;
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadStage(currentIndex);
        }
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < stageTexts.Length; i++)
        {
            stageTexts[i].color = (i == currentIndex) ? Color.yellow : Color.white;
            stageTexts[i].transform.localScale = (i == currentIndex) ? Vector3.one * 1.2f : Vector3.one;
        }
    }

    void LoadStage(int index)
    {
        switch (index)
        {
            case 0: SceneManager.LoadScene("Stage_Easy"); break;
            case 1: SceneManager.LoadScene("Stage_Mid"); break;
            case 2: SceneManager.LoadScene("Stage_BitDifficult"); break;
            case 3: SceneManager.LoadScene("Stage_Difficult"); break;
            case 4: SceneManager.LoadScene("Stage_Hard"); break;
        }
    }
}
