using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class tutorialManager : MonoBehaviour
{
    [Header("UI")]
    public Canvas tutorialCanvas;
    public Button blockButton;
    public TextMeshProUGUI tutorialText;
    public Image handImage;

    [Header("Hand Movement")]
    public float handMoveDistance = 40f;
    public float handMoveSpeed = 2f;

    private Vector3 handStartPos;
    private bool moveHand = false;
    private int step = 0;
    private float lookStartY;
    private bool waitingForLook = false;
    private int storyIndex = 0;

    [Header("Referanslar")]
    public FirstPersonLook firstPersonLook;
    public Gamemanager gamemanager;

    string[] storyKeys =
    {
    "TUTORIAL_STORY_1",
    "TUTORIAL_STORY_2",
    "TUTORIAL_STORY_3",
    "TUTORIAL_STORY_4",
    "TUTORIAL_STORY_5"
    };
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventSystem.current.SetSelectedGameObject(null);

        handStartPos = handImage.rectTransform.localPosition;
        handImage.gameObject.SetActive(false);

        blockButton.onClick.RemoveAllListeners();
        blockButton.onClick.AddListener(OnScreenClicked);

        StartLookAroundStep();
    }
    void OnEnable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshText;
    }

    void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshText;
    }

    void RefreshText()
    {
        if (!tutorialCanvas.gameObject.activeSelf)
            return;

        switch (step)
        {
            case 0:
                tutorialText.text =
                    LocalizationManager.Instance.GetText("TUTORIAL_LOOK_AROUND");
                break;

            case 2:
                tutorialText.text =
                    LocalizationManager.Instance.GetText("TUTORIAL_OPEN_BOOK");
                break;

            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                tutorialText.text =
                    LocalizationManager.Instance.GetText(storyKeys[storyIndex]);
                break;

            case 11:
                tutorialText.text =
                    LocalizationManager.Instance.GetText("TUTORIAL_READY");
                break;
        }
    }

    void Update()
    {
        // 🖐️ El animasyonu
        if (moveHand)
        {
            float offset = Mathf.Sin(Time.time * handMoveSpeed) * handMoveDistance;
            handImage.rectTransform.localPosition =
                handStartPos + new Vector3(offset, 0f, 0f);
        }

        // 👀 Oyuncu gerçekten etrafa baktı mı?
        if (waitingForLook)
        {
            float currentY = firstPersonLook.transform.eulerAngles.y;
            float diff = Mathf.Abs(Mathf.DeltaAngle(lookStartY, currentY));

            if (diff > 30f)
            {
                waitingForLook = false;
                StartCoroutine(ShowNextStepAfterDelay(0.5f));
            }
        }

        // 📖 Kitap açıldı mı?
        if (step == 3 && gamemanager.isBookOpen)
        {
            step = 33; // Kitap açıldı, şimdi kapanmasını bekleme moduna geç
        }

        // Adım 33: Kitap açılmıştı, şimdi kapandı mı?
        if (step == 33 && !gamemanager.isBookOpen)
        {
            StartCoroutine(SmoothResetCamera());
            step = 4;
            OnBookTutorialCompleted();
        }
    }
    // Yumuşak dönüş Coroutine'i
    IEnumerator SmoothResetCamera()
    {
        float duration = 1.4f;
        float elapsed = 0f;

        // Mevcut rotasyonu tam bir Quaternion olarak alıyoruz (X, Y ve Z dahil)
        Quaternion startRotation = firstPersonLook.transform.rotation;

        // Hedef rotasyon tamamen düz bakış (0, 0, 0)
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Senin kullandığın "Smooth Step" formülü (Hızlanıp yavaşlayan geçiş)
            t = t * t * (3f - 2f * t);

            // Slerp (Spherical Interpolation) rotasyonlar arası en yumuşak geçişi sağlar
            firstPersonLook.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        firstPersonLook.ResetLookVariables();
        firstPersonLook.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    void SetHandVisible(bool visible)
    {
        moveHand = visible;
        handImage.gameObject.SetActive(visible);
    }

    // ===============================
    // UI BUTTON CLICK
    // ===============================
    void OnScreenClicked()
    {
        HideUI();
        SetHandVisible(false);

        // Etrafa bakma başlıyor
        if (step == 0)
        {
            lookStartY = firstPersonLook.transform.eulerAngles.y;
            waitingForLook = true;
            step = 1;
        }

        // Hikaye akışı
        if (step >= 4 && step <= 8)
        {
            storyIndex++;

            if (storyIndex >= storyKeys.Length)
            {
                FinishTutorial();
                return;
            }

            ShowStoryStep();
        }
    }

    // ===============================
    // TUTORIAL STEPS
    // ===============================
    void StartLookAroundStep()
    {        
        step = 0;
        gamemanager.bookInteractionLocked = true;
        tutorialText.text =
LocalizationManager.Instance.GetText("TUTORIAL_LOOK_AROUND");
        SetHandVisible(true);
        ShowUI();
    }

    void ShowBookClickStep()
    {
        step = 2;
        gamemanager.bookInteractionLocked = false;
        tutorialText.text =
LocalizationManager.Instance.GetText("TUTORIAL_OPEN_BOOK");
        ShowUI();
    }

    IEnumerator ShowNextStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowBookClickStep();
    }

    void OnBookTutorialCompleted()
    {
        step = 4;
        ShowStoryStep();
    }
    void ShowStoryStep()
    {
        tutorialCanvas.gameObject.SetActive(true);
        firstPersonLook.canLook = false;

        moveHand = false;
        tutorialText.text =
    LocalizationManager.Instance.GetText(storyKeys[storyIndex]);
    }

    void FinishTutorial()
    {
        step = 9;
        HideUI();
        firstPersonLook.canLook = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gamemanager.SpawnTutorialEnemy(false);
    }

    // Bu fonksiyonu GameManager, oyuncu karar verdiğinde çağıracak
    public void OnTutorialCharacterProcessed()
    {
        if (step == 9)
        {
            step = 10;
            firstPersonLook.canLook = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            StartCoroutine(WaitAndSpawnNext(5f, true)); // 5 saniye bekle ve Suret gönder
        }
        else if (step == 10)
        {
            step = 11;
            StartCoroutine(ShowFinalReadyMessageDelayed(5f));
        }
    }
    IEnumerator WaitAndSpawnNext(float delay, bool isSuret)
    {
        yield return new WaitForSeconds(delay);
        gamemanager.SpawnTutorialEnemy(isSuret);
    }
    IEnumerator ShowFinalReadyMessageDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowFinalReadyMessage();
    }

    void ShowFinalReadyMessage()
    {
        SaveManager.Instance.CompleteTutorial();

        tutorialText.text =
LocalizationManager.Instance.GetText("TUTORIAL_READY");
        ShowUI();
        // Bu sefer butona basınca gerçekten oyun başlasın
        blockButton.onClick.RemoveAllListeners();
        blockButton.onClick.AddListener(() => {

            if (SaveManager.Instance.cameFromOptions)
            {
                // Eğer Options'tan geldiyse, bayrağı sıfırla ve Menüye dön
                SaveManager.Instance.cameFromOptions = false;
                SceneManager.LoadScene("mainMenuScene");
            }
            else
            {
                // Eğer ilk defa (Start butonuyla) geldiyse, GameScene'e git
                SceneManager.LoadScene("GameScene");
            }
        });
    }
    // ===============================
    // UI HELPERS
    // ===============================
    void ShowUI()
    {
        tutorialCanvas.gameObject.SetActive(true);
        firstPersonLook.canLook = false; // 🔒 kamera kilit
        firstPersonLook.mobileButton.gameObject.SetActive(false);
    }

    void HideUI()
    {
        tutorialCanvas.gameObject.SetActive(false);
        firstPersonLook.canLook = true; // 🔓 kamera serbest
        firstPersonLook.mobileButton.gameObject.SetActive(true);

        if (step == 2)
        {
            step = 3; // kitap açılmasını bekle
        }
    }
}
