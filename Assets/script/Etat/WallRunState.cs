using UnityEngine;
using UnityEngine.AI;
 /// <summary>

/// État WallRun pour l’ennemi Miann.

///
/// Objectif :
/// déplacement stable sur mur
/// — suppression du shake
/// — sortie fluide sans retour en arrière
/// — possibilité de suivre le mur même si la détection est momentanément perdue
/// — mouvement dans toutes les directions (pas seulement vers le haut)
/// Logique :
/// 1. Détecte les murs devant, à droite et à gauche de l’
/// ennemi à l’aide de raycasts
/// 2. Si un mur est détecté, calcule la direction de déplacement en projetant la direction vers le joueur sur le plan du ,
/// mur pour permettre de se déplacer dans toutes les directions
/// 3. Oriente le haut de l’ennemi vers la normale du mur pour un effet de collage réaliste
/// 4. Si le mur est perdu temporairement, continue tout droit pour éviter les secousses
/// 5. Lors de la sortie du mur, téléporte l’agent interne à la position du transform pour éviter le bug de retour arrière
/// </summary> 
public class WallRunState : IState
{
    private EnnemyReferences _ennemyRef; // Référence aux données de l'ennemi, notamment les paramètres de déplacement et les références aux composants
    private NavMeshAgent _agent; // Référence au NavMeshAgent de l'ennemi
    private Transform _player; // Référence à la cible (le joueur)

    private RaycastHit _wallHit;  // Stocke les informations du mur détecté
    private Vector3 _wallNormal; // Stocke la normale du mur détecté pour orienter le mouvement
    private bool _wallDetected; // Indique si un mur est actuellement détecté
    private Vector3 _lastWallNormal;    // Mémorise la dernière normale de mur détecté pour continuer à suivre le mur même si la détection est momentanément perdue
    private bool _wasWallDetected;  // Indique si un mur a été détecté au moins une fois pour permettre de continuer à suivre le mur même si la détection est momentanément perdue

    public WallRunState(EnnemyReferences ennemyRef)
    {
        _ennemyRef = ennemyRef;
        _agent = ennemyRef.agent;
        _player = ennemyRef.player;
    }

    public void OnEnter()
    {
        _agent.isStopped = true; // Arrête l'agent pour manipuler la position manuellement
        _agent.ResetPath(); // Vide le chemin actuel
        _agent.updatePosition = false; // Désactive la synchro auto de la position
        _agent.updateRotation = false; // Désactive la synchro auto de la rotation
    }

    public void Tick()
    {
        DetectWall(); 

        Vector3 normalToUse = _wallDetected ? _wallNormal : _lastWallNormal;

        if (_wallDetected)
        {
            // Calcule la direction vers le joueur
            Vector3 versJoueur = (_player.position - _agent.transform.position).normalized;

            // Projette le mouvement sur le plan du mur pour bouger dans toutes les directions
            Vector3 directionMur = Vector3.ProjectOnPlane(versJoueur, normalToUse).normalized;

            // Déplace le transform le long du mur selon la vitesse de crawl
            _agent.transform.position += directionMur * _ennemyRef.vitesseCrawl * Time.deltaTime;

            // Calcule la position de collage avec un décalage de 1 unité par rapport au mur
            Vector3 pointColle = _wallHit.point + normalToUse * 1f;
            
            // Lissage du mouvement vers la position de collage
            _agent.transform.position = Vector3.Lerp(_agent.transform.position, pointColle, Time.deltaTime * 10f);

            if (directionMur != Vector3.zero)
            {
                // Oriente le haut du monstre vers la normale du mur
                Quaternion targetRot = Quaternion.LookRotation(directionMur, normalToUse);
                // Lissage de la rotation pour suivre le mur
                _agent.transform.rotation = Quaternion.Slerp(_agent.transform.rotation, targetRot, Time.deltaTime * 8f);
            }
        }
        else if (_wasWallDetected)
        {
            // Continue tout droit si le mur est perdu temporairement
            _agent.transform.position += _agent.transform.forward * _ennemyRef.vitesseCrawl * Time.deltaTime;
        }
    }

    public void OnExit()
    {
        // Téléporte l'agent interne à la position du transform pour éviter le bug de retour arrière
        _agent.Warp(_agent.transform.position); 

        _agent.isStopped = false; // Réactive l'agent
        _agent.updatePosition = true; // Réactive la synchro de position
        _agent.updateRotation = true; // Réactive la synchro de rotation
    }

        public Color GizmoColor()
    {
        return Color.green;
    }

    // Méthode pour détecter les murs autour de l'ennemi
    private void DetectWall()
    {
        Vector3 origin = _agent.transform.position; 
        
        // Vérifie s'il y a un mur devant, à droite ou à gauche selon la distance de détection et le layer mur
        bool hit = Physics.Raycast(origin, _agent.transform.forward, out _wallHit, _ennemyRef.wallCheckDistance, _ennemyRef.murLayer) ||
                   Physics.Raycast(origin, _agent.transform.right, out _wallHit, _ennemyRef.wallCheckDistance, _ennemyRef.murLayer) ||
                   Physics.Raycast(origin, -_agent.transform.right, out _wallHit, _ennemyRef.wallCheckDistance, _ennemyRef.murLayer);

        if (hit) // Si un mur est détecté
        {
            _wallDetected = true;
            _wallNormal = _wallHit.normal; // Stocke la normale du mur touché
            _lastWallNormal = _wallNormal; // Mémorise la dernière normale
            _wasWallDetected = true; // Indique qu'on a détecté un mur au moins une fois
        }
        else
        {
            _wallDetected = false;
        }
    }
}