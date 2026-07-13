using UnityEngine;

public class QuitMenu : MonoBehaviour
{
    public void Yes()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void No()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}