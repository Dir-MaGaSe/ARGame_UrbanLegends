using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Versión modificada del CreatureManager
public class CreatureManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 15f;
    private float nextSpawnTime;

    [Header("Creature Prefabs")]
    [SerializeField] private GameObject[] creaturePrefabs;
    private List<GameObject> activeCreatures = new List<GameObject>();

    [Header("References")]
    [SerializeField] private RhythmCaptureSystem rhythmSystem;
    [SerializeField] private Transform playerTransform;

    private void Start()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
    }

    private void Update()
    {
        // Spawn creatures over time
        if (Time.time >= nextSpawnTime)
        {
            SpawnRandomCreature();
            nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
        }

        // Check for creature selection
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckCreatureSelection(Input.GetTouch(0).position);
        }
    }

    private void SpawnRandomCreature()
    {
        // Generate random position around player
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = playerTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Select random creature prefab
        int randomIndex = Random.Range(0, creaturePrefabs.Length);
        GameObject creature = Instantiate(creaturePrefabs[randomIndex], spawnPosition, Quaternion.identity);
        
        // Add to active creatures list
        activeCreatures.Add(creature);
    }

    private void CheckCreatureSelection(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            CreatureIdentifier creature = hit.collider.GetComponent<CreatureIdentifier>();
            if (creature != null)
            {
                rhythmSystem.StartCapture(creature.creatureType);
                // Optional: Hide creature during capture
                creature.gameObject.SetActive(false);
            }
        }
    }

    // Call this when capture succeeds or fails
    public void OnCaptureComplete(bool success, GameObject creature)
    {
        if (success)
        {
            activeCreatures.Remove(creature);
            Destroy(creature);
        }
        else
        {
            // Show creature again if capture failed
            creature.SetActive(true);
        }
    }
}