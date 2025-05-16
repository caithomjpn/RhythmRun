using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalLine : MonoBehaviour
{
    public int stageIndexToClear = 0; // 0 = Easy, 1 = Mid, etc.

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StageProgress.MarkStageCleared(stageIndexToClear);
            StageProgress.UnlockStage(stageIndexToClear + 1);
            SceneManager.LoadScene("StageComplete");
        }
    }
}
