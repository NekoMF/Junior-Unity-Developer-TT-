using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;  
    public Button resumeButton;
    public Button exitButton;
    public static bool isPaused = false;  // Static variable to track pause state

    private void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        exitButton.onClick.AddListener(ExitToMenu);
        pauseMenuUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        UnlockMouse();
    }

    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        LockMouse();
    }

    private void ExitToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;  // Ensure the pause state is reset
        SceneManager.LoadScene("Menu");
    }

    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
