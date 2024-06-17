using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    // Enumeración para los tipos de comportamientos de dirección
    private enum SteeringBehaviorType
    {
        WanderPatrol,   // Comportamiento de patrullaje
        Pursuit,        // Comportamiento de persecución
        MAX             // Valor máximo (no se usa directamente)
    };

    private SteeringBehaviorType currentBehavior = SteeringBehaviorType.WanderPatrol; // Comportamiento actual inicializado a WanderPatrol

    // Variables públicas que se pueden ajustar desde el editor de Unity
    [SerializeField] private float MaxSpeed = 10f;   // Velocidad máxima del agente
    [SerializeField] private float Force = 10f;      // Fuerza de dirección máxima aplicada al agente

    private Rigidbody rb;       // Referencia al componente Rigidbody del agente

    public Transform[] patrolPoints;    // Puntos de patrulla definidos como array de Transform
    private int currentPatrolIndex = 0; // Índice del punto de patrulla actual

    private GameObject player;  // Referencia al GameObject del jugador

    void Awake()
    {
        rb = GetComponent<Rigidbody>();    // Obtener el componente Rigidbody del GameObject al que está adjunto este script

        // Si no hay puntos de patrulla asignados, busca automáticamente los puntos de patrulla en la escena
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            FindPatrolPoints();
        }

        // Buscar y asignar el GameObject del jugador por su nombre en la jerarquía
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        Vector3 currentSteeringForce = Vector3.zero;    // Fuerza de dirección actual inicializada a cero

        // Cambiar el comportamiento basado en si el jugador está detectado o no
        if (currentBehavior == SteeringBehaviorType.WanderPatrol)
        {
            // Si el jugador es detectado, cambiar al comportamiento de persecución
            if (IsPlayerDetected())
            {
                currentBehavior = SteeringBehaviorType.Pursuit;
            }
            else
            {
                // Si no se detecta al jugador, aplicar el comportamiento de patrullaje
                currentSteeringForce = WanderPatrol();
            }
        }
        else if (currentBehavior == SteeringBehaviorType.Pursuit)
        {
            // Si el jugador ya no es detectado, volver al comportamiento de patrullaje
            if (!IsPlayerDetected())
            {
                currentBehavior = SteeringBehaviorType.WanderPatrol;
            }
            else
            {
                // Si el jugador es detectado, aplicar el comportamiento de persecución
                currentSteeringForce = Pursuit();
            }
        }

        // Limitar la fuerza de dirección para no exceder la fuerza máxima definida
        currentSteeringForce = Vector3.ClampMagnitude(currentSteeringForce, Force);

        // Aplicar la fuerza de dirección al Rigidbody del agente
        rb.AddForce(currentSteeringForce, ForceMode.Acceleration);

        // Limitar la velocidad del agente para que no supere la velocidad máxima definida
        LimitToMaxSpeed();

        // Alinear la rotación del agente con la dirección de movimiento solo si se está moviendo
        if (rb.velocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
        }
    }

    // Método para aplicar el comportamiento de patrullaje
    Vector3 WanderPatrol()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No hay puntos de patrulla definidos.");
            return Vector3.zero;    // Devolver una dirección nula si no hay puntos de patrulla definidos
        }

        // Obtener el punto de patrulla actual
        Transform currentPatrolPoint = patrolPoints[currentPatrolIndex];

        // Calcular la dirección deseada hacia el punto de patrulla actual
        Vector3 desiredDirection = currentPatrolPoint.position - transform.position;

        // Calcular la velocidad deseada basada en la dirección hacia el punto de patrulla
        Vector3 desiredVelocity = desiredDirection.normalized * MaxSpeed;

        // Si el agente está cerca del punto de patrulla actual, pasar al siguiente punto de patrulla
        if (desiredDirection.magnitude < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        // Calcular y devolver la fuerza de dirección para alcanzar la velocidad deseada
        Vector3 steeringForce = desiredVelocity - rb.velocity;
        return steeringForce;
    }

    // Método para aplicar el comportamiento de persecución
    Vector3 Pursuit()
    {
        // Calcular la dirección hacia el jugador
        Vector3 directionToPlayer = player.transform.position - transform.position;

        // Calcular la velocidad deseada basada en la dirección hacia el jugador
        Vector3 desiredVelocity = directionToPlayer.normalized * MaxSpeed;

        // Calcular y devolver la fuerza de dirección para alcanzar la velocidad deseada
        Vector3 steeringForce = desiredVelocity - rb.velocity;
        return steeringForce;
    }

    // Método para limitar la velocidad del agente a la velocidad máxima definida
    void LimitToMaxSpeed()
    {
        if (rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxSpeed;
        }
    }

    // Método para encontrar y asignar automáticamente puntos de patrulla en la escena
    void FindPatrolPoints()
    {
        // Buscar todos los objetos con la etiqueta "PatrolPoint" en la escena
        GameObject[] patrolPointObjects = GameObject.FindGameObjectsWithTag("PatrolPoint");

        // Asignar los objetos encontrados como puntos de patrulla al array de puntos de patrulla
        patrolPoints = new Transform[patrolPointObjects.Length];
        for (int i = 0; i < patrolPointObjects.Length; i++)
        {
            patrolPoints[i] = patrolPointObjects[i].transform;
        }
    }

    // Método para determinar si el jugador está detectado por el agente
    bool IsPlayerDetected()
    {
        // La lógica para determinar si el jugador está dentro del campo de visión
        // utilizando el componente AgentSenses u otra lógica de detección que tengas implementada.
     

        // Ejemplo básico de detección por distancia: calcular la distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Devolver verdadero si el jugador está dentro de una distancia determinada (10 unidades en este caso)
        if (distanceToPlayer < 10f)  // Ajusta la distancia según tu escenario
        {
            return true;
        }
        return false;
    }
}
