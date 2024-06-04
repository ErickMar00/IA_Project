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
        Vector3 playerVector = playerTransform.position - agentTransform.position;

        if (Vector3.Angle(playerVector.normalized, agentTransform.forward) < visionAngle * 0.5f)
        {
            if (playerVector.magnitude < visionDistance)
            {
                playerDetected = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (visionAngle <= 0f) return;

        float halfVisionAngle = visionAngle * 0.5f;

        Vector3 point1 = PointForAngle(halfVisionAngle, visionDistance);
        Vector3 point2 = PointForAngle(-halfVisionAngle, visionDistance);

        Gizmos.color = playerDetected ? Color.green : Color.red;

        // Draw vision cone
        Gizmos.DrawLine(agentTransform.position, agentTransform.position + point1);
        Gizmos.DrawLine(agentTransform.position, agentTransform.position + point2);
        Gizmos.DrawLine(agentTransform.position + point1, agentTransform.position + point2);

        // Draw forward direction
        Gizmos.DrawRay(agentTransform.position, agentTransform.forward * 5f);
    }
    Vector3 PointForAngle(float angle, float distance)
    {
        
        Vector3 forward = agentTransform.forward; 
        Vector3 right = agentTransform.right;     
        Vector3 up = agentTransform.up;           

        Quaternion rotation = Quaternion.AngleAxis(angle, up);
 
        Vector3 direction = rotation * forward;

        // Multiplicar la dirección resultante por la distancia para obtener el punto final
        return direction * distance;
    }

    // Fuentes de los recursos consultados para el proyecto
    // https://youtu.be/lV47ED8h61k?si=6m012cxUMIkJvd5z
}
