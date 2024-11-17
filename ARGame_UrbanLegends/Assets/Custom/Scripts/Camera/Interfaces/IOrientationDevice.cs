using UnityEngine;

// Define las operaciones básicas para un dispositivo de orientación
public interface IOrientationDevice
{
    bool IsSupported { get; }
    Quaternion GetOrientation();
    void Enable();
    void Disable();
}