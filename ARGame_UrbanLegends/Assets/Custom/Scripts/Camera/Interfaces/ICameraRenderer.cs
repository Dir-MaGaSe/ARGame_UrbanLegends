using UnityEngine;

// Interfaz para el renderizado de la cámara
public interface ICameraRenderer
{
    void UpdateRotation(float angle);
    void UpdateAspectRatio(float ratio);
    void UpdateUVRect(bool isMirrored);
    void SetTexture(Texture texture);
}
