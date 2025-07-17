using UnityEngine;

public class EngineSound : MonoBehaviour
{
    [Header("Ссылки")]
    public CarInput carInput;

    [Header("Настройки BPM")]
    public float minBPM = 80f; // Холостые обороты
    public float maxBPM = 522f; // Отсечка

    [Header("Аудио")]
    public AudioClip engineLoopClip;

    private AudioSource engineSource;

    void Start()
    {
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.clip = engineLoopClip;
        engineSource.loop = true;
        engineSource.playOnAwake = true;
        engineSource.spatialBlend = 1f; // 3D звук
        engineSource.volume = 1f;
        engineSource.Play();
    }

    void Update()
    {
        float rpm01 = Mathf.Clamp01(carInput.curRPM);

        // Вычисляем текущий BPM и устанавливаем pitch
        float currentBPM = Mathf.Lerp(minBPM, maxBPM, rpm01);
        engineSource.pitch = currentBPM / minBPM;
    }
}
