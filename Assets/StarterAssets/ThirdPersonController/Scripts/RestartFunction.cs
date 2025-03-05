using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RestartFunction : MonoBehaviour
{

    // Triggers when plyaer touches the object
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + " entered the DeathZone");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player fell into the hole!");
            RestartGame();
        }
    }
    //restart function
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
