using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField] private int _health;

    [field:SerializeField] public float Speed { get; private set; }
    [field:SerializeField] public int Damage { get; private set; }
    [field:SerializeField] public int Score { get; private set; }

    [field: SerializeField] public bool CanShoot { get; private set; }
    [field: SerializeField] public bool CanHit { get; private set; }
    [field: SerializeField] public Transform ProjectileSpawnPoint { get; private set; }
    [field: SerializeField] public SphereCollider HitSphereCollider { get; private set; }
    [field: SerializeField] public Rigidbody Projectile { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
    [field: SerializeField] public float FireRate { get; private set; }
    private float _timer = 0;

    public static event Action<GameObject> OnEnemyDestroyed;

    private void OnEnable()
    {
        EnemyController.OnDeathAnimationPlay += ResetEnemy;
    }

    private void OnDisable()
    {
        EnemyController.OnDeathAnimationPlay -= ResetEnemy;
    }

    private void ResetEnemy(BaseEnemy thisEnemy)
    {
        thisEnemy.Damage = 0;
        thisEnemy.Speed = 0;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Hit");
            if (TryGetBullet(collision.gameObject, out Bullet bullet))
            {
                _health -= bullet.damage;
                if (_health <= 0)
                {
                    Die();
                }
            }
        }
    }

    private void Die()
    {
        // НЕ Destroy! Просто отключаем и сбрасываем позже, когда снова понадобится
        //gameObject.SetActive(false);
        this.gameObject.GetComponent<CharacterController>().enabled = false;
        OnEnemyDestroyed?.Invoke(this.gameObject);
        // Если нужно что-то сделать сразу (частицы, звук смерти) — делай тут
    }

    private bool TryGetBullet(GameObject obj, out Bullet bullet)
    {
        bullet = obj.GetComponent<Bullet>();
        return bullet != null;
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Bullet"))
    //    {
    //        Debug.Log("Hit");
    //        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
    //        _heath -= bullet.damage;
    //        if (_heath <= 0)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}

}
