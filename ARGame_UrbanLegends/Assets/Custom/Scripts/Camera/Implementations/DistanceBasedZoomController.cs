using UnityEngine;

// Controlador de zoom basado en distancia
public class DistanceBasedZoomController : IZoomController
{
    private readonly Transform _zoomTransform;
    private readonly float _minZoom;
    private readonly float _maxZoom;

    public DistanceBasedZoomController(Transform zoomTransform, float minZoom = 2f, float maxZoom = 10f)
    {
        _zoomTransform = zoomTransform;
        _minZoom = minZoom;
        _maxZoom = maxZoom;
    }

    public void UpdateZoom(Vector3 hitPoint)
    {
        float distance = GetZoomDistance(hitPoint);
        _zoomTransform.localPosition = new Vector3(
            0f,
            _zoomTransform.localPosition.y,
            Mathf.Clamp(distance, _minZoom, _maxZoom)
        );
    }

    public float GetZoomDistance(Vector3 hitPoint)
    {
        hitPoint.y = 0;
        return Vector3.Distance(Vector3.zero, hitPoint);
    }
}