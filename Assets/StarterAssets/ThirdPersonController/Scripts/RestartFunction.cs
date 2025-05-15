using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartFunction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PlayerArmature entered the DeathZone");
            Debug.Log("Player fell into the hole!");
            RestartGame();
        }
    }

    private void RestartGame()
    {
        // Reload the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
