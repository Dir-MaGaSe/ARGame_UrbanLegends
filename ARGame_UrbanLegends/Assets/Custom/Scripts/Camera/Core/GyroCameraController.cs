using UnityEngine;

// Controlador principal de la cámara AR usando giroscopio
public class GyroCameraController : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] private Transform worldObject;
    [SerializeField] private Transform zoomObj;
    [SerializeField] private float raycastDistance = 500f;
    [SerializeField] private Vector3 cameraParentRotation = new Vector3(90f, 180f, 0f);
    #endregion

    #region Private Fields
    private IOrientationDevice _orientationDevice;
    private IZoomController _zoomController;
    private IWorldRotationHandler _worldRotationHandler;
    private Transform _cameraParent;
    private float _startY;
    private bool _isInitialized;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        InitializeComponents();
    }

    private void OnEnable()
    {
        _orientationDevice?.Enable();
    }

    private void OnDisable()
    {
        _orientationDevice?.Disable();
    }

    private void Update()
    {
        if (!_isInitialized || !_orientationDevice.IsSupported) return;

        if (_startY == 0)
        {
            ResetRotation();
        }

        UpdateCameraRotation();
    }
    #endregion

    #region Private Methods
    private void InitializeComponents()
    {
        // Inicializa los componentes principales
        _orientationDevice = new GyroscopeDevice();
        _zoomController = new DistanceBasedZoomController(zoomObj);
        _worldRotationHandler = new WorldRotationHandler(worldObject);

        if (_orientationDevice.IsSupported)
        {
            SetupCameraHierarchy();
            _isInitialized = true;
        }
        else
        {
            Debug.LogWarning("Gyroscope not supported on this device");
        }
    }

    private void SetupCameraHierarchy()
    {
        _cameraParent = new GameObject("CameraParent").transform;
        _cameraParent.position = transform.position;
        _cameraParent.rotation = Quaternion.Euler(cameraParentRotation);
        transform.SetParent(_cameraParent);
    }

    private void UpdateCameraRotation()
    {
        transform.localRotation = _orientationDevice.GetOrientation();
    }
    #endregion

    #region Public Methods
    public void ResetRotation()
    {
        var raycastResult = PerformCenterScreenRaycast();
        if (raycastResult.HasValue)
        {
            _zoomController.UpdateZoom(raycastResult.Value);
            _startY = transform.eulerAngles.y;
            _worldRotationHandler.UpdateWorldRotation(_startY);
        }
    }
    #endregion

    #region Helper Methods
    private Vector3? PerformCenterScreenRaycast()
    {
        var screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);
        var ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            return hit.point;
        }

        return null;
    }
    #endregion
}