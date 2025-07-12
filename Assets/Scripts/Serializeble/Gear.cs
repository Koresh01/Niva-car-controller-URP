using System;
using UnityEngine;

[Serializable]
public class Gear
{
    [Tooltip("Максимальная скорость на этой передаче в км/ч.")]
    public float maxSpeed;

    [Tooltip("Крутящий момент на этой передаче.")]
    public float force;
}