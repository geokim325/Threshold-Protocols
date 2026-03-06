using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting.Dependencies.NCalc;

// ******************************************************************************
// ********* FirstPersonLook: Ana Kamera Kontrol ve Etkileşim Scripti **********
// ******************************************************************************

public class FirstPersonLook : MonoBehaviour
{
    [Header("Mobil Input")]
    public bool isMobile = true;
    public GameObject mobileButton;
    private bool mobileInteractPressed = false;

    private Vector2 lastTouchPos;
    private bool isLookingWithTouch = false;

    [Header("Mouse Hassasiyet Ayarları")]
    public float mouseSensitivity = 150f;

    [Header("Bakış Limitleri")]
    [Tooltip("Kameranın yukarıya bakabileceği maksimum açı")]
    public float upperLookLimit = 80f;

    [Tooltip("Kameranın aşağıya bakabileceği maksimum açı")]
    public float lowerLookLimit = -80f;

    [Tooltip("Sağa bakış limiti (örnek: 60)")]
    public float rightLookLimit = 90f;

    [Tooltip("Sola bakış limiti (örnek: -60)")]
    public float leftLookLimit = -90f;

    [Header("Raycast Ayarları")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;
    public Gamemanager gameManager;
    private Camera cam;

    [Header("Crosshair")]
    public Image crosshairImage;
    public Color normalColor = Color.white;
    public Color interactColor = Color.green;

    [Header("Buton Animasyon")]
    public Animator checksAnimator;

    public GameObject approveObject;
    public GameObject rejectObject;

    [Header("Termometre")]
    public GameObject thermometer;
    public int thermoUses = 3;
    [Range(0f, 100f)]
    public float thermoAccuracy = 50f;
    public GameObject thermometerCanvas;
    public TextMeshProUGUI thermometerUIText;
    public TextMeshProUGUI thermometerUsesUIText;
    public Animator thermoAnimator;

    [Header("Işık")]
    public GameObject lightObject;
    public int lightUses = 3;
    [Range(0f, 100f)]
    public float lightAccuracy = 70f;
    public TextMeshProUGUI lightUsesUIText;
    public GameObject lightObjectPrefab;
    [SerializeField] Material lampEmissionMaterial;
    public Color emissionColor = Color.yellow;
    public float emissionIntensity = 2f;

    [Header("Kapsül Sistemi")]
    public GameObject mirrorObject;
    public int mirrorUses = 2;
    [Range(0f, 100f)]
    public float mirrorAccuracy = 90f;
    public TextMeshProUGUI capsulUsesUIText;
    public GameObject characterCopyPrefab;
    public Transform cloneSpawnPosition;
    private GameObject currentClone = null; // Aktif kopyayı tutar.
    public Animator capsulAnimator;

    [Header("Saldırı Mekaniği")]
    [Range(0f, 100f)]
    public float suretAttackChance = 40f;

    private float xRotation = 0f; // Yukarı-aşağı dönüş
    private float yRotation = 0f; // Sağa-sola dönüş
    private Vector3 fixedPosition; // Sabit pozisyon
    private Coroutine shadowCoroutine;

    [HideInInspector] public int thermoRemaining;
    [HideInInspector] public int lightRemaining;
    [HideInInspector] public int mirrorRemaining;

    [HideInInspector] public bool canLook = true;

    [HideInInspector] public CharacterData currentTarget; // Hangi karakter için kullanılıyor

    bool isOn = false;
    private bool isCapsuleOpen = false;
    private bool isProcessingDecision = false;

    public EnemyMovement enemyMovement;
    public SlidingDoorTrigger slidingDoorTrigger;
    public static FirstPersonLook Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        SaveManager.Instance.LoadGame();

        // Mouse'u gizle ve kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentTarget = null;
        isProcessingDecision = false;

        cam = GetComponent<Camera>();

        if (thermometerCanvas != null)
            thermometerCanvas.SetActive(false);

        SetEmission(false);

        // Kameranın sabit pozisyonunu al
        fixedPosition = transform.position;

        lightObject.SetActive(false);
        crosshairImage.color = normalColor;

        thermoRemaining = thermoUses;
        lightRemaining = lightUses;
        mirrorRemaining = mirrorUses;

        UsesUI();
    }

