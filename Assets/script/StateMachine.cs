using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using Object = System.Object;
using System.Runtime.CompilerServices;

/// <summary>
/// Écrit par : Emile Lucas Wilson
/// Inspiré de : https://youtu.be/V75hgcsCGOM?si=nJxayvdoLrVPSUsa
/// Ce script est un FSM :
/// Un FSM est un système de gestion d'état.
/// Il sert
/// 1. IA de : 
///     Miann : Avarice : boss : les animations :
/// 2. Part de la SM est :
///     Etat et Transtions
/// 3. Etats :
///     Tick - pourquoi c'est pas Update (): // Objet ou Var non optimal pour le Update
///     OnEnter/OnExit
/// 4. Transitions
///     Sépare les états pour qu'il soit réutiliser
///     Transitions Faciles à utilisé
/// 
/// </summary>
public class StateMachine
{
    //1. On recupere les ref, var et plus
    private IState _currentState; // L'état actuel du FSM
    // Un dictionnaire est une representation de Cle et valeur
    private Dictionary<Type, List<Transition>> _transition = new Dictionary<Type, List<Transition>>(); // Dictionnaire qui store les etats de la transition
    private List<Transition> _currentTransitions = new List<Transition>(); // Liste des transitions actuelles pour l'état actuel
    private List<Transition> _anyTransitions = new List<Transition>(); // Liste des transitions qui peuvent se produire à n'importe quel moment

    private static List<Transition> EmptyTransitions = new List<Transition>(); // Liste vide pour les états sans transitions

    //2. On cree les methodes pour le FSM
    // Méthode qui remplace update : elle permet de récupérer et update la transition
    public void Tick()
    {
        var transition = GetTransition();  // On récupère la transition actuelle
        if (transition != null) // Si il n'y a pas de transition, on continue dans l'état actuel
            SetState(transition.To); // Si une transition est trouvée, on change d'état
        _currentState?.Tick(); // On appelle la méthode Tick de l'état actuel
    }

    // Methode qui defini l'etat
    public void SetState(IState state)
    {
        if (state == _currentState) // si l'état est pareil retourne 
            return;
        _currentState?.OnExit(); 
        _currentState = state; 

        _transition.TryGetValue(_currentState.GetType(), out _currentTransitions); // On récupère les transitions pour le nouvel état
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions; // Si il n'y a pas de transition pour cet état, on utilise la liste vide

        _currentState.OnEnter(); // On appelle la méthode OnEnter de l'état actuel
    }
    

    // Methode ajoute une regle / transition : " Si je suis dans l'état A et que la condition est vraie, va vers B"
    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        if (!_transition.TryGetValue(from.GetType(), out var transitions)) // Si il n'y a pas de transition pour cet état, on en crée une nouvelle
        {
            transitions = new List<Transition>();
            _transition[from.GetType()] = transitions;
        }

        transitions.Add(new Transition(to, predicate)); // On ajoute la transition à la liste des transitions pour cet état
    }

    // Methode ajoute une regle / transition : " Si la condition est vraie, va vers B"
    public void AddAnyTransition(IState to, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(to, predicate)); // On ajoute la transition à la liste des transitions qui peuvent se produire à n'importe quel moment
    }

    // classe qui defini la transition : elle contient l'état vers lequel on veut aller et la condition pour y aller
    private class Transition
    {
        public Func<bool> Condition { get; } // La condition pour que la transition se produise
        public IState To { get; } // L'état vers lequel on veut aller
        public Transition(IState to, Func<bool> condition) // Constructeur de la classe Transition
        {
            To = to; // On définit l'état vers lequel on veut aller
            Condition = condition; // On définit la condition pour que la transition se produise
        }
    }

    // Cette function permet de récupérer la couleur d'un state en utilisation
    public Color GetGizmoColor()
    {
        if (_currentState != null)
        {
            return _currentState.GizmoColor();
        }
        return Color.grey;
    }

    // Methode qui récupère la transition actuelle : elle vérifie d'abord les transitions qui peuvent se produire à n'importe quel moment, puis les transitions pour l'état actuel
    private Transition GetTransition()
    {
        foreach (var transition in _anyTransitions) // On vérifie les transitions qui peuvent se produire à n'importe quel moment
            if (transition.Condition()) // Si la condition pour que la transition se produise est vraie
                return transition; // On retourne la transition

        foreach (var transition in _currentTransitions) // On vérifie les transitions pour l'état actuel
            if (transition.Condition()) // Si la condition pour que la transition se produise est vraie
                return transition; // On retourne la transition

        return null; // Si aucune transition n'est trouvée, on retourne null
    }
}
