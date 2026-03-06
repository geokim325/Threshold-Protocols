using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


[System.Serializable]
public class SaveData
{
    public int currentDay;
    public bool tutorialCompleted;
    public bool hasSave;

    //LANGUAGE
    public int languageIndex = 0; // 0 = EN, 1 = TR

    //MUSIC
    public float musicVolume = 0.75f;
    public float sfxVolume = 0.75f;

    // OPTIONS
    public float sensitivity = 1.0f;   // kamera hassasiyeti
    public bool vibrationEnabled = true;
}

public class SaveManager : MonoBehaviour
{
    [HideInInspector]
    public bool cameFromOptions = false;
    public static SaveManager Instance;
    public SaveData data;
    public AudioMixer mainMixer;

    private const string SAVE_KEY = "SAVE_DATA";

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGame();
    }
    void Start()
    {
        // Oyun ilk açıldığında kaydedilen ses ayarlarını uygula
        ApplySavedAudio();
    }

    // -----------------------
    // LOAD
    // -----------------------
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            data = new SaveData(); // zaten default
            SaveGame(); // önemli
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        data = JsonUtility.FromJson<SaveData>(json);
    }
    void ApplySavedAudio()
    {
        if (mainMixer == null) return;

        // Music
        float mVol = data.musicVolume;
        float mdB = Mathf.Log10(Mathf.Clamp(mVol, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("MusicVol", mdB);

        // SFX
        float sVol = data.sfxVolume;
        float sdB = Mathf.Log10(Mathf.Clamp(sVol, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("SFXVol", sdB);
    }

    // -----------------------
    // SAVE
    // -----------------------
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    // -----------------------
    // HELPER METODLAR
    // -----------------------

    // Gün bitince çağır
    public void CompleteDay()
    {
        data.currentDay++;
        data.hasSave = true;
        SaveGame();
    }

    // Tutorial bitince çağır
    public void CompleteTutorial()
    {
        data.tutorialCompleted = true;
        data.hasSave = true;
        SaveGame();
    }

    // Bu fonksiyon diğer scriptler tarafından çağrılacak
    public void Vibrate()
    {
        if (!data.vibrationEnabled)
            return;

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    // RESET ALL DATA
    public void ResetAllData()
    {
        // PlayerPrefs sil
        PlayerPrefs.DeleteKey(SAVE_KEY);

        EndingUIItem[] items = FindObjectsOfType<EndingUIItem>(true);

        foreach (EndingUIItem item in items)
        {
            item.RefreshUI();
        }

        // 🔓 TÜM ENDINGLERİ SİL
        EndingManager.ResetAllEndings();

        // Default değerleri ayarla
        data = new SaveData
        {
            currentDay = 1,
            musicVolume = 0.75f,
            sfxVolume = 0.75f,
            tutorialCompleted = false,
            hasSave = false,
            sensitivity = 1f,
            vibrationEnabled = true
        };

        // Kaydet
        SaveGame();
    }
}
