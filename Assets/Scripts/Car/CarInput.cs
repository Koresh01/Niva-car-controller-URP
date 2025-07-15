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
    [SerializeField] Rigidbody rb;

    [Header("MovementStatistics:")]
    [Tooltip("Текущая скорость автомобиля в км/ч.")] public float curSpeed;
    [Tooltip("Текущее кол-во оборотов двигателя")] public float curRPM;
    [Tooltip("Индекс текущей передачи.")] public int curGearInx = 1;  // 1 - соответствует нейтральной передаче
    [Tooltip("Максимальная скорость для текущей передачи.")] public float maxSpeedForThisGear;
    [Tooltip("Крутящий момент с коробки передач на колёса.")] public float RotationalMomentForce;
    [Tooltip("Радиус колеса в МЕТРАХ.")] public float wheelRadius;


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
    [SerializeField] float _brakeForce;
    [SerializeField] float _maxAngle;

    [Header("КПП:")]

    public List<Gear> gears;


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
    }

    void CollectMovementStatistics()
    {
        curSpeed = rb.linearVelocity.magnitude * 3.6f; // Rigidbody-скорость

        maxSpeedForThisGear = gears[curGearInx].maxSpeed;

        float wheelRpm = (BL.rpm + BR.rpm + FL.rpm + FR.rpm) / 4f;

        float wheelSpeed = 2 * Mathf.PI * wheelRadius * wheelRpm / 60f * 3.6f; // Скорость от колеса, км/ч

        if (curGearInx != 1) // не нейтраль
            curRPM = wheelSpeed / maxSpeedForThisGear;
        else
            curRPM = throttleInput;

        RotationalMomentForce = gears[curGearInx].force;
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
        if (curSpeed < maxSpeedForThisGear)
        {
            FL.motorTorque = throttleInput * RotationalMomentForce;
            FR.motorTorque = throttleInput * RotationalMomentForce;
            BL.motorTorque = throttleInput * RotationalMomentForce;
            BR.motorTorque = throttleInput * RotationalMomentForce;
        }
        else
        {
            // Перестаём крутить колёса.
            FL.motorTorque = 0f;
            FR.motorTorque = 0f;
            BL.motorTorque = 0f;
            BR.motorTorque = 0f;

            // Торможение двигателем
            float engineBrakeTorque = -Mathf.Sign(RotationalMomentForce) * RotationalMomentForce * 0.5f; // Можешь настроить силу
            FL.motorTorque = engineBrakeTorque;
            FR.motorTorque = engineBrakeTorque;
            BL.motorTorque = engineBrakeTorque;
            BR.motorTorque = engineBrakeTorque;
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
}
