using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject optionsPanel;
    public GameObject endingsPanel;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventSystem.current.SetSelectedGameObject(null);
    }

    // ▶ PLAY
    public void PlayGame()
    {
        if (SaveManager.Instance == null ||
        SaveManager.Instance.data == null ||
        !SaveManager.Instance.data.tutorialCompleted)
        {
            SceneManager.LoadScene("tutorialScene");
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    // || OPTIONS
    public void OptionsGame()
    {
        menuPanel.SetActive(false);
        endingsPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // || ENDINGS
    public void EndingsGame()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        endingsPanel.SetActive(true);
    }

    // ❌ EXIT
    public void ExitGame()
    {
        Application.Quit();
    }

    // BACK
    public void BackGame()
    {
        //Ayarları Kaydet
        SaveManager.Instance.SaveGame();
        SaveManager.Instance.data.hasSave = true;

        if (menuPanel != null)
            menuPanel.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (endingsPanel != null)
            endingsPanel.SetActive(false);
    }
}
