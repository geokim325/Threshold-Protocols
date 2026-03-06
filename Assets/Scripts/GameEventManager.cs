using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

#region ENUMS
public enum CampType
{
    PurgeCamp,          // Kendi Kampın
    DeadzoneGarrison,   // Askeriye
    RustyAltar,         // Dini İnanç
    BedlamAsylum,       // Deliler
    BrokenRadioStation  // Bilim Adamları
}

#endregion

#region DATA CLASSES

[System.Serializable]
public class EventConsequence
{
    // ---- GAME MANAGER ----

    public int suretChanceDelta;
    public int injuredChanceDelta;

    // ---- FIRST PERSON LOOK (USES) ----
    public int thermoUsesDelta;
    public int lightUsesDelta;
    public int mirrorUsesDelta;

    // ---- FIRST PERSON LOOK (ACCURACY) ----
    public int thermoAccuracyDelta;
    public int lightAccuracyDelta;
    public int mirrorAccuracyDelta;

    // ---- CHARACTER DATA ----
    public int humanDocumentErrorDelta;
    public int originSoftErrorDelta;

    // ---- TEXT FEEDBACK ----
    [TextArea] public string feedbackKey;
}

[System.Serializable]
public class GameEvent
{
    public int day;

    public string descriptionKey;

    public string optionAKey;
    public Dictionary<CampType, int> optionAEffects;
    public EventConsequence optionAConsequence;

    public string optionBKey;
    public Dictionary<CampType, int> optionBEffects;
    public EventConsequence optionBConsequence;
}

[System.Serializable]
public class CampEnding
{
    public CampType camp;
    public GameObject positiveEndingUI;
    public GameObject negativeEndingUI;
}

#endregion

public class GameEventManager : MonoBehaviour
{
    #region UI

    [Header("UI")]
    public GameObject eventCanvas;
    public TextMeshProUGUI descriptionText;
    public Button optionAButton;
    public Button optionBButton;
    public TextMeshProUGUI optionAText;
    public TextMeshProUGUI optionBText;

    [Header("Feedback UI")]
    public GameObject feedbackCanvas;
    public TextMeshProUGUI feedbackUIText;
    public CanvasGroup feedbackCanvasGroup;
    public float feedbackDuration = 3f;
    public float fadeDuration = 0.3f;

    #endregion

    #region REFERENCES

    [Header("Referanslar")]
    public Gamemanager gameManager;
    public FirstPersonLook firstPersonLook;
    public CharacterData characterData;

    #endregion

    #region INTERNAL DATA

    private Dictionary<CampType, int> campScores = new Dictionary<CampType, int>();
    private List<GameEvent> events = new List<GameEvent>();
    private GameEvent currentEvent;

    [Header("Endings")]
    public List<CampEnding> campEndings;

    [Header("Special Ending")]
    public GameObject silentBalanceEndingUI;

    private bool eventAlreadyTriggered = false;

    #endregion

    #region UNITY

    void Awake()
    {
        foreach (CampType camp in System.Enum.GetValues(typeof(CampType)))
            campScores[camp] = 0;

        CreateEvents();

        optionAButton.onClick.AddListener(() => ApplyDecision(true));
        optionBButton.onClick.AddListener(() => ApplyDecision(false));
    }
    void OnEnable()
    {
        LocalizationManager.Instance.OnLanguageChanged += RefreshEventUI;
    }

    void OnDisable()
    {
        LocalizationManager.Instance.OnLanguageChanged -= RefreshEventUI;
    }

    void RefreshEventUI()
    {
        if (currentEvent == null) return;

        descriptionText.text = LocalizationManager.Instance.GetText(currentEvent.descriptionKey);
        optionAText.text = LocalizationManager.Instance.GetText(currentEvent.optionAKey);
        optionBText.text = LocalizationManager.Instance.GetText(currentEvent.optionBKey);
    }

    #endregion

    #region EVENT FLOW

    // 🔹 GÜN BAŞINDA ÇAĞIR
    public void CheckForEvent(int day)
    {
        if (eventAlreadyTriggered) return;

        currentEvent = events.Find(e => e.day == day);

        if (currentEvent == null) return;

        eventAlreadyTriggered = true;

        Time.timeScale = 0f;
        eventCanvas.SetActive(true);
        gameManager.usesInfoCanvas.SetActive(false);
        gameManager.pausedCanvas.SetActive(false);
        feedbackCanvas.SetActive(false);

        if (firstPersonLook != null)
            firstPersonLook.canLook = false;

        descriptionText.text = LocalizationManager.Instance.GetText(currentEvent.descriptionKey);
        optionAText.text = LocalizationManager.Instance.GetText(currentEvent.optionAKey);
        optionBText.text = LocalizationManager.Instance.GetText(currentEvent.optionBKey);
    }

