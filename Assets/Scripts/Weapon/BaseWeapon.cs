using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Main attributes")]
    [field:SerializeField] public Weapons WeaponType { get; protected set; }
    [field:SerializeField] public int Damage { get; protected set; }
    [field:SerializeField] public bool IsFullAuto { get; protected set; }
    [field:SerializeField] public float RateOfFire { get; protected set; }
    [field:SerializeField] public int MagCapacity {  get; protected set; }
    [field:SerializeField] public bool IsAvailable { get; protected set; }
    [field:SerializeField] public float ReloadTime { get; protected set; }

    [SerializeField] protected float _projectileSpeed;

    [SerializeField] protected Rigidbody _bulletPrefab;

    [Header("Effects")]
    [field:SerializeField] public Transform MuzzleEnd {  get; set; }

    [Header("Sound")]
    [field:SerializeField] public AudioClip GunSound { get; protected set; }


    [field:SerializeField] public bool IsActive { get; set; }


    public virtual void ShootProjectile()
    {
        Rigidbody _rb = Instantiate(_bulletPrefab, MuzzleEnd.transform.position, Quaternion.identity);
        Bullet bullet = _rb.GetComponent<Bullet>();
        bullet.damage = this.Damage;
        _rb.linearVelocity = MuzzleEnd.forward * _projectileSpeed;

        Debug.DrawRay(MuzzleEnd.position, MuzzleEnd.forward * 100f, Color.red, 5f);
    }




}
