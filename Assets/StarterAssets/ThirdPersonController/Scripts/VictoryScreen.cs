using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryScreen : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public static int clearedStageIndex = 0;

    void Start()
    {
        StageProgress.MarkStageCleared(clearedStageIndex);
        StageProgress.UnlockStage(clearedStageIndex + 1);

        if (messageText != null)
        {
            messageText.text = $"You Cleared the Stage!\n Deaths: {PlayerStats.deathCount}\nPress SPACE to return to Stage Select";
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerStats.Reset(); 
            SceneManager.LoadScene("SelectStage");
        }
    }
}
