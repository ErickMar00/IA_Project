using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSenses : MonoBehaviour
{
    public Transform playerTransform;
    public Transform agentTransform;

    [Range(0f, 360f)]
    [SerializeField] private float visionAngle = 30f;
    [SerializeField] private float visionDistance = 10f;

    [Range(0f, 180f)] // Ampliar el rango para permitir �ngulos mayores si es necesario
    [SerializeField] private float additionalVisionAngle = 15f; // Variable para el aumento del �ngulo de visi�n

    private bool playerDetected = false;

    public bool IsPlayerDetected()
    {
        // Vector del jugador relativo al agente
        Vector3 playerVector = playerTransform.position - agentTransform.position;

        // Verifica si el jugador est� dentro del �ngulo de visi�n del agente y dentro de la distancia de visi�n
        if (Vector3.Angle(playerVector.normalized, agentTransform.forward) < visionAngle * 0.5f)
        {
            if (playerVector.magnitude < visionDistance)
            {
                playerDetected = true;
            }
            else
            {
                playerDetected = false;
            }
        }
        else
        {
            playerDetected = false;
        }

        return playerDetected;
    }

    private void OnDrawGizmos()
    {
        if (visionAngle <= 0f) return;

        float halfVisionAngle = visionAngle * 0.5f;

        // Aumenta el �ngulo de visi�n si el jugador est� detectado
        float currentVisionAngle = playerDetected ? visionAngle + additionalVisionAngle : visionAngle;

        // Calcula los puntos del cono de visi�n del agente con el �ngulo actual
        Vector3 point1 = PointForAngle(halfVisionAngle, visionDistance);
        Vector3 point2 = PointForAngle(-halfVisionAngle, visionDistance);

        // Define el color del cono de visi�n seg�n si el jugador est� detectado o no
        Gizmos.color = playerDetected ? Color.green : Color.red;

        // Dibuja el cono de visi�n y la direcci�n hacia adelante del agente
        Gizmos.DrawLine(agentTransform.position, agentTransform.position + PointForAngle(halfVisionAngle, visionDistance));
        Gizmos.DrawLine(agentTransform.position, agentTransform.position + PointForAngle(-halfVisionAngle, visionDistance));
        Gizmos.DrawLine(agentTransform.position + PointForAngle(halfVisionAngle, visionDistance), agentTransform.position + PointForAngle(-halfVisionAngle, visionDistance));
        Gizmos.DrawRay(agentTransform.position, agentTransform.forward * visionDistance);

        // Dibuja el cono de visi�n ampliado si el jugador est� detectado
        if (playerDetected)
        {
            float halfCurrentVisionAngle = currentVisionAngle * 0.5f;
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Color verde transparente
            Gizmos.DrawLine(agentTransform.position, agentTransform.position + PointForAngle(halfCurrentVisionAngle, visionDistance));
            Gizmos.DrawLine(agentTransform.position, agentTransform.position + PointForAngle(-halfCurrentVisionAngle, visionDistance));
            Gizmos.DrawLine(agentTransform.position + PointForAngle(halfCurrentVisionAngle, visionDistance), agentTransform.position + PointForAngle(-halfCurrentVisionAngle, visionDistance));
        }
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


