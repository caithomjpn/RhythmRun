using UnityEngine;
using UnityEngine.SceneManagement;

public class PressSpace : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE pressed — loading SelectStage scene...");
            SceneManager.LoadScene("SelectStage");
        }
    }
}
