using UnityEngine;

/// <summary>
/// ESERCIZI: PRODOTTO SCALARE (DOT PRODUCT)
/// Uno dei concetti PIÙ IMPORTANTI per game development!
/// 
/// DOT PRODUCT FORMULA: A · B = |A| * |B| * cos(θ)
/// - Se vettori normalizzati: A · B = cos(θ)
/// - Risultato > 0: angolo acuto (< 90°) - stesso verso
/// - Risultato = 0: perpendicolari (90°)
/// - Risultato < 0: angolo ottuso (> 90°) - verso opposto
/// </summary>
public class DotProductExercises : MonoBehaviour
{
    // ============================================
    // ESERCIZIO 1: Field of View (FOV) Check
    // ============================================
    // USO PIÙ COMUNE: Capire se un oggetto è "davanti" a noi
    
    public bool IsInFieldOfView(Transform observer, Transform target, float fovAngle)
    {
        Vector3 directionToTarget = (target.position - observer.position).normalized;
        Vector3 forward = observer.forward;
        
        // Dot product per trovare l'angolo
        float dot = Vector3.Dot(forward, directionToTarget);
        
        // Converti FOV in coseno per comparazione veloce
        float minDot = Mathf.Cos(fovAngle * 0.5f * Mathf.Deg2Rad);
        
        return dot >= minDot;
    }
    
    // Versione più completa con distanza
    public bool CanSeeTarget(Transform observer, Transform target, float fovAngle, float maxDistance)
    {
        Vector3 toTarget = target.position - observer.position;
        float distance = toTarget.magnitude;
        
        if (distance > maxDistance)
            return false; // Troppo lontano
        
        Vector3 directionToTarget = toTarget / distance; // Normalizza
        float dot = Vector3.Dot(observer.forward, directionToTarget);
        float minDot = Mathf.Cos(fovAngle * 0.5f * Mathf.Deg2Rad);
        
        return dot >= minDot;
    }

    // ============================================
    // ESERCIZIO 2: Is Behind Check
    // ============================================
    // Capire se qualcosa è dietro di noi
    
    public bool IsTargetBehind(Transform observer, Transform target)
    {
        Vector3 directionToTarget = (target.position - observer.position).normalized;
        float dot = Vector3.Dot(observer.forward, directionToTarget);
        
        return dot < 0; // Negativo = dietro
    }
    
    public bool IsToTheRight(Transform observer, Transform target)
    {
        Vector3 directionToTarget = (target.position - observer.position).normalized;
        float dot = Vector3.Dot(observer.right, directionToTarget);
        
        return dot > 0; // Positivo = a destra
    }

    // ============================================
    // ESERCIZIO 3: Surface Orientation (Slope Check)
    // ============================================
    // Capire se una superficie è troppo ripida per camminare
    
    public bool IsWalkable(Vector3 surfaceNormal, float maxSlopeAngle)
    {
        // Dot product tra normale superficie e "su"
        float dot = Vector3.Dot(surfaceNormal, Vector3.up);
        
        // Converti angolo in coseno
        float minDot = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        
        return dot >= minDot;
    }
    
    public float GetSlopeAngle(Vector3 surfaceNormal)
    {
        float dot = Vector3.Dot(surfaceNormal, Vector3.up);
        return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }

    // ============================================
    // ESERCIZIO 4: Proiezione di Vettore
    // ============================================
    // Proietta velocità su una superficie (es. movimento su pendio)
    
    public Vector3 ProjectVelocityOnSurface(Vector3 velocity, Vector3 surfaceNormal)
    {
        // Rimuovi componente perpendicolare alla superficie
        return velocity - surfaceNormal * Vector3.Dot(velocity, surfaceNormal);
    }
    
    // Esempio pratico: movimento del player su pendio
    public Vector3 AdjustMoveDirectionToSlope(Vector3 moveDirection, RaycastHit slopeHit)
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    // ============================================
    // ESERCIZIO 5: Damage Direction Indicator
    // ============================================
    // Mostra da che direzione arriva il danno (UI)
    
