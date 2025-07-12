using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Обрабатывает ввод от игрока для управления автомобилем.
/// Использует новую систему ввода Input System.
/// Получает значения руля, газа и тормоза и сохраняет их в публичные поля.
/// </summary>
public class CarInput : MonoBehaviour
{
    // Класс сгенерированных действий ввода (создаётся из Input Actions Asset)
    private CarControls controls;

    // Текущее значение поворота руля (от -1 до 1)
    public float steeringInput;

    // Текущее значение газа (от 0 до 1, либо от -1 до 1, в зависимости от биндинга)
    public float throttleInput;

    // Текущее значение тормоза (если используется)
    public float brakeInput;

    // Инициализация схемы ввода
    void Awake()
    {
        controls = new CarControls();
    }

    // Подписка на события ввода
    void OnEnable()
    {
        // Подписка на события рулевого управления
        controls.Driving.Steer.performed += ctx => steeringInput = ctx.ReadValue<float>();
        controls.Driving.Steer.canceled += ctx => steeringInput = 0;

        // Подписка на события газа
        controls.Driving.Throttle.performed += ctx => throttleInput = ctx.ReadValue<float>();
        controls.Driving.Throttle.canceled += ctx => throttleInput = 0;

        // Подписка на события торможения (если назначено)
        controls.Driving.Brake.performed += ctx => brakeInput = ctx.ReadValue<float>();
        controls.Driving.Brake.canceled += ctx => brakeInput = 0;

        // Включение карты действий в Input System
        controls.Driving.Enable();
    }

    // Отписка от событий ввода
    void OnDisable()
    {
        // ❗ ВАЖНО: такие лямбды не удаляются — это баг. Лучше использовать методы (см. предыдущий ответ)
        controls.Driving.Steer.performed -= ctx => steeringInput = ctx.ReadValue<float>();
        controls.Driving.Steer.canceled -= ctx => steeringInput = 0;

        controls.Driving.Throttle.performed -= ctx => throttleInput = ctx.ReadValue<float>();
        controls.Driving.Throttle.canceled -= ctx => throttleInput = 0;

        controls.Driving.Brake.performed -= ctx => brakeInput = ctx.ReadValue<float>();
        controls.Driving.Brake.canceled -= ctx => brakeInput = 0;

        // Отключение карты действий
        controls.Driving.Disable();
    }

    // Отладочный вывод значений ввода каждую физическую итерацию
    private void FixedUpdate()
    {
        Debug.Log($"Управление: {steeringInput} \t Газ: {throttleInput} \t Тормоз: {brakeInput}");
    }
}
