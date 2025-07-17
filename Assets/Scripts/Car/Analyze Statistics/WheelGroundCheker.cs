using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Проверяет касание колеса земли.
/// </summary>
public class WheelGroundCheker : MonoBehaviour
{
    WheelCollider wheel;

    [Header("Изображение колеса на UI:")]
    public Image wheelImg;
    public Text slipText;

    [Header("Цвета визуализации сцепления:")]
    public Color goodColor = Color.green;         // при хорошем сцеплении (низкий slip)
    public Color wheelGazColor = Color.red;               // при пробуксовке (вперёд, положительный slip)   -> резкий газ
    public Color wheelBreakColor = Color.blue;            // при блокировке (отрицательный slip)            -> резкое торможение
    public Color nonContactColor = Color.black;         // когда колесо не касается земли


    [Header("Величина проскальзывания:")]
    public float forwardSlip;   //— это разность скоростей между фактическим движением точки контакта с землёй и теоретической скоростью, с которой колесо "должно" двигаться, вдоль направления движения.

    /*
    Значение forwardSlip	Что означает

    0.0	                    Колесо идеально катится, нет пробуксовки
    0.1 – 0.3	            Лёгкая пробуксовка
    0.5 – 1.0+	            Сильная пробуксовка (резкое ускорение, дрифт)
    -0.1 – -0.3	            Лёгкое торможение или блокировка колеса
    < -0.5	                Колесо сильно блокировано, возможно юзом тащится
     */

    private void Start()
    {
        wheel = GetComponent<WheelCollider>();
    }
    private void Update()
    {
        // Проверяем касание с землёй
        WheelHit hit;
        bool isGrounded = wheel.GetGroundHit(out hit);

        if (isGrounded)
        {
            forwardSlip = hit.forwardSlip;

            // Вычисляем цвет в зависимости от проскальзывания
            Color slipColor;
            if (forwardSlip >= 0f)
            {
                // Пробуксовка при разгоне — красный
                float slipAmount = Mathf.Clamp01(forwardSlip);
                slipColor = Color.Lerp(goodColor, wheelGazColor, slipAmount);
            }
            else
            {
                // Блокировка колеса — синий
                float slipAmount = Mathf.Clamp01(-forwardSlip);
                slipColor = Color.Lerp(goodColor, wheelBreakColor, slipAmount);
            }

            ApplyData(slipColor, forwardSlip.ToString());
            
        }
        else
        {
            // Нет касания с землёй — отображаем цвет отсутствия контакта
            forwardSlip = 0f;

            ApplyData(nonContactColor, "No contact");
        }
    }

    void ApplyData(Color slipColor, string text)
    {
        // Цвет колеса
        wheelImg.color = slipColor;
        
        // Подпись под колесом
        //slipText.color = slipColor;
        slipText.text = forwardSlip.ToString("F2");
    }
}
