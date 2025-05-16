using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartFunction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {


            PlayerStats.deathCount++; 

            RestartGame();
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
