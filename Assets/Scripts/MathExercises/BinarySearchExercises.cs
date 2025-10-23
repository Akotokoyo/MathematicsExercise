using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ESERCIZI: RICERCA DICOTOMICA (Binary Search)
/// Utile per trovare velocemente elementi in array ordinati in O(log n)
/// </summary>
public class BinarySearchExercises : MonoBehaviour
{
    // ============================================
    // ESERCIZIO 1: Binary Search Base
    // ============================================
    // Domanda: Trova l'indice di un valore in un array ordinato di float
    // Complessità: O(log n) invece di O(n) della ricerca lineare
    
    public int BinarySearch(float[] sortedArray, float target)
    {
        int left = 0;
        int right = sortedArray.Length - 1;
        
        while (left <= right)
        {
            int mid = left + (right - left) / 2; // Evita overflow
            
            if (Mathf.Approximately(sortedArray[mid], target))
                return mid;
            
            if (sortedArray[mid] < target)
                left = mid + 1;
            else
                right = mid - 1;
        }
        
        return -1; // Non trovato
    }

    // ============================================
    // ESERCIZIO 2: Trova il GameObject più vicino per distanza (ottimizzato)
    // ============================================
    // Scenario: Hai 1000 nemici ordinati per posizione X, trova il più vicino
    
    private List<Enemy> enemiesSortedByX = new List<Enemy>();
    
    public Enemy FindNearestEnemyOptimized(Vector3 playerPosition)
    {
        if (enemiesSortedByX.Count == 0) return null;
        
        // Binary search per trovare la zona di ricerca
        int index = BinarySearchClosestX(playerPosition.x);
        
        // Cerca solo in una piccola area invece che in tutti i 1000 nemici
        int searchRadius = 10;
        int start = Mathf.Max(0, index - searchRadius);
        int end = Mathf.Min(enemiesSortedByX.Count - 1, index + searchRadius);
        
        Enemy nearest = null;
        float minDistance = float.MaxValue;
        
        for (int i = start; i <= end; i++)
        {
            float distance = Vector3.Distance(playerPosition, enemiesSortedByX[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemiesSortedByX[i];
            }
        }
        
        return nearest;
    }
    
    private int BinarySearchClosestX(float targetX)
    {
        int left = 0;
        int right = enemiesSortedByX.Count - 1;
        
        while (left < right)
        {
            int mid = left + (right - left) / 2;
            
            if (enemiesSortedByX[mid].transform.position.x < targetX)
                left = mid + 1;
            else
                right = mid;
        }
        
        return left;
    }

    // ============================================
    // ESERCIZIO 3: Trova il livello appropriato per il punteggio
    // ============================================
    // Scenario: Array di soglie di punteggio [100, 500, 1000, 5000, 10000]
    // Trova quale livello corrisponde al punteggio del giocatore
    
    public int FindLevelForScore(int[] levelThresholds, int playerScore)
    {
        int left = 0;
        int right = levelThresholds.Length - 1;
        int result = 0;
        
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            
            if (levelThresholds[mid] <= playerScore)
            {
                result = mid + 1; // Il livello è mid + 1
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        
        return result;
    }

    // ============================================
    // ESERCIZIO 4: LOD (Level of Detail) System
    // ============================================
    // Usa binary search per trovare quale LOD usare in base alla distanza
    
    [System.Serializable]
    public struct LODLevel
    {
        public float distance;
        public int meshQuality; // 0=low, 1=medium, 2=high
    }
    
    public LODLevel[] lodLevels = new LODLevel[]
    {
        new LODLevel { distance = 10f, meshQuality = 2 },  // High quality
        new LODLevel { distance = 50f, meshQuality = 1 },  // Medium
        new LODLevel { distance = 100f, meshQuality = 0 }  // Low
    };
    
    public int GetLODForDistance(float distance)
    {
        // Binary search per trovare il LOD appropriato
        for (int i = 0; i < lodLevels.Length; i++)
        {
            if (distance <= lodLevels[i].distance)
                return lodLevels[i].meshQuality;
        }
        return 0; // Qualità più bassa per default
    }

    // ============================================
    // TEST FUNCTIONS
    // ============================================
    
    void Start()
    {
        TestBinarySearch();
        TestLevelSearch();
    }
    
    void TestBinarySearch()
    {
        float[] scores = { 10.5f, 23.7f, 45.2f, 67.8f, 89.1f, 120.3f };
        int index = BinarySearch(scores, 67.8f);
        Debug.Log($"Binary Search Test: Trovato 67.8 all'indice {index} (expected: 3)");
    }
    
    void TestLevelSearch()
    {
        int[] thresholds = { 0, 100, 500, 1000, 5000, 10000 };
        int level = FindLevelForScore(thresholds, 750);
        Debug.Log($"Level Search Test: Score 750 = Livello {level} (expected: 2)");
    }
}

// Classe helper per l'esempio
public class Enemy : MonoBehaviour
{
    // Placeholder
}

