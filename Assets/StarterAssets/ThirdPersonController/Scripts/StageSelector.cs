using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class StageSelector : MonoBehaviour
{
    public TextMeshProUGUI[] stageTexts; // assign in Inspector
    private int currentIndex = 0;
    private int unlockedIndex;

    public TextMeshProUGUI errorText; // assign this in the Inspector
    public float errorDisplayTime = 2f;
    private Coroutine errorCoroutine;
    private void Start()
    {
        unlockedIndex = StageProgress.GetUnlockedStageIndex();
        errorText.gameObject.SetActive(false);
        UpdateHighlight();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("StartPage"); 
        }
        if (stageTexts == null || stageTexts.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % stageTexts.Length;
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentIndex <= unlockedIndex || currentIndex == 5)
            {
                LoadStage(currentIndex);
            }
            else
            {
                ShowError("Stage is locked.");
            }
        }

    }

    void UpdateHighlight()
    {
        for (int i = 0; i < stageTexts.Length; i++)
        {
            var tmp = stageTexts[i];

            bool isUnlocked = i <= unlockedIndex;

            // Visual feedback
            tmp.color = (i == currentIndex)
                ? (isUnlocked ? Color.yellow : Color.gray)
                : (isUnlocked ? Color.white : Color.gray);

            tmp.transform.localScale = (i == currentIndex) ? Vector3.one * 1.2f : Vector3.one;

            // Optionally append  for cleared stages
            tmp.text = GetStageName(i);
        }
    }
    void ShowError(string message)
    {
        if (errorCoroutine != null)
        {
            StopCoroutine(errorCoroutine);
        }
        errorCoroutine = StartCoroutine(ShowErrorRoutine(message));
    }

    IEnumerator ShowErrorRoutine(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(errorDisplayTime);
        errorText.gameObject.SetActive(false);
    }

    string GetStageName(int i)
    {
        string[] stageNames = { "Easy", "Mid", "Bit Difficult", "Difficult", "Hard" , "Randomly generated stage!" };
        bool cleared = StageProgress.IsStageCleared(i);
        return cleared ? $"{stageNames[i]}     /" : stageNames[i];
    }

    void LoadStage(int index)
    {
        VictoryScreen.clearedStageIndex = index;

        switch (index)
        {
            case 0: SceneManager.LoadScene("Stage_Easy"); break;
            case 1: SceneManager.LoadScene("Stage_Mid"); break;
            case 2: SceneManager.LoadScene("Stage_BitDifficult"); break;
            case 3: SceneManager.LoadScene("Stage_Difficult"); break;
            case 4: SceneManager.LoadScene("Stage_Hard"); break;
            case 5: SceneManager.LoadScene("RandomStage"); break;
        }
    }
}