    void Update()
    {
        if (gameManager != null && gameManager.gamePaused)
            return;

        if (isMobile && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
        }

        HandleMouseLook();

        // Raycast ve etkileşimleri her zaman kontrol et (butona basma ve döndürme başlatma)
        HandleRaycastAndInteraction();

    }

    public void UsesUI()
    {
        if (thermometerUsesUIText != null)
            thermometerUsesUIText.text = Mathf.Max(0, thermoRemaining).ToString();

        if (lightUsesUIText != null)
            lightUsesUIText.text = Mathf.Max(0, lightRemaining).ToString();

        if (capsulUsesUIText != null)
            capsulUsesUIText.text = Mathf.Max(0, mirrorRemaining).ToString();
    }

    //mobil
    public void MobileInteractDown()
    {
        mobileInteractPressed = true;
    }

    //mobil
    public void MobileInteractUp()
    {
        mobileInteractPressed = false;
    }
    public void ResetLookVariables()
    {
        xRotation = 0f;
        yRotation = 0f;
    }

    private void HandleMouseLook()
    {
        if (!canLook) return;

        float deltaX = 0f;
        float deltaY = 0f;

        if (!isMobile)
        {
            deltaX = Input.GetAxis("Mouse X");
            deltaY = Input.GetAxis("Mouse Y");
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Eğer bu parmak UI üzerindeyse (buton), kamerayı etkilemesin
                if (UnityEngine.EventSystems.EventSystem.current
                    .IsPointerOverGameObject(touch.fingerId))
                    return;

                if (touch.phase == TouchPhase.Began)
                {
                    lastTouchPos = touch.position;
                    isLookingWithTouch = true;
                }
                else if (touch.phase == TouchPhase.Moved && isLookingWithTouch)
                {
                    Vector2 delta = touch.position - lastTouchPos;
                    lastTouchPos = touch.position;

                    deltaX = delta.x * 0.05f;
                    deltaY = delta.y * 0.05f;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isLookingWithTouch = false;
                }
            }
        }

        float sensitivity = isMobile ? SaveManager.Instance.data.sensitivity : mouseSensitivity;

        xRotation -= deltaY * sensitivity;
        xRotation = Mathf.Clamp(xRotation, lowerLookLimit, upperLookLimit);

