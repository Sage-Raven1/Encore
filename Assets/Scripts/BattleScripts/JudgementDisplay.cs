using UnityEngine;
using TMPro;
using System.Collections;

public class JudgementDisplay : MonoBehaviour
{
    [Header("Judgement Prefabs")]
    public GameObject perfectPrefab;
    public GameObject greatPrefab;
    public GameObject goodPrefab;
    public GameObject missPrefab;

    [Header("Display Settings")]
    public Transform displayPoint;  // Where to spawn the judgement feedback
    public float displayDuration = 0.5f;  // How long to show the feedback

    void Start()
    {
        // If no display point assigned, use this transform's position
        if (displayPoint == null)
            displayPoint = transform;
    }

    public void ShowJudgement(string judgement)
    {
        GameObject prefabToSpawn = null;

        switch (judgement.ToLower())
        {
            case "perfect":
                prefabToSpawn = perfectPrefab;
                break;
            case "great":
                prefabToSpawn = greatPrefab;
                break;
            case "good":
                prefabToSpawn = goodPrefab;
                break;
            case "miss":
                prefabToSpawn = missPrefab;
                break;
            default:
                UnityEngine.Debug.LogWarning($"Unknown judgement type: {judgement}");
                return;
        }

        if (prefabToSpawn == null)
        {
            UnityEngine.Debug.LogWarning($"No prefab assigned for judgement: {judgement}");
            return;
        }

        // Spawn the judgement prefab at the display point
        GameObject judgementObj = Instantiate(prefabToSpawn, displayPoint.position, Quaternion.identity);

        // Destroy after display duration
        Destroy(judgementObj, displayDuration);
    }
}