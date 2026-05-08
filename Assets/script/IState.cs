using UnityEngine;
/**
Ce script est interface.
Elle definit les regles : chaque état avoir ces trois methodes

*/
public interface IState
{
    void Tick(); // Execute a chaque frame REMPLACE UPDATE  
    void OnEnter(); // Execute quand on entre dans l'état
    void OnExit(); // Execute quand on sort de l'état
    Color GizmoColor(); 
}
