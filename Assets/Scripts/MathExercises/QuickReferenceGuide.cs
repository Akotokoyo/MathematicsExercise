using UnityEngine;

/// <summary>
/// QUICK REFERENCE GUIDE - Copia-incolla veloce per il colloquio
/// Tutti i pattern più comuni in un unico file
/// </summary>
public class QuickReferenceGuide : MonoBehaviour
{
    // ============================================
    // 1. DISTANCE CHECK (FAST) ⭐⭐⭐
    // ============================================
    public bool IsInRange(Vector3 a, Vector3 b, float range)
    {
        // ❌ LENTO: return Vector3.Distance(a, b) <= range;
        // ✅ VELOCE (3x più veloce!):
        return (b - a).sqrMagnitude <= range * range;
    }

    // ============================================
    // 2. FIELD OF VIEW CHECK ⭐⭐⭐
    // ============================================
    public bool IsInFOV(Transform observer, Vector3 targetPos, float fovAngle)
    {
        Vector3 dirToTarget = (targetPos - observer.position).normalized;
        float dot = Vector3.Dot(observer.forward, dirToTarget);
        float minDot = Mathf.Cos(fovAngle * 0.5f * Mathf.Deg2Rad);
        return dot >= minDot;
    }

    // ============================================
    // 3. FIND NEAREST OBJECT ⭐⭐⭐
    // ============================================
    public Transform FindNearest(Vector3 position, Transform[] objects)
    {
        Transform nearest = null;
        float minSqrDist = float.MaxValue;
        
        foreach (Transform obj in objects)
        {
            float sqrDist = (obj.position - position).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = obj;
            }
        }
        
