using UnityEngine;
using UnityEngine.UI;

public class Tachometer : MonoBehaviour
{
    [Header("Ссылки")]
    public CarInput carInput;           // Скрипт, откуда берется curRPM
    public RectTransform needle;        // Ссылка на стрелку тахометра (Image -> RectTransform)

    [Header("Скорость перемещения стрелки")]
    public float needleSpeed = 1.0f;

    [Header("Настройки двигателя")]
    public float minRPM = 800f;         // Холостой ход
    public float maxRPM = 7000f;        // Максимальные обороты

    [Header("Настройки стрелки")]
    [Tooltip("Угол поворота стрелки при минимальных оборотах (обычно влево)")]
    public float minAngle = -120f;

    [Tooltip("Угол поворота стрелки при максимальных оборотах (обычно вправо)")]
    public float maxAngle = 120f;

    void Update()
    {
        // Переводим нормализованный curRPM в реальный RPM
        float rpm = Mathf.Lerp(minRPM, maxRPM, Mathf.Clamp01(carInput.curRPM));

        // Интерполируем угол поворота стрелки по реальному RPM
        float rpm01 = (rpm - minRPM) / (maxRPM - minRPM); // нормализуем
        float angle = Mathf.Lerp(minAngle, maxAngle, rpm01);

        // Применяем вращение к стрелке
        Quaternion targetNeedleRotation = Quaternion.Euler(0, 0, -angle);
        needle.localRotation = Quaternion.Lerp(needle.localRotation, targetNeedleRotation, needleSpeed * Time.deltaTime);

    }
}
