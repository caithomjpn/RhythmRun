using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryScreen : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public int clearedStageIndex = 0; // Set this in Inspector (0 = Easy, etc.)

    void Start()
    {
        // Mark stage cleared and unlock the next
        StageProgress.MarkStageCleared(clearedStageIndex);
        StageProgress.UnlockStage(clearedStageIndex + 1);

        // Show player stats
        if (messageText != null)
        {
            messageText.text = $"You Cleared the Stage!\n Deaths: {PlayerStats.deathCount}\nPress SPACE to return to Stage Select";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerStats.Reset(); // Reset death count for next stage
            SceneManager.LoadScene("SelectStage");
        }
    }
}
