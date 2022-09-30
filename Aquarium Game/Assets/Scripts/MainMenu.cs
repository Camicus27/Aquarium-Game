using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator overlayAnim;
    public Button continueButton;
    public Button newGameButton;
    public Button soundsButton;
    public Button musicButton;
    public Button ratingButton;
    public AudioManager audioManager;

    void Start()
    {
        //// Get the saved data in the file
        //playerSave = (SaveData)SerializationManager.Load();
        //// Save exists
        //if (playerSave != null)
        //{
        //    // Load information from the save game
        //}
        //// No save, only new game
        //else
        //{
        //    // Create new save file
        //}

        // Save doesn't exist
        if (true)
        {
            newGameButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
        }
        //else
        //{
        //    continueButton.gameObject.SetActive(true);
        //    newGameButton.gameObject.SetActive(false);
        //}

        audioManager.PlayMusic("music");
    }

    void Update()
    {

    }

    /// <summary>
    /// Disable all buttons, start intro animation, create a new game
    /// </summary>
    public void StartButtonPressed()
    {
        newGameButton.interactable = false;
        musicButton.interactable = false;
        soundsButton.interactable = false;
        ratingButton.interactable = false;

        // playerSave = Create new save file

        // Start the game
        SceneManager.LoadSceneAsync("Aquarium");
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Disable all buttons, start intro animation, continue game
    /// </summary>
    public void ContinueButtonPressed()
    {
        continueButton.interactable = false;
        musicButton.interactable = false;
        soundsButton.interactable = false;
        ratingButton.interactable = false;

        // Load information from the save game
        // playerSave = (SaveData)SerializationManager.Load();


        // Start the game
        SceneManager.LoadSceneAsync("Game");
        StartCoroutine(FadeOut());
    }

    public void MusicButtonPressed()
    {
        audioManager.ToggleMuteMusic();
    }

    public void SoundsButtonPressed()
    {
        audioManager.ToggleMuteSound();
    }

    public void RatingButtonPressed()
    {
        // Rating popup????
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitUntil(IsLoaded);
        overlayAnim.SetTrigger("FadeOut");
        yield return new WaitUntil(IsOpaque);
    }
    private bool IsOpaque()
    { return overlayAnim.GetCurrentAnimatorStateInfo(0).IsName("Opaque"); }
    private bool IsLoaded()
    { return GameManager.instance.sceneIsLoaded; }
}
