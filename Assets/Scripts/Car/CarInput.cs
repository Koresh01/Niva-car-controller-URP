using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Обрабатывает ввод от игрока для управления автомобилем.
/// Использует новую систему ввода Input System.
/// Получает значения руля, газа и тормоза и сохраняет их в публичные поля.
/// </summary>
public class CarInput : MonoBehaviour
{
    [Header("Состояние управления ввода:")]
    // Класс сгенерированных действий ввода (создаётся из Input Actions Asset)
    private CarControls controls;

    // Текущее значение поворота руля (от -1 до 1)
    public float steeringInput;

    // Текущее значение газа (от 0 до 1, либо от -1 до 1, в зависимости от биндинга)
    public float throttleInput;

    // Текущее значение тормоза (если используется)
    public float brakeInput;

    [Header("Характеристики автомобиля:")]
    [SerializeField] float _gazForce;
    [SerializeField] float _brakeForce;
    [SerializeField] float _maxAngle;


    [Header("Модельки колёс:")]
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelBL;
    public Transform wheelBR;

    [Header("WheelColliders:")]
    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider BL;
    public WheelCollider BR;

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
        SteeringHandle();
        ThrottleHandler();
        BrakeHandler();
    }

    /// <summary>
    /// Хэндлер руления.
    /// </summary>
    void SteeringHandle()
    {
        FL.steerAngle = _maxAngle * steeringInput;
        FR.steerAngle = _maxAngle * steeringInput;

        RotateWheel(FL, wheelFL);
        RotateWheel(FR, wheelFR);
        
        // задние колеса
        RotateWheel(BR, wheelBR);
        RotateWheel(BL, wheelBL);
    }

    /// <summary>
    /// Хэндлер подачи топлива.
    /// </summary>
    void ThrottleHandler()
    {
        FL.motorTorque = throttleInput * _gazForce;
        FR.motorTorque = throttleInput * _gazForce;
        BL.motorTorque = throttleInput * _gazForce;
        BR.motorTorque = throttleInput * _gazForce;
    }

    /// <summary>
    /// Хэндлер торможения.
    /// </summary>
    void BrakeHandler()
    {
        if (brakeInput > 0.05)
        {
            FL.brakeTorque = brakeInput * _brakeForce;
            FR.brakeTorque = brakeInput * _brakeForce;
            BL.brakeTorque = brakeInput * _brakeForce;
            BR.brakeTorque = brakeInput * _brakeForce;
        }
        else
        {
            FL.brakeTorque = 0;
            FR.brakeTorque = 0;
            BL.brakeTorque = 0;
            BR.brakeTorque = 0;
        }
    }

    /// <summary>
    /// Поворачивает модельку колеса вслед за её колайдером.
    /// </summary>
    void RotateWheel(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);
        
        transform.position = position;
        transform.rotation = rotation;
    }
}
