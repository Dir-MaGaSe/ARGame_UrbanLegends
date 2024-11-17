// Estructura que encapsula las propiedades de la cámara
public struct CameraProperties
{
    public float RotationAngle { get; set; }
    public bool IsVerticallyMirrored { get; set; }
    public float AspectRatio { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
