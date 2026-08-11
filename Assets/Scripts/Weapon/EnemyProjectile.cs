using System;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage;
    [SerializeField]private int _projectileDestroyTime;

    public static event Action<int> OnEnemyProjectileHitPlayer;


    public void Update()
    {
        Destroy(this.gameObject, _projectileDestroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnEnemyProjectileHitPlayer?.Invoke(damage);
            Debug.Log("Collision work well");
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);

        }
    }
}