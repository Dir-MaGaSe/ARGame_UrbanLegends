using UnityEngine;

// Componente para las notas individuales
public class Note : MonoBehaviour 
{
    public NoteType noteType { get; private set; }
    private float speed;
    private RectTransform rectTransform;

    public void Initialize(NoteType type, float noteSpeed) 
    {
        noteType = type;
        speed = noteSpeed;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() 
    {
        rectTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;
    }
}

public enum NoteType {
    UP,
    DOWN,
    LEFT,
    RIGHT
}