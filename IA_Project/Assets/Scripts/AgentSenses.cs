using UnityEngine;

public class AgentSenses : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform agentTransform;

    [Range(0f, 360f)]
    [SerializeField] private float visionAngle = 30f;
    [SerializeField] private float visionDistance = 10f;

    private bool playerDetected = false;

    private void Update()
    {
        playerDetected = false;

        // Vector del jugador relativo al agente
        Vector3 playerVector = playerTransform.position - agentTransform.position;

        // Verifica si el jugador est� dentro del �ngulo y la distancia de visi�n del agente
        if (Vector3.Angle(playerVector.normalized, agentTransform.forward) < visionAngle * 0.5f)
        {
            if (playerVector.magnitude < visionDistance)
            {
                playerDetected = true;
                Debug.Log("Estas en  mi vision!!, Te puedo ver Chabal!!!");
            }
            else 
            {
                Debug.Log("Onde andas??, Que no te veo!!");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (visionAngle <= 0f) return;

        float halfVisionAngle = visionAngle * 0.5f;

        // Calcula los puntos del cono de visi�n del agente
        Vector3 point1 = PointForAngle(halfVisionAngle, visionDistance);
        Vector3 point2 = PointForAngle(-halfVisionAngle, visionDistance);

        // Define el color del cono de visi�n seg�n si el jugador est� detectado o no
        Gizmos.color = playerDetected ? Color.green : Color.red;

        // Dibuja el cono de visi�n y la direcci�n hacia adelante del agente
        Gizmos.DrawLine(agentTransform.position, agentTransform.position + point1);
        Gizmos.DrawLine(agentTransform.position, agentTransform.position + point2);
        Gizmos.DrawLine(agentTransform.position + point1, agentTransform.position + point2);

        Gizmos.DrawRay(agentTransform.position, agentTransform.forward * 5f);

    }

    // Funci�n para calcular un punto en el cono de visi�n del agente
    Vector3 PointForAngle(float angle, float distance)
    {
        
        Vector3 forward = agentTransform.forward; 
        Vector3 right = agentTransform.right;     
        Vector3 up = agentTransform.up;

        // Calcula la rotaci�n para el �ngulo dado
        Quaternion rotation = Quaternion.AngleAxis(angle, up);

        // Calcula la direcci�n en funci�n del �ngulo y la direcci�n hacia adelante del agente
        Vector3 direction = rotation * forward;

        // Multiplicar la direcci�n resultante por la distancia para obtener el punto final
        return direction * distance;
    }

    // Fuentes de los recursos consultados para el proyecto
    // https://youtu.be/lV47ED8h61k?si=6m012cxUMIkJvd5z
}
