using UnityEngine;
using UnityEngine.SceneManagement;

public class PressSpace : MonoBehaviour
{
    public string sceneToLoad = "SelectStage"; // Set in Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneToLoad);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Resetting progress and quitting...");

            StageProgress.ResetProgress();

            PlayerStats.Reset();

            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#endif
        }
    }
}
