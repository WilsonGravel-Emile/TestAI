using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Écrit par : Emile Lucas Wilson
/// Inspiré de : https://youtu.be/V75hgcsCGOM?si=nJxayvdoLrVPSUsa
/// Ce script est un état de poursuite pour une IA.
/// Il implémente l'interface IState, ce qui signifie qu'il doit définir les méthodes Tick, OnEnter et OnExit.
/// Dans cet état, l'IA poursuivra une cible en utilisant un NavMeshAgent pour se déplacer vers la position de la cible.
/// </summary>
public class ChaseState : IState
{
    private NavMeshAgent _agent; // Référence au NavMeshAgent de l'ennemi
    private Transform _player; // Référence à la cible à poursuivre

    // Constructeur pour initialiser les références
    public ChaseState(EnnemyReferences ennemyRef)
    {
        _agent = ennemyRef.agent;  // Initialisation de la référence au NavMeshAgent 
        _player = ennemyRef.player; // Initialisation de la référence à la cible

        Debug.Log("Miann commence la poursuite");
    }
    //Commence la transition
    public void OnEnter()
    {
        // assure que l'agent peut bouger
        _agent.isStopped = false;

    }

    public void Tick()
    {
        // si le joueur existe la destination devient le joueur
        if (_player != null)
        {
            _agent.SetDestination(_player.position);
        }
    }

    // sort de la transition
    public void OnExit()
    {
        // Quand on quitte l'etat, on arrete l'agent
        if (_agent.gameObject.activeSelf)
        {
            _agent.isStopped = true;
        }
    }

    // Coloration du Gizmo
    public Color GizmoColor()
    {
        return Color.blue;
    }

}
