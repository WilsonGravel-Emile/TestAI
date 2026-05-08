using System;
using UnityEngine;
using UnityEngine.AI;

public class DeathState : IState
{

    public void OnEnter(){}

    public void Tick(){}
    public void OnExit(){}
    public Color GizmoColor()
    {
        return Color.black;
    }
}