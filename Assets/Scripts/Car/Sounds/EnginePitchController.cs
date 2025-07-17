using UnityEngine;

public class EnginePitchController : MonoBehaviour
{
    [Header("������")]
    public CarInput carInput;

    [Header("�������� �������")]
    public AudioClip engineClip;

    [Header("��������� Pitch")]
    [Tooltip("����������� ���� (pitch) ��� �������� ��������")]
    public float minPitch = 0.5f;

    [Tooltip("������������ ���� (pitch) ��� ������������ ��������")]
    public float maxPitch = 2.0f;

    [Tooltip("�������� ������������ pitch")]
    public float pitchLerpSpeed = 2f;

    [Header("���������")]
    [Range(0f, 1f)] public float volume = 1f;

    private AudioSource engineSource;
    private float targetPitch = 1f;

    void Start()
    {
        if (engineClip == null)
        {
            Debug.LogError("EnginePitchController: �� �������� engineClip!");
            return;
        }

        // ������ � ����������� AudioSource
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.clip = engineClip;
        engineSource.loop = true;
        engineSource.playOnAwake = false;
        engineSource.spatialBlend = 1f; // 3D ����
        engineSource.volume = volume;

        engineSource.Play();
    }

    void Update()
    {
        if (engineSource == null || carInput == null)
            return;

        float rpm01 = Mathf.Clamp01(carInput.curRPM); // �������� �� 0 �� 1

        // ������� ������������� pitch �� minPitch �� maxPitch
        targetPitch = Mathf.Lerp(minPitch, maxPitch, rpm01);

        // ������ ������ pitch
        engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, Time.deltaTime * pitchLerpSpeed);
    }
}
