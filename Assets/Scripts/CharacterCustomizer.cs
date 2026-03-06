using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizer : MonoBehaviour
{
    [Header("1. Cinsiyet Kökleri")]
    public GameObject maleRoot;
    public GameObject femaleRoot;

    [Header("2. Gözler")]
    public GameObject eyeColorsParent;
    public GameObject redEye;
    public GameObject whiteEye;

    [Header("3. Saç / Çanta / Kýyafet")]
    public GameObject maleHairParent;
    public GameObject femaleHairParent;
    public GameObject maleBagParent;
    public GameObject femaleBagParent;
    public GameObject maleClothesParent;
    public GameObject femaleClothesParent;

    [Header("Rastgelelik (%)")]
    [Range(0, 100)] public float maleChance = 50f;
    [Range(0, 100)] public float baldChance = 10f;
    [Range(0, 100)] public float bagChance = 30f;
    [Range(0, 100)] public float hatChance = 30f;
    [Range(0, 100)] public float abnormalEyeChance = 20f;

    [Header("Skin Materials")]
    public Material normalMaterial;
    public Material suspiciousMaterial;
    public Material suretMaterial;

    [Header("Skin Chance (%)")]
    [Range(0, 100)] public float suspiciousSkinChance = 20f;
    [Range(0, 100)] public float suretAnomalousSkinChance = 20f;

    [HideInInspector] public bool IsSuret;
    [HideInInspector] public bool isMale;

    private GameObject currentActiveHair;
    private SkinnedMeshRenderer skinRenderer;
    private Gamemanager gameManager;

    // GC önlemek için reusable liste
    private readonly List<GameObject> tempList = new List<GameObject>(16);

    void Awake()
    {
        if (Gamemanager.Instance == null)
        {
            return;
        }

        gameManager = Gamemanager.Instance;
        skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (!gameManager.tutorialMode)
        {
            InitializeCustomization();
        }
    }

    // -------------------- MAIN --------------------

    public void InitializeCustomization(bool? forcedSuret = null)
    {
        DetermineGender();

        // Eðer forcedSuret null deðilse, rastgelelik yerine gelen deðeri kullan
        if (forcedSuret.HasValue)
        {
            IsSuret = forcedSuret.Value;
        }
        else
        {
            DetermineSuret(); // Normal rastgele akýþ
        }

        ApplySkinColor();
        DetermineEyeColor();

        GameObject clothes = RandomlyActivateChild(isMale ? maleClothesParent : femaleClothesParent);
        currentActiveHair = DetermineHair(isMale ? maleHairParent : femaleHairParent);

        if (clothes != null)
            CheckForHat(clothes);

        DetermineBag(isMale ? maleBagParent : femaleBagParent);

        DebugSuretStatus();
    }

    // -------------------- CORE --------------------

    void DetermineGender()
    {
        isMale = Random.Range(0f, 100f) < maleChance;
        if (maleRoot) maleRoot.SetActive(isMale);
        if (femaleRoot) femaleRoot.SetActive(!isMale);
    }

    void DetermineSuret()
    {
        float chance = gameManager ? gameManager.suretChance : 30f;
        IsSuret = Random.Range(0f, 100f) < chance;
    }

    void ApplySkinColor()
    {
        if (!skinRenderer) return;

        if (IsSuret)
        {
            skinRenderer.material =
                Random.Range(0f, 100f) < suretAnomalousSkinChance
                ? suretMaterial
                : normalMaterial;
        }
        else
        {
            skinRenderer.material =
                Random.Range(0f, 100f) < suspiciousSkinChance
                ? suspiciousMaterial
                : normalMaterial;
        }
    }

    void DetermineEyeColor()
    {
        SetAllChildrenInactive(eyeColorsParent);

        if (IsSuret && Random.Range(0f, 100f) < abnormalEyeChance)
        {
            if (Random.Range(0, 2) == 0 && redEye)
                redEye.SetActive(true);
            else if (whiteEye)
                whiteEye.SetActive(true);
            return;
        }

        tempList.Clear();
        foreach (Transform t in eyeColorsParent.transform)
        {
            if (t.gameObject != redEye && t.gameObject != whiteEye)
                tempList.Add(t.gameObject);
        }

        if (tempList.Count > 0)
            tempList[Random.Range(0, tempList.Count)].SetActive(true);
    }

    GameObject DetermineHair(GameObject parent)
    {
        if (!isMale || Random.Range(0f, 100f) >= baldChance)
            return RandomlyActivateChild(parent);

        SetAllChildrenInactive(parent);
        return null;
    }

    void DetermineBag(GameObject parent)
    {
        if (Random.Range(0f, 100f) < bagChance)
            RandomlyActivateChild(parent);
        else
            SetAllChildrenInactive(parent);
    }

    void CheckForHat(GameObject clothes)
    {
        Transform hat = clothes.transform.Find("Hat");
        if (!hat) return;

        bool active = Random.Range(0f, 100f) < hatChance;
        hat.gameObject.SetActive(active);

        if (active && currentActiveHair)
            currentActiveHair.SetActive(false);
    }

    // -------------------- HELPERS --------------------

    void SetAllChildrenInactive(GameObject parent)
    {
        if (!parent) return;
        foreach (Transform t in parent.transform)
            t.gameObject.SetActive(false);
    }

    GameObject RandomlyActivateChild(GameObject parent)
    {
        if (!parent || parent.transform.childCount == 0)
            return null;

        SetAllChildrenInactive(parent);

        int index = Random.Range(0, parent.transform.childCount);
        GameObject selected = parent.transform.GetChild(index).gameObject;
        selected.SetActive(true);
        return selected;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    void DebugSuretStatus()
    {
        UnityEngine.Debug.Log(
            $"[CharacterCustomizer] {gameObject.name} | IsSuret: {IsSuret}"
        );
    }
}
