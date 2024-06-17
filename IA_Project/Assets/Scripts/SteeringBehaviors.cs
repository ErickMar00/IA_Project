using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    // Enumeraci�n para los tipos de comportamientos de direcci�n
    private enum SteeringBehaviorType
    {
        WanderPatrol,   // Comportamiento de patrullaje
        Pursuit,        // Comportamiento de persecuci�n
        MAX             // Valor m�ximo (no se usa directamente)
    };

    private SteeringBehaviorType currentBehavior = SteeringBehaviorType.WanderPatrol; // Comportamiento actual inicializado a WanderPatrol

    // Variables p�blicas que se pueden ajustar desde el editor de Unity
    [SerializeField] private float MaxSpeed = 10f;   // Velocidad m�xima del agente
    [SerializeField] private float Force = 10f;      // Fuerza de direcci�n m�xima aplicada al agente

    private Rigidbody rb;       // Referencia al componente Rigidbody del agente

    public Transform[] patrolPoints;    // Puntos de patrulla definidos como array de Transform
    private int currentPatrolIndex = 0; // �ndice del punto de patrulla actual

    private GameObject player;  // Referencia al GameObject del jugador

    void Awake()
    {
        rb = GetComponent<Rigidbody>();    // Obtener el componente Rigidbody del GameObject al que est� adjunto este script

        // Si no hay puntos de patrulla asignados, busca autom�ticamente los puntos de patrulla en la escena
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            FindPatrolPoints();
        }

        // Buscar y asignar el GameObject del jugador por su nombre en la jerarqu�a
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        Vector3 currentSteeringForce = Vector3.zero;    // Fuerza de direcci�n actual inicializada a cero

        // Cambiar el comportamiento basado en si el jugador est� detectado o no
        if (currentBehavior == SteeringBehaviorType.WanderPatrol)
        {
            // Si el jugador es detectado, cambiar al comportamiento de persecuci�n
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
                // Si el jugador es detectado, aplicar el comportamiento de persecuci�n
                currentSteeringForce = Pursuit();
            }
        }

        // Limitar la fuerza de direcci�n para no exceder la fuerza m�xima definida
        currentSteeringForce = Vector3.ClampMagnitude(currentSteeringForce, Force);

        // Aplicar la fuerza de direcci�n al Rigidbody del agente
        rb.AddForce(currentSteeringForce, ForceMode.Acceleration);

        // Limitar la velocidad del agente para que no supere la velocidad m�xima definida
        LimitToMaxSpeed();

        // Alinear la rotaci�n del agente con la direcci�n de movimiento solo si se est� moviendo
        if (rb.velocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
        }
    }

    // M�todo para aplicar el comportamiento de patrullaje
    Vector3 WanderPatrol()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No hay puntos de patrulla definidos.");
            return Vector3.zero;    // Devolver una direcci�n nula si no hay puntos de patrulla definidos
        }

        // Obtener el punto de patrulla actual
        Transform currentPatrolPoint = patrolPoints[currentPatrolIndex];

        // Calcular la direcci�n deseada hacia el punto de patrulla actual
        Vector3 desiredDirection = currentPatrolPoint.position - transform.position;

        // Calcular la velocidad deseada basada en la direcci�n hacia el punto de patrulla
        Vector3 desiredVelocity = desiredDirection.normalized * MaxSpeed;

        // Si el agente est� cerca del punto de patrulla actual, pasar al siguiente punto de patrulla
        if (desiredDirection.magnitude < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        // Calcular y devolver la fuerza de direcci�n para alcanzar la velocidad deseada
        Vector3 steeringForce = desiredVelocity - rb.velocity;
        return steeringForce;
    }

    // M�todo para aplicar el comportamiento de persecuci�n
    Vector3 Pursuit()
    {
        // Calcular la direcci�n hacia el jugador
        Vector3 directionToPlayer = player.transform.position - transform.position;

        // Calcular la velocidad deseada basada en la direcci�n hacia el jugador
        Vector3 desiredVelocity = directionToPlayer.normalized * MaxSpeed;

        // Calcular y devolver la fuerza de direcci�n para alcanzar la velocidad deseada
        Vector3 steeringForce = desiredVelocity - rb.velocity;
        return steeringForce;
    }

    // M�todo para limitar la velocidad del agente a la velocidad m�xima definida
    void LimitToMaxSpeed()
    {
        if (rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxSpeed;
        }
    }

    // M�todo para encontrar y asignar autom�ticamente puntos de patrulla en la escena
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

    // M�todo para determinar si el jugador est� detectado por el agente
    bool IsPlayerDetected()
    {
        // La l�gica para determinar si el jugador est� dentro del campo de visi�n
        // utilizando el componente AgentSenses u otra l�gica de detecci�n que tengas implementada.
     

        // Ejemplo b�sico de detecci�n por distancia: calcular la distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Devolver verdadero si el jugador est� dentro de una distancia determinada (10 unidades en este caso)
        if (distanceToPlayer < 10f)  // Ajusta la distancia seg�n tu escenario
        {
            return true;
        }
        return false;
    }
}
