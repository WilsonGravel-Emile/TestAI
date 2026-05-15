using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Fait par Emile Lucas Wilson
/// Ce script est l'état de patrouille des Ennemies:
/// Il sert : 
/// 1. Localiser les waypoints
/// 2. se déplacer vers eux
/// 3. gagner des points par le nombre de waypoints toucher
/// </summary>
public class PatrolState : IState
{
    //Récupère les refs et plus
    private readonly NavMeshAgent _agent;
    private readonly Transform[] _waypoints; 
    private int currentWaypointIndex;

    // Constructeur pour initialiser les références
    public PatrolState(NavMeshAgent agent , Transform[] waypoints)
    {
        _agent = agent;
        _waypoints = waypoints;
    }

    public void OnEnter()
    {
        _agent.isStopped = false; // arrête Ennemy
        
        //le Waypoint actuel devient le plus proche du point de départ de l'ennemi
        currentWaypointIndex = GetClosestWaypointIndex();
        // On recommence au point le plus proche ou on continue la boucle
        SetDestinationToWaypoint();
        
        Debug.Log("Miann commence sa ronde dans l'hôpital...");
    }
    // Méthode pour définir la destination vers le waypoint actuel
    public void Tick()
    {
        if (_waypoints == null|| _waypoints.Length == 0) return; // Si il n'y a pas de waypoints, on ne fait rien
        if (!_agent.pathPending &&_agent.remainingDistance < 0.5f) // Si l'ennemi est proche du waypoint actuel
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % _waypoints.Length; // Passer au waypoint suivant (boucle)
            SetDestinationToWaypoint(); // Mettre à jour la destination vers le nouveau waypoint
            Debug.Log("Patrolling to waypoint " + currentWaypointIndex);
        }
    }
    public void OnExit()
    {
        _agent.isStopped = true; // arrête Ennemy
    }
        public Color GizmoColor()
    {
        return Color.yellow;
    }
    // Méthode pour trouver l'index du waypoint le plus proche
    private int GetClosestWaypointIndex()
    {
        // index le plus proche est égal à zero
        int closestIndex = 0;
        float closestDistance = Mathf.Infinity;
        // boucle sort quand l'ennemi à fini de trouver le waypoint le plus proche
        for (int i = 0; i < _waypoints.Length; i++)
        {
            float dist = Vector3.Distance(_agent.transform.position, _waypoints[i].position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;

            }
        }
        // retourne la valeur de l'index et devient la destination de l'ennemi
        return closestIndex;
    }
    private void SetDestinationToWaypoint()
    {
        if (_waypoints == null || _waypoints.Length == 0) return; // Si il n'y a pas de waypoints, on ne fait rien
        _agent.SetDestination(_waypoints[currentWaypointIndex].position); // Mettre à jour la destination vers le waypoint actuel
    }

}