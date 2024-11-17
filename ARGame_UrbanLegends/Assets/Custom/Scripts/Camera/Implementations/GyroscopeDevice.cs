using UnityEngine;

// Implementación del giroscopio como dispositivo de orientación
public class GyroscopeDevice : IOrientationDevice
{
    private readonly Gyroscope _gyro;
    private readonly Quaternion _rotationFix;

    public bool IsSupported => SystemInfo.supportsGyroscope;

    public GyroscopeDevice()
    {
        if (IsSupported)
        {
            _gyro = Input.gyro;
            _rotationFix = new Quaternion(0, 0, 1, 0);
        }
    }

    public Quaternion GetOrientation()
    {
        return IsSupported ? _gyro.attitude * _rotationFix : Quaternion.identity;
    }

    public void Enable()
    {
        if (IsSupported) _gyro.enabled = true;
    }

    public void Disable()
    {
        if (IsSupported) _gyro.enabled = false;
    }
}