        yRotation += deltaX * sensitivity;
        yRotation = Mathf.Clamp(yRotation, leftLookLimit, rightLookLimit);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.position = fixedPosition;
    }

    private void HandleRaycastAndInteraction()
    {
        if (cam == null || gameManager == null) return;

        bool interactPressed = false;

        if (!isMobile && Input.GetMouseButtonDown(0))
            interactPressed = true;

        if (isMobile && mobileInteractPressed)
        {
            interactPressed = true;
            mobileInteractPressed = false; // ⚡ tek frame çalışsın
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            crosshairImage.color = interactColor;

            if (interactPressed)
            {
                string hitName = hit.collider.name;

                // ================= BUTTONS =================
                if (hit.collider.gameObject == approveObject)
                {
                    HandleToolInteraction(hit.point, (character) =>
                    {
                        if (isProcessingDecision)
                            return false;

                        isProcessingDecision = true;

                        SaveManager.Instance.Vibrate();

                        Invoke(nameof(CloseLampSystem), 2f);
                        gameManager.OnApproveButton();
                        thermometerCanvas.SetActive(false);

                        if (isCapsuleOpen)
                        {
                            if (capsulAnimator != null)
                            {
                                capsulAnimator.Play("capsulClosed");
                            }
                            isCapsuleOpen = false; // Kapanınca tekrar false yap
                        }

                        Invoke(nameof(DestroyReflectedCopy), 1f); // Onay/Red durumunda kopyayı temizle
                        currentTarget = character;
                        return true;
                    });
                }
                else if (hit.collider.gameObject == rejectObject)
                {
                    HandleToolInteraction(hit.point, (character) =>
                    {
                        if (isProcessingDecision)
                            return false;

                        isProcessingDecision = true;

                        SaveManager.Instance.Vibrate();

                        Invoke(nameof(CloseLampSystem), 2f);
                        gameManager.OnRejectButton();
                        thermometerCanvas.SetActive(false);

                        if (isCapsuleOpen)
                        {
                            if (capsulAnimator != null)
                            {
                                capsulAnimator.Play("capsulClosed");
                            }
                            isCapsuleOpen = false; // Kapanınca tekrar false yap
                        }

                        Invoke(nameof(DestroyReflectedCopy), 1f); // Onay/Red durumunda kopyayı temizle
                        currentTarget = character;
                        return true;
                    });                      
                }

                // ================= BOOK =================
                else if (hit.collider.gameObject == gameManager.bookObject)
                {
                    if (isProcessingDecision) return;

                    if (gameManager.isBookOpen)
                        return;

                    gameManager.OpenBook();
                }

                // ================= TERMOMETER =================
                else if (hit.collider.gameObject == thermometer)
                {
                    if (isProcessingDecision) return;

                    HandleToolInteraction(hit.point, (character) =>
                    {
                        if (character.usedThermo)
                        {                           
                            return false;
                        }

                        if (!CanUse(ref thermoRemaining, "Termometre"))
                            return false;

                        SaveManager.Instance.Vibrate();

                        thermoAnimator.ResetTrigger("thermo");
                        thermoAnimator.SetTrigger("thermo");

                        if (thermometerCanvas != null)
                            thermometerCanvas.SetActive(true);

                        character.usedThermo = true;
                        currentTarget = character;
                        ApplyThermometer(character);
                        TrySuretAttack();
                        return true;
                    });
                }

                // ================= LIGHT =================
                else if (hit.collider.gameObject == lightObjectPrefab)
                {
                    if (isProcessingDecision) return;

                    HandleToolInteraction(hit.point, (character) =>
                    {
                        if (!isOn)
                        {
                            OpenLampSystem();
                        }

                        if (character.usedLight)
                        {
                            return false;
                        }

                        if (!CanUse(ref lightRemaining, "Işık"))
                            return false;

                        SaveManager.Instance.Vibrate();

                        character.usedLight = true;

                        currentTarget = character;
                        lightObject.SetActive(true);
                        ApplyLightInspection(character);
                        TrySuretAttack();
                        return true;
                    });
                }

                // ================= MIRROR =================
                else if (mirrorObject != null && hit.collider.gameObject == mirrorObject)
                {
                    if (isProcessingDecision) return;

                    HandleToolInteraction(hit.point, (character) =>
                    {
                        if (character.usedMirror)
                        {
                            return false;
                        }

                        if (!CanUse(ref mirrorRemaining, "Ayna"))
                            return false;

                        SaveManager.Instance.Vibrate();

                        character.usedMirror = true;

                        checksAnimator.ResetTrigger("capsul");
                        checksAnimator.SetTrigger("capsul");

                        if (capsulAnimator != null)
                        {
                            capsulAnimator.Play("capsulOpen");
                            isCapsuleOpen = true; // Kapsülün açıldığını not et
                        }
                        // Mevcut hedefi ayarla ve kapsül denetimini uygula
                        currentTarget = character;
                        TrySuretAttack();
                        return ApplyMirrorInspection(character);
                    });
                }
            }
        }
        else
        {
            crosshairImage.color = normalColor;
        }
    }

    public void UnlockTools()
    {
        isProcessingDecision = false;
        if(currentTarget != null)
        {
            currentTarget = null;
        }
        slidingDoorTrigger.CloseDoor();
    }

    public void AnimatorCloseOptimized()
    {
        if (checksAnimator) checksAnimator.enabled = false;
        if (thermoAnimator) thermoAnimator.enabled = false;
        if (capsulAnimator) capsulAnimator.enabled = false;

        if (gameManager != null)
        {
            if (gameManager.approveStampAnimator != null)
                gameManager.approveStampAnimator.enabled = false;

            if (gameManager.rejectStampAnimator != null)
                gameManager.rejectStampAnimator.enabled = false;
        }
    }
    public void AnimatorOpenOptimized()
    {
        if (checksAnimator) checksAnimator.enabled = true;
        if (thermoAnimator) thermoAnimator.enabled = true;
        if (capsulAnimator) capsulAnimator.enabled = true;

        if (gameManager != null)
        {
            if (gameManager.approveStampAnimator != null)
                gameManager.approveStampAnimator.enabled = true;

            if (gameManager.rejectStampAnimator != null)
                gameManager.rejectStampAnimator.enabled = true;
        }
    }

    /// <summary>
    /// Tekrarlanan araç etkileşim mantığını soyutlar.
    /// </summary>
    private void HandleToolInteraction(Vector3 hitPoint, System.Func<CharacterData, bool> action)
    {
        // Raycast noktasının etrafındaki colliderları kontrol et
        Collider[] colliders = Physics.OverlapSphere(hitPoint, interactDistance);

        foreach (Collider col in colliders)
        {
            CharacterData character = col.GetComponent<CharacterData>();

            if (character != null && character.enemyMovement.reachedControlPoint)
            {
                // Karakter masada duruyorsa, aksiyonu çağır.
                if (action(character))
                {
                    // İşlem başarılı (ilk uygun karakteri bulduk ve işledik)
                    break;
                }
            }
        }
    }

    private bool CanUse(ref int remaining, string toolName)
    {
        if (remaining <= 0)
        {
            return false;
        }

        remaining--;
        UsesUI();
        return true;
    }

    // TERMOMETRE //
    private void ApplyThermometer(CharacterData character)
    {
        float accuracy = thermoAccuracy; // %50 doğruluk
        bool actualSuret = character.characterCustomizer.IsSuret;
        bool detectSURET;

        if (Random.Range(0f, 100f) < accuracy)
        {
            detectSURET = actualSuret;
        }
        else
        {
            detectSURET = !actualSuret;
        }

        character.thermoResultIsSuret = detectSURET;
        character.usedThermo = true;

        float bodyTemp;

        if (detectSURET)
        {
            float anomalyType = Random.value;

            if (anomalyType < 0.5f)
            {
                bodyTemp = Random.Range(38f, 40f);
            }
            else
            {
                bodyTemp = Random.Range(35f, 36f);
            }
        }
        else
        {
            bodyTemp = Random.Range(36f, 37.5f);
        }

        StartCoroutine(ShowThermoResultAfterDelay(bodyTemp, 1f));
    }
    private IEnumerator ShowThermoResultAfterDelay(float tempToShow, float delay)
    {
        // Önce UI'ı temizleyelim veya "Ölçülüyor..." yazdıralım
        if (thermometerUIText != null)
            thermometerUIText.text = "--.-°C";

        if (thermometerCanvas != null)
            thermometerCanvas.SetActive(true);

        // Animasyon süresi kadar bekle
        yield return new WaitForSeconds(delay);

        // Süre dolunca sonucu yazdır
        if (thermometerUIText != null)
        {
            thermometerUIText.text = $"{tempToShow:0.0}°C";
        }
    }

    // IŞIK //
    private void ApplyLightInspection(CharacterData character)
    {
        float accuracy = lightAccuracy; // %70 doğruluk
        bool actualSuret = character.characterCustomizer.IsSuret;
        bool detectSURET;

        if (Random.Range(0f, 100f) < accuracy)
        {
            detectSURET = actualSuret;
        }
        else
        {
            detectSURET = !actualSuret;
        }

        character.lightResultIsSuret = detectSURET;
        character.usedLight = true;

        if (lightObject != null)
        {
            Light spot = lightObject.GetComponent<Light>();
            if (spot != null)
            {
                if (shadowCoroutine != null)
                    StopCoroutine(shadowCoroutine);

                if (detectSURET)
                {
                    // Suret tespit edilirse gölge dalgalanmasını başlat
                    shadowCoroutine = StartCoroutine(ShadowWave(spot));
                }
                else
                {
                    // Normal insan ise gölgeyi normal yap
                    spot.shadowStrength = 0.5f;
                }
            }
        }
    }
    public void OpenLampSystem()
    {
        isOn = true;
        SetEmission(true);

        checksAnimator.SetTrigger("light");
    }
    public void CloseLampSystem()
    {
        isOn = false;
        SetEmission(false);
    }

    void SetEmission(bool on)
    {
        if (lampEmissionMaterial == null)
        {
            return;
        }

        if (on)
        {
            lampEmissionMaterial.EnableKeyword("_EMISSION");
            lampEmissionMaterial.SetColor(
                "_EmissionColor",
                emissionColor * emissionIntensity
            );
        }
        else
        {
            lampEmissionMaterial.SetColor("_EmissionColor", Color.black);
            lampEmissionMaterial.DisableKeyword("_EMISSION");
        }
    }

    // Coroutine ile gölge dalgalanması
    private IEnumerator ShadowWave(Light spot)
    {
        float timer = 0f;
        while (spot != null && spot.enabled)
        {
            timer += Time.deltaTime;
            float wave = Mathf.PerlinNoise(timer * 0.9f, 0f);
            spot.shadowStrength = Mathf.Lerp(0.2f, 0.8f, wave);
            yield return null;
        }

        if (spot != null)
            spot.shadowStrength = 0.5f;
    }

    public void TurnOffLight()
    {
        if (lightObject != null)
        {
            lightObject.SetActive(false);

            if (shadowCoroutine != null)
            {
                StopCoroutine(shadowCoroutine);
                Light spot = lightObject.GetComponent<Light>();
                if (spot != null)
                    spot.shadowStrength = 0.5f;
                shadowCoroutine = null;
            }
        }
    }

    // Kapsül İŞLEMLERİ //

    /// <summary>
    /// %90 doğrulukla ayna denetimini uygular.
    /// Suretse (tespit edilirse) yansıma oluşturulmaz.
    /// Normal insansa yansıma oluşturulur.
    /// </summary>
    private bool ApplyMirrorInspection(CharacterData character)
    {
        float accuracy = mirrorAccuracy; // %90 doğruluk
        bool actualSuret = character.characterCustomizer.IsSuret;
        bool detectSURET;

        // Doğruluk kontrolü
        if (Random.Range(0f, 100f) < accuracy)
        {
            detectSURET = actualSuret;
        }
        else
        {
            detectSURET = !actualSuret; // Yanlış sonuç
        }

        character.mirrorResultIsSuret = detectSURET;
        character.usedMirror = true;

        // Klonun görünür olup olmayacağını belirle: Suret değilse görünür olmalı.
        bool shouldBeVisible = !detectSURET;

        // Önceki klonu temizle
        DestroyReflectedCopy();

        if (shouldBeVisible)
        {
            // Normal İnsan → Klon oluştur
            CreateReflectedCopy(character.characterCustomizer);
        }

        return true;
    }

    /// <summary>
    /// Ana karakterin kopyasını oluşturur, klonlama script'ini atar ve pozisyonunu ayarlar.
    /// </summary>
    private void CreateReflectedCopy(CharacterCustomizer mainCharacterCustomizer)
    {
        // HATA DÜZELTME: Eğer klon zaten varsa, yeni bir klon oluşturmayı engelle ve hemen çık.
        // Bu, hızlı veya birden fazla tıklamayla yeni klonlar oluşmasını önler.
        if (currentClone != null)
        {
            // Debug.LogWarning("Yansıma klonu zaten aktif. Yeniden oluşturma engellendi.");
            return;
        }

        if (mainCharacterCustomizer == null)
        {
            return;
        }

        if (characterCopyPrefab == null || cloneSpawnPosition == null)
        {
            return;
        }

        // Kopyayı belirlenen konumda oluştur
        currentClone = Instantiate(characterCopyPrefab, cloneSpawnPosition.position, cloneSpawnPosition.rotation);
        currentClone.name = "Reflected_Copy_of_" + mainCharacterCustomizer.gameObject.name;

        // Klonlama script'ini al veya ekle
        CharacterCloner cloner = currentClone.GetComponent<CharacterCloner>();
        if (cloner == null)
        {
            cloner = currentClone.AddComponent<CharacterCloner>();
        }

        // Görsel klonlamayı yap ve animasyon senkronizasyonunu başlat
        cloner.CloneVisualsFrom(mainCharacterCustomizer);
        cloner.EnableSync(true);
        // Ana karakterin kendi Transform'unu kullanın (parent'ı olmasa bile çalışır)
        Transform sourceRoot = mainCharacterCustomizer.transform;
        cloner.SetupBoneMapping(sourceRoot);
    }

    /// <summary>
    /// Aktif kopyayı sahneden kaldırır. 
    /// NOT: Bu metot, karakter kontrol noktasından ayrılırken GameManager/EnemyMovement tarafından çağrılmalıdır.
    /// </summary>
    public void DestroyReflectedCopy()
    {
        if (currentClone != null)
        {
            CharacterCloner cloner = currentClone.GetComponent<CharacterCloner>();
            cloner.EnableSync(false);

            Destroy(currentClone);
            currentClone = null;     
        }
    }

    public void TrySuretAttack()
    {
        // Yalnızca şu anki hedef (currentTarget) bir Suret ise kontrolü yap.
        if (currentTarget == null || !currentTarget.characterCustomizer.IsSuret)
        {
            return;
        }

        // 1. Alet Sayısını Hesapla
        int usedTools = 0;
        if (currentTarget.usedThermo) usedTools++;
        if (currentTarget.usedLight) usedTools++;
        if (currentTarget.usedMirror) usedTools++;

        // 2. Kural: Alet sayısı 2'den fazla olmalı (yani 3 olmalı)
        if (usedTools > 2)
        {
            // 3. Saldırı Şansını Hesapla (0-100 aralığını kullanıyoruz)
            float roll = Random.Range(0f, 100f);

            if (roll <= suretAttackChance)
            {
                // EnemyMovement script'i üzerinden saldırıyı tetikle
                EnemyMovement enemy = currentTarget.enemyMovement;
                if (enemy != null)
                {
                    // 'Attack' animasyonunu tetikle ve hareketi durdur
                    StartCoroutine(StartSuretAttackWithDelay(enemy));
                }
            }
        }
    }
    private IEnumerator StartSuretAttackWithDelay(EnemyMovement enemy)
    {
        // 1. Durdurma Gecikmesi: Saldırı animasyonunun tetiklenmesi için bekle
        // Bu gecikme sırasında düşman hala yerinde durur veya yavaşlar.
        yield return new WaitForSeconds(enemy.attackAnimStartTime); // <-- 1 SANİYE BEKLE

        // 2. Saldırıyı Tetikle
        if (enemy != null)
        {
            SaveManager.Instance.Vibrate();
            // EnemyMovement script'i üzerindeki saldırı metodunu çağır
            enemy.SuretAttack();
        }
    }

    /// <summary>
    /// Alet sonuçlarını kontrol eder ve kuralı uygular:
    /// Eğer 3 alet de Suret demişse, gerçek karakter temiz olsa bile onu Suret olarak işaretler.
    /// </summary>
    public bool CheckFinalDetectionResult(CharacterData character)
    {
        bool isActuallySuret = character.characterCustomizer.IsSuret;

        // 3 ALETİN HEPSİ "SURET" DEDİ Mİ?
        bool allToolsSaySuret =
        character.thermoResultIsSuret &&
        character.lightResultIsSuret &&
        character.mirrorResultIsSuret;

        // Oyuncunun istediği özel kural:
        // Eğer karakter GERÇEKTE TEMİZ ise (isActuallySuret == false)
        // VE 3 alet yanlışlıkla "suret" dediyse → suret olarak işaretlenir.
        if (!isActuallySuret && allToolsSaySuret)
        {
            return true; // SURET diye işaretle
        }

        // Diğer tüm durumlarda gerçek veriyi döndür
        return isActuallySuret;
    }
    public void ResetInteractionState()
    {
        // Raycast / hedef temizliği
        currentTarget = null;

        // Input & karar kilitleri
        isProcessingDecision = false;
        isCapsuleOpen = false;
        mobileInteractPressed = false;

        // UI temizliği
        if (thermometerCanvas != null)
            thermometerCanvas.SetActive(false);

        // Aydınlatma ve tool state
        CloseLampSystem();
        TurnOffLight();
        DestroyReflectedCopy();

        // Crosshair reset
        if (crosshairImage != null)
            crosshairImage.color = normalColor;
    }
}

