using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Reference to the Pause Menu UI
    public Button resumeButton;
    public Button exitButton;

    private bool isPaused = false;

    private void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        exitButton.onClick.AddListener(ExitToMenu);

        // Ensure the pause menu is hidden at the start
        pauseMenuUI.SetActive(false);
        UnlockMouse();  // Ensure mouse is unlocked at the start
    }

    private void Update()
    {
        // Toggle pause menu with the Escape key
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
        Time.timeScale = 0f;  // Stop the game time
        isPaused = true;
        UnlockMouse();  // Unlock the mouse when paused
    }

    private void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;  // Resume the game time
        isPaused = false;
        LockMouse();  // Lock the mouse when resuming
    }

    private void ExitToMenu()
    {
        Time.timeScale = 1f;  // Ensure the game time is resumed
        SceneManager.LoadScene("Menu");  // Load the first scene named "Menu"
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
