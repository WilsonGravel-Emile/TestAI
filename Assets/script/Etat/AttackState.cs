using UnityEngine;
using System.Collections;
using UnityEngine.AI;


/// <summary>
/// Etat d'attaque de l'ennemi
/// L'ennemi attaque le joueur après avoir sauter ou si le joueur est à portée
///     -  Si le joueur est à portée, l'ennemi attaque
///     - l'ennemi peut aussi attaquer après un saut, même si le joueur n'est pas à portée, pour rendre le combo plus fluide
/// 
/// </summary>
public class AttackState : IState
{
    // Les références nécessaires pour l'état d'attaque
    private EnnemyReferences _ennemyRef;
    private Transform player;
    private Vector3 startPosition;
    private  Vector3 targetPosition;
    private int maxCombo;
    private int currentDash;
    private float timePassed;
    public bool IsComplete { get; private set; }


    // Constructeur pour initialiser les références
    public AttackState(EnnemyReferences ennemyRef)
    {
        _ennemyRef = ennemyRef; // Initialisation de la référence à l'ennemi
        player = ennemyRef.player; // Initialisation de la référence au joueur
        maxCombo = ennemyRef.maxCombo;

        Debug.Log("Miann attaque");
    }

    public void OnEnter()
    {
        _ennemyRef.agent.isStopped = true; // Arrête le NavMeshAgent pour l'attaque
        _ennemyRef.agent.enabled = false; // Désactive le NavMeshAgent pour permettre un contrôle total de l'ennemi pendant l'attaque
        currentDash = 0;
        //Animation d'attaque
        Debug.Log("L'ennemi attaque le joueur !");
        startPosition = _ennemyRef.transform.position;

        Vector3 direction = (player.position - startPosition).normalized;
        targetPosition = player.position + direction * 1f; // La position cible est légèrement devant le joueur pour éviter que l'ennemi ne se téléporte directement sur lui
        _ennemyRef.attackCooldown = _ennemyRef.attackCooldownDuration; // réinitialise le cooldown de l'attaque après l'attaque

        timePassed = 0;
        IsComplete = false;
    }
    public void Tick()
    {
        // Attaque de base : Dash vers la dernière position du joueur
        // 1 - Premiere Attaque : Dash vers la position du joueur
        // 2 - Extra Attaque : Si le dash est plus petit que la max combo continue de dash vers le joueur
        // Cela permet de mieux controller l'animation du personnage attaché

        timePassed += Time.deltaTime;
        if (currentDash < maxCombo)
        {
            if (timePassed < _ennemyRef.attackDuration)
            {
                // float d'accélération
                float acceleration = timePassed / _ennemyRef.attackDuration; // Accélère progressivement pendant l'attaque
                acceleration = Mathf.Pow(acceleration, 2f); // Courbe d'accélération pour un effet plus fluide
                // Dash vers la position vers le joueur
                // Lerp pour un mouvement plus fluide vers la cible, ajout de +1 pour qu'il dépassse légèrement la position du joueur et éviter les problèmes de collision ou de synchronisation avec le NavMeshAgent
                _ennemyRef.transform.position = Vector3.Lerp(startPosition, targetPosition, acceleration);
                return;
            }
            else
            {
                Vector3 direction = (targetPosition - startPosition).normalized;
                // Après le dash, Miann se réoriente vers le joueur pour le prochain dash
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _ennemyRef.transform.rotation = Quaternion.Slerp(_ennemyRef.transform.rotation, targetRotation, Time.deltaTime * 8f);
                // Fin du segment : Passe au suivant
                currentDash++;
                startPosition = _ennemyRef.transform.position;  // Nouvelle position de départ
                targetPosition = player.position + direction * 1f;  // Met à jour vers la dernière position du joueur
                timePassed = 0;  // Reset timer pour le prochain segment
                Debug.Log("Dash " + currentDash + " vers " + targetPosition);
            }
        }
        else
        {
            IsComplete = true;
        }


    }
    public Color GizmoColor()
    {
        return Gizmos.color = new Color(255, 0, 0, 0.5f); // rouge semi-transparent
    }
    public void OnExit()
    {
        _ennemyRef.agent.enabled = true; // Réactive le NavMeshAgent après l'attaque
        _ennemyRef.agent.isStopped = false; // Permet au NavMeshAgent de reprendre le contrôle du mouvement
    }
}