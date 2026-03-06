using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereController : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Pitch Settings")]
    public float normalPitch = 1.0f;
    public float highPitch = 1.2f;
    public float lowPitch = 0.9f;

    [Header("Timing")]
    public float changeInterval = 120f; // Kaç saniyede bir deðiþsin? (2 dk = 120s)
    public float transitionSpeed = 2.0f; // Perde deðiþiminin hýzý (yumuþak geçiþ için)

    private float targetPitch;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        targetPitch = normalPitch;

        // Periyodik deðiþimi baþlat
        InvokeRepeating(nameof(UpdateTargetPitch), changeInterval, changeInterval);
    }

    void Update()
    {
        // Pitch ayarýný hedef deðere yumuþak bir þekilde yaklaþtýr
        if (Mathf.Abs(audioSource.pitch - targetPitch) > 0.01f)
        {
            audioSource.pitch = Mathf.Lerp(audioSource.pitch, targetPitch, Time.deltaTime * transitionSpeed);
        }
    }

    void UpdateTargetPitch()
    {
        // Burada bir döngü veya rastgelelik kurabiliriz
        // Þimdilik senin istediðin gibi: Eðer normaldeyse yükselt, yüksekse normale çek
        if (targetPitch == normalPitch)
        {
            // Rastgele bir seçenek: Bazen biraz daha tiz, bazen biraz daha pes
            targetPitch = Random.value > 0.5f ? highPitch : lowPitch;
        }
        else
        {
            targetPitch = normalPitch;
        }
    }
}
