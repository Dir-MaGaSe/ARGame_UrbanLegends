using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Sistema de captura rítmica
public class RhythmCaptureSystem : MonoBehaviour 
{
    [Header("Configuración General")]
    [SerializeField] private float baseNoteSpeed = 5f;
    [SerializeField] private float noteTriggerPosition = 0.1f; // Posición donde se debe presionar el botón
    [SerializeField] private int maxMisses = 3;
    
    [Header("Referencias UI")]
    [SerializeField] private RectTransform noteContainer;
    [SerializeField] private GameObject[] notePrefabs; // Prefabs para cada tipo de nota
    [SerializeField] private Transform noteSpawnPoint;
    [SerializeField] private Transform noteTriggerPoint;
    [SerializeField] private TMPro.TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject successEffect;
    [SerializeField] private GameObject failEffect;

    private List<Note> activeNotes = new List<Note>();
    private Queue<NoteType> patternQueue = new Queue<NoteType>();
    private int currentMisses = 0;
    private bool isCapturing = false;
    private CreatureType currentCreature;

    [System.Serializable]
    public class CreatureDifficulty 
    {
        public CreatureType creatureType;
        public float noteSpeed;
        public int patternLength;
    }

    [Header("Configuración de Dificultad")]
    [SerializeField] private CreatureDifficulty[] creatureDifficulties;

    public void StartCapture(CreatureType creature) 
    {
        currentCreature = creature;
        currentMisses = 0;
        isCapturing = true;
        GeneratePattern();
        StartCoroutine(SpawnNotes());
    }

    private void GeneratePattern() 
    {
        patternQueue.Clear();
        var difficulty = GetCreatureDifficulty(currentCreature);
        
        for(int i = 0; i < difficulty.patternLength; i++) 
        {
            patternQueue.Enqueue((NoteType)Random.Range(0, 4));
        }
    }

    private CreatureDifficulty GetCreatureDifficulty(CreatureType type) 
    {
        return creatureDifficulties.FirstOrDefault(d => d.creatureType == type);
    }

    private IEnumerator SpawnNotes() 
    {
        while(patternQueue.Count > 0 && isCapturing) 
        {
            var noteType = patternQueue.Dequeue();
            SpawnNote(noteType);
            yield return new WaitForSeconds(1f);
        }
    }

    private void SpawnNote(NoteType type) 
    {
        GameObject notePrefab = notePrefabs[(int)type];
        GameObject noteObj = Instantiate(notePrefab, noteSpawnPoint.position, Quaternion.identity, noteContainer);
        Note note = noteObj.GetComponent<Note>();
        note.Initialize(type, GetCreatureDifficulty(currentCreature).noteSpeed);
        activeNotes.Add(note);
    }

    private void Update() 
    {
        if(!isCapturing) return;

        // Verificar input del jugador
        if(Input.GetKeyDown(KeyCode.UpArrow)) CheckNoteHit(NoteType.UP);
        if(Input.GetKeyDown(KeyCode.DownArrow)) CheckNoteHit(NoteType.DOWN);
        if(Input.GetKeyDown(KeyCode.LeftArrow)) CheckNoteHit(NoteType.LEFT);
        if(Input.GetKeyDown(KeyCode.RightArrow)) CheckNoteHit(NoteType.RIGHT);

        // Actualizar y verificar notas perdidas
        CheckMissedNotes();
    }

    private void CheckNoteHit(NoteType inputType) 
    {
        var nearestNote = activeNotes
            .Where(n => n.noteType == inputType)
            .OrderBy(n => Mathf.Abs(n.transform.position.x - noteTriggerPoint.position.x))
            .FirstOrDefault();

        if(nearestNote != null) 
        {
            float distance = Mathf.Abs(nearestNote.transform.position.x - noteTriggerPoint.position.x);
            
            if(distance < noteTriggerPosition) 
            {
                ShowFeedback("¡Bien!", true);
                Instantiate(successEffect, nearestNote.transform.position, Quaternion.identity);
                activeNotes.Remove(nearestNote);
                Destroy(nearestNote.gameObject);
            }
        }
    }

    private void CheckMissedNotes() 
    {
        for(int i = activeNotes.Count - 1; i >= 0; i--) 
        {
            if(activeNotes[i].transform.position.x < noteTriggerPoint.position.x - noteTriggerPosition) 
            {
                ShowFeedback("¡Fallaste!", false);
                Instantiate(failEffect, activeNotes[i].transform.position, Quaternion.identity);
                currentMisses++;
                
                if(currentMisses >= maxMisses) 
                {
                    EndCapture(false);
                }

                activeNotes.RemoveAt(i);
                Destroy(activeNotes[i].gameObject);
            }
        }

        if(activeNotes.Count == 0 && patternQueue.Count == 0) 
        {
            EndCapture(true);
        }
    }

    private void ShowFeedback(string message, bool success) 
    {
        feedbackText.text = message;
        feedbackText.color = success ? Color.green : Color.red;
        StartCoroutine(FadeFeedback());
    }

    private IEnumerator FadeFeedback() 
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = feedbackText.color;

        while(elapsed < duration) 
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed/duration);
            feedbackText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
    }

    private void EndCapture(bool success) 
    {
        isCapturing = false;
        ShowFeedback(success ? "¡Capturado!" : "¡Se escapó!", success);
        
        // Limpiar notas restantes
        foreach(var note in activeNotes) 
        {
            Destroy(note.gameObject);
        }
        activeNotes.Clear();
    }
}

public enum CreatureType {
    CREATURE_1,
    CREATURE_2,
    CREATURE_3,
    CREATURE_4,
    CREATURE_5
}