using UnityEngine;

// Interfaz que define las operaciones básicas de una cámara
public interface ICamera
{
    void Initialize(int width, int height);
    void StartCamera();
    void StopCamera();
    bool IsInitialized { get; }
    Texture GetCameraTexture();
    CameraProperties GetCameraProperties();
}