using UnityEngine;

public class EngineSoundController : MonoBehaviour
{
    [Header("Ссылки")]
    public CarInput carInput;

    [Header("Audio Clips")]
    public AudioClip idleClip;
    public AudioClip middleClip;
    public AudioClip highClip;

    [Header("RPM параметры")]
    [Range(0f, 1f), Tooltip("Макс. громкость холостых оборотов при RPM == ")] public float lowRPMThreshold = 0.0f;
    [Range(0f, 1f), Tooltip("Макс. громкость средних оборотов при RPM == ")] public float middleRPMThreshold = 0.5f;
    [Range(0f, 1f), Tooltip("Макс. громкость высоких оборотов при RPM == ")] public float highRPMThreshold = 1f;

    

    [Header("Громкость")]
    public float volumeFadeSpeed = 5f;
    [Range(0, 1)] public float lowClipMaxVolume = 1f;
    [Range(0, 1)] public float midClipMaxVolume = 1f;
    [Range(0, 1)] public float highClipMaxVolume = 1f;

    [Header("Ширина диапазона каждой дорожки")]
    [Tooltip("Определяет, насколько широкая область RPM влияет на громкость холостых оборотов. Чем больше значение, тем дольше звучит дорожка.")]
    [Range(0f, 1f)] public float low_spread = 0.5f;

    [Tooltip("Определяет ширину зоны, в которой слышна дорожка средних оборотов.")]
    [Range(0f, 1f)] public float mid_spread = 0.5f;

    [Tooltip("Определяет ширину зоны, в которой слышна дорожка высоких оборотов.")]
    [Range(0f, 1f)] public float high_spread = 0.5f;


    private AudioSource idleSource;
    private AudioSource middleSource;
    private AudioSource highSource;

    private float rpm01 = 0f; // Нормализованный RPM от 0 до 1

    void Start()
    {
        // Создаем три отдельные аудиодорожки
        idleSource = CreateAudioSource(idleClip);
        middleSource = CreateAudioSource(middleClip);
        highSource = CreateAudioSource(highClip);
    }

    void Update()
    {
        // Пример: RPM = скорость / макс. скорость текущей передачи
        rpm01 = Mathf.Clamp01(carInput.curRPM); // carInput.throttleInput

        // Обновляем громкость дорожек в зависимости от RPM
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        float idleTarget = TriangularVolume(rpm01, 0f, low_spread) * lowClipMaxVolume;
        float middleTarget = TriangularVolume(rpm01, 0.5f, mid_spread) * midClipMaxVolume;
        float highTarget = TriangularVolume(rpm01, 1f, high_spread) * highClipMaxVolume;

        idleSource.volume = Mathf.Lerp(idleSource.volume, idleTarget, Time.deltaTime * volumeFadeSpeed);
        middleSource.volume = Mathf.Lerp(middleSource.volume, middleTarget, Time.deltaTime * volumeFadeSpeed);
        highSource.volume = Mathf.Lerp(highSource.volume, highTarget, Time.deltaTime * volumeFadeSpeed);

        //Debug.Log($"idle: {idleSource.volume:F2}, middle: {middleSource.volume:F2}, high: {highSource.volume:F2}");
    }


    private float TriangularVolume(float value, float center, float spread)
    {
        float distance = Mathf.Abs(value - center);
        return Mathf.Clamp01(1f - distance / spread);
    }

    private AudioSource CreateAudioSource(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = true;
        source.volume = 0f;
        source.spatialBlend = 1f; // 3D звук
        source.Play();
        return source;
    }
}
