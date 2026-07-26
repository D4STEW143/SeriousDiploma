using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player settings")]
    [SerializeField] private GameObject _player;
    private PlayerWeaponManager _playerWeaponManager;
    private PlayerHealthManager _playerHealthManager;
    private BaseWeapon _weapon;
    private int _score = 0;
    [Header("HUD settings")]
    [SerializeField] private TextMeshProUGUI _playerHealth;
    [SerializeField] private TextMeshProUGUI _playerArmor;
    [SerializeField] private TextMeshProUGUI _weaponAmmo;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _crosshair;
    
    private void Start()
    {
        _playerWeaponManager = _player.GetComponent<PlayerWeaponManager>();
        if (_playerWeaponManager == null) Debug.LogError("Weapon Manager не найден на объекте!");
        _playerHealthManager = _player.GetComponent<PlayerHealthManager>();
        if (_playerWeaponManager == null) Debug.LogError("Health Manager не найден на объекте!");
        _crosshair.SetActive(true);
    }

    private void Update()
    {
        UpdateHUD();   
    }

    private void OnEnable()
    {
        PlayerWeaponManager.OnWeaponChange += WeaponChanged;
        GameManager.OnScoreChanged += ScoreChanged;
    }

    private void OnDisable()
    {
        PlayerWeaponManager.OnWeaponChange -= WeaponChanged;
        GameManager.OnScoreChanged -= ScoreChanged;
    }
    private void WeaponChanged(BaseWeapon _newWeapon)
    {
        this._weapon = _newWeapon;
    }

    private void ScoreChanged(int score)
    {
        this._score = score;
    }

    private void UpdateHUD()
    {
        if(_playerHealthManager != null)
        {
            _playerHealth.text = _playerHealthManager.PlayerHealth.ToString();
            _playerArmor.text = _playerHealthManager.PlayerArmor.ToString();
        }
        if(_playerWeaponManager != null)
        {
            _weaponAmmo.text = $"{_playerWeaponManager.BulletsInMagLeft(_weapon.WeaponType)}/{_playerWeaponManager.CurrentAmmoAmount(_weapon.WeaponType)}";
        }
         if(_scoreText!=null)_scoreText.text = _score.ToString();
    }

}
