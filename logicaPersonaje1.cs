using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class logicaPersonaje1 : MonoBehaviour
{
    public Transform[] zonasAleatorias;
    public float velocidadDesplazamiento = 3.5f;
    public float esperaIdle = 2f;
    public float esperaListening = 3f;
    public float esperaGreeting = 3f;

    private NavMeshAgent agente;
    private Animator animator;
    private int destinoActual;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadDesplazamiento;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("El Animator no fue encontrado.");
        }
        else
        {
            if (zonasAleatorias.Length > 0)
            {
                StartCoroutine(VisitarZonasAleatorias());
            }
            else
            {
                Debug.LogError("No se han asignado zonas aleatorias.");
            }
        }
    }

    IEnumerator VisitarZonasAleatorias()
    {
        while (true)
        {
            nuevoDestino();
            agente.SetDestination(zonasAleatorias[destinoActual].position);

            yield return new WaitUntil(() => AgenteLlegoAlDestino());

            // Detener al agente
            agente.isStopped = true;

            // Animación idle
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isListening", false);
            animator.SetBool("isGreeting", false);

            yield return new WaitForSeconds(esperaIdle);

            // Animación listening
            animator.SetBool("isIdle", false);
            animator.SetBool("isListening", true);

            yield return new WaitForSeconds(esperaListening);

            // Animación greeting
            animator.SetBool("isListening", false);
            animator.SetBool("isGreeting", true);

            yield return new WaitForSeconds(esperaGreeting);

            // Reanudar movimiento del agente
            agente.isStopped = false;

            // Animación walking
            animator.SetBool("isWalking", true);
            animator.SetBool("isGreeting", false);
        }
    }

    void nuevoDestino()
    {
        int nuevoDestino;
        do
        {
            nuevoDestino = Random.Range(0, zonasAleatorias.Length);
        } while (nuevoDestino == destinoActual);

        destinoActual = nuevoDestino;
    }

    bool AgenteLlegoAlDestino()
    {
        if (!agente.pathPending)
        {
            if (agente.remainingDistance <= agente.stoppingDistance)
            {
                if (!agente.hasPath || agente.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
