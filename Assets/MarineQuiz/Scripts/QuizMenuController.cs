using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizMenuController : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}