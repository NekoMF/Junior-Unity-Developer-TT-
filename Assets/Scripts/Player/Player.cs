using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float playerHP = 100f;
    public GameObject bloodyScreen;
    public TextMeshProUGUI playerHealthUI;
    public GameObject gameOverUI;
    public bool isDead = false;



    private void Start() {
        playerHealthUI.text = $"Health: {playerHP}";
        }

    public void TakeDamage (int damageAmount)
    {
        playerHP -= damageAmount;
        
        if (playerHP <= 0)
        {
           //print("DEATH");
           PlayerDeath();

        }
        else
        {
            //print("Player Hit");
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health: {playerHP}";
        }
    }

   private void PlayerDeath()
{
    // Disable player controls
    GetComponent<PlayerMovement>().enabled = false;
    GetComponent<AimStateManager>().enabled = false;
    playerHealthUI.gameObject.SetActive(false);

    // Trigger death animation
    Animator animator = GetComponentInChildren<Animator>();
    if (animator != null)
    {
        animator.SetTrigger("DEATH");
    }
    // Fade the screen
    GetComponent<ScreenFader>().StartFade();
    StartCoroutine(ShowGameOverUI());

    int waveSurvived = GlobalReferences.Instance.waveNumber;
    int savedHighScore = SaveLoadManager.Instance.LoadHighScore();

    // Save the maximum of the current wave or the previously saved high score
    SaveLoadManager.Instance.SaveHighScore(Mathf.Max(waveSurvived - 1, savedHighScore));

    isDead = true;
}


    private IEnumerator ShowGameOverUI()
    {
        
        AudioManager.Instance.PlaySFX("GameOver", 1f);
        yield return new WaitForSeconds(1f);
        gameOverUI.gameObject.SetActive(true);
        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds (2f);
        SceneManager.LoadScene("Menu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();
 
        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;
 
        float duration = 1f;
        float elapsedTime = 0f;
 
        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
 
            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
 
            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;
 
            yield return null; ; // Wait for the next frame.
        }

        if (bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is a Zombie's hand
        if (other.CompareTag("ZombieHand") && !isDead)
        {
            // Get the parent Zombie component
            Zombie zombie = other.GetComponentInParent<Zombie>();

            // Retrieve ZombieData from the Zombie component
            ZombieData zombieData = zombie.GetZombieData();

            // Deal damage to the player using the damage value from ZombieData
            TakeDamage(zombieData.damage);

        }
    }

    private void TestDeathAnimation()
{
    
}
}
