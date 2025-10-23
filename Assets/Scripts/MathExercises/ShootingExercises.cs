using UnityEngine;

/// <summary>
/// ESERCIZI: SHOOTING SYSTEM SENSATI
/// Balistica, predizione, leading targets, etc.
/// </summary>
public class ShootingExercises : MonoBehaviour
{
    // ============================================
    // ESERCIZIO 1: Shooting Base (Raycast)
    // ============================================
    
    public void SimpleShoot(Vector3 origin, Vector3 direction, float range)
    {
        RaycastHit hit;
        
        if (Physics.Raycast(origin, direction, out hit, range))
        {
            Debug.Log($"Colpito: {hit.collider.gameObject.name} a distanza {hit.distance}");
            
            // Applica danno, effetti, etc.
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(10);
            }
        }
    }

    // ============================================
    // ESERCIZIO 2: Shoot con Gravity (Balistica)
    // ============================================
    // Formula fisica: y = y0 + v*sin(θ)*t - (1/2)*g*t²
    
    public Vector3 CalculateBallisticVelocity(Vector3 target, Vector3 origin, float angle)
    {
        float gravity = Physics.gravity.magnitude;
        
        // Scomponi in componenti
        Vector3 direction = target - origin;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);
        float horizontalDistance = directionXZ.magnitude;
        float verticalDistance = direction.y;
        
        float angleRad = angle * Mathf.Deg2Rad;
        
        // Calcola velocità iniziale necessaria
        float velocity = Mathf.Sqrt(
            (horizontalDistance * gravity) / 
            (Mathf.Sin(2 * angleRad))
        );
        
        // Calcola direzione
        Vector3 velocityXZ = directionXZ.normalized * velocity * Mathf.Cos(angleRad);
        Vector3 velocityY = Vector3.up * velocity * Mathf.Sin(angleRad);
        
        return velocityXZ + velocityY;
    }

    // ============================================
    // ESERCIZIO 3: Predictive Shooting (Leading Target)
    // ============================================
    // MOLTO IMPORTANTE per colloqui!
    // Calcola dove sparare per colpire un bersaglio in movimento
    
    public Vector3 PredictTargetPosition(
        Vector3 targetPosition, 
        Vector3 targetVelocity,
        Vector3 shooterPosition,
        float projectileSpeed)
    {
        // Risolvi equazione: |targetPos + targetVel*t - shooterPos| = projectileSpeed * t
        
        Vector3 toTarget = targetPosition - shooterPosition;
        float a = targetVelocity.sqrMagnitude - projectileSpeed * projectileSpeed;
        float b = 2 * Vector3.Dot(targetVelocity, toTarget);
        float c = toTarget.sqrMagnitude;
        
        // Risolvi equazione quadratica: a*t² + b*t + c = 0
        float discriminant = b * b - 4 * a * c;
        
        if (discriminant < 0)
        {
            // Nessuna soluzione - bersaglio troppo veloce
            return targetPosition; // Spara alla posizione attuale come fallback
        }
        
        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        
        // Prendi il tempo positivo minore
        float t = Mathf.Max(t1, t2);
        if (t < 0)
            t = Mathf.Min(t1, t2);
        
        if (t < 0)
            return targetPosition; // Fallback
        
        // Calcola posizione futura
        return targetPosition + targetVelocity * t;
    }

    // ============================================
    // ESERCIZIO 4: Spread Pattern (Recoil)
    // ============================================
    // Simula rinculo e imprecisione
    
    public Vector3 ApplySpread(Vector3 direction, float spreadAngle)
    {
        // Crea spread circolare casuale
        float spreadX = Random.Range(-spreadAngle, spreadAngle);
        float spreadY = Random.Range(-spreadAngle, spreadAngle);
        
        Quaternion spread = Quaternion.Euler(spreadY, spreadX, 0);
        
        return spread * direction;
    }
    
    // Pattern più realistico con Gaussian distribution
    public Vector3 ApplyGaussianSpread(Vector3 direction, float spreadAngle)
    {
        // Box-Muller transform per distribuzione gaussiana
        float u1 = Random.value;
        float u2 = Random.value;
        
        float gaussian = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(2.0f * Mathf.PI * u2);
        
        float spreadX = gaussian * spreadAngle;
        float spreadY = Random.Range(-spreadAngle, spreadAngle);
        
        Quaternion spread = Quaternion.Euler(spreadY, spreadX, 0);
        return spread * direction;
    }

    // ============================================
    // ESERCIZIO 5: Shotgun Pattern
    // ============================================
    // Multipli proiettili con spread
    
    public void ShootShotgun(Vector3 origin, Vector3 direction, int pelletCount, float spread)
    {
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDirection = ApplySpread(direction, spread);
            
            RaycastHit hit;
            if (Physics.Raycast(origin, spreadDirection, out hit, 50f))
            {
                // Applica danno per ogni pellet
                Debug.DrawRay(origin, spreadDirection * hit.distance, Color.red, 1f);
            }
        }
    }

    // ============================================
    // ESERCIZIO 6: Projectile Arc Trajectory
    // ============================================
    // Per visualizzare traiettoria (es. granate)
    
    public Vector3[] CalculateArcPoints(Vector3 start, Vector3 velocity, int pointCount, float timeStep)
    {
        Vector3[] points = new Vector3[pointCount];
        Vector3 currentPos = start;
        Vector3 currentVel = velocity;
        
        for (int i = 0; i < pointCount; i++)
        {
            points[i] = currentPos;
            
            // Integrazione Euler
            currentVel += Physics.gravity * timeStep;
            currentPos += currentVel * timeStep;
        }
        
        return points;
    }
    
    // Visualizza traiettoria con LineRenderer
    public void DrawTrajectory(LineRenderer lineRenderer, Vector3 start, Vector3 velocity)
    {
        Vector3[] points = CalculateArcPoints(start, velocity, 50, 0.1f);
        
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    // ============================================
    // ESERCIZIO 7: Hitscan vs Projectile
    // ============================================
    
    // HITSCAN: instantaneo (fucili)
    public void HitscanShoot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit))
        {
            // Colpisce istantaneamente
            ApplyDamage(hit.point, hit.collider.gameObject);
        }
    }
    
    // PROJECTILE: ha velocità (razzi, frecce)
    public void ProjectileShoot(Vector3 origin, Vector3 direction, float speed)
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = origin;
        
        Rigidbody rb = projectile.AddComponent<Rigidbody>();
        rb.velocity = direction * speed;
        rb.useGravity = true; // Per archi balistici
        
        // Aggiungi script per gestire collisioni
    }

    // ============================================
    // ESERCIZIO 8: Penetration Shooting
    // ============================================
    // Proiettile che attraversa multipli oggetti
    
    public void PenetrationShoot(Vector3 origin, Vector3 direction, float maxDistance, int maxPenetrations)
    {
        Vector3 currentOrigin = origin;
        int penetrations = 0;
        float remainingDistance = maxDistance;
        
        while (penetrations < maxPenetrations && remainingDistance > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentOrigin, direction, out hit, remainingDistance))
            {
                ApplyDamage(hit.point, hit.collider.gameObject);
                
                // Continua dall'altra parte dell'oggetto
                currentOrigin = hit.point + direction * 0.01f;
                remainingDistance -= hit.distance;
                penetrations++;
            }
            else
            {
                break; // Nessun altro oggetto
            }
        }
    }

    // ============================================
    // ESERCIZIO 9: Ricochet (Rimbalzo)
    // ============================================
    
    public void RicochetShoot(Vector3 origin, Vector3 direction, int maxBounces)
    {
        Vector3 currentOrigin = origin;
        Vector3 currentDirection = direction;
        
        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentOrigin, currentDirection, out hit))
            {
                // Visualizza raggio
                Debug.DrawLine(currentOrigin, hit.point, Color.yellow, 2f);
                
                ApplyDamage(hit.point, hit.collider.gameObject);
                
                // Calcola direzione di rimbalzo usando Vector3.Reflect
                currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                currentOrigin = hit.point + hit.normal * 0.01f;
            }
            else
            {
                break;
            }
        }
    }

    // ============================================
    // ESERCIZIO 10: Charge Shot
    // ============================================
    // Sparo che si carica nel tempo (potenza variabile)
    
    private float chargeTime = 0f;
    private float maxChargeTime = 3f;
    
    public void StartCharging()
    {
        chargeTime = 0f;
    }
    
    public void UpdateCharge()
    {
        chargeTime += Time.deltaTime;
        chargeTime = Mathf.Min(chargeTime, maxChargeTime);
    }
    
    public void ReleaseChargedShot(Vector3 origin, Vector3 direction)
    {
        float chargePercent = chargeTime / maxChargeTime;
        
        // Interpolazione non lineare per effetto più drammatico
        float damage = Mathf.Lerp(10f, 100f, chargePercent * chargePercent);
        float speed = Mathf.Lerp(20f, 100f, chargePercent);
        float size = Mathf.Lerp(0.1f, 1f, chargePercent);
        
        Debug.Log($"Charged Shot: {chargePercent:P0} - Damage: {damage:F0}, Speed: {speed:F0}");
        
        // Spara con parametri basati su carica
        ProjectileShoot(origin, direction, speed);
    }

    // ============================================
    // Helper Functions
    // ============================================
    
    private void ApplyDamage(Vector3 hitPoint, GameObject target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(10);
        }
        
        // Effetti visivi
        Debug.Log($"Hit: {target.name} at {hitPoint}");
    }
}

// Interface per oggetti che possono ricevere danno
public interface IDamageable
{
    void TakeDamage(float damage);
}

