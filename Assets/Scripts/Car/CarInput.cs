using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Обрабатывает ввод от игрока для управления автомобилем.
/// Использует новую систему ввода Input System.
/// Получает значения руля, газа и тормоза и сохраняет их в публичные поля.
/// </summary>
public class CarInput : MonoBehaviour
{
    [Header("Rigidbody автомобиля.")]
    public Rigidbody rb;

    [Header("Movement Statistics --------------------------------------------------")]

    [Header("Все скорости")]
    [Tooltip("Текущая скорость автомобиля в км/ч.")] public float curSpeed;
    [Tooltip("Линейная скорость колёс в км/ч")] public float wheelSpeed;
    [Tooltip("Направление движения (вперёд или назад), со знаком")] public float velocityDirection;
    [Tooltip("Направленная скорость колёс в км/ч")] public float directedSpeedKmh;

    [Header("Все RPM:")]
    [Tooltip("Текущее значение оборотов двигателя в минуту")] public float curRPM;
    [Tooltip("Среднее значение оборотов колёс в минуту")] public float wheelRpm_average;

    [Header("КПП:")]
    [Tooltip("Индекс текущей передачи.")] public int curGearInx = 1;  // 1 - соответствует нейтральной передаче
    [Tooltip("Максимальная скорость для текущей передачи.")] public float maxSpeedForThisGear;
    [Tooltip("Крутящий момент с коробки передач на колёса.")] public float RotationalMomentForce;
    [Tooltip("Радиус колеса в МЕТРАХ.")] public float wheelRadius;



    // Класс сгенерированных действий ввода (создаётся из Input Actions Asset)
    private CarControls controls;

    [Header("-------------------------------------------------- Driver Input")]
    // Текущее значение поворота руля (от -1 до 1)
    public float steeringInput;

    // Текущее значение газа (от 0 до 1, либо от -1 до 1, в зависимости от биндинга)
    public float throttleInput;

    // Текущее значение тормоза (если используется)
    public float brakeInput;

    [Header("Характеристики автомобиля:")]
    [SerializeField] float _brakeForce;
    [SerializeField] float _maxAngle;

    [Header("КПП:")]

    public List<Gear> gears;

    [Header("Максимально допустимые RPM колеса при текущей передаче")]
    public float maxWheelRpm;


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

    #region INPUTS
    void Awake()
    {
        controls = new CarControls();
        rb = GetComponent<Rigidbody>();
        wheelRadius = BL.radius;    // the radius of the wheel measurment in a local space?? Мне надо в метрах
    }

    void OnEnable()
    {
        controls.Driving.Steer.performed += OnSteer;
        controls.Driving.Steer.canceled += OnSteerCanceled;

        controls.Driving.Throttle.performed += OnThrottle;
        controls.Driving.Throttle.canceled += OnThrottleCanceled;

        controls.Driving.Brake.performed += OnBrake;
        controls.Driving.Brake.canceled += OnBrakeCanceled;

        controls.Driving.GearUP.performed += OnGearUp;
        controls.Driving.GearDOWN.performed += OnGearDown;

        controls.Driving.Enable();
    }

    void OnDisable()
    {
        controls.Driving.Steer.performed -= OnSteer;
        controls.Driving.Steer.canceled -= OnSteerCanceled;

        controls.Driving.Throttle.performed -= OnThrottle;
        controls.Driving.Throttle.canceled -= OnThrottleCanceled;

        controls.Driving.Brake.performed -= OnBrake;
        controls.Driving.Brake.canceled -= OnBrakeCanceled;

        controls.Driving.GearUP.performed -= OnGearUp;
        controls.Driving.GearDOWN.performed -= OnGearDown;

        controls.Driving.Disable();
    }

    private void OnSteer(InputAction.CallbackContext ctx) => steeringInput = ctx.ReadValue<float>();
    private void OnSteerCanceled(InputAction.CallbackContext ctx) => steeringInput = 0;

    private void OnThrottle(InputAction.CallbackContext ctx) => throttleInput = ctx.ReadValue<float>();
    private void OnThrottleCanceled(InputAction.CallbackContext ctx) => throttleInput = 0;

    private void OnBrake(InputAction.CallbackContext ctx) => brakeInput = ctx.ReadValue<float>();
    private void OnBrakeCanceled(InputAction.CallbackContext ctx) => brakeInput = 0;

    private void OnGearUp(InputAction.CallbackContext ctx)
    {
        if (curGearInx < gears.Count - 1)
            curGearInx++;
    }

