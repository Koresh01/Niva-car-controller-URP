using UnityEngine;

public class EnginePitchController : MonoBehaviour
{
    [Header("Ссылки")]
    public CarInput carInput;

    [Header("Звуковая дорожка")]
    public AudioClip engineClip;

    [Header("Параметры Pitch")]
    [Tooltip("Минимальный темп (pitch) при холостых оборотах")]
    public float minPitch = 0.5f;

    [Tooltip("Максимальный темп (pitch) при максимальных оборотах")]
    public float maxPitch = 2.0f;

    [Tooltip("Скорость интерполяции pitch")]
    public float pitchLerpSpeed = 2f;

    [Header("Громкость")]
    [Range(0f, 1f)] public float volume = 1f;

    private AudioSource engineSource;
    private float targetPitch = 1f;

    void Start()
    {
        if (engineClip == null)
        {
            Debug.LogError("EnginePitchController: Не назначен engineClip!");
            return;
        }

        // Создаём и настраиваем AudioSource
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.clip = engineClip;
        engineSource.loop = true;
        engineSource.playOnAwake = false;
        engineSource.spatialBlend = 1f; // 3D звук
        engineSource.volume = volume;

        engineSource.Play();
    }

    void Update()
    {
        if (engineSource == null || carInput == null)
            return;

        float rpm01 = Mathf.Clamp01(carInput.curRPM); // значение от 0 до 1

        // Линейно интерполируем pitch от minPitch до maxPitch
        targetPitch = Mathf.Lerp(minPitch, maxPitch, rpm01);

        // Плавно меняем pitch
        engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, Time.deltaTime * pitchLerpSpeed);
    }
}
