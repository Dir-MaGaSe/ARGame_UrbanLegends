using UnityEngine;
using UnityEngine.UI;

// Componente principal que coordina la cámara y su renderizado
// Implementa el patrón Facade para simplificar la interacción con el sistema
public class CameraAsBackground : MonoBehaviour
{
    private ICamera _camera;
    private ICameraRenderer _renderer;
    
    [SerializeField]
    private bool _autoInitialize = true;

    [SerializeField]
    private bool _forceLandscape = false;

    private void Awake()
    {
        if (_forceLandscape)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        var image = GetComponent<RawImage>();
        var aspectRatioFitter = GetComponent<AspectRatioFitter>();

        if (image == null || aspectRatioFitter == null)
        {
            Debug.LogError("Required components are missing!");
            enabled = false;
            return;
        }

        _renderer = new UICameraRenderer(image, aspectRatioFitter);
        _camera = new WebCamera();
    }

    private void Start()
    {
        if (_autoInitialize)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        _camera.Initialize(Screen.height, Screen.width);
        _renderer.SetTexture(_camera.GetCameraTexture());
        _camera.StartCamera();
    }

    private void OnDisable()
    {
        _camera?.StopCamera();
    }

    private void Update()
    {
        if (!_camera.IsInitialized) return;

        var properties = _camera.GetCameraProperties();
        UpdateRenderer(properties);
    }

    private void UpdateRenderer(CameraProperties properties)
    {
        float rotation = properties.RotationAngle;
        
        // Simplificamos el manejo de la rotación
        _renderer.UpdateRotation(rotation);
        _renderer.UpdateAspectRatio(properties.AspectRatio);
        _renderer.UpdateUVRect(properties.IsVerticallyMirrored);
    }

    /// <summary>
    /// Maneja los cambios de orientación del dispositivo
    /// </summary>
    private void OnRectTransformDimensionsChange()
    {
        if (_camera?.IsInitialized == true)
        {
            var properties = _camera.GetCameraProperties();
            UpdateRenderer(properties);
        }
    }
}