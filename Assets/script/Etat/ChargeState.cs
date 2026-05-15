using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Fait par Emile Lucas Wilson
/// Cet état permet Avarice de charger vers le joueur
/// Dans cet état:
///     - Avarice va plus vite
///     - Avarice poursuit la derniere position du joueur (comme attaque)
///     - Avarice gagne des grabPoints par le nombre de chrage fait
///     - si le temps super charge
/// </summary>

public class ChargeState : IState
{
    //Ref
    private Transform _player;
    private EnnemyReferences _ennemyRef;
    private Vector3 startPosition;
    private  Vector3 targetPosition;
    private int chargePoint;
    private float timePassed;
    public bool IsComplete { get; private set; }


    //Constructeur
    public ChargeState(EnnemyReferences ennemyRef)
    {   
        _ennemyRef = ennemyRef;
        _player = ennemyRef.player;
    }
    
    public void OnEnter()
    {
        _ennemyRef.agent.isStopped = true; // Arrête le NavMeshAgent pour le Charge
        _ennemyRef.agent.enabled = false; // Désactive le NavMeshAgent pour permettre un contrôle total de l'ennemi pendant le charge
        //Animation Charge
        Debug.Log("Avarice charge le joueur !");
        startPosition = _ennemyRef.transform.position;
        Vector3 direction = (_player.position - startPosition).normalized;
        targetPosition = _player.position + direction * 1f; // La position cible est légèrement devant le joueur pour éviter que l'ennemi ne se téléporte directement sur lui
        _ennemyRef.chargeCooldown = _ennemyRef.chargeCooldownDuration; // réinitialise le cooldown de l'attaque après le charg
        timePassed = 0;
        IsComplete = false;
    }
    public void Tick()
    {
        // Comme attaque charge est un état qui joue avec le Vector de l'ennemy
        // Elle est plus vite et a une distance plus longue que l'attaque
        //      - mais elle ne peut pas être enchaîner : GROS COOLDOWN

        timePassed += Time.deltaTime;
        if (timePassed < _ennemyRef.attackDuration)
        {
            float acceleration = timePassed / _ennemyRef.chargeDuration; // Accélération progressive
            acceleration = Mathf.Pow(acceleration, 2f); // Courbe d'accélération (plus rapide à la fin)
            if (chargePoint == 3)
            {
                _ennemyRef.transform.position = Vector3.Lerp(startPosition, targetPosition, acceleration * 1.5f); // Charge super rapide pour le super charge
            }
            else{
            // Charge vers la dernière position du joueur
            _ennemyRef.transform.position = Vector3.Lerp(startPosition, targetPosition, acceleration);
            }
        }
        else
        {
            IsComplete = true;
            // Après le charge, Avarice gagne un point de charge et de grab
            chargePoint++; // ajoute des points de charge
            _ennemyRef.grabPoint++;  // ajoute des points de grab

        }


    }
    public void OnExit()
    {
        _ennemyRef.agent.enabled = true;
        _ennemyRef.agent.isStopped = false; // Réactive le NavMeshAgent après le charge
    }
    
    public Color GizmoColor()
    {
        return Color.green;
    }
}