    void ApplyDecision(bool optionA)
    {
        SaveManager.Instance.Vibrate();

        var scoreEffects = optionA
            ? currentEvent.optionAEffects
            : currentEvent.optionBEffects;

        var consequence = optionA
            ? currentEvent.optionAConsequence
            : currentEvent.optionBConsequence;

        foreach (var effect in scoreEffects)
            campScores[effect.Key] += effect.Value;

        ApplyConsequence(consequence);

        //LogCampScores();

        eventCanvas.SetActive(false);
        feedbackCanvas.SetActive(true);

        if (firstPersonLook != null)
            firstPersonLook.canLook = true;
    }

    #endregion

    #region CONSEQUENCE APPLY

    void ApplyConsequence(EventConsequence c)
    {
        if (c == null) return;

        // ---- GameManager ----
        if (gameManager != null)
        {
            gameManager.suretChance += c.suretChanceDelta;
            gameManager.injuredChance += c.injuredChanceDelta;
        }

        // ---- FirstPersonLook ----
        if (firstPersonLook != null)
        {
            firstPersonLook.thermoUses += c.thermoUsesDelta;
            firstPersonLook.lightUses += c.lightUsesDelta;
            firstPersonLook.mirrorUses += c.mirrorUsesDelta;

            firstPersonLook.thermoRemaining += c.thermoUsesDelta;
            firstPersonLook.lightRemaining += c.lightUsesDelta;
            firstPersonLook.mirrorRemaining += c.mirrorUsesDelta;

            firstPersonLook.UsesUI();

            firstPersonLook.thermoAccuracy += c.thermoAccuracyDelta;
            firstPersonLook.lightAccuracy += c.lightAccuracyDelta;
            firstPersonLook.mirrorAccuracy += c.mirrorAccuracyDelta;
        }

        // ---- CharacterData ----
        if (characterData != null)
        {
            characterData.humanDocumentErrorChance += c.humanDocumentErrorDelta;
            characterData.originSoftErrorChance += c.originSoftErrorDelta;
        }

        if (!string.IsNullOrEmpty(c.feedbackKey))
        {
            ShowFeedback(c.feedbackKey);
        }
    }

    #endregion

    #region ENDING

    // 🔹 OYUN SONU
    public void GetFinalEnding()
    {
        Time.timeScale = 0f;

        int highestAbs = int.MinValue;
        List<CampType> topCamps = new List<CampType>();

        foreach (var camp in campScores)
        {
            int abs = Mathf.Abs(camp.Value);

            if (abs > highestAbs)
            {
                highestAbs = abs;
                topCamps.Clear();
                topCamps.Add(camp.Key);
            }
            else if (abs == highestAbs)
            {
                topCamps.Add(camp.Key);
            }
        }

        // 🔹 EŞİTLİK VARSA → SILENT BALANCE
        if (topCamps.Count > 1)
        {
            ActivateSilentBalanceEnding();
            return;
        }

        // 🔹 NORMAL ENDING
        CampType resultCamp = topCamps[0];
        int finalScore = campScores[resultCamp];
        bool positive = finalScore >= 0;

        ActivateEndingUI(resultCamp, positive);
    }

    void ActivateEndingUI(CampType camp, bool positive)
    {
        SaveManager.Instance.Vibrate();

        // Önce hepsini kapat
        foreach (var ending in campEndings)
        {
            if (ending.positiveEndingUI != null)
                ending.positiveEndingUI.SetActive(false);

            if (ending.negativeEndingUI != null)
                ending.negativeEndingUI.SetActive(false);
        }

        // Doğru ending’i aç
        CampEnding selected = campEndings.Find(e => e.camp == camp);
        if (selected == null) return;

        int endingID = GetEndingID(camp, positive);

        // 🔓 BURADA UNLOCK EDİYORUZ
        EndingManager.UnlockEnding(endingID);

        if (positive)
        {
            if (selected.positiveEndingUI != null)
                selected.positiveEndingUI.SetActive(true);
        }
        else
        {
            if (selected.negativeEndingUI != null)
                selected.negativeEndingUI.SetActive(true);
        }
    }
    int GetEndingID(CampType camp, bool positive)
    {
        switch (camp)
        {
            case CampType.PurgeCamp:
                return positive ? 1 : 2;

            case CampType.DeadzoneGarrison:
                return positive ? 3 : 4;

            case CampType.RustyAltar:
                return positive ? 5 : 6;

            case CampType.BedlamAsylum:
                return positive ? 7 : 8;

            case CampType.BrokenRadioStation:
                return positive ? 9 : 10;

            default:
                return 0;
        }
    }

