using UnityEngine;

public class QuitTrigger : MonoBehaviour
{
    public GameObject quitPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quitPanel.SetActive(true);
            Time.timeScale = 0f;   // Pause the game
        }
    }
}