using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ESERCIZI: DISTANZA TRA OGGETTI
/// Fondamentale per AI, detection, gameplay mechanics
/// </summary>
public class DistanceExercises : MonoBehaviour
{
    // ============================================
    // ESERCIZIO 1: Distance vs SqrMagnitude
    // ============================================
    // IMPORTANTE: SqrMagnitude è MOLTO più veloce (no sqrt)
    // Usa SqrMagnitude quando possibile per ottimizzazione!
    
    public bool IsEnemyInRange(Vector3 playerPos, Vector3 enemyPos, float range)
    {
        // ❌ LENTO: usa Vector3.Distance (calcola sqrt)
        // return Vector3.Distance(playerPos, enemyPos) <= range;
        
        // ✅ VELOCE: usa SqrMagnitude (no sqrt)
        float sqrDistance = (enemyPos - playerPos).sqrMagnitude;
        return sqrDistance <= range * range;
    }

    // ============================================
    // ESERCIZIO 2: Trova il nemico più vicino
    // ============================================
    // Scenario classico di colloquio!
    
    public Transform FindNearestEnemy(Vector3 playerPosition, Transform[] enemies)
    {
        Transform nearest = null;
        float minSqrDistance = float.MaxValue;
        
        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;
            
            float sqrDistance = (enemy.position - playerPosition).sqrMagnitude;
            
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearest = enemy;
            }
        }
        
        return nearest;
    }

    // ============================================
    // ESERCIZIO 3: Distanza su asse specifico (es. solo orizzontale)
    // ============================================
    // Utile per giochi 2D o per ignorare l'altezza
    
    public float GetHorizontalDistance(Vector3 pos1, Vector3 pos2)
    {
        // Ignora Y (altezza)
        Vector3 pos1Flat = new Vector3(pos1.x, 0, pos1.z);
        Vector3 pos2Flat = new Vector3(pos2.x, 0, pos2.z);
        
        return Vector3.Distance(pos1Flat, pos2Flat);
    }
    
    // Versione ottimizzata
    public float GetHorizontalDistanceSqr(Vector3 pos1, Vector3 pos2)
    {
        float dx = pos2.x - pos1.x;
        float dz = pos2.z - pos1.z;
        return dx * dx + dz * dz;
    }

    // ============================================
    // ESERCIZIO 4: Manhattan Distance
    // ============================================
    // Ancora più veloce! Usata per pathfinding su griglia
    
    public float ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
    
    public float ManhattanDistance2D(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // ============================================
    // ESERCIZIO 5: Chebyshev Distance
    // ============================================
    // Usata per movimento "king's move" (come gli scacchi)
    
    public float ChebyshevDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    // ============================================
    // ESERCIZIO 6: Distance Check con Early Exit
    // ============================================
    // Ottimizzazione: conta quanti nemici sono in range
    
    public int CountEnemiesInRange(Vector3 center, Transform[] enemies, float range)
    {
        int count = 0;
        float sqrRange = range * range;
        
        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;
            
            if ((enemy.position - center).sqrMagnitude <= sqrRange)
            {
                count++;
            }
        }
        
        return count;
    }

    // ============================================
    // ESERCIZIO 7: Proximity Check per Cluster Detection
    // ============================================
    // Trova se ci sono almeno N nemici entro una certa distanza
    
    public bool AreEnemiesClustered(Transform[] enemies, int minEnemies, float clusterRadius)
    {
        float sqrRadius = clusterRadius * clusterRadius;
        
        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;
            
            int nearbyCount = 0;
            
            foreach (Transform other in enemies)
            {
                if (other == null || other == enemy) continue;
                
                if ((other.position - enemy.position).sqrMagnitude <= sqrRadius)
                {
                    nearbyCount++;
                    
                    if (nearbyCount >= minEnemies)
                        return true; // Early exit - ottimizzazione!
                }
            }
        }
        
        return false;
    }

    // ============================================
    // ESERCIZIO 8: Distanza da linea/segmento
    // ============================================
    // Utile per AI che seguono percorsi
    
    public float DistanceToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        Vector3 pointDirection = point - lineStart;
        
        float lineLengthSqr = lineDirection.sqrMagnitude;
        
        if (lineLengthSqr < 0.001f) // Linea degenere
            return Vector3.Distance(point, lineStart);
        
        // Proiezione del punto sulla linea
        float t = Vector3.Dot(pointDirection, lineDirection) / lineLengthSqr;
        
        // Clamp per segmento (0 a 1)
        t = Mathf.Clamp01(t);
        
        Vector3 closestPoint = lineStart + lineDirection * t;
        
        return Vector3.Distance(point, closestPoint);
    }

    // ============================================
    // ESERCIZIO 9: Spatial Hashing per ottimizzazione
    // ============================================
    // Avanzato: dividi il mondo in celle per ricerche veloci
    
    private Dictionary<Vector2Int, List<Transform>> spatialGrid = new Dictionary<Vector2Int, List<Transform>>();
    private float cellSize = 10f;
    
    public void BuildSpatialGrid(Transform[] objects)
    {
        spatialGrid.Clear();
        
        foreach (Transform obj in objects)
        {
            if (obj == null) continue;
            
            Vector2Int cell = GetCell(obj.position);
            
            if (!spatialGrid.ContainsKey(cell))
                spatialGrid[cell] = new List<Transform>();
            
            spatialGrid[cell].Add(obj);
        }
    }
    
    private Vector2Int GetCell(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }
    
    public List<Transform> GetNearbyObjectsFast(Vector3 position)
    {
        List<Transform> nearby = new List<Transform>();
        Vector2Int center = GetCell(position);
        
        // Cerca nelle 9 celle circostanti (3x3)
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector2Int cell = center + new Vector2Int(x, z);
                
                if (spatialGrid.ContainsKey(cell))
                    nearby.AddRange(spatialGrid[cell]);
            }
        }
        
        return nearby;
    }

    // ============================================
    // TEST FUNCTIONS
    // ============================================
    
    void Start()
    {
        TestPerformance();
        TestManhattanVsEuclidean();
    }
    
    void TestPerformance()
    {
        Vector3 a = new Vector3(10, 20, 30);
        Vector3 b = new Vector3(15, 25, 35);
        
        // Misura performance
        var watch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < 100000; i++)
        {
            float d = Vector3.Distance(a, b); // Con sqrt
        }
        watch.Stop();
        long timeWithSqrt = watch.ElapsedMilliseconds;
        
        watch.Restart();
        for (int i = 0; i < 100000; i++)
        {
            float d = (b - a).sqrMagnitude; // Senza sqrt
        }
        watch.Stop();
        long timeWithoutSqrt = watch.ElapsedMilliseconds;
        
        Debug.Log($"Distance (sqrt): {timeWithSqrt}ms vs SqrMagnitude: {timeWithoutSqrt}ms");
        Debug.Log($"SqrMagnitude è {(float)timeWithSqrt/timeWithoutSqrt:F1}x più veloce!");
    }
    
    void TestManhattanVsEuclidean()
    {
        Vector3 a = Vector3.zero;
        Vector3 b = new Vector3(3, 4, 0);
        
        float euclidean = Vector3.Distance(a, b);
        float manhattan = ManhattanDistance(a, b);
        
        Debug.Log($"Euclidean: {euclidean:F2}, Manhattan: {manhattan:F2}");
    }
}

