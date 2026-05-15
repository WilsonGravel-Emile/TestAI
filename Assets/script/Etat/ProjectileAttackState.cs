using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System;
using UnityEngine.AI;

/// <summary>
/// Etat d'attaque à distance de l'ennemi
/// L'ennemi attaque le joueur à distance en lançant un projectile
///     -  Si le joueur est à portée d'attaque à distance, l'ennemi lance un projectile vers le joueur
/// </summary>
public class ProjectileAttackState : IState
{
    // Les références nécessaires pour l'état d'attaque à distance
    private EnnemyReferences _ennemyRef;

    private MonoBehaviour routineHost; // Référence à un MonoBehaviour pour lancer des coroutines
    
    public bool IsComplete { get; private set; } // Propriété pour indiquer si l'état est terminé
    // Constructeur pour initialiser les références
    public ProjectileAttackState(EnnemyReferences ennemyRef)
    {
        _ennemyRef = ennemyRef; // Initialisation de la référence à l'ennemi
        routineHost = ennemyRef;
        Debug.Log("Miann attaque à distance");
    }

    public void OnEnter()
    {
        _ennemyRef.projectileCooldown = _ennemyRef.projectileCooldownDuration; // réinitialise le cooldown de l'attaque à distance après l'attaque
        // Animation de lancement de projectile

        // Logique de lancement de projectile
        IsComplete = false; // Réinitialise l'état de lancement du projectile 
        Debug.Log("L'ennemi lance un projectile vers le joueur !"); 
        LaunchProjectile();
    }
    
    private void LaunchProjectile()
    {
        // Logique de lancement de projectile
        // 1 - Instancier le projectile
        // 2 - Diriger le projectile vers la position du joueur
        // 3 - Gérer les dégâts et la destruction du projectile

        if (_ennemyRef.player != null)
        {
            // Logique d'instanciation et de lancement du projectile
            // Par exemple :
            Vector3 direction = (_ennemyRef.player.position - _ennemyRef.transform.position).normalized;
            // Instancier le projectile à la position de l'ennemi et le faire avancer dans la direction du joueur
            if (Physics.Raycast(_ennemyRef.transform.position, direction, out RaycastHit hit, _ennemyRef.projectileRange))
            {
                Debug.DrawRay(_ennemyRef.transform.position, direction * _ennemyRef.projectileRange, Color.green, 1f); // Visualisation du rayon de tir

                // Instancier le projectile et lui donner une direction
                TrailRenderer trail = GameObject.Instantiate(_ennemyRef.projectilePrefab, _ennemyRef.firePoint.position, Quaternion.LookRotation(direction)).GetComponent<TrailRenderer>();
                routineHost.StartCoroutine(SpawnFireTrail(trail, hit));


                if (hit.collider.CompareTag("Player"))
                {
                    // Appliquer des dégâts au joueur
                    Debug.Log("Le projectile a touché le joueur !");
                }
                IsComplete = true; // Indique que le projectile a été lancé même s'il n'a pas touché le joueur
                Debug.Log("Projectile lancé vers le joueur !");
            }
        }
    }
    // Chaque frame update la rotation de Miann au joueur
    // - Regarde toujours le joueur grace à Quaternion et lookPos (la position du joueur - Miann)
    public void Tick()
    {
        // vise vers le joueur
        Vector3 lookPos = _ennemyRef.player.position - _ennemyRef.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        _ennemyRef.transform.rotation = Quaternion.Slerp(_ennemyRef.transform.rotation, rotation, Time.deltaTime * 0.8f); // Ajustez la vitesse de rotation selon les besoins
            

    }

    // Coroutine pour gérer la trajectoire du projectile et son impact
    // - elle permet de créer un trail grace au trail renderer
    // - elle permet le prefab d'être modifier (vitesse / durer)
    // - pas optimal mais pas le temps de faire quelque chose d'autre
    private IEnumerator  SpawnFireTrail(TrailRenderer trail, RaycastHit hit)
    {
        // Paramètre de la projectile
        float speed = _ennemyRef.projectileSpeed; 
        float maxLifetime = 2.0f; // Sécurité pour détruire le projectile s'il ne touche rien
        float timer = 0f;

        // On ne vise plus le hit.point, on utilise la direction pure
        Vector3 direction = trail.transform.forward;

        while (timer < maxLifetime) // si, le timer est plus petit que lavie maximale de la projectile
        {
            // Avance de façon constante
            trail.transform.position += direction * speed * Time.deltaTime;
            
            // Detection de collision en temps réel (plus précis pour l'esquive)
            if (Physics.Raycast(trail.transform.position, direction, out RaycastHit frameHit, speed * Time.deltaTime))
            {
                trail.transform.position = frameHit.point;
                
                if (frameHit.collider.CompareTag("Player"))
                {
                    Debug.Log("Touché pendant le vol !");
                    // Applique les dégâts ici
                }
                break; // Sort de la boucle car on a touché quelque chose
            }

            timer += Time.deltaTime; // Update le timer
            yield return null; 
        }

        GameObject.Destroy(trail.gameObject, trail.time); 
    }
    public Color GizmoColor()
    {
        return Gizmos.color = new Color(0, 255, 0, 0.5f); // vert lime semi-transparent pour indiquer la zone d'attaque à distance
    }

    public void OnExit()
    {
        // Logique de fin d'attaque à distance (si nécessaire)
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile"); // Assurez-vous que les projectiles ont le tag "Projectile"
    }
}