// ******************************************************************************
// ********* CharacterCloner: Görsel Kopyalama ve Poz Senkronizasyonu ***********
// ******************************************************************************

public class CharacterCloner : MonoBehaviour
{
    private Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
    private struct BonePair
    {
        public Transform source;
        public Transform target;
    }

    private List<BonePair> bonePairs = new List<BonePair>(64);

    private Transform sourceRootTransform;
    private bool syncEnabled = false;

    public void EnableSync(bool enable)
    {
        syncEnabled = enable;
    }

    /// <summary>
    /// Kaynak karakterden tüm görsel ayarları (saç, kıyafet, göz rengi, materyal) kopyalar.
    /// </summary>
    public void CloneVisualsFrom(CharacterCustomizer mainCharacter)
    {
        // 1. Cinsiyeti Kopyala
        CopyGender(mainCharacter.isMale, mainCharacter.maleRoot, mainCharacter.femaleRoot);

        // 2. Kıyafet, Saç, Çanta, Göz Kopyalama
        CopyActiveComponents(mainCharacter.maleHairParent, mainCharacter.femaleHairParent, mainCharacter.isMale);
        CopyActiveComponents(mainCharacter.maleBagParent, mainCharacter.femaleBagParent, mainCharacter.isMale);
        CopyActiveComponents(mainCharacter.maleClothesParent, mainCharacter.femaleClothesParent, mainCharacter.isMale);
        CopyActiveComponents(mainCharacter.eyeColorsParent, null, true); // Göz rengi cinsiyetten bağımsız

        CopyHatFromClothesSet(mainCharacter);

        // 3. Ten Materyalini Kopyala
        Material targetMaterial = mainCharacter.IsSuret ? mainCharacter.suretMaterial : mainCharacter.normalMaterial;
        ApplyMaterial(targetMaterial);
    }
    public void SetupBoneMapping(Transform sourceRoot)
    {
        sourceRootTransform = sourceRoot;

        boneMap.Clear();
        bonePairs.Clear();

        // 1️⃣ Kaynak kemikleri map'le
        Dictionary<string, Transform> sourceMap = new Dictionary<string, Transform>(64);
        MapAllTransforms(sourceRootTransform, sourceMap);

        // 2️⃣ Klon kemikleri map'le
        Dictionary<string, Transform> targetMap = new Dictionary<string, Transform>(64);
        MapAllTransforms(transform, targetMap);

        // 3️⃣ İsim eşleşmesine göre BonePair oluştur
        foreach (var kvp in sourceMap)
        {
            if (targetMap.TryGetValue(kvp.Key, out Transform targetBone))
            {
                bonePairs.Add(new BonePair
                {
                    source = kvp.Value,
                    target = targetBone
                });
            }
        }
    }

