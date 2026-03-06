using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class OriginSoftErrorEntry
{
    public string originalOrigin;
    public string[] softErrorVariants;
}

public class CharacterData : MonoBehaviour
{
    [Header("Karakter Bilgileri (UI için)")]
    public string fullName;
    public string gender;
    public int age;
    public string country;
    public string originCity;
    public string reasonForComing;
    public string illnessOrInjury;
    public bool hasBelongings;

    [Header("Tutarsýzlýk Ayarlarý")]
    [Range(0f, 100f)] public float humanDocumentErrorChance = 10f;
    [Range(0f, 100f)] public float suretDocumentErrorChance = 20f;
    [Range(0f, 100f)] public float originSoftErrorChance = 40f;
    public OriginSoftErrorEntry[] originSoftErrors;

    [HideInInspector] public CharacterCustomizer characterCustomizer;
    [HideInInspector] public EnemyMovement enemyMovement;

    [HideInInspector] public bool usedThermo;
    [HideInInspector] public bool usedLight;
    [HideInInspector] public bool usedMirror;

    [HideInInspector] public bool thermoResultIsSuret;
    [HideInInspector] public bool lightResultIsSuret;
    [HideInInspector] public bool mirrorResultIsSuret;

    // ---- DATA TABLES ----
    private static readonly string[] maleFirstNames =
    { "John", "Mark", "Daniel", "Ethan", "Lucas", "Victor", "Noah", "Adrian", "Leo", "Michael" };

    private static readonly string[] femaleFirstNames =
    { "Alice", "Emma", "Clara", "Sofia", "Elena", "Julia", "Lina", "Nora", "Regina", "Luciana" };

    private static readonly string[] lastNames =
    { "Spencer", "Keller", "Novak", "Turner", "Weiss", "Morin", "Petrov", "Coleman", "Varga", "Lorain" };

    private static readonly string[] countries =
    { "Arkovia", "Belmaris", "Nordek", "Valtrion", "Sereva", "Karsin", "Drovnia", "Elmyr", "Tavosk", "Rhenna" };

    private static readonly string[] origins =
    {
        "Cadaver Wharf", "Broken Radio Station", "Ash Granary", "Rusty Cog Valley",
        "Seven Graves Junction", "Crow's Perch", "Rusty Altar", "Lonely Lumber Camp",
        "Muddy Well Village", "Deadzone Garrison", "Bedlam Asylum", "Eclipse Hollow",
    };

    private static readonly string[] reasons =
    {
    "REASON_SHELTER",
    "REASON_CHEF",
    "REASON_HUNGRY",
    "REASON_DANGER",
    "REASON_FAMILY",
    "REASON_ALONE",
    "REASON_FRIENDS",
    "REASON_SOLDIER",
    "REASON_ANY_JOB",
    "REASON_ENTER",
    "REASON_PATH",
    "REASON_DARKNESS",
    "REASON_CURIOUS",
    "REASON_WATCH",
    "REASON_WHISPER"
    };

    private static readonly string[] softErrorFields =
    {
        "Gender",
        "Illness",
        "Belongings"
    };

    void Awake()
    {
        characterCustomizer = GetComponent<CharacterCustomizer>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    // ----------------------------------------------------

    public void GenerateRandomData()
    {
        if (!characterCustomizer || !enemyMovement)
            return;

        bool actualIsMale = characterCustomizer.isMale;
        bool actualIsSuret = characterCustomizer.IsSuret;
        bool actualIsInjured = enemyMovement.isInjured;

        gender = actualIsMale ? "MALE" : "FEMALE";
        fullName = GenerateRandomName(actualIsMale);

        age = Random.Range(18, 60);

        country = countries[Random.Range(0, countries.Length)];
        originCity = origins[Random.Range(0, origins.Length)];
        reasonForComing = reasons[Random.Range(0, reasons.Length)];

        illnessOrInjury = actualIsInjured ? "INJURED" : "NO_INJURY";

        hasBelongings = CheckHasBag();

        bool criticalError = false;

        if (actualIsSuret)
        {
            if (Random.Range(0f, 100f) < suretDocumentErrorChance)
            {
                age = Random.Range(300, 1400);
                criticalError = true;
            }

            if (!criticalError)
            {
                GenerateSoftInconsistency(actualIsMale, actualIsInjured, hasBelongings);

                if (Random.Range(0f, 100f) < originSoftErrorChance)
                    ApplyOriginSoftError();
            }
        }
        else
        {
            if (Random.Range(0f, 100f) < humanDocumentErrorChance)
                GenerateSoftInconsistency(actualIsMale, actualIsInjured, hasBelongings);

            if (Random.Range(0f, 100f) < originSoftErrorChance)
                ApplyOriginSoftError();
        }
    }

    // ----------------------------------------------------

    private void GenerateSoftInconsistency(bool actualIsMale, bool actualIsInjured, bool actualHasBelongings)
    {
        for (int i = 0; i < 5; i++)
        {
            string field = softErrorFields[Random.Range(0, softErrorFields.Length)];

            if (field == "Gender")
            {
                gender = actualIsMale ? "Female" : "Male";
                return;
            }
            else if (field == "Illness" && actualIsInjured)
            {
                illnessOrInjury = "No";
                return;
            }
            else if (field == "Belongings")
            {
                hasBelongings = !actualHasBelongings;
                return;
            }
        }
    }

    private void ApplyOriginSoftError()
    {
        for (int i = 0; i < originSoftErrors.Length; i++)
        {
            var entry = originSoftErrors[i];
            if (entry.originalOrigin == originCity && entry.softErrorVariants.Length > 0)
            {
                originCity = entry.softErrorVariants[Random.Range(0, entry.softErrorVariants.Length)];
                return;
            }
        }
    }

    private string GenerateRandomName(bool isMale)
    {
        string firstName = isMale
            ? maleFirstNames[Random.Range(0, maleFirstNames.Length)]
            : femaleFirstNames[Random.Range(0, femaleFirstNames.Length)];

        string lastName = lastNames[Random.Range(0, lastNames.Length)];
        return firstName + " " + lastName;
    }

    private bool CheckHasBag()
    {
        GameObject bagParent = characterCustomizer.isMale
            ? characterCustomizer.maleBagParent
            : characterCustomizer.femaleBagParent;

        if (!bagParent) return false;

        foreach (Transform t in bagParent.transform)
        {
            if (t.gameObject.activeSelf)
                return true;
        }

        return false;
    }
}