    void ActivateSilentBalanceEnding()
    {
        SaveManager.Instance.Vibrate();

        // Tüm kamp endinglerini kapat
        foreach (var ending in campEndings)
        {
            if (ending.positiveEndingUI != null)
                ending.positiveEndingUI.SetActive(false);

            if (ending.negativeEndingUI != null)
                ending.negativeEndingUI.SetActive(false);
        }

        // 🔓 ID 11
        EndingManager.UnlockEnding(11);

        // Silent Balance aç
        if (silentBalanceEndingUI != null)
            silentBalanceEndingUI.SetActive(true);
    }

    #endregion

    #region EVENTS

    Coroutine feedbackRoutine;
    void ShowFeedback(string key)
    {
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(FeedbackCoroutine(key));
    }

    IEnumerator FeedbackCoroutine(string key)
    {
        LocalizedText localized = feedbackUIText.GetComponent<LocalizedText>();

        if (localized != null)
        {
            localized.key = key;
        }

        feedbackCanvas.SetActive(true);

        // 🔹 FADE IN
        yield return StartCoroutine(FadeCanvas(0f, 1f));

        // 🔹 BEKLE
        yield return new WaitForSecondsRealtime(feedbackDuration);

        // 🔹 FADE OUT
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        Time.timeScale = 1f;
        feedbackCanvas.SetActive(false);
        feedbackRoutine = null;
        gameManager.usesInfoCanvas.SetActive(true);
        gameManager.pausedCanvas.SetActive(true);
    }
    IEnumerator FadeCanvas(float from, float to)
    {
        float elapsed = 0f;
        feedbackCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            feedbackCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        feedbackCanvasGroup.alpha = to;
    }

