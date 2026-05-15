using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Fait par Emile
/// Cerveau de l'ennemi Avarice :
/// - Patrouille entre des points de passage
/// - Poursuit le joueur s'il est détecté
/// - Attaque au corps à corps si le joueur est à portée d'attaque
/// - Sprint vers le joueur après l'avoir détecté
/// - Utilise des délais après les attaques pour éviter les comportements bizarres
/// - Grab le joueur et le jette contre les murs pour infliger des dégâts supplémentaires
/// - Peut être étourdi par le joueur, ce qui l'empêche de bouger
/// </summary>
public class Cerveau_Avarice : MonoBehaviour
{
 // Prend le script du FSM et les refs de MIANn
    private StateMachine stateMachine; // script 
    private EnnemyReferences ennemyRef; // prends les pref de l'ennemy : difficile ou Facile
    private NavMeshAgent agentAvarice; 
    private float distance; 
    private bool estAuSol;
    void Awake()
    {
        // Recupere les composant du gameObject
        ennemyRef = GetComponent<EnnemyReferences>();
        agentAvarice = GetComponent<NavMeshAgent>();
        //Initialisation de la FSM
        stateMachine = new StateMachine();
        //1. Déclaration des états spécifique à Avarice
        var patrol = new PatrolState(agentAvarice, ennemyRef.waypoints); // Etat de base : patrouille
        var chase = new ChaseState(ennemyRef);

        // TODO: ChargeState
        // Avarice sprint vers le Joueur
        // Cela fait des dégats et recul le joueur
        // Dans cette état:
        //      -> Avarice ne peut pas tourner (sprint sans tour)
        //          -> Avarice gagne 1 point de charge
        //      -> A 3 SUPER CHARGE (super vite et fait aproxitevement 75% pv)
        //          -> Gagne 1 point de grab
        var charge = new ChargeState(ennemyRef);
        // TODO: GRABSTATE
        // Avarice prend le joueur et la lance vers un murs
        // Seulement actif après 3 points de grab
        // 

        // ACTIVE QUAND FINI LE SCRIPT 
        // TODO: DeathState peut seulement être activer quand les scripts vont être intégrer dans la scène principale
        // var death = new DeathState(ennemyRef.player, ennemyRef.degats); // Etat de mort : joue une animation de mort et désactive l'ennemi
        
        // TODO: StunnedState :
        // - Sert à arrêter le Agent et aussi empêche d'attaquer, patrol, chase, jump et plus
        // - Seulement activer par un damageThresold qui revient tanquilement à zero
        //      par exemple : plus au que 3 déclanche le damage thresold
        //      -> Ennemy prend plus de dégats
        //      -> Ennemy prend aucun damageThresold ( empêche de spam stun)
        //      -> Si le joueur utilise le fusil à pompe => RAGDOLLL LL L  L L
        //      -> En dessous de trois reviens à la normal
        // var stunned = new StunnedState(agentAvarice); // Etat d'étourdissement : joue une animation d'étourdissement et empêche l'ennemi de bouger

        var attack = new AttackState(ennemyRef); // Etat d'attaque au corps à corps : utilise le NavMeshAgent pour se déplacer et attaquer le joueur
        //2. Définition des transitions
        // elle permet d'utiliser At et AtAny pour ajouter des transitions entre les états
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition); // Fonction pour ajouter une transition
        void AtAny(IState to, Func<bool> condition) => stateMachine.AddAnyTransition (to, condition); // Fonction pour ajouter une transition qui peut se produire à n'importe quel moment

        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        //  DÉPLACEMENT =================================================================
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        // Patrol → Chase
        At(patrol, chase, () => distance < ennemyRef.detectionRange); // Si le joueur est à moins de la distance de détection, passer de la patrouille à la poursuite
        // Chase → Patrol (perte du joueur)
        At(chase, patrol, () => distance > ennemyRef.detectionRange); // Si le joueur est à plus de la distance de perte, retourner à la patrouille
        //Enlève quand fini
        At(chase, patrol, () => ennemyRef.player == null); // Si le joueur n'est plus trouvé, retourner à la patrouille

        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        //  ATTAQUES (zones exclusives) =================================================================
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        // Chase → Melee
        At(chase, attack, () =>
            distance < ennemyRef.attackRange && ennemyRef.attackCooldown <= 0); // Si le joueur est à moins de la distance d'attaque au corps à corps et à plus de la distance d'attaque de saut, passer à l'état d'attaque au corps à corps
        // Chase → Charge
        At(chase, charge, () =>
            distance < ennemyRef.chargeRange && ennemyRef.chargeCooldown <= 0); // Si le joueur est à moins de la distance de charge et que le cooldown de charge est écoulé, passer à l'état de charge
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        // RETOURS =================================================================
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> 

        // Après melee → retour chase
        At(attack, chase, () => distance >= ennemyRef.attackRange || attack.IsComplete); // Si le joueur est à plus de la distance d'attaque au corps à corps après une attaque, retourner à l'état de poursuite

        // Après charge → retour chase
        At(charge, chase, () => charge.IsComplete); // Si le joueur est à plus de la distance de charge après un charge, retourner à l'état de poursuite

        /*
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        //  GLOBAL =================================================================
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        // Stun
        AtAny(stunned, () => ennemyRef.isStunned); // Si l'ennemi est étourdi, passer à l'état d'étourdissement

        // Sortie du stun
        At(stunned, chase, () =>
            !ennemyRef.isStunned &&
            distance < ennemyRef.detectionRange); // Si le joueur est à moins de la distance de détection après le stun, retourner à la poursuite

        At(stunned, patrol, () =>
            !ennemyRef.isStunned &&
            distance >= ennemyRef.detectionRange); // Si le joueur est à plus de la distance de détection après le stun, retourner à la patrouille

        AtAny(death, () => ennemyRef.isDead); // Si l'ennemi est mort, passer à l'état de mort
        */
        // 3. État de départ
        stateMachine.SetState(patrol); // L'état de départ est la patrouille, mais tu peux le changer si tu veux que Miann commence dans un autre état
    }
    void Update()
    {
        if (ennemyRef.player != null) // Si le joueur n'est pas trouvé, on considère qu'il est hors de portée
        {
            distance = Vector3.Distance(transform.position, ennemyRef.player.position); // Calcule la distance entre l'ennemi et le joueur
        }
        else
        {
            distance = float.MaxValue;
        }
        estAuSol = EstAuSol(); // Vérifie si l'ennemi est au sol à chaque frame
        CooldownUpdate(); // Met à jour le cooldown du saut à chaque frame
        stateMachine.Tick(); // Appelle la méthode Tick du FSM à chaque frame

    }
    private void CooldownUpdate()
    {
        if (ennemyRef.attackCooldown > 0)
        {
            ennemyRef.attackCooldown -= Time.deltaTime; // Réduit le cooldown de l'attaque au corps à corps au fil du temps
        }
        if (ennemyRef.chargeCooldown > 0)
        {
            ennemyRef.chargeCooldown -= Time.deltaTime;
        }
    }
    private bool EstAuSol() => Physics.Raycast(transform.position, Vector3.down, 1.1f);
    
    private void OnDrawGizmos()
    {
        if (stateMachine != null)
        {
            // Affiche les différentes portées de détection et d'attaque de l'ennemi dans la scène pour faciliter le debug
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ennemyRef.detectionRange); // Affiche la portée de détection
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ennemyRef.attackRange); // Affiche la portée d'attaque au corps à corp
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, ennemyRef.chargeRange); // Affiche la portée de charge
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 2f, 0.5f);
        }
    }
}
