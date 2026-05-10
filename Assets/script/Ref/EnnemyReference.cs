using UnityEngine;
using UnityEngine.AI;
public class EnnemyReferences : MonoBehaviour
{
    [Header("reférences à Ennemi")]
    public NavMeshAgent agent;
    [Header("Cible")]
    public Transform player;

    [Header("Déplacements")]
    public Transform[] waypoints;
    public float vitesseBase;
    public float vitesseCrawl;
    public float jumpDistance; // placeholder, à ajuster selon les besoins pour le saut de l'ennemi
    public float jumpDuration; // placeholder, à ajuster selon les besoins pour la durée du saut de l'ennemi
    public float jumpCooldown; // temps de recharge courant du saut
    public float jumpCooldownDuration = 2f; // durée de recharge après une attaque de saut
    public float jumpHeight; // placeholder, à ajuster selon les besoins pour la hauteur du saut de l'ennemi
    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Distances")]
    public float detectionRange = 10f;
    public float attackRange = 2f; // placeholder
    public float jumpAttackRange = 4f;
    public float projectileRange = 8f;
    public float lostRange = 12f;
    [Header("ParametreMur")]
    public float stickForce;
    public float wallCheckDistance = 3f;


    [Header("Combat stats")]
    public float attackDamage = 10f; //placeholder
    public float degats = 10f; // placeholder

    [Header("Etat")]
    public bool isStunned;
    public bool isDead;

    [Header("Layers")]
    public LayerMask murLayer;


}