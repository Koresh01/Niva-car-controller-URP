using UnityEngine;

public class EngineSound : MonoBehaviour
{
    [Header("������")]
    public CarInput carInput;

    [Header("��������� BPM")]
    public float minBPM = 80f; // �������� �������
    public float maxBPM = 522f; // �������

    [Header("�����")]
    public AudioClip engineLoopClip;

    private AudioSource engineSource;

    void Start()
    {
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.clip = engineLoopClip;
        engineSource.loop = true;
        engineSource.playOnAwake = true;
        engineSource.spatialBlend = 1f; // 3D ����
        engineSource.volume = 1f;
        engineSource.Play();
    }

    void Update()
    {
        float rpm01 = Mathf.Clamp01(carInput.curRPM);

        // ��������� ������� BPM � ������������� pitch
        float currentBPM = Mathf.Lerp(minBPM, maxBPM, rpm01);
        engineSource.pitch = currentBPM / minBPM;
    }
}