    private void OnGearDown(InputAction.CallbackContext ctx)
    {
        if (curGearInx > 0)
            curGearInx--;
    }
    #endregion

    // Отладочный вывод значений ввода каждую физическую итерацию
    private void FixedUpdate()
    {
        CollectMovementStatistics();
        SteeringHandle();
        ThrottleHandler();
        BrakeHandler();

        LimitWheelRPM(FL);
        LimitWheelRPM(FR);
        LimitWheelRPM(BL);
        LimitWheelRPM(BR);
    }

    void CollectMovementStatistics()
    {
        // Определяем все скорости:
        curSpeed = rb.linearVelocity.magnitude * 3.6f; // Rigidbody-скорость в км/ч
        maxSpeedForThisGear = gears[curGearInx].maxSpeed;

        velocityDirection = Mathf.Sign(Vector3.Dot(rb.linearVelocity, transform.forward));

        // Абсолютная скорость (м/с) умножаем на 3.6 -> км/ч
        directedSpeedKmh = rb.linearVelocity.magnitude * 3.6f * velocityDirection;


        wheelRpm_average = (BL.rpm + BR.rpm + FL.rpm + FR.rpm) / 4f;

        wheelSpeed = 2 * Mathf.PI * wheelRadius * wheelRpm_average / 60f * 3.6f; // Скорость от колеса, км/ч

        // Определяем кол-во оборотов двигателя в мин
        if (curGearInx != 1) // не нейтраль
            curRPM = wheelSpeed / maxSpeedForThisGear;
        else
            curRPM = throttleInput;

        // Задаём крутящий момент:
        RotationalMomentForce = gears[curGearInx].force;

        // Максимально допустимый RPM колеса (по ограничению на текущей передаче)
        maxWheelRpm = (Mathf.Abs(maxSpeedForThisGear) / (2f * Mathf.PI * wheelRadius)) * (1000f / 60f); // единица — об/мин
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
        bool isForwardGear = curGearInx >= 2;
        bool isReverseGear = curGearInx == 0;
        /*
         Для задней передачи maxSpeedForThisGear = -26 км/ч
         Для передней передачи maxSpeedForThisGear = 43 км/ч
        
        Когда едем вперед, то условие должно выглядеть if (directedSpeedKmh < maxSpeedForThisGear) -> подгоняем вперёд если водитель держит газ
        Когда едем назад, то условие должно выглядеть  if (directedSpeedKmh > maxSpeedForThisGear) -> подгоняем назад если водитель держит газ
         */
        // Условие: торможения двигателем
        if ((isForwardGear && directedSpeedKmh > maxSpeedForThisGear) ||
            (isReverseGear && (directedSpeedKmh < maxSpeedForThisGear || velocityDirection > 0f)))  // Включили заднюю передачу, когда ехали вперёд
        {
            // Перестаём крутить колёса.
            FL.motorTorque = 0f;
            FR.motorTorque = 0f;
            BL.motorTorque = 0f;
            BR.motorTorque = 0f;

            // Торможение двигателем (в сторону противоположную движению)
            float engineBrakeTorque = -velocityDirection * Mathf.Abs(RotationalMomentForce) * 0.5f; // Можешь настроить силу
            FL.motorTorque = engineBrakeTorque;
            FR.motorTorque = engineBrakeTorque;
            BL.motorTorque = engineBrakeTorque;
            BR.motorTorque = engineBrakeTorque;
        }
        else
        {
            FL.motorTorque = throttleInput * RotationalMomentForce;
            FR.motorTorque = throttleInput * RotationalMomentForce;
            BL.motorTorque = throttleInput * RotationalMomentForce;
            BR.motorTorque = throttleInput * RotationalMomentForce;
        }
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


    // Ограничитель скорости вращения колеса. (чтоб при срывании колеса оно не раскручивалось до бесконечности)
    void LimitWheelRPM(WheelCollider wheel)
    {
        float absRpm = Mathf.Abs(wheel.rpm);
        if (absRpm > maxWheelRpm)
        {
            // Сила торможения пропорциональна превышению
            float excessRatio = (absRpm - maxWheelRpm) / maxWheelRpm;
            float correctiveBrake = Mathf.Clamp01(excessRatio) * _brakeForce;

            wheel.brakeTorque = correctiveBrake;
            // Можно также отключить моторный момент:
            wheel.motorTorque = 0f;
        }
    }

}
