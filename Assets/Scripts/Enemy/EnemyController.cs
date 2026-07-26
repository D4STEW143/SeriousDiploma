using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private BaseEnemy _enemy;
    private CharacterController _characterController;
    private GameObject _player;
    private Vector3 _gravity = new Vector3(0f, -9.8f,0f);
    private float _timer = 0;
    public bool IsDead { get; private set; } = false;

    private Animator _animator;

    public static event Action<int> OnHitPlayer;
    public static event Action<BaseEnemy> OnDeathAnimationPlay;

    void Start()
    {
        _enemy = GetComponent<BaseEnemy>();
        _characterController = GetComponent<CharacterController>();
        _player = GameObject.Find("Player");
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsDead)
        {
            MoveToPlayer();
            Shoot();
            PlayerCheck();
        }
    }

    private void OnEnable()
    {
        BaseEnemy.OnEnemyDestroyed += Dead;
    }

    private void OnDisable()
    {
        BaseEnemy.OnEnemyDestroyed -= Dead;
    }

    private void MoveToPlayer()
    {
        Vector3 move = (_player.transform.position - _characterController.transform.position).normalized;
        _characterController.transform.rotation = Quaternion.LookRotation(move);
        float speed = move.magnitude * _enemy.Speed;
        this._characterController.Move((move + _gravity) * _enemy.Speed * Time.deltaTime);
        _animator.SetFloat("Speed", speed);
        if(this.isActiveAndEnabled)Debug.Log($"Enemy speed " + speed);
    }

    private void PlayerCheck()  //функция проверяет есть ли игрок в зоне удара и если да запускает анимацию
    {
        if (_enemy.CanHit)
        {
            _timer += Time.deltaTime;
            if (_timer >= _enemy.FireRate)
            {
                Collider[] _colliders = Physics.OverlapSphere(_enemy.HitSphereCollider.transform.position, 1f);
                foreach (Collider collider in _colliders)
                {
                    if (collider.CompareTag("Player"))
                    {
                        StartCoroutine(DoAttack());
                        _timer = 0;
                    }
                }
            }
        }
    }

    private IEnumerator DoAttack()
    {
        _animator.SetBool("Attack", true);
        yield return new WaitForSeconds(1f);
        Hit();
        _animator.SetBool("Attack", false);
    }
    private void Hit()
    {
        Collider[] _colliders = Physics.OverlapSphere(_enemy.HitSphereCollider.transform.position, 1f);
        foreach (Collider collider in _colliders)
        {
            if (collider.CompareTag("Player"))
            {
                OnHitPlayer?.Invoke(_enemy.Damage);
            }
        }
    }
    private void Shoot()
    {
        if (_enemy.CanShoot)
        {
            _timer += Time.deltaTime;
            if (_timer >= _enemy.FireRate)
            {
                StartCoroutine(DoShoot());
                _timer = 0;
            }
        }
    }

    private IEnumerator DoShoot()
    {
        _animator.SetBool("Attack", true);
        yield return new WaitForSeconds(1f);
        Rigidbody _rb = Instantiate(_enemy.Projectile, _enemy.ProjectileSpawnPoint.transform.position, Quaternion.identity);
        EnemyProjectile bullet = _rb.GetComponent<EnemyProjectile>();
        bullet.damage = _enemy.Damage;
        _rb.linearVelocity = _enemy.ProjectileSpawnPoint.forward * _enemy.ProjectileSpeed;
        _animator.SetBool("Attack", false);
    }

    private void Dead(GameObject thisEnemy)
    {
        StartCoroutine(Death(thisEnemy));
    }

    private IEnumerator Death(GameObject thisEnemy)
    {
        thisEnemy.GetComponent<Animator>().SetBool("Dead", true);
        thisEnemy.GetComponent<EnemyController>().IsDead = true;
        OnDeathAnimationPlay?.Invoke(thisEnemy.GetComponent<BaseEnemy>());
        yield return new WaitForSeconds(10f);
        thisEnemy.gameObject.SetActive(false);
    }


}