    // 🔹 EVENT TANIMLARI
    void CreateEvents()
    {
        events.Add(new GameEvent
        {
            day = 5,
            descriptionKey = "EVENT_5_DESC",
            optionAKey = "EVENT_5_OPTION_A",
            optionBKey = "EVENT_5_OPTION_B",

            optionAEffects = new Dictionary<CampType, int>
            {

                { CampType.DeadzoneGarrison, +7 },
                { CampType.PurgeCamp, -7 }
                
            },
            optionAConsequence = new EventConsequence
            {
                suretChanceDelta = -5,
                feedbackKey = "EVENT_5_FEEDBACK_A"
            },

            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.DeadzoneGarrison, -7 },
                { CampType.PurgeCamp, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_5_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 8,
            descriptionKey = "EVENT_8_DESC",
            optionAKey = "EVENT_8_OPTION_A",
            optionBKey = "EVENT_8_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.RustyAltar, +7 },
                { CampType.BrokenRadioStation, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_8_FEEDBACK_A",
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.RustyAltar, -7 },
                { CampType.BrokenRadioStation, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                thermoAccuracyDelta = -5,
                lightAccuracyDelta = -5,
                mirrorAccuracyDelta = -5,
                feedbackKey = "EVENT_8_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 11,
            descriptionKey = "EVENT_11_DESC",
            optionAKey = "EVENT_11_OPTION_A",
            optionBKey = "EVENT_11_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.BedlamAsylum, +7 },
                { CampType.PurgeCamp, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_11_FEEDBACK_A"
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.BedlamAsylum, -13 },
                { CampType.PurgeCamp, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_11_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 14,
            descriptionKey = "EVENT_14_DESC",
            optionAKey = "EVENT_14_OPTION_A",
            optionBKey = "EVENT_14_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.RustyAltar, +7 },
                { CampType.DeadzoneGarrison, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                injuredChanceDelta = -5,
                feedbackKey = "EVENT_14_FEEDBACK_A"
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.RustyAltar, -7 },
                { CampType.DeadzoneGarrison, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_14_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 17,
            descriptionKey = "EVENT_17_DESC",
            optionAKey = "EVENT_17_OPTION_A",
            optionBKey = "EVENT_17_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.BedlamAsylum, +7 },
                { CampType.BrokenRadioStation, +7 },
                { CampType.PurgeCamp, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_17_FEEDBACK_A"
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.BedlamAsylum, -13 },
                { CampType.BrokenRadioStation, -7 },
                { CampType.PurgeCamp, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_17_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 20,
            descriptionKey = "EVENT_20_DESC",
            optionAKey = "EVENT_20_OPTION_A",
            optionBKey = "EVENT_20_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.RustyAltar, +13 },
                { CampType.PurgeCamp, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                humanDocumentErrorDelta = -5,
                originSoftErrorDelta = -5,
                feedbackKey = "EVENT_20_FEEDBACK_A"
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.RustyAltar, -7 },
                { CampType.PurgeCamp, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_20_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 23,
            descriptionKey = "EVENT_23_DESC",
            optionAKey = "EVENT_23_OPTION_A",
            optionBKey = "EVENT_23_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.DeadzoneGarrison, +7 },
                { CampType.BrokenRadioStation, -7 },
                { CampType.BedlamAsylum, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                thermoUsesDelta = -3,
                lightUsesDelta = -2,
                mirrorUsesDelta = -1,
                feedbackKey = "EVENT_23_FEEDBACK_A"
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.DeadzoneGarrison, -7 },
                { CampType.BrokenRadioStation, +7 },
                { CampType.BedlamAsylum, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                thermoAccuracyDelta = +5,
                lightAccuracyDelta = +5,
                mirrorAccuracyDelta = +5,
                feedbackKey = "EVENT_23_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 26,
            descriptionKey = "EVENT_26_DESC",
            optionAKey = "EVENT_26_OPTION_A",
            optionBKey = "EVENT_26_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.BedlamAsylum, +7 },
                { CampType.RustyAltar, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                suretChanceDelta = +5,
                feedbackKey = "EVENT_26_FEEDBACK_A"
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.BedlamAsylum, -7 },
                { CampType.RustyAltar, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                suretChanceDelta = -5,
                feedbackKey = "EVENT_26_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 29,
            descriptionKey = "EVENT_29_DESC",
            optionAKey = "EVENT_29_OPTION_A",
            optionBKey = "EVENT_29_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.BrokenRadioStation, +7 },
                { CampType.RustyAltar, -7 },
                { CampType.PurgeCamp, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                thermoUsesDelta = +3,
                lightUsesDelta = +2,
                mirrorUsesDelta = +1,
                suretChanceDelta = +10,
                feedbackKey = "EVENT_29_FEEDBACK_A"
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.BrokenRadioStation, -7 },
                { CampType.RustyAltar, +7 },
                { CampType.PurgeCamp, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                feedbackKey = "EVENT_29_FEEDBACK_B"
            }
        });

        events.Add(new GameEvent
        {
            day = 32,
            descriptionKey = "EVENT_32_DESC",
            optionAKey = "EVENT_32_OPTION_A",
            optionBKey = "EVENT_32_OPTION_B",
            optionAEffects = new Dictionary<CampType, int>
            {
                { CampType.DeadzoneGarrison, +13 },
                { CampType.BedlamAsylum, -7 }
            },
            optionAConsequence = new EventConsequence
            {
                thermoUsesDelta = -3,
                lightUsesDelta = -2,
                mirrorUsesDelta = -1,
                feedbackKey = "EVENT_32_FEEDBACK_A"
            },
            optionBEffects = new Dictionary<CampType, int>
            {
                { CampType.DeadzoneGarrison, -7 },
                { CampType.BedlamAsylum, +7 }
            },
            optionBConsequence = new EventConsequence
            {
                suretChanceDelta = +5,
                feedbackKey = "EVENT_32_FEEDBACK_B"
            }
        });
        // 👉 Diğer günleri aynı şekilde ekleyebilirsin
    }
    // void LogCampScores()
    //  {
    //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    //    sb.AppendLine("=== CAMP SCORES ===");

    //   foreach (var camp in campScores)
    //   {
    //       sb.AppendLine($"{camp.Key}: {camp.Value}");
    //   }

    //   Debug.Log(sb.ToString());
    // }
    public void ResetEventTrigger()
    {
        eventAlreadyTriggered = false;
    }
    #endregion
}
