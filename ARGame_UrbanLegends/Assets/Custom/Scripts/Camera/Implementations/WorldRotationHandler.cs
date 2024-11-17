using UnityEngine;

// Manejador de la rotación del mundo
public class WorldRotationHandler : IWorldRotationHandler
{
    private readonly Transform _worldTransform;

    public WorldRotationHandler(Transform worldTransform)
    {
        _worldTransform = worldTransform;
    }

    public void UpdateWorldRotation(float angleY)
    {
        _worldTransform.rotation = Quaternion.Euler(0f, angleY, 0f);
    }
}