    void LateUpdate()
    {
        if (!syncEnabled) return;
        if (bonePairs.Count == 0) return;

        for (int i = 0; i < bonePairs.Count; i++)
        {
            BonePair pair = bonePairs[i];
            pair.target.localPosition = pair.source.localPosition;
            pair.target.localRotation = pair.source.localRotation;
        }
    }

    /// <summary>
    /// Kaynak karakterin aktif kıyafet setindeki "Hat" alt objesinin durumunu ve materyalini kopyalar.
    /// </summary>
    private void CopyHatFromClothesSet(CharacterCustomizer mainCharacter)
    {
        // A. Kaynak Karakterin Aktif Kıyafet Setini Bul
        // Bu kısım, CharacterCustomizer'ın hangi kıyafet setinin aktif olduğunu bilmesini varsayar.
        // Eğer mainCharacter.currentActiveClothes adında bir alanınız varsa onu kullanın.
        // Aksi takdirde, aktif olanı bulmaya çalışmalısınız. En basit yol, kıyafet setlerinin parent'ını bulmaktır.

        GameObject sourceClothesParent = mainCharacter.isMale
            ? mainCharacter.maleClothesParent
            : mainCharacter.femaleClothesParent;

        if (sourceClothesParent == null) return;

        // 1. Kaynak Karakterde Hangi Kıyafet Seti Aktif?
        Transform sourceActiveClothesSet = null;
        foreach (Transform child in sourceClothesParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                sourceActiveClothesSet = child;
                break;
            }
        }

