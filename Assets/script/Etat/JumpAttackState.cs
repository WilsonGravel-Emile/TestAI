using UnityEngine;
using UnityEngine.AI;
using System;


///<summary>
/// Fait par Emile Lucas Wilson
/// Ce script permet Miann de sauter vers le joueur pour initialiser le combo d'attaque
/// 1. Jump Attack 
/// 2. AttackState pour le reste du combo
/// Exclusif à Miann
/// </summary>
// NOTE PEUT_ÊTRE CRÉER UN ÉTAT NEUTRE 
public class JumpAttackState : IState
{
    // Ref
    private Transform player;
    private EnnemyReferences _ennemyRef;
    private Transform transform;
    private NavMeshAgent agent;

    private Vector3 startPosition; // va être Miann : = transform.positionm 
    private Vector3 targetposition; // va être le joueur
    private float timePassed;
    public bool IsComplete { get; private set; } // Propriété pour indiquer si l'état est terminé


    //Constructeur
    public JumpAttackState (EnnemyReferences ennemyRef)
    {
        player = ennemyRef.player;
        agent = ennemyRef.agent;
        ennemyRef.jumpDuration = 0.8f;
        transform = ennemyRef.transform;
        _ennemyRef = ennemyRef;
        // pas besoin de déclarer un variable transform peut utilise ennemyRef.Transform et les paramètres de sauts, jumpDuration, jumpHeight, jumpDistance
    }

    public void OnEnter()
    {
        // arrête le Agent
        agent.isStopped = true;
        agent.enabled = false; // coupe la navigation pour voler librement
        // nous voulons seulement la position au début de l'état
        startPosition = transform.position;
        targetposition = player.position; // Le joueur

        timePassed = 0;
        IsComplete = false;
        _ennemyRef.jumpCooldown = _ennemyRef.jumpCooldownDuration; // réinitialise le cooldown du saut après l'attaque

        Debug.Log("!! Miann commence son starter : jumpAttack !!");
    }

    /** Dans le tick :
        - Faut un Vector 3 qui capture la position courrente graçe au Lerp (a => b, progrès)
        - Une variable qui enregistre le progrès du saut et ajoute une condition
            - Un float yOffset avec un Math sin (permet de faire un arche)
        Mathf.PI est une constante dans Unity représentant le ratio entre la circonférence d'un cercle et son diamètre, 
            - alors le progrès fois pi fois la jump height de vrait marcher
            - enregistre la position y
            - Oriente Miann avec un Vector 3 direction 
            - (targetPosition - transform.position): This creates a new Vector3 that represents the distance and direction from the current object to the target.
                It calculates the displacement vector,
            -.normalized: This property takes that resulting vector and scales it to a length of \(1\) (a "unit vector"). 
                It removes the distance information, leaving only the pure direction information.
            avec un Quaternion devrait marcher

    */
    public void Tick()
    {
        timePassed += Time.deltaTime; // Incrémente le temps écoulé depuis le début de l'état
        float progress = timePassed / _ennemyRef.jumpDuration; // Calcule le progrès du saut  

        if(progress < 1f)
        {

            // Calcule la position courante en interpolant entre la position de départ et la position cible en fonction du progrès
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetposition, progress);// Calcule la position courante en interpolant entre la position de départ et la position cible en fonction du progrès  
            float yOffset = Mathf.Sin(progress * Mathf.PI) * _ennemyRef.jumpHeight; // Calcule l'offset vertical pour créer une trajectoire en arche
            currentPosition.y += yOffset;
            transform.position = currentPosition;

            // oriente le monstre vers la cible
            Vector3 direction = (targetposition - transform.position).normalized; // Calcule la direction vers la cible
            if(direction != Vector3.zero) // Vérifie que la direction n'est pas un vecteur nul pour éviter les erreurs de rotation
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction); // Calcule la rotation cible pour faire face à la cible
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Oriente progressivement le monstre vers la cible
            }
        }
        else
        {
            IsComplete = true;
        }
    }

    public Color GizmoColor()
    {
        return Color.cyan;
    }
    //Réactive le NavMesh et update sa position avec warp
    public void OnExit()
    {
        agent.enabled = true; // réactive la navigation
        agent.Warp(transform.position); // met à jour la position du NavMeshAgent pour correspondre à la position actuelle de Miann
        agent.isStopped = false; // réactive le mouvement du NavMeshAgent
    }
}
