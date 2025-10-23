using UnityEngine;

/// <summary>
/// BONUS: PRODOTTO VETTORIALE (CROSS PRODUCT)
/// Complementare al dot product, altrettanto importante!
/// 
/// CROSS PRODUCT: A × B = vettore perpendicolare a entrambi
/// - Magnitudo: |A × B| = |A| * |B| * sin(θ)
/// - Direzione: regola della mano destra
/// - A × B = -B × A (non commutativo!)
/// </summary>
public class CrossProductExercises : MonoBehaviour
{
    // ============================================
    // ESERCIZIO 1: Determinare Direzione (Destra/Sinistra)
    // ============================================
    // USO PRINCIPALE: Capire se qualcosa è a sinistra o destra
    
    public bool IsTargetOnRight(Transform observer, Transform target)
    {
        Vector3 toTarget = target.position - observer.position;
        
        // Cross product tra forward e toTarget
        Vector3 cross = Vector3.Cross(observer.forward, toTarget);
        
        // Se Y positivo = destra, Y negativo = sinistra
        return cross.y > 0;
    }
    
    public enum RelativeDirection
    {
        Left,
        Right,
        Forward,
        Behind
    }
    
    public RelativeDirection GetRelativeDirection(Transform observer, Transform target)
    {
        Vector3 toTarget = (target.position - observer.position).normalized;
        
        float dot = Vector3.Dot(observer.forward, toTarget);
        Vector3 cross = Vector3.Cross(observer.forward, toTarget);
        
        // Combina dot e cross per direzione precisa
        if (Mathf.Abs(dot) > 0.7f) // Principalmente davanti/dietro
        {
            return dot > 0 ? RelativeDirection.Forward : RelativeDirection.Behind;
        }
        else // Principalmente destra/sinistra
        {
            return cross.y > 0 ? RelativeDirection.Right : RelativeDirection.Left;
        }
    }

    // ============================================
    // ESERCIZIO 2: Calcolo Normale di Superficie
    // ============================================
    // Date 3 punti di un triangolo, calcola la normale
    
    public Vector3 CalculateTriangleNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 edge1 = p2 - p1;
        Vector3 edge2 = p3 - p1;
        
        // Cross product dà vettore perpendicolare al piano
        Vector3 normal = Vector3.Cross(edge1, edge2);
        
