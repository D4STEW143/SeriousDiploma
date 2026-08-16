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
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private GameObject _crosshair;
    public float Timer { get; private set; } = 0f;
    
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
        TimerTick();
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
        _timer.text = TimeDisplayment(Timer);
        if(_scoreText!=null)_scoreText.text = _score.ToString();
        if(_playerHealthManager != null)
        {
            _playerHealth.text = _playerHealthManager.PlayerHealth.ToString();
            _playerArmor.text = _playerHealthManager.PlayerArmor.ToString();
        }
        if(_playerWeaponManager != null)
        {
            _weaponAmmo.text = $"{_playerWeaponManager.BulletsInMagLeft(_weapon.WeaponType)}/{_playerWeaponManager.CurrentAmmoAmount(_weapon.WeaponType)}";
        }
    }

    private void TimerTick()
    {
        Timer += Time.deltaTime;
    }

    private string TimeDisplayment(float _time)
    {
        string toReturn;
        double milisec;
        int sec = 0;
        int min = 0;
        int hour = 0;
        milisec = (double)_time - Math.Truncate(_time);
        if(Math.Truncate(_time) < 60d) sec = (int)Math.Truncate(_time);
        else
        {
            min = (int)Math.Truncate(_time) / 60;
            sec = (int)Math.Truncate(_time) - (min * 60);
        }
        if(min > 60)
        {
            hour = min / 60;
            min = min - (min * 60);
        }

        toReturn = $"{hour}:{min}:{sec}.{(int)(milisec * 10)}";

        return toReturn;
    }

}
