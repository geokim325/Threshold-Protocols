using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    // --- GÜN SİSTEMİ DEĞİŞKENLERİ ---
    [Header("Gün Sistemi")]
    [Tooltip("Her bir gün için işlenecek maksimum karakter sayısı. (Index 0 = Gün 1, Index 1 = Gün 2, vb.)")]
    public List<int> enemiesPerDay = new List<int> { 4, 5, 6, 7 }; // Örnek: Gün 1'de 4, Gün 2'de 5 kişi

    public float dayEndDelay = 3f;         // Gün bittikten sonra yeni güne geçmeden beklenecek süre
    public GameEventManager gameEventManager;

    [Header("Karakter Ayarları")]
    [Range(0f, 100f)]
    [Tooltip("Karakterin sakatlanmış (injured) olma yüzdesi şansı. (Örn: 30 = %30)")]
    public float injuredChance = 30f; // YENİ: Sakatlık şansı %

    [Range(0f, 100f)]
    [Tooltip("Gelen karakterin İNSAN formunu taklit eden SURET olma şansı.")]
    public float suretChance = 30f; // YENİ: Suret olma şansı %

    [Tooltip("Normal karakterlerin yürüme hızı.")]
    public float normalSpeed = 3.0f; // YENİ: Normal yürüme hızı

    [Tooltip("Sakat karakterlerin yürüme hızı.")]
    public float injuredSpeed = 2.0f; // YENİ: Sakat yürüme hızı

    // --- HATA LİMİTİ VE KONTROL DEĞİŞKENLERİ (YENİ) ---
    [Header("Hata Limit Kontrolü")]
    [Tooltip("Maksimum izin verilen hata sayısı (Örn: 2).")]
    public int totalMistakeCount = 0;
    public int maxAllowedMistakes = 2;
    public TextMeshProUGUI mistakeText; // TV'deki metin objesi
    public GameObject tvCanvas; // TV'deki Canvas objesi

    public int approvedSuretCount = 0;
    public int rejectedHumanCount = 0;

    private bool gameOverPending = false;

    [Header("Günlük İstatistikler")]
    public int CurrentDay = 1;             // Mevcut gün numarası
    public int enemiesProcessedToday = 0;  // Bugün işlenen karakter sayısı
    public int approvedCountToday = 0;     // Bugün onaylanan karakter sayısı
    public int rejectCountToday = 0;       // Bugün reddedilen karakter sayısı

    [Header("Character Info UI")]
    public TextMeshProUGUI fullNameText;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI countryText;
    public TextMeshProUGUI originCityText;
    public TextMeshProUGUI reasonText;
    public TextMeshProUGUI genderText;
    public TextMeshProUGUI illnessText;
    public TextMeshProUGUI belongingsText;

    // --- UI REFERANSLARI VE TARİH ---
    [Header("UI Referansları ve Tarih")]
    [Tooltip("Durum Raporu Canvas'ı.")]
    public GameObject situationCanvas;
    [Tooltip("Gün Sonu Özet Ekranı.")]
    public GameObject summaryScreen;
    [Tooltip("Ölüm Sonu Ekranı.")]
    public GameObject deathScreen;
    [Tooltip("Başarısızlık Sonu Ekranı.")]
    public GameObject failedScreen;

    [Tooltip("Gün başlangıcında GÜN X bilgisini 3 saniye gösteren Canvas.")]
    public GameObject dayAnnounceCanvas;
    public CanvasGroup dayAnnounceCanvasGroup;
    public float dayannounceDuration = 3f;
    public float fadeDuration = 0.3f;

    [Tooltip("Gün başlangıcında gün yazısı.")]
    public TextMeshProUGUI dayAnnounceText;
    [Tooltip("Gün sonu özet ekranındaki özet yazısı.")]
    public TextMeshProUGUI summaryText;
    Dictionary<int, string> dayEndMessages = new Dictionary<int, string>();
    [Tooltip("Özet ekranındaki onaylanan sayısı yazısı.")]
    public TextMeshProUGUI approvedCountText;   // Onaylanan sayısı yazısı
    [Tooltip("Özet ekranındaki reddedilen sayısı yazısı.")]
    public TextMeshProUGUI rejectCountText;     // Reddedilen sayısı yazısı
    [Tooltip("Gün yazısı")]
    public TextMeshProUGUI dayText;      // "1" (gün) yazısı
    [Tooltip("Gün yazısını gizleme")]
    public GameObject dayObject;
    [Tooltip("'Next Day' (Sonraki Gün) butonu objesi.")]
    public GameObject nextDayButton;
    [Tooltip("'Try Again' (Yeniden Dene) butonu.")]
    public GameObject tryAgainButton;
    [Tooltip("'New Game' (Yeni Oyun) butonu.")]
    public GameObject newGameButton;
    [Tooltip("'Main Menu' (Ana Menü) butonu.")]
    public GameObject mainMenuButton;

    [Header("Paused UI")]
    public GameObject pausedPanel;
    public GameObject pausedButton;
    public GameObject pausedCanvas;

    [Header("UsesUI")]
    public GameObject usesInfoCanvas;

    private System.DateTime currentDate; // Tarihi tutmak için

    // HAREKET NOKTALARI (Inspector'dan atayacaksın)
    [Header("Hareket Noktaları")]
    [Tooltip("Karakterin sahneye girdiği Transform.")]
    public Transform startPoint;          // Başlangıç konumu
    [Tooltip("Karakterin sorgu için duracağı Transform.")]
    public Transform controlPoint;        // Kontrol noktası (karakterin duracağı yer)
    [Tooltip("Onaylananların ayrılacağı Transform.")]
    public Transform approvedPoint;       // Onaylananların gideceği yer
    [Tooltip("Reddedilenlerin ayrılacağı Transform.")]
    public Transform notApprovedPoint;    // Reddedilenlerin gideceği yer

    // AYARLAR (Inspector'dan atayacaksın)
    [Header("Ayarlar")]
    public float stopDistance = 0.5f;
    public float interactDistance = 3f;
    public float waitBeforeDecision = 1f;
    public LayerMask interactableLayer;    // Butonların Layer'ını burada seçmelisin.
    public float disappearDelay = 5f;

    [Header("Oyun Referansları")]
    [Tooltip("Oyuncu karakteri Prefab'ı.")]
    public GameObject enemyPrefab;
    [Tooltip("Sorgu Butonlarını içeren Canvas.")]
    public GameObject sorguCanvas;
    public FirstPersonLook playerController;
    public tutorialManager tManager;

    [Header("Damga Sistemi")]
    [Tooltip("Onay kararı verildiğinde açılacak Image objesi.")]
    public GameObject approvedDecisionImage;
    [Tooltip("Red kararı verildiğinde açılacak Image objesi.")]
    public GameObject rejectedDecisionImage;
    public Animator approveStampAnimator;
    public Animator rejectStampAnimator;

    [Header("Kitap Sistemi")]
    [Tooltip("Kitap objesine basılınca açılacak canvas")]
    public GameObject bookCanvas;
    [Tooltip("Kitap objesi")]
    public GameObject bookObject;
    [Tooltip("Sayfalar")]
    public GameObject[] pages;
    [Tooltip("Sonraki Buton")]
    public Button nextButton;
    [Tooltip("Önceki Buton")]
    public Button prevButton;

    private EnemyMovement currentEnemy;
    private bool canStamp = false;
    private int currentPage = 0;
    [HideInInspector] public bool isBookOpen = false;
    public static Gamemanager Instance;

    public bool gamePaused;
    public bool tutorialMode;
    public bool bookInteractionLocked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Tarihi başlangıç değerine ayarla: 1 Kasım 1882
        currentDate = new System.DateTime(1882, 11, 1);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameOverPending = false;

        if (approvedDecisionImage != null) approvedDecisionImage.SetActive(false);
        if (rejectedDecisionImage != null) rejectedDecisionImage.SetActive(false);

        tvCanvas.SetActive(false);
        bookCanvas.SetActive(false);


        if(pausedPanel != null)
        {
            pausedPanel.SetActive(false);
        }

        if(dayObject != null)
        {
            dayObject.SetActive(false);
        }

        InitDayEndTexts();
    }
    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += UpdateDateUI;

        LocalizationManager.Instance.OnLanguageChanged += RefreshDayEnd;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateDateUI;

        LocalizationManager.Instance.OnLanguageChanged -= RefreshDayEnd;
    }

    void RefreshDayEnd()
    {
        if (CurrentDay == 0) return;

        string key = dayEndMessages[CurrentDay];
        summaryText.text = LocalizationManager.Instance.GetText(key);
    }

    private void Update()
    {
        if (situationCanvas == null || playerController.mobileButton == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGameUI();
        }
    }
    public void StartNormalGame()
    {
        tutorialMode = false;
        SaveManager.Instance.data.tutorialCompleted = true;

        CurrentDay = SaveManager.Instance.data.currentDay;
        StartNewDay();
    }

    public void StartTutorialGame()
    {
        tutorialMode = true;
        SaveManager.Instance.data.tutorialCompleted = false;

        HideSorgu();
        HideSummary();
        UpdateMistakeUI();
        ShowPage(0);
    }

    public void PauseGame()
    {
        gamePaused = true;
    }

    public void ResumeGame()
    {
        gamePaused = false;
    }

    public void PauseGameUI()
    {
        if (pausedPanel != null)
            pausedPanel.SetActive(true);

        usesInfoCanvas.SetActive(false);
        pausedButton.SetActive(false);

        Time.timeScale = 0;
        playerController.AnimatorCloseOptimized();
        PauseGame();
    }
    public void BackGameUI()
    {
        if (pausedPanel != null)
            pausedPanel.SetActive(false);

        usesInfoCanvas.SetActive(true);
        pausedButton.SetActive(true);

        Time.timeScale = 1;
        playerController.AnimatorOpenOptimized();
        ResumeGame();
    }

    // Yeni bir güne başlar ve ilk karakteri çağırır.
    void StartNewDay()
    {
        playerController.ResetLookVariables();
        playerController.transform.rotation = Quaternion.Euler(0, 0, 0);

        playerController.AnimatorOpenOptimized();
        ResumeGame();

        if (gameEventManager != null)
        {
            gameEventManager.ResetEventTrigger();
        }

        enemiesProcessedToday = 0;
        approvedCountToday = 0;
        rejectCountToday = 0;
        approvedSuretCount = 0;
        totalMistakeCount = 0;

        if (totalMistakeCount == 0)
        {
            mistakeText.fontSize = 0.039f;
            mistakeText.fontStyle = FontStyles.Bold;
            mistakeText.text = "GOOD\nLUCK!";
        }

        // Eğer gün limitleri listesi yeterli uzunlukta değilse, oyunu bitir.
        if (enemiesPerDay.Count < CurrentDay)
        {
            EndDay(true);
            return;
        }

        playerController.canLook = true;

        UpdateDateUI(); // Tarih textlerini güncelle
        // Gün başlangıcı duyurusunu başlat
        StartCoroutine(ShowDayAnnounceCoroutine());

        CancelInvoke(nameof(SpawnNewEnemy));
        // İlk karakteri çağır (Duyuru bittikten sonra gelmesi için)
        Invoke(nameof(SpawnNewEnemy), dayEndDelay);

        ResetToolUsages();
    }

    // TutorialManager'ın çağıracağı özel spawn metodu
    public void SpawnTutorialEnemy(bool forcedSuret)
    {
        // Önceki karakter varsa temizle (önlem olarak)
        if (currentEnemy != null) Destroy(currentEnemy.gameObject);

        GameObject newEnemy = Instantiate(enemyPrefab, startPoint.position, startPoint.rotation);
        currentEnemy = newEnemy.GetComponent<EnemyMovement>();

        if (currentEnemy != null)
        {
            // Önce Customizer'ı alıyoruz
            CharacterCustomizer customizer = currentEnemy.GetComponent<CharacterCustomizer>();

            // Rastgele verileri oluştur (İsim, yaş vb.)
            CharacterData data = currentEnemy.GetComponent<CharacterData>();
            data.GenerateRandomData();

            // ÖNEMLİ: Customization'ı biz el ile ve istediğimiz değerle başlatıyoruz
            customizer.InitializeCustomization(forcedSuret);

            // Karakterin IsSuret bilgisini Data kısmına da geçiyoruz (Eğer tutuyorsa)
            // data.isSuret = forcedSuret;

            UpdateCharacterInfoUI(data);
            currentEnemy.StartMovementToControlPoint();
        }
    }

    // SURET SALDIRMASI
    public void SuretAttackGameOver()
    {
        // 1. Oyunu durdur
        Time.timeScale = 0f;

        // 2. Fare imlecini serbest bırak ve görünür yap
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Mevcut UI'ları (Summary, Sorgu) gizle
        HideSorgu();

        if (situationCanvas != null)
        {
            dayObject.SetActive(true);

            if (situationCanvas != null) situationCanvas.SetActive(true);

            if (deathScreen != null) deathScreen.SetActive(true);

            if (summaryScreen != null) summaryScreen.SetActive(false);

            if (failedScreen != null) failedScreen.SetActive(false);

            if (nextDayButton != null) nextDayButton.SetActive(false);

            if (tryAgainButton != null) tryAgainButton.SetActive(true);

            if (mainMenuButton != null) mainMenuButton.SetActive(true);

            if(newGameButton != null) newGameButton.SetActive(false);

            playerController.AnimatorCloseOptimized();
            PauseGame();
            CloseBook();
        }
    }

    // TRY AGAİN BUTONU
    public void RestartCurrentDay()
    {
        totalMistakeCount = 0;
        gameOverPending = false;

        // 1. Canvası kapat
        HideSummary();
        tvCanvas.SetActive(false);

        // 2. Oyunu tekrar başlat (TimeScale'ı normale çek)
        Time.timeScale = 1f;

        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
            currentEnemy = null; // Referansı temizle
        }

        StartNewDay();
    }

    private void ResetToolUsages()
    {
        if (playerController != null)
        {
            playerController.thermoRemaining = playerController.thermoUses;
            playerController.lightRemaining = playerController.lightUses;
            playerController.mirrorRemaining = playerController.mirrorUses;
            playerController.UsesUI();
        }
    }

    IEnumerator ShowDayAnnounceCoroutine()
    {
        Time.timeScale = 0f;
        playerController.canLook = false;
        // Fade in + bekleme + fade out
        ShowDayAnnounce();

        yield return new WaitForSecondsRealtime(dayannounceDuration + fadeDuration);

        HideDayAnnounce();

        // 🔹 Day announce tamamen bitti
        // 🔹 1 saniye ekstra bekle
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;
        playerController.canLook = true;
        // 🔹 ARTIK EVENT KONTROLÜ GÜVENLİ
        gameEventManager.CheckForEvent(CurrentDay);
    }

    // Yeni bir karakteri sahneye çağırır.
    void SpawnNewEnemy()
    {
        if (!tutorialMode && playerController != null)
        {
            playerController.UnlockTools();
        }

        // Gün limitini listeye bakarak belirle. currentDay 1'den başladığı için index (currentDay - 1) olmalı.
        int dayIndex = CurrentDay - 1;
        int maxEnemiesForCurrentDay = 0;

        if (enemiesPerDay.Count > dayIndex)
        {
            maxEnemiesForCurrentDay = enemiesPerDay[dayIndex];
        }
        else
        {
            // Bu gün için limit tanımlı değil, yani tüm günler tamamlandı.
            EndDay(true); // Oyunu sonlandır
            return;
        }

        // Gün limitine ulaşıp ulaşmadığımızı kontrol et.
        if (enemiesProcessedToday >= maxEnemiesForCurrentDay)
        {
            EndDay(); // Normal gün sonu
            return;
        }

        if (enemyPrefab == null || startPoint == null)
        {
            Debug.LogError("HATA: Enemy Prefab veya Start Point atanmamış!");
            return;
        }

        // StartPoint'te yeni bir Enemy Prefab'ı yarat.
        GameObject newEnemy = Instantiate(enemyPrefab, startPoint.position, startPoint.rotation);
        currentEnemy = newEnemy.GetComponent<EnemyMovement>();

        // Karakteri kontrol noktasına gönder.
        if (currentEnemy != null)
        {
            // 1. Karakterin verilerini rastgele oluştur
            currentEnemy.GetComponent<CharacterData>().GenerateRandomData();

            // 2. UI Text'leri güncelle
            UpdateCharacterInfoUI(currentEnemy.GetComponent<CharacterData>());

            // 3. Karakteri kontrol noktasına gönder
            currentEnemy.StartMovementToControlPoint();
        }
    }

    void UpdateCharacterInfoUI(CharacterData data)
    {
        if (fullNameText != null) fullNameText.text = data.fullName;
        if (ageText != null) ageText.text = data.age.ToString();
        if (countryText != null) countryText.text = data.country;
        if (originCityText != null) originCityText.text = data.originCity;
        if (reasonText != null)
            reasonText.text = LocalizationManager.Instance.GetText(data.reasonForComing);
        if (genderText != null)
            genderText.text = LocalizationManager.Instance.GetText(data.gender);
        if (illnessText != null)
            illnessText.text = LocalizationManager.Instance.GetText(data.illnessOrInjury);
        if (belongingsText != null)
            belongingsText.text = LocalizationManager.Instance.GetText(data.hasBelongings ? "YES" : "NO");
    }

    // Karar verildikten sonra sonraki karakteri çağırmadan önce gün limitini kontrol eder.
    void CheckForNextAction()
    {
        if (gameOverPending) return;

        if (tutorialMode)
        {         
            if (tManager != null) tManager.OnTutorialCharacterProcessed();
            return;
        }

        CancelInvoke(nameof(SpawnNewEnemy));
        // Yeni karakterin gelmesi için karakterin yok olmasından sonra gecikmeli olarak ayarla.
        Invoke(nameof(SpawnNewEnemy), disappearDelay + 4f);
    }

    // Kontrol noktasına ulaşıldığında çağrılır.
    public void StartSorgu()
    {
        // UI'ı göster ve sorgu başlasın.
        ShowSorgu();
        canStamp = true;
    }

    public void ProcessDecision(bool approved, bool actualSuret)
    {
        // 1. KURALI KONTROL ET (3 alet hatası varsa TRUE döner)
        // Bu, oyuncunun hata yapıp yapmadığını belirlemede kullanılan nihai kural sonucudur.
        bool suretDetectedByTools = playerController.CheckFinalDetectionResult(currentEnemy.GetComponent<CharacterData>());

        // 2. HATA HESAPLAMA MANTIĞI:

        // Oyuncu Onayladı (approved = true)
        if (approved)
        {
            // Kural 1: Gerçek Suret'i Onaylamak her zaman hatadır.
            if (actualSuret)
            {
                totalMistakeCount++;
            }
        }
        // Oyuncu Reddedildi (approved = false)
        else // Red kararı
        {
            // Kural 2: Gerçek İnsan'ı Reddetmek hatadır.
            if (!actualSuret)
            {
                // İSTİSNA KONTROLÜ: Eğer İnsan'ı reddetti ama suretDetectedByTools true ise (3 alet Suret demişse), bu hata sayılmaz.
                if (suretDetectedByTools)
                {
                    Debug.Log($"[KURAL AKTİF] İnsan reddedildi ancak 3 alet 'Suret' dediği için hata sayılmadı.");
                }
                else // Kural aktifleşmediyse ve insan reddedildiyse HATA.
                {
                    totalMistakeCount++;
                }
            }
        }

        UpdateMistakeUI();
    }
    private void UpdateMistakeUI()
    {
        if (mistakeText == null) return;

        if (!tvCanvas.activeSelf)
            tvCanvas.SetActive(true);

        if (totalMistakeCount == 1)
        {
            mistakeText.fontSize = 0.08f;
            mistakeText.text = "X";
        }
        else if (totalMistakeCount == 2)
        {
            mistakeText.fontSize = 0.08f;
            mistakeText.text = "XX";
        }
        else if (totalMistakeCount >= 3)
        {
            if (tvCanvas != null)
            {
                mistakeText.fontSize = 0.034f;
                mistakeText.fontStyle = FontStyles.Bold;
                mistakeText.text = "FAILED!";
            }
        }

        if (totalMistakeCount == 0)
        {
            mistakeText.fontSize = 0.039f;
            mistakeText.fontStyle = FontStyles.Bold;
            mistakeText.text = "GOOD\nLUCK!"; // 0 hata durumunda boş
        }
    }

    public void OnApproveButton()
    {
        if (!canStamp) return;
        canStamp = false;

        if (gameOverPending) return;

        approveStampAnimator.ResetTrigger("Stamp");
        approveStampAnimator.SetTrigger("Stamp");
        Invoke(nameof(ShowToImage), 0.5f);

        bool isSuret = currentEnemy.GetComponent<CharacterCustomizer>().IsSuret;

        ProcessDecision(true, isSuret);

        approvedCountToday++;
        enemiesProcessedToday++; // İşlenen sayısını artır

        // Karakter çıkışa yürüyor
        Invoke(nameof(SendEnemyToEndPoint), 2f);

        if (totalMistakeCount > maxAllowedMistakes)
        {
            gameOverPending = true;       // Yeni karakter spawn yok
            Invoke(nameof(TriggerGameOverAfterMovement), 6f);
        }
        else
        {
            CheckForNextAction();
        }

        Invoke(nameof(ResetStampUI), 2f);
    }
    void ShowToImage()
    {
        approvedDecisionImage.SetActive(true);
    }
    void SendEnemyToEndPoint()
    {
        if (currentEnemy != null)
            currentEnemy.GoToFinalPoint(true);
    }

    // Reddet Butonuna basıldığında çağrılacak fonksiyon.
    public void OnRejectButton()
    {
        if (!canStamp) return;
        canStamp = false;

        if (gameOverPending) return;

        rejectStampAnimator.ResetTrigger("Stamp");
        rejectStampAnimator.SetTrigger("Stamp");
        Invoke(nameof(ShowImage), 0.5f);

        bool isSuret = currentEnemy.GetComponent<CharacterCustomizer>().IsSuret;

        ProcessDecision(false, isSuret);

        rejectCountToday++;
        enemiesProcessedToday++; // İşlenen sayısını artır

        Invoke(nameof(SendEnemyToFinalPoint), 2f);

        if (totalMistakeCount > maxAllowedMistakes)
        {
            gameOverPending = true;
            Invoke(nameof(TriggerGameOverAfterMovement), 6f);
        }
        else
        {
            CheckForNextAction();
        }

        Invoke(nameof(ResetStampUI), 2f);
    }
    void ShowImage()
    {
        rejectedDecisionImage.SetActive(true);
    }

    void SendEnemyToFinalPoint()
    {
        currentEnemy.GoToFinalPoint(false);
    }

    void ResetStampUI()
    {
        HideSorgu();
    }

    //YOU FAİLED
    public void TriggerGameOverAfterMovement()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
            currentEnemy = null;
        }

        if(situationCanvas != null)
        {
            dayObject.SetActive(true);

            if (situationCanvas != null) situationCanvas.SetActive(true);
            if (summaryScreen != null) summaryScreen.SetActive(false);
            if (deathScreen != null) deathScreen.SetActive(false);
            if (nextDayButton != null) nextDayButton.SetActive(false);
            if (newGameButton != null) newGameButton.SetActive(false);
            if (failedScreen != null) failedScreen.SetActive(true);
            if (tryAgainButton != null) tryAgainButton.SetActive(true);
            if (mainMenuButton != null) mainMenuButton.SetActive(true);


            if (approvedCountText != null) approvedCountText.text = approvedCountToday.ToString();
            if (rejectCountText != null) rejectCountText.text = rejectCountToday.ToString();

            playerController.AnimatorCloseOptimized();
            PauseGame();
            CloseBook();
        } 
    }

    // BOOK
    void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == index);

        // Butonların görünürlüğü
        prevButton.gameObject.SetActive(index > 0);
        nextButton.gameObject.SetActive(index < pages.Length - 1);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }
    public void OpenBook()
    {
        if (tutorialMode && bookInteractionLocked)
            return;

        bookCanvas.SetActive(true);
        currentPage = 0;
        ShowPage(currentPage);

        isBookOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerController.canLook = false;
        PauseGame();
    }

    public void CloseBook()
    {
        bookCanvas.SetActive(false);

        isBookOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerController.canLook = true;
        ResumeGame();
    }

    // --- GÜN MANTIKLARI ---

    void EndDay(bool finalDay = false)
    {        
        if (situationCanvas != null) situationCanvas.SetActive(true);
        if (mainMenuButton != null) mainMenuButton.SetActive(true);
        if (newGameButton != null) newGameButton.SetActive(true);

        playerController.AnimatorCloseOptimized();
        PauseGame();
        CloseBook();

        if (finalDay)
        {
            dayObject.SetActive(false);

            if (situationCanvas != null) situationCanvas.SetActive(true);
            if (summaryScreen != null) summaryScreen.SetActive(false);
            if (tryAgainButton != null) tryAgainButton.SetActive(false);
            if (nextDayButton != null) nextDayButton.SetActive(false);
            if (failedScreen != null) failedScreen.SetActive(false);
            if (deathScreen != null) deathScreen.SetActive(false);
            gameEventManager.GetFinalEnding();
            playerController.AnimatorCloseOptimized();
            PauseGame();
            CloseBook();
        }
        else
        {
            // 📊 NORMAL GÜN BİTİŞİ
            ShowSummary();
        }
    }

    // NEXT DAY butonundan çağrılacak fonksiyon.
    public void GoToNextDay()
    {
        // Özet ekranını gizle
        HideSummary();

        CurrentDay++;

        // Yeni bir gün tanımlıysa, yeni günü başlat
        if (enemiesPerDay.Count >= CurrentDay)
        {
            StartNewDay();
            Time.timeScale = 1f;
        }
        else
        {
            EndDay(true); // Tanımlı gün kalmadı, oyunu bitir.
        }
    }
    // MAIN MENU butonundan çağrılacak fonksiyon.
    public void ReturnToMainMenu()
    {
        // Oyunu normale al (çok önemli)
        Time.timeScale = 1f;

        SaveManager.Instance.SaveGame();

        SceneManager.LoadScene("mainMenuScene");
    }

    // NEW GAME butonundan çağrılacak fonksiyon.
    public void RestartGameFromBeginning()
    {
        Time.timeScale = 1f;

        // Günü sıfırla
        CurrentDay = - 1;

        // Referansı temizle
        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
            currentEnemy = null;
        }

        // Save sistemin varsa:
        SaveManager.Instance.data.currentDay = 1;
        SaveManager.Instance.SaveGame();

        SceneManager.LoadScene("GameScene");

        StartNewDay();
    }

    // --- UI YÖNETİMİ ---

    // Tarih ve Gün Numarasını günceller (Hem oyun içi hem özet için)
    void UpdateDateUI()
    {
        // Başlangıç tarihi sabit
        System.DateTime startDate = new System.DateTime(1862, 11, 1);

        // Gün numarasına göre tarihi hesapla
        currentDate = startDate.AddDays(CurrentDay - 1);

        System.Globalization.CultureInfo culture;

        if (LocalizationManager.Instance.CurrentLanguageIndex == 0)
            culture = new System.Globalization.CultureInfo("en-US");
        else
            culture = new System.Globalization.CultureInfo("tr-TR");

        string day = currentDate.ToString("dd", culture);
        string month = currentDate.ToString("MMMM", culture);
        string year = currentDate.ToString("yyyy", culture);

        string formattedDate = day + "\n" + month + "\n" + year;

        if (dayAnnounceText != null)
            dayAnnounceText.text = formattedDate;

        string summaryDate = currentDate.ToString("dd MMMM yyyy", culture);

        if (dayText != null)
            dayText.text = summaryDate;
    }

    public void ShowDayAnnounce()
    {
        StartCoroutine(DayAnnounceFadeInCoroutine());
    }

    public void HideDayAnnounce()
    {
        StartCoroutine(DayAnnounceFadeOutCoroutine());
    }
    IEnumerator DayAnnounceFadeInCoroutine()
    {
        if (dayAnnounceCanvas != null) dayAnnounceCanvas.SetActive(true);

        usesInfoCanvas.SetActive(false);
        pausedCanvas.SetActive(false);

        // 🔹 FADE IN
        yield return StartCoroutine(FadeCanvas(0f, 1f));

        // 🔹 BEKLE
        yield return new WaitForSecondsRealtime(dayannounceDuration);
    }
    IEnumerator DayAnnounceFadeOutCoroutine()
    {
        // 🔹 FADE OUT
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        if (dayAnnounceCanvas != null) dayAnnounceCanvas.SetActive(false);

        usesInfoCanvas.SetActive(true);
        pausedCanvas.SetActive(true);
    }
    IEnumerator FadeCanvas(float from, float to)
    {
        float elapsed = 0f;
        dayAnnounceCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            dayAnnounceCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        dayAnnounceCanvasGroup.alpha = to;
    }
    // Gün Sonu Özet UI'ını günceller ve gösterir
    public void ShowSummary()
    {
        ShowDayEndText(CurrentDay);

        SaveManager.Instance.data.currentDay = CurrentDay +1;
        SaveManager.Instance.SaveGame();

        // Tarih ve Gün Numarasını güncelle
        UpdateDateUI();

        if (situationCanvas != null)
        {
            bool isLastDay = CurrentDay >= enemiesPerDay.Count;

            if (nextDayButton != null)
            {
                // Eğer mevcut gün listedeki son günse (veya listeyi aştıysa) butonu kapat.
                nextDayButton.SetActive(!isLastDay);
            }

            dayObject.SetActive(true);

            if (situationCanvas != null) situationCanvas.SetActive(true);

            if (tryAgainButton != null) tryAgainButton.SetActive(false);

            if (failedScreen != null) failedScreen.SetActive(false);    

            if (deathScreen != null) deathScreen.SetActive(false);

            if (newGameButton != null) newGameButton.SetActive(false);

            if (summaryScreen != null) summaryScreen.SetActive(true);

            if (nextDayButton != null) nextDayButton.SetActive(true);

            if (mainMenuButton != null) mainMenuButton.SetActive(true);

            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject);
                currentEnemy = null; // Referansı temizle
            }

            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // İstatistikleri güncelle
            if (approvedCountText != null) approvedCountText.text = approvedCountToday.ToString();
            if (rejectCountText != null) rejectCountText.text = rejectCountToday.ToString();

            playerController.AnimatorCloseOptimized();
            PauseGame();
            CloseBook();
        }
    }
    void InitDayEndTexts()
    {
        dayEndMessages.Add(1, "DAY_1_END");
        dayEndMessages.Add(2, "DAY_2_END");
        dayEndMessages.Add(3, "DAY_3_END");
        dayEndMessages.Add(4, "DAY_4_END");
        dayEndMessages.Add(5, "DAY_5_END");
        dayEndMessages.Add(6, "DAY_6_END");
        dayEndMessages.Add(7, "DAY_7_END");
        dayEndMessages.Add(8, "DAY_8_END");
        dayEndMessages.Add(9, "DAY_9_END");
        dayEndMessages.Add(10, "DAY_10_END");
        dayEndMessages.Add(11, "DAY_11_END");
        dayEndMessages.Add(12, "DAY_12_END");
        dayEndMessages.Add(13, "DAY_13_END");
        dayEndMessages.Add(14, "DAY_14_END");
        dayEndMessages.Add(15, "DAY_15_END");
        dayEndMessages.Add(16, "DAY_16_END");
        dayEndMessages.Add(17, "DAY_17_END");
        dayEndMessages.Add(18, "DAY_18_END");
        dayEndMessages.Add(19, "DAY_19_END");
        dayEndMessages.Add(20, "DAY_20_END");
        dayEndMessages.Add(21, "DAY_21_END");
        dayEndMessages.Add(22, "DAY_22_END");
        dayEndMessages.Add(23, "DAY_23_END");
        dayEndMessages.Add(24, "DAY_24_END");
        dayEndMessages.Add(25, "DAY_25_END");
        dayEndMessages.Add(26, "DAY_26_END");
        dayEndMessages.Add(27, "DAY_27_END");
        dayEndMessages.Add(28, "DAY_28_END");
        dayEndMessages.Add(29, "DAY_29_END");
        dayEndMessages.Add(30, "DAY_30_END");
        dayEndMessages.Add(31, "DAY_31_END");
        dayEndMessages.Add(32, "DAY_32_END");
        dayEndMessages.Add(33, "DAY_33_END");
        dayEndMessages.Add(34, "DAY_34_END");
        dayEndMessages.Add(35, "DAY_35_END");
    }
    public void ShowDayEndText(int day)
    {
        if (dayEndMessages.ContainsKey(day))
        {
            string key = dayEndMessages[day];
            summaryText.text = LocalizationManager.Instance.GetText(key);
        }
        else
        {
            summaryText.text = LocalizationManager.Instance.GetText("DAY_DEFAULT_END");
        }
    }

    public void HideSummary()
    {
        if (situationCanvas != null)
        {
            situationCanvas.SetActive(false);
            mainMenuButton.SetActive(false);
            nextDayButton.SetActive(false);
            if (newGameButton != null) newGameButton.SetActive(false);

            // YENİ: Özet ekranı kapandığında fare imlecini tekrar kilitle ve gizle
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // UI'ı göster ve fare imlecini serbest bırak
    public void ShowSorgu()
    {
        if (sorguCanvas != null) sorguCanvas.SetActive(true);
    }   

    // UI'ı gizle ve fare imlecini tekrar kilitle
    public void HideSorgu()
    {
        if (sorguCanvas != null) sorguCanvas.SetActive(false);

        if (approvedDecisionImage != null) approvedDecisionImage.SetActive(false);
        if (rejectedDecisionImage != null) rejectedDecisionImage.SetActive(false);
    }
}
