using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Fait par Emile Lucas Wilson
/// Se script créer un delay pour l'ennemi:
/// Delay Attaque
/// Delay le saut
/// Delay les projectiles 
/// Delay l'attaque de saut
/// et plus
///  </summary>
public class EnnemyState_Delay : IState
{
    // Référence et Variable
    private float waitForSeconds;
    private float deadline;
    //Constructeur
    public EnnemyState_Delay(float waitForSeconds)
    {
        this.waitForSeconds = waitForSeconds;
    }
    public void OnEnter(){}

    public void Tick(){}
    public void OnExit(){}

    public Color GizmoColor()
    {
        return Gizmos.color = new Color(185, 185, 185, 0); // gris pale
    }
}