        return nearest;
    }

    // ============================================
    // 4. LEFT/RIGHT CHECK ⭐⭐
    // ============================================
    public bool IsOnRight(Transform observer, Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - observer.position;
        Vector3 cross = Vector3.Cross(observer.forward, toTarget);
        return cross.y > 0; // Positivo = destra
    }

    // ============================================
    // 5. IS BEHIND CHECK ⭐⭐
    // ============================================
    public bool IsBehind(Transform observer, Vector3 targetPos)
    {
        Vector3 dirToTarget = (targetPos - observer.position).normalized;
        float dot = Vector3.Dot(observer.forward, dirToTarget);
        return dot < 0; // Negativo = dietro
    }

    // ============================================
    // 6. WALKABLE SURFACE ⭐⭐
    // ============================================
    public bool IsWalkable(Vector3 normal, float maxSlope = 45f)
    {
        float dot = Vector3.Dot(normal, Vector3.up);
        float minDot = Mathf.Cos(maxSlope * Mathf.Deg2Rad);
        return dot >= minDot;
    }

    // ============================================
    // 7. PREDICTIVE SHOOTING ⭐⭐⭐
    // ============================================
    public Vector3 PredictTargetPos(Vector3 targetPos, Vector3 targetVel, 
                                    Vector3 shooterPos, float bulletSpeed)
    {
        Vector3 toTarget = targetPos - shooterPos;
        
        // Risolvi equazione quadratica: a*t² + b*t + c = 0
        float a = targetVel.sqrMagnitude - bulletSpeed * bulletSpeed;
        float b = 2 * Vector3.Dot(targetVel, toTarget);
        float c = toTarget.sqrMagnitude;
        
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return targetPos; // Impossibile
        
        float t = Mathf.Max(
            (-b + Mathf.Sqrt(discriminant)) / (2 * a),
            (-b - Mathf.Sqrt(discriminant)) / (2 * a)
        );
        
        return targetPos + targetVel * t;
    }

    // ============================================
    // 8. RAYCAST SHOOTING ⭐⭐
    // ============================================
    public bool Shoot(Vector3 origin, Vector3 direction, float range, out RaycastHit hit)
    {
        return Physics.Raycast(origin, direction, out hit, range);
    }

    // ============================================
    // 9. BINARY SEARCH ⭐⭐
    // ============================================
    public int BinarySearch(float[] sortedArray, float target)
    {
        int left = 0, right = sortedArray.Length - 1;
        
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            
            if (Mathf.Approximately(sortedArray[mid], target))
                return mid;
            
            if (sortedArray[mid] < target)
                left = mid + 1;
            else
                right = mid - 1;
        }
        
        return -1;
    }

    // ============================================
    // 10. SMOOTH ROTATION TOWARDS ⭐
    // ============================================
    public void RotateTowards(Transform obj, Vector3 targetPos, float speed)
    {
        Vector3 direction = (targetPos - obj.position).normalized;
        direction.y = 0;
        
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            obj.rotation = Quaternion.Slerp(obj.rotation, targetRot, speed * Time.deltaTime);
        }
    }

    // ============================================
    // 11. RICOCHET/BOUNCE ⭐
    // ============================================
    public Vector3 Bounce(Vector3 direction, Vector3 normal)
    {
        return Vector3.Reflect(direction, normal);
    }

    // ============================================
    // 12. PROJECT ON PLANE ⭐
    // ============================================
    public Vector3 ProjectOnSurface(Vector3 vector, Vector3 surfaceNormal)
    {
        return vector - surfaceNormal * Vector3.Dot(vector, surfaceNormal);
        // O semplicemente: Vector3.ProjectOnPlane(vector, surfaceNormal);
    }

    // ============================================
    // 13. CLAMP MAGNITUDE ⭐
    // ============================================
    public Vector3 ClampMagnitude(Vector3 vector, float maxLength)
    {
        if (vector.sqrMagnitude > maxLength * maxLength)
            return vector.normalized * maxLength;
        return vector;
    }

    // ============================================
    // 14. LERP SMOOTH ⭐
    // ============================================
    public Vector3 SmoothFollow(Vector3 current, Vector3 target, float smoothTime)
    {
        return Vector3.Lerp(current, target, smoothTime * Time.deltaTime);
    }

    // ============================================
    // 15. MANHATTAN DISTANCE ⭐
    // ============================================
    public float ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    // ============================================
    // 16. GET ANGLE BETWEEN VECTORS
    // ============================================
    public float GetAngle(Vector3 from, Vector3 to)
    {
        // Vector3.Angle restituisce sempre positivo (0-180)
        return Vector3.Angle(from, to);
    }

    // ============================================
    // 17. GET SIGNED ANGLE
    // ============================================
    public float GetSignedAngle(Vector3 from, Vector3 to, Vector3 axis)
    {
        float angle = Vector3.Angle(from, to);
        float sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
        return angle * sign;
    }

    // ============================================
    // 18. HORIZONTAL DISTANCE (IGNORA Y)
    // ============================================
    public float HorizontalDistance(Vector3 a, Vector3 b)
    {
        float dx = b.x - a.x;
        float dz = b.z - a.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    // ============================================
    // 19. COUNT IN RADIUS
    // ============================================
    public int CountInRadius(Vector3 center, Transform[] objects, float radius)
    {
        int count = 0;
        float sqrRadius = radius * radius;
        
        foreach (Transform obj in objects)
        {
            if ((obj.position - center).sqrMagnitude <= sqrRadius)
                count++;
        }
        
        return count;
    }

    // ============================================
    // 20. DIRECTION TO (NORMALIZED)
    // ============================================
    public Vector3 DirectionTo(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    // ============================================
    // FORMULAS CHEAT SHEET
    // ============================================
    /*
     * 
     * DOT PRODUCT:
     * -----------
     * A · B = |A| * |B| * cos(θ)
     * If normalized: A · B = cos(θ)
     * 
     * Result > 0:  Same direction (< 90°)
     * Result = 0:  Perpendicular (90°)
     * Result < 0:  Opposite direction (> 90°)
     * 
     * 
     * CROSS PRODUCT:
     * -------------
     * A × B = perpendicular vector
     * |A × B| = |A| * |B| * sin(θ)
     * 
     * Right hand rule: X × Y = Z
     * A × B = -(B × A)  [NOT commutative!]
     * 
     * 
     * DISTANCE:
     * --------
     * Euclidean: sqrt((x2-x1)² + (y2-y1)² + (z2-z1)²)
     * Manhattan: |x2-x1| + |y2-y1| + |z2-z1|
     * Chebyshev: max(|x2-x1|, |y2-y1|, |z2-z1|)
     * 
     * 
     * BALLISTICS:
     * ----------
     * y = y₀ + v*sin(θ)*t - (1/2)*g*t²
     * x = x₀ + v*cos(θ)*t
     * 
     * 
     * COMPLEXITY:
     * ----------
     * Linear Search:  O(n)
     * Binary Search:  O(log n)
     * Distance calc:  O(1)
     * sqrMagnitude:   O(1) - 3x faster than Distance
     * 
     */
}

/*
 * ============================================
 * MOST COMMON INTERVIEW QUESTIONS
 * ============================================
 * 
 * 1. "Perché sqrMagnitude è più veloce di Distance?"
 *    → Distance calcola sqrt che è costosa. sqrMagnitude no.
 *    
 * 2. "Come implementi FOV check per AI?"
 *    → Dot product tra forward e direction-to-target, confronta con cos(fov/2)
 *    
 * 3. "Come colpisci un target in movimento?"
 *    → Predictive shooting con equazione quadratica
 *    
 * 4. "Differenza tra hitscan e projectile?"
 *    → Hitscan instantaneo (raycast), projectile ha fisica
 *    
 * 5. "Come determini se oggetto è a destra o sinistra?"
 *    → Cross product, guarda segno di Y
 *    
 * 6. "Come ottimizzi ricerca con 1000 oggetti?"
 *    → Spatial partitioning, binary search su ordinati, sqrMagnitude
 *    
 * 7. "Cos'è il torque?"
 *    → Forza rotazionale = r × F (cross product)
 *    
 * 8. "Come verifichi se superficie è camminabile?"
 *    → Dot product tra normal e Vector3.up, confronta con cos(maxSlope)
 *    
 * 9. "Differenza tra dot e cross product?"
 *    → Dot ritorna scalare (misura allineamento)
 *    → Cross ritorna vettore (perpendicolare)
 *    
 * 10. "Complessità di binary search?"
 *     → O(log n) invece di O(n) lineare
 *     
 * ============================================
 */

