using UnityEngine;
using UnityEngine.AI;
public class EnnemyReferences : MonoBehaviour
{
    // TODO: !!! Si temps organise les ref mieux !!!
    [Header("reférences à Ennemi")]
    public NavMeshAgent agent;
    [Header("Cible")]
    public Transform player;
    
    // Gestion de la vitesse, les waypoints, plus
    [Header("Déplacements")]
    public Transform[] waypoints;
    [SerializeField, Tooltip ("Gestion Deplacement de Miann")]
    public float vitesseBase;
    [SerializeField, Tooltip ("Gestion Deplacement du Crawl de Miann")]
    public float vitesseCrawl;
    [SerializeField, Tooltip ("Gestion de la distance de saut Miann")]
    public float jumpDistance; // placeholder, à ajuster selon les besoins pour le saut de l'ennemi
    public float jumpDuration; // placeholder, à ajuster selon les besoins pour la durée du saut de l'ennemi
    public float jumpHeight; // placeholder, à ajuster selon les besoins pour la hauteur du saut de l'ennemi
    [SerializeField, Tooltip ("Gestion du charge d'avarice")]
    public float chargeDistance;
    public float chargeDuration;
    [Header("Combat")]
    [SerializeField, Tooltip ("Universel : Gère le nb d'attaque qu'un ennemi peut faire")]
    public int maxCombo;
    public int grabPoint;
    public GameObject projectilePrefab;
    public Transform firePoint;
    [SerializeField, Tooltip ("Réference au gameobjet à avarice grabPoint")]
    public Transform grabHoldPoint; // AVARICE SEULEMENT
    public float projectileSpeed; 
    public float attackDuration; // placeholder, à ajuster selon les besoins pour la durée de l'attaque de l'ennemi
    
    // Gere le cooldown 
    [Header("Cooldown")]
    public float attackCooldown;
    public float attackCooldownDuration = 1f;
    public float projectileCooldown;
    public float projectileCooldownDuration;
    // MIANN SEULEMENT
    public float jumpCooldown; // temps de recharge courant du saut
    public float jumpCooldownDuration = 2f; // durée de recharge après une attaque de saut

    // AVARICE SEULEMENT
    public float grabCooldown; 
    public float grabCooldownDuration;
    // Gère la distance d'une attaque, detection, projectile et plus
    // Elle permet de reglementer les transition et sert comme principale condition
    public float chargeCooldown;
    public float chargeCooldownDuration;
    [Header("Distances")]
    public float detectionRange = 10f;
    public float attackRange = 2f; // placeholder

    // MIANN SEULEMENT
    public float chargeRange;
    public float jumpAttackRange;
    public float projectileRange;
    [Header("ParametreMur")]
    public float stickForce;
    public float wallCheckDistance = 3f;

    // Stats de l'ennemi : modifiable dans le futur 
    [Header("Combat stats")]
    public float attackDamage = 10f; //placeholder
    public float degats = 10f; // placeholder
    [Header("Etat")]
    public bool isStunned;
    public bool isDead;
    public bool estLancer;
    [Header("Layers")]
    public LayerMask murLayer;


}