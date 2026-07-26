using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Player settings")]
    [field:SerializeField] public int PlayerHealth { get; private set; }
    private int _maxPlayerHealth = 100;
    [field:SerializeField] public int PlayerArmor { get; private set; }
    private int _maxPlayerArmor = 200;
    [SerializeField] private PlayerHUD _playerHUD;

    public static event Action<string> OnPlayerDead;


    private void OnEnable()
    {
        PickableObject.OnPickUpHealth += AddHealth;
        PickableObject.OnPickUpArmor += AddArmor;
        EnemyProjectile.OnEnemyProjectileHitPlayer += GetDamage;
        EnemyController.OnHitPlayer += GetDamage;
        EnemyZone_DamageZone.OnPlayerTouch += PlayerDead;
    }

    private void OnDisable()
    {
        PickableObject.OnPickUpHealth -= AddHealth;
        PickableObject.OnPickUpArmor += AddArmor;
        EnemyProjectile.OnEnemyProjectileHitPlayer -= GetDamage;
        EnemyController.OnHitPlayer -= GetDamage;
        EnemyZone_DamageZone.OnPlayerTouch -= PlayerDead;
    }

    private void AddHealth(int healthAmount)
    {
        if (PlayerHealth + healthAmount < _maxPlayerHealth) this.PlayerHealth += healthAmount;
        else this.PlayerHealth = _maxPlayerHealth;
    }
    private void AddArmor(int armorAmount)
    {
        if (PlayerArmor + armorAmount < _maxPlayerArmor) this.PlayerArmor += armorAmount;
        else this.PlayerArmor = _maxPlayerArmor;
    }

    private void GetDamage(int damageAmount)
    {
        if (PlayerArmor > 0)
        {
            int armorDamage = (int)(damageAmount * 0.75);
            PlayerArmor = Math.Max(0, PlayerArmor - armorDamage);
        }
        else
        {
            PlayerHealth -= damageAmount;
            PlayerHealth = Math.Max(0, PlayerHealth);
        }

        if (PlayerHealth <= 0) {
            PlayerDead(0);
        }
    }

    private void PlayerDead(int _int)
    {
        OnPlayerDead?.Invoke("");
    }
}