        return normal.normalized;
    }
    
    // Calcola area del triangolo
    public float CalculateTriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 edge1 = p2 - p1;
        Vector3 edge2 = p3 - p1;
        
        // Magnitudo del cross product = 2 * area
        return Vector3.Cross(edge1, edge2).magnitude * 0.5f;
    }

    // ============================================
    // ESERCIZIO 3: Rotation Direction
    // ============================================
    // Determina in che direzione ruotare (verso target)
    
    public float GetRotationDirection(Transform rotator, Vector3 targetPosition)
    {
        Vector3 toTarget = (targetPosition - rotator.position).normalized;
        Vector3 cross = Vector3.Cross(rotator.forward, toTarget);
        
        // Ritorna -1 (sinistra), 0 (allineato), o 1 (destra)
        return Mathf.Sign(cross.y);
    }
    
    // Ruota nella direzione più breve
    public void RotateTowardsOptimal(Transform rotator, Vector3 targetPosition, float speed)
    {
        float direction = GetRotationDirection(rotator, targetPosition);
        rotator.Rotate(Vector3.up, direction * speed * Time.deltaTime);
    }

    // ============================================
    // ESERCIZIO 4: Torque Calculation
    // ============================================
    // Fisica: Torque = r × F (posizione × forza)
    
    public Vector3 CalculateTorque(Vector3 pivotPoint, Vector3 forcePoint, Vector3 force)
    {
        Vector3 r = forcePoint - pivotPoint; // Vettore posizione
        Vector3 torque = Vector3.Cross(r, force);
        
        return torque;
    }
    
    // Esempio pratico: porta che ruota
    public void ApplyTorqueToDoor(Rigidbody door, Vector3 hitPoint, Vector3 force)
    {
        Vector3 torque = CalculateTorque(door.transform.position, hitPoint, force);
        door.AddTorque(torque);
    }

    // ============================================
    // ESERCIZIO 5: Path Following (AI)
    // ============================================
    // Determina quanto correggere il percorso
    
    public float GetPathDeviation(Vector3 currentPosition, Vector3 pathDirection, Vector3 desiredPosition)
    {
        Vector3 toDesired = desiredPosition - currentPosition;
        Vector3 cross = Vector3.Cross(pathDirection, toDesired);
        
        // Magnitudo indica quanto siamo fuori dal percorso
        return cross.magnitude;
    }

    // ============================================
    // ESERCIZIO 6: Camera System (Third Person)
    // ============================================
    // Calcola "right" vector della camera
    
    public Vector3 CalculateCameraRight(Vector3 cameraForward)
    {
        // Cross product tra forward e world up
        Vector3 right = Vector3.Cross(cameraForward, Vector3.up);
        return right.normalized;
    }
    
    // Calcola sistema di coordinate completo
    public void CalculateCameraVectors(Vector3 cameraForward, out Vector3 right, out Vector3 up)
    {
        right = Vector3.Cross(cameraForward, Vector3.up).normalized;
        up = Vector3.Cross(right, cameraForward).normalized;
    }

    // ============================================
    // ESERCIZIO 7: Winding Order Check
    // ============================================
    // Determina se i punti sono in ordine orario o antiorario
    
    public bool IsClockwise(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 edge1 = p2 - p1;
        Vector3 edge2 = p3 - p2;
        
        Vector3 cross = Vector3.Cross(edge1, edge2);
        
        // Se Y negativo (guardando dall'alto) = orario
        return cross.y < 0;
    }

    // ============================================
    // ESERCIZIO 8: Perpendicular Vector
    // ============================================
    // Crea vettore perpendicolare (utile per effetti, trails, etc.)
    
    public Vector3 GetPerpendicularVector(Vector3 vector)
    {
        // Cross con asse più diverso
        Vector3 axis = Mathf.Abs(vector.y) < 0.9f ? Vector3.up : Vector3.right;
        return Vector3.Cross(vector, axis).normalized;
    }
    
    // Esempio: crea billboard perpendicolare a movimento
    public void OrientBillboard(Transform billboard, Vector3 movementDirection)
    {
        Vector3 right = Vector3.Cross(movementDirection, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, movementDirection).normalized;
        
        billboard.rotation = Quaternion.LookRotation(movementDirection, up);
    }

    // ============================================
    // ESERCIZIO 9: Steering Behavior
    // ============================================
    // AI: calcola forza di sterzo per seguire percorso
    
    public Vector3 CalculateSteeringForce(Vector3 currentVelocity, Vector3 desiredDirection, float maxForce)
    {
        Vector3 desiredVelocity = desiredDirection.normalized * currentVelocity.magnitude;
        Vector3 steering = desiredVelocity - currentVelocity;
        
        // Limita forza
        if (steering.magnitude > maxForce)
            steering = steering.normalized * maxForce;
        
        return steering;
    }

    // ============================================
    // ESERCIZIO 10: Signed Angle
    // ============================================
    // Calcola angolo con segno (positivo/negativo)
    
    public float GetSignedAngle(Vector3 from, Vector3 to, Vector3 axis)
    {
        float angle = Vector3.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);
        
        // Usa dot product per determinare segno
        float sign = Mathf.Sign(Vector3.Dot(axis, cross));
        
        return angle * sign;
    }
    
    // Esempio 2D (top-down)
    public float GetSignedAngle2D(Vector3 from, Vector3 to)
    {
        return GetSignedAngle(from, to, Vector3.up);
    }

    // ============================================
    // ESERCIZIO 11: Vehicle Physics
    // ============================================
    // Calcola sideways force per drifting
    
    public Vector3 CalculateSidewaysForce(Transform vehicle, Vector3 velocity)
    {
        Vector3 right = vehicle.right;
        
        // Componente laterale della velocità
        float sidewaysSpeed = Vector3.Dot(velocity, right);
        
        // Forza per annullare movimento laterale (grip)
        return -right * sidewaysSpeed;
    }

    // ============================================
    // VISUALIZZAZIONE DEBUG
    // ============================================
    
    public Transform testTarget;
    
    void OnDrawGizmos()
    {
        if (testTarget == null) return;
        
        Vector3 toTarget = testTarget.position - transform.position;
        Vector3 cross = Vector3.Cross(transform.forward, toTarget);
        
        // Disegna cross product
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, cross.normalized * 2f);
        
        // Colore basato su direzione
        Gizmos.color = cross.y > 0 ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, testTarget.position);
        
        // Label
        bool isRight = cross.y > 0;
        Debug.DrawRay(transform.position, transform.right * 2f, isRight ? Color.green : Color.gray);
    }

    // ============================================
    // TEST FUNCTIONS
    // ============================================
    
    void Start()
    {
        DemonstrateProperties();
    }
    
    void DemonstrateProperties()
    {
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;
        Vector3 up = Vector3.up;
        
        Debug.Log("=== CROSS PRODUCT PROPERTIES ===");
        
        // Perpendicular result
        Vector3 result = Vector3.Cross(forward, right);
        Debug.Log($"Forward × Right = {result} (expected: Up)");
        
        // Anti-commutative
        Vector3 a = Vector3.Cross(forward, right);
        Vector3 b = Vector3.Cross(right, forward);
        Debug.Log($"A × B = {a}, B × A = {b} (opposti!)");
        
        // Parallel vectors = zero
        Vector3 parallel = Vector3.Cross(forward, forward);
        Debug.Log($"Forward × Forward = {parallel} (expected: zero)");
        
        // Right hand rule
        Debug.Log($"X × Y = {Vector3.Cross(Vector3.right, Vector3.up)} (expected: forward)");
        Debug.Log($"Y × Z = {Vector3.Cross(Vector3.up, Vector3.forward)} (expected: right)");
        Debug.Log($"Z × X = {Vector3.Cross(Vector3.forward, Vector3.right)} (expected: up)");
    }
}

