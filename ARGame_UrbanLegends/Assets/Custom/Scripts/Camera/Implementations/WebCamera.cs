using UnityEngine;
using System;

// Implementación concreta de la cámara usando WebCamTexture
public class WebCamera : ICamera
{
    private WebCamTexture _webCam;
    private bool _isInitialized;
    private const int MIN_CAMERA_WIDTH = 100;

    public bool IsInitialized => _isInitialized && _webCam.width >= MIN_CAMERA_WIDTH;

    public void Initialize(int width, int height)
    {
        try
        {
            // Obtener la cámara trasera por defecto
            WebCamDevice[] devices = WebCamTexture.devices;
            string backCameraName = "";
            
            for (int i = 0; i < devices.Length; i++)
            {
                if (!devices[i].isFrontFacing)
                {
                    backCameraName = devices[i].name;
                    break;
                }
            }

            // Si no se encuentra cámara trasera, usar la primera disponible
            if (string.IsNullOrEmpty(backCameraName) && devices.Length > 0)
            {
                backCameraName = devices[0].name;
            }

            // Inicializar con la orientación correcta
            _webCam = new WebCamTexture(backCameraName, height, width, 30);
            _isInitialized = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing camera: {e.Message}");
            _isInitialized = false;
        }
    }

    public void StartCamera()
    {
        if (_isInitialized)
        {
            _webCam.Play();
        }
    }

    public void StopCamera()
    {
        if (_isInitialized && _webCam.isPlaying)
        {
            _webCam.Stop();
        }
    }

    public Texture GetCameraTexture() => _webCam;

    public CameraProperties GetCameraProperties()
    {
        if (!IsInitialized) return default;

        float rotationAngle = _webCam.videoRotationAngle;
        bool isMirrored = _webCam.videoVerticallyMirrored;

        // Ajustar rotación según la orientación del dispositivo
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            rotationAngle += 90f;
        }

        return new CameraProperties
        {
            RotationAngle = rotationAngle,
            IsVerticallyMirrored = isMirrored,
            AspectRatio = (float)_webCam.width / _webCam.height,
            Width = _webCam.width,
            Height = _webCam.height
        };
    }
}
