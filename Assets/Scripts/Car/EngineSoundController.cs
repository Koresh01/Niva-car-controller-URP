using UnityEngine;

public class EngineSoundController : MonoBehaviour
{
    [Header("������")]
    public CarInput carInput;

    [Header("Audio Clips")]
    public AudioClip idleClip;
    public AudioClip middleClip;
    public AudioClip highClip;

    [Header("RPM ���������")]
    [Range(0f, 1f), Tooltip("����. ��������� �������� �������� ��� RPM == ")] public float lowRPMThreshold = 0.0f;
    [Range(0f, 1f), Tooltip("����. ��������� ������� �������� ��� RPM == ")] public float middleRPMThreshold = 0.5f;
    [Range(0f, 1f), Tooltip("����. ��������� ������� �������� ��� RPM == ")] public float highRPMThreshold = 1f;

    [Header("���������")]
    public float volumeFadeSpeed = 5f;
    [Range(0, 1)] public float lowClipMaxVolume = 1f;
    [Range(0, 1)] public float midClipMaxVolume = 1f;
    [Range(0, 1)] public float highClipMaxVolume = 1f;

    private AudioSource idleSource;
    private AudioSource middleSource;
    private AudioSource highSource;

    private float rpm01 = 0f; // ��������������� RPM �� 0 �� 1

    void Start()
    {
        // ������� ��� ��������� ������������
        idleSource = CreateAudioSource(idleClip);
        middleSource = CreateAudioSource(middleClip);
        highSource = CreateAudioSource(highClip);
    }

    void Update()
    {
        // ������: RPM = �������� / ����. �������� ������� ��������
        rpm01 = Mathf.Clamp01(carInput.curRPM * carInput.throttleInput);
        Debug.Log($"rpm01: {rpm01}");

        // ��������� ��������� ������� � ����������� �� RPM
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        float idleTarget = TriangularVolume(rpm01, 0f, 0.5f) * lowClipMaxVolume;
        float middleTarget = TriangularVolume(rpm01, 0.5f, 0.5f) * midClipMaxVolume;
        float highTarget = TriangularVolume(rpm01, 1f, 0.5f) * highClipMaxVolume;

        idleSource.volume = Mathf.Lerp(idleSource.volume, idleTarget, Time.deltaTime * volumeFadeSpeed);
        middleSource.volume = Mathf.Lerp(middleSource.volume, middleTarget, Time.deltaTime * volumeFadeSpeed);
        highSource.volume = Mathf.Lerp(highSource.volume, highTarget, Time.deltaTime * volumeFadeSpeed);

        Debug.Log($"idle: {idleSource.volume:F2}, middle: {middleSource.volume:F2}, high: {highSource.volume:F2}");
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
        source.spatialBlend = 1f; // 3D ����
        source.Play();
        return source;
    }
}
