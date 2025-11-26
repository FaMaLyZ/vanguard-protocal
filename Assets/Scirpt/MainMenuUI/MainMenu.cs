using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        PlayerPrefs.SetInt("CurrentWave", 1);
        SceneManager.LoadScene("SampleScene"); // ← ชื่อ gameplay scene ของคุณ
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
