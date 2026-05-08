using System;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Ce script sert a differencier les etats de Miann de : Avarice, Anomalie, Boss
///  Fait par Emile Lucas Wilson
/// 1. idle = fait rien
/// 2. chase = poursuit le joueur
/// 3. WallRun = déplace sur les murs
/// 4. JumpAttack = saute d'un murs au joueur
/// 5. Attack = Facile a comprendre
/// 6. Projectile = lance une boule de feu
/// 7. meurt 
/// </summary>
public class Cerveau_Miann : MonoBehaviour
{
    // Prend le script du FSM et les refs de MIANn
    private StateMachine stateMachine; // script 
    private EnnemyReferences ennemyRef; // prends les pref de l'ennemy : difficile ou Facile
    private NavMeshAgent agentMiann; 
    private float distance; 
    private bool toucheMur;
    private bool estAuSol;
    private float wallRunContinuationTime = 0f; // Timer pour continuer wallrun après perte de mur
    void Awake()
    {
        // Recupere les composant du gameObject
        ennemyRef = GetComponent<EnnemyReferences>();
        agentMiann = GetComponent<NavMeshAgent>();

        //Initialisation de la FSM
        stateMachine = new StateMachine();

        //1. Déclaration des états spécifique à Miann
        var patrol = new PatrolState(agentMiann, ennemyRef.waypoints); // Etat de base : patrouille
        var chase = new ChaseState(ennemyRef);
        var wallRun = new WallRunState(ennemyRef); // Etat de déplacement sur les murs : utilise le NavMeshAgent pour se déplacer sur les murs
        var delayAfterJump = new EnnemyState_Delay(2f); // Etat de délai après une attaque de saut : empêche l'ennemi de bouger pendant un court instant après une attaque de saut pour éviter les comportements bizarres
        var delayAttack = new EnnemyState_Delay(1f); // Etat de délai après une attaque au corps à corps : empêche l'ennemi de bouger pendant un court instant après une attaque au corps à corps pour éviter les comportements bizarres
        var jumpAttack = new JumpAttackState(ennemyRef); // 
        //============================================================================================================
        

        //var jumpAttack = new JumpAttackState(ennemyRef); // Etat d'attaque : utilise le NavMeshAgent pour sauter sur le joueur
        // ACTIVE QUAND FINI LE SCRIPT
        // var projectileAttack = new ProjectileAttackState(ennemyRef.projectilePrefab, ennemyRef.firePoint, ennemyRef.player); // Etat d'attaque à distance : utilise le NavMeshAgent pour se déplacer et lancer des projectiles
        // ACTIVE QUAND FINI LE SCRIPT
        // var death = new DeathState(ennemyRef.player, ennemyRef.degats); // Etat de mort : joue une animation de mort et désactive l'ennemi
        // ACTIVE QUAND FINI LE SCRIPT
        // var stunned = new StunnedState(agentMiann); // Etat d'étourdissement : joue une animation d'étourdissement et empêche l'ennemi de bouger
        // ACTIVE QUAND FINI LE SCRIPT
        // var attack = new AttackState(ennemyRef.player, ennemyRef.attackDamage); // Etat d'attaque au corps à corps : utilise le NavMeshAgent pour se déplacer et attaquer le joueur
        //2. Définition des transitions
        // elle permet d'utiliser At et AtAny pour ajouter des transitions entre les états
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition); // Fonction pour ajouter une transition
        void AtAny(IState to, Func<bool> condition) => stateMachine.AddAnyTransition (to, condition); // Fonction pour ajouter une transition qui peut se produire à n'importe quel moment

        // =========================
        //  DÉPLACEMENT
        // =========================

        // Patrol → Chase
        At(patrol, chase, () => distance < ennemyRef.detectionRange); // Si le joueur est à moins de la distance de détection, passer de la patrouille à la poursuite

        // Chase → Patrol (perte du joueur)
        At(chase, patrol, () => distance > ennemyRef.lostRange); // Si le joueur est à plus de la distance de perte, retourner à la patrouille
        //Enlève quand fini
        At(chase, patrol, () => ennemyRef.player == null); // Si le joueur n'est plus trouvé, retourner à la patrouille
        // Chase → WallRun (seulement si pas déjà en train d’attaquer)
        At(chase, wallRun, () =>
            toucheMur &&
            distance > ennemyRef.attackRange); // Si l'ennemi touche un mur pendant la poursuite et que le joueur n'est pas à portée d'attaque, passer à l'état de déplacement sur les murs

