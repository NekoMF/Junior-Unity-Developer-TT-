using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI; 
    string newGameScene  = "Main";
    // Start is called before the first frame update

    public AudioClip menuMusic;
    public AudioSource mainChannel;
    void Start()
    {
        mainChannel.PlayOneShot(menuMusic);
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNewGame()
    {
        mainChannel.Stop();
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitGame()
    {
         Application.Quit();
    }
}
