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
    private Transform transform;
    private NavMeshAgent agent;

    private Vector3 startPosition; // va être Miann : = transform.positionm 
    private Vector3 targetposition; // va être le joueur
    private float timePassed;


    //Constructeur
    public JumpAttackState (EnnemyReferences ennemyRef)
    {
        player = ennemyRef.player;
        agent = ennemyRef.agent;
        ennemyRef.jumpDuration = 0f;
        transform = ennemyRef.transform;
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
    public void Tick(){}

    public Color GizmoColor()
    {
        return Color.cyan;
    }
    //Réactive le NavMesh et update sa position avec warp
    public void OnExit(){}
}