        // WallRun → Chase (mur perdu)
        At(wallRun, chase, () => wallRunContinuationTime > 0.4f); // Si l'ennemi ne touche plus un mur pendant le déplacement sur les murs, retourner à l'état de poursuite
        /*
        // =========================
        //  ATTAQUES (zones exclusives)
        // =========================

        // WallRun → JumpAttack (logique principale du jump)
        At(wallRun, jumpAttack, () => distance < ennemyRef.jumpAttackRange); // Si le joueur est à moins de la distance d'attaque de saut, passer à l'état d'attaque de saut

        // Chase → JumpAttack (seulement si mur)
        At(chase, jumpAttack, () =>
            distance < ennemyRef.jumpAttackRange &&
            toucheMur); // Si le joueur est à moins de la distance d'attaque de saut et que l'ennemi touche un mur, passer à l'état d'attaque de saut

        // Chase → Melee
        At(chase, attack, () =>
            distance >= ennemyRef.jumpAttackRange &&
            distance < ennemyRef.attackRange); // Si le joueur est à moins de la distance d'attaque au corps à corps et à plus de la distance d'attaque de saut, passer à l'état d'attaque au corps à corps

        // Chase → Projectile
        At(chase, projectileAttack, () =>
            distance >= ennemyRef.attackRange &&
            distance < ennemyRef.projectileRange); // Si le joueur est à moins de la distance d'attaque à distance et à plus de la distance d'attaque au corps à corps, passer à l'état d'attaque à distance

        // =========================
        // RETOURS
        // =========================

        // Après jump → retour chase
        At(jumpAttack, chase, () => estAuSol); // Si l'ennemi est au sol après une attaque de saut, retourner à l'état de poursuite

         // Après melee → retour chase
        At(attack, chase, () => distance >= ennemyRef.attackRange); // Si le joueur est à plus de la distance d'attaque au corps à corps après une attaque, retourner à l'état de poursuite
        At(projectileAttack, chase, () => distance >= ennemyRef.projectileRange); // Si le joueur est à plus de la distance d'attaque à distance après une attaque, retourner à l'état de poursuite


        // =========================
        //  GLOBAL
        // =========================

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
        if (ennemyRef.player != null) // Si le joueur n'est pas trouvé, ne rien faire
        {
            distance = Vector3.Distance(transform.position, ennemyRef.player.position); // Calcule la distance entre l'ennemi et le joueur
        }
        distance = Vector3.Distance(transform.position, ennemyRef.player.position); // Calcule la distance entre l'ennemi et le joueur à chaque frame
        toucheMur = ToucheMur(); // Vérifie si l'ennemi touche un mur à chaque frame
        if (toucheMur)
        {
            wallRunContinuationTime = 0f; // Réinitialise le timer
        }
        else
        {
            wallRunContinuationTime += Time.deltaTime; // Incrémente le timer
        }
        estAuSol = EstAuSol(); // Vérifie si l'ennemi est au sol à chaque frame

        stateMachine.Tick(); // Appelle la méthode Tick du FSM à chaque frame

    }

    private bool ToucheMur() // Fonction pour vérifier si l'ennemi touche un mur
    {
        Vector3 origin = transform.position + Vector3.up *1f;
        // Implémentation de la détection de mur (exemple avec un raycast)
        return Physics.Raycast(origin, transform.forward, 2f, ennemyRef.murLayer) || // Vérifie s'il y a un mur à 1 unité devant l'ennemi
               Physics.Raycast(origin, transform.right, 2f, ennemyRef.murLayer) || // Vérifie s'il y a un mur à 1 unité à droite de l'ennemi
               Physics.Raycast(origin, -transform.right, 2f, ennemyRef.murLayer); // Vérifie s'il y a un mur à 1 unité à gauche de l'ennemi
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
            Gizmos.DrawWireSphere(transform.position, ennemyRef.attackRange); // Affiche la portée d'attaque au corps à corps
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ennemyRef.firePoint.position, ennemyRef.projectileRange); // Affiche la portée de l'attaque à distance
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, ennemyRef.wallCheckDistance);

            // Affiche Une sphere qui montre l'état actuel de l'ennemi :
            // - Jaune pour la patrouille
            // - Rouge pour la poursuite
            // - vert pour le déplacement sur les murs
            // - Bleu pour l'attaque de saut
            // - Magenta pour l'attaque au corps à corps
            // - Cyan pour l'attaque à distance
            // - Noir pour la mort
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 2f, 0.5f);
        }
    }
}