        if (sourceActiveClothesSet == null) return;

        // 2. Kaynak Kıyafet Setinde "Hat" var mı?
        Transform sourceHatTransform = sourceActiveClothesSet.Find("Hat");
        if (sourceHatTransform == null) return; // Kaynakta şapka yoksa, kopyada da olmaz.

        // 3. Klon Üzerindeki Hedef Kıyafet Setini Bul
        Transform targetClothesParentTransform = FindChildRecursively(transform, sourceClothesParent.name);
        if (targetClothesParentTransform == null) return;

        // Klon üzerinde aynı isimli kıyafet setini bul
        Transform targetActiveClothesSet = targetClothesParentTransform.Find(sourceActiveClothesSet.name);
        if (targetActiveClothesSet == null) return;

        // 4. Klon Üzerindeki "Hat" Objesini Bul ve Durumu Kopyala
        Transform targetHatTransform = targetActiveClothesSet.Find("Hat");

        if (targetHatTransform != null)
        {
            // Aktiflik durumunu kopyala
            bool isActive = sourceHatTransform.gameObject.activeSelf;
            targetHatTransform.gameObject.SetActive(isActive);

            // Materyal ve Renk Kopyalama (Görsel tutarlılık için)
            MeshRenderer sourceMesh = sourceHatTransform.GetComponent<MeshRenderer>();
            MeshRenderer targetMesh = targetHatTransform.GetComponent<MeshRenderer>();

            if (sourceMesh != null && targetMesh != null)
            {
                targetMesh.sharedMaterials = sourceMesh.sharedMaterials;
            }
        }
    }
    private void CopyGender(bool isMale, GameObject sourceMaleRoot, GameObject sourceFemaleRoot)
    {
        GameObject targetMaleRoot = transform.Find(sourceMaleRoot.name)?.gameObject;
        GameObject targetFemaleRoot = transform.Find(sourceFemaleRoot.name)?.gameObject;

        if (targetMaleRoot != null) targetMaleRoot.SetActive(isMale);
        if (targetFemaleRoot != null) targetFemaleRoot.SetActive(!isMale);
    }

    private void CopyActiveComponents(GameObject sourceMaleParent, GameObject sourceFemaleParent, bool isMale)
    {
        GameObject sourceParent = isMale ? sourceMaleParent : sourceFemaleParent;
        if (sourceParent == null) return;

        Transform targetParentTransform =
            FindChildRecursively(transform, sourceParent.name);

        if (targetParentTransform == null) return;

        foreach (Transform sourceChild in sourceParent.transform)
        {
            if (sourceChild.name == "Hat") continue;

            Transform targetChild =
                FindChildRecursively(targetParentTransform, sourceChild.name);

            if (targetChild == null) continue;

            targetChild.gameObject.SetActive(sourceChild.gameObject.activeSelf);

            MeshRenderer sourceMesh = sourceChild.GetComponent<MeshRenderer>();
            SkinnedMeshRenderer sourceSkinned = sourceChild.GetComponent<SkinnedMeshRenderer>();

            MeshRenderer targetMesh = targetChild.GetComponent<MeshRenderer>();
            SkinnedMeshRenderer targetSkinned = targetChild.GetComponent<SkinnedMeshRenderer>();

            if (sourceMesh != null && targetMesh != null)
            {
                targetMesh.sharedMaterials = sourceMesh.sharedMaterials;
            }
            else if (sourceSkinned != null && targetSkinned != null)
            {
                targetSkinned.sharedMaterials = sourceSkinned.sharedMaterials;
            }
        }
    }
    private void ApplyMaterial(Material mat)
    {
        SkinnedMeshRenderer mr = GetComponentInChildren<SkinnedMeshRenderer>();
        if (mr == null || mat == null) return;

        Material[] mats = mr.materials;
        if (mats.Length > 0)
        {
            mats[0] = mat;
            mr.materials = mats;
        }
    }
    /// <summary>
    /// Klon üzerindeki tüm Transform'ları isimleriyle haritalar.
    /// </summary>
    private void MapAllTransforms(Transform parent, Dictionary<string, Transform> map)
    {
        if (!map.ContainsKey(parent.name))
        {
            map.Add(parent.name, parent);
        }

        foreach (Transform child in parent)
        {
            MapAllTransforms(child, map);
        }
    }

    /// <summary>
    /// Recursive (Özyinelemeli) çocuk arama metodu.
    /// </summary>
    private Transform FindChildRecursively(Transform parent, string name)
    {
        if (parent.name == name) return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindChildRecursively(child, name);
            if (result != null) return result;
        }
        return null;
    }
}