    public float GetDamageDirectionAngle(Transform player, Vector3 damageSource)
    {
        Vector3 toSource = (damageSource - player.position).normalized;
        
        // Rimuovi componente Y per direzione orizzontale
        toSource.y = 0;
        Vector3 forward = player.forward;
        forward.y = 0;
        
        float dot = Vector3.Dot(forward.normalized, toSource);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        
        // Usa cross product per determinare segno (sinistra/destra)
        Vector3 cross = Vector3.Cross(forward, toSource);
        if (cross.y < 0)
            angle = -angle;
        
        return angle; // -180 a 180 gradi
    }

    // ============================================
    // ESERCIZIO 6: Cover System
    // ============================================
    // Determina se il player è al riparo
    
    public bool IsInCover(Transform player, Transform cover, Transform enemy)
    {
        Vector3 playerToCover = (cover.position - player.position).normalized;
        Vector3 playerToEnemy = (enemy.position - player.position).normalized;
        
        // Dot product per vedere se il cover è tra player e nemico
        float dot = Vector3.Dot(playerToCover, playerToEnemy);
        
        // Cover efficace se nella stessa direzione del nemico
        return dot > 0.7f; // ~45 gradi di tolleranza
    }

    // ============================================
    // ESERCIZIO 7: AI Flanking Detection
    // ============================================
    // Rileva se il nemico sta attaccando di fianco
    
    public enum AttackDirection
    {
        Front,
        Back,
        Left,
        Right
    }
    
    public AttackDirection GetAttackDirection(Transform defender, Transform attacker)
    {
        Vector3 toAttacker = (attacker.position - defender.position).normalized;
        
        float dotForward = Vector3.Dot(defender.forward, toAttacker);
        float dotRight = Vector3.Dot(defender.right, toAttacker);
        
        // Usa i dot products per determinare direzione
        if (Mathf.Abs(dotForward) > Mathf.Abs(dotRight))
        {
            return dotForward > 0 ? AttackDirection.Front : AttackDirection.Back;
        }
        else
        {
            return dotRight > 0 ? AttackDirection.Right : AttackDirection.Left;
        }
    }

    // ============================================
    // ESERCIZIO 8: Facing Target (AI)
    // ============================================
    // Verifica se stiamo guardando abbastanza verso il target per sparare
    
    public bool IsFacingTarget(Transform shooter, Transform target, float accuracy = 0.95f)
    {
        Vector3 toTarget = (target.position - shooter.position).normalized;
        float dot = Vector3.Dot(shooter.forward, toTarget);
        
        // accuracy = 0.95 corrisponde a ~18 gradi
        // accuracy = 0.99 corrisponde a ~8 gradi
        return dot >= accuracy;
    }
    
    public float GetAimAccuracy(Transform shooter, Transform target)
    {
        Vector3 toTarget = (target.position - shooter.position).normalized;
        float dot = Vector3.Dot(shooter.forward, toTarget);
        
        // Converti in percentuale (0 a 100)
        // -1 (opposto) = 0%, 1 (perfetto) = 100%
        return (dot + 1f) * 50f;
    }

    // ============================================
    // ESERCIZIO 9: Smooth Rotation Towards Target
    // ============================================
    // Ruota gradualmente verso target (con limite di velocità)
    
    public void RotateTowardsTarget(Transform rotator, Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - rotator.position).normalized;
        direction.y = 0; // Mantieni orizzontale
        
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rotator.rotation = Quaternion.Slerp(
                rotator.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    // Check se ha finito di ruotare
    public bool HasFinishedRotating(Transform rotator, Vector3 targetPosition, float threshold = 0.99f)
    {
        Vector3 direction = (targetPosition - rotator.position).normalized;
        direction.y = 0;
        
        Vector3 forward = rotator.forward;
        forward.y = 0;
        
        float dot = Vector3.Dot(forward.normalized, direction);
        return dot >= threshold;
    }

    // ============================================
    // ESERCIZIO 10: Stealth Detection
    // ============================================
    // Sistema furtivo: più sei davanti alla guardia, più ti vede facilmente
    
    public float GetDetectionMultiplier(Transform guard, Transform player)
    {
        Vector3 toPlayer = (player.position - guard.position).normalized;
        float dot = Vector3.Dot(guard.forward, toPlayer);
        
        // Map da [-1, 1] a [0.2, 1.0]
        // Davanti (dot=1) = 1.0x detection (facile da vedere)
        // Dietro (dot=-1) = 0.2x detection (difficile da vedere)
        return Mathf.Lerp(0.2f, 1.0f, (dot + 1f) * 0.5f);
    }
    
    public bool IsPlayerDetected(Transform guard, Transform player, float baseDetectionRange)
    {
        float distance = Vector3.Distance(guard.position, player.position);
        float multiplier = GetDetectionMultiplier(guard, player);
        float effectiveRange = baseDetectionRange * multiplier;
        
        return distance <= effectiveRange;
    }

    // ============================================
    // ESERCIZIO 11: Camera Alignment
    // ============================================
    // Allinea movimento alla camera (Resident Evil style)
    
    public Vector3 GetCameraRelativeMovement(Vector3 inputDirection, Transform camera)
    {
        Vector3 forward = camera.forward;
        Vector3 right = camera.right;
        
        // Mantieni orizzontale
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        return forward * inputDirection.z + right * inputDirection.x;
    }

    // ============================================
    // ESERCIZIO 12: Light Intensity Falloff
    // ============================================
    // Calcola intensità luce basata su direzione e distanza
    
    public float GetSpotlightIntensity(Vector3 lightPosition, Vector3 lightDirection, 
                                       Vector3 targetPosition, float angle)
    {
        Vector3 toTarget = (targetPosition - lightPosition).normalized;
        float dot = Vector3.Dot(lightDirection, toTarget);
        
        float minDot = Mathf.Cos(angle * Mathf.Deg2Rad);
        
        if (dot < minDot)
            return 0f; // Fuori dal cono di luce
        
        // Smooth falloff
        float t = (dot - minDot) / (1f - minDot);
        return t * t; // Quadratico per falloff più naturale
    }

    // ============================================
    // VISUALIZZAZIONE DEBUG
    // ============================================
    
    public Transform testTarget;
    public float fovAngle = 90f;
    
    void OnDrawGizmos()
    {
        if (testTarget == null) return;
        
        // Disegna FOV
        Vector3 forward = transform.forward;
        float halfAngle = fovAngle * 0.5f;
        
        Vector3 leftBound = Quaternion.Euler(0, -halfAngle, 0) * forward * 10f;
        Vector3 rightBound = Quaternion.Euler(0, halfAngle, 0) * forward * 10f;
        
        bool inFOV = IsInFieldOfView(transform, testTarget, fovAngle);
        
        Gizmos.color = inFOV ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBound);
        Gizmos.DrawLine(transform.position, transform.position + rightBound);
        Gizmos.DrawLine(transform.position, testTarget.position);
    }

    // ============================================
    // TEST FUNCTIONS
    // ============================================
    
    void Start()
    {
        DemonstrateAlgebraicProperties();
    }
    
    void DemonstrateAlgebraicProperties()
    {
        Vector3 a = new Vector3(1, 0, 0);
        Vector3 b = new Vector3(0, 1, 0);
        Vector3 c = new Vector3(1, 1, 0).normalized;
        
        Debug.Log("=== DOT PRODUCT PROPERTIES ===");
        
        // Perpendicular vectors
        Debug.Log($"Perpendicular (90°): {Vector3.Dot(a, b):F2} (expected: 0)");
        
        // Same direction
        Debug.Log($"Same direction (0°): {Vector3.Dot(a, a):F2} (expected: 1)");
        
        // Opposite direction
        Debug.Log($"Opposite (180°): {Vector3.Dot(a, -a):F2} (expected: -1)");
        
        // 45 degrees
        Debug.Log($"45 degrees: {Vector3.Dot(a, c):F2} (expected: 0.707)");
        
        // Commutative property
        Debug.Log($"Commutative: A·B={Vector3.Dot(a,c):F2}, B·A={Vector3.Dot(c,a):F2}");
    }
}

