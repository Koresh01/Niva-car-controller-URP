using UnityEngine;

public class Speedometr : MonoBehaviour
{
    [Header("Ссылки")]
    public CarInput carInput;           // Скрипт, откуда берется curRPM
    public RectTransform needle;        // Ссылка на стрелку тахометра (Image -> RectTransform)

    [Header("Скорость перемещения стрелки")]
    public float needleSpeed = 1.0f;

    [Header("Максимальная скорость")]
    public float maxSpeed = 180;        // Максимальные обороты

    [Header("Настройки стрелки")]
    [Tooltip("Угол поворота стрелки при минимальных оборотах (обычно влево)")]
    public float minAngle = -120f;

    [Tooltip("Угол поворота стрелки при максимальных оборотах (обычно вправо)")]
    public float maxAngle = 120f;

    void Update()
    {
        // Переводим скорость в нормализованную от 0 до 1.
        float normalizedSpeed = carInput.curSpeed / maxSpeed;

        // Интерполируем угол поворота стрелки
        float angle = Mathf.Lerp(minAngle, maxAngle, normalizedSpeed);

        // Применяем вращение к стрелке
        Quaternion targetNeedleRotation = Quaternion.Euler(0, 0, -angle);
        needle.localRotation = Quaternion.Lerp(needle.localRotation, targetNeedleRotation, needleSpeed * Time.deltaTime);

    }
}
