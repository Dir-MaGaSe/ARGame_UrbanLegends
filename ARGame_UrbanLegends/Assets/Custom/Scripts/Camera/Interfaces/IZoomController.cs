using UnityEngine;

// Define las operaciones para el zoom de la cámara
public interface IZoomController
{
    void UpdateZoom(Vector3 hitPoint);
    float GetZoomDistance(Vector3 hitPoint);
}