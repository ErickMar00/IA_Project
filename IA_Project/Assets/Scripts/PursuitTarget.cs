using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitTarget : MonoBehaviour
{
    private AgentSenses agentSenses; // Referencia al componente AgentSenses

    [SerializeField] private float enemySpeed = 15f; // Velocidad del enemigo

    [SerializeField] private GameObject target; // Referencia a nuestro objetivo jugador en la jerarquía

    void Start()
    {
        target = GameObject.Find("Player"); // Busca y asigna el objeto llamado "Player" en la jerarquía
        agentSenses = GetComponent<AgentSenses>(); // Obtiene el componente AgentSenses de este objeto
    }

    void Update()
    {
        Behavior(); // Llama al método Behavior en cada frame
    }

    public void Behavior()
    {
        if (agentSenses != null && agentSenses.IsPlayerDetected()) // Verifica si el jugador es detectado por AgentSenses
        {
            // Mueve hacia el jugador usando la dirección determinada por agentSenses
            Vector3 directionToPlayer = target.transform.position - transform.position; // Calcula la dirección hacia el jugador
            Quaternion rotation = Quaternion.LookRotation(directionToPlayer); // Calcula la rotación hacia el jugador
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2); // Rota gradualmente hacia la dirección del jugador
            transform.Translate(Vector3.forward.normalized * enemySpeed * Time.deltaTime); // Avanza hacia adelante hacia el jugador (movimiento normalizado)
        }
    }
}
//Fuentes
//Codigo con ayuda de Ivan Medina