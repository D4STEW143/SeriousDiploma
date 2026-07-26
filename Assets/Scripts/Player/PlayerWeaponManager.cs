using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

//TODO:Перезарядка через корутину
//TODO:Сделать коментарии к методам на английском

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon Pool Settings")]
    [SerializeField] private List<GameObject> _playerWeaponsPool;
    [SerializeField] private GameObject _weaponHolder;
    [SerializeField] private List<GameObject> _playerWeapons;
    private int _currentWeaponIndex = 0;
    private BaseWeapon _currentWeapon;
    private float _nextFireTime = 0f;

    private Dictionary<Weapons, int> _ammoInMag = new Dictionary<Weapons, int> 
    {
        {Weapons.Pistol, 0 }, {Weapons.SMG, 0 }, {Weapons.Rifle, 0 }, {Weapons.Shotgun, 0 }, {Weapons.Rocketlauncher, 0 }
    };

    private Dictionary<Weapons, int> _currentAmmo = new Dictionary<Weapons, int>
    {
        {Weapons.Pistol, 100 }, {Weapons.SMG, 250 }, {Weapons.Rifle, 120 }, {Weapons.Shotgun, 100 }, {Weapons.Rocketlauncher, 0}
    };

    private Dictionary<Weapons, int> _maxAmmo = new Dictionary<Weapons, int>
    {
        {Weapons.Pistol, 149 }, {Weapons.SMG, 1200}, {Weapons.Rifle, 450}, {Weapons.Shotgun, 200}, {Weapons.Rocketlauncher, 70}
    };

    [SerializeField] protected Animator _animator;
    private float _currentPlayerSpeed;


    //Sound
    protected AudioSource _gunAudioCreator;

    //Reload
    private bool _isReloading;

    //Events
    public static event Action<BaseWeapon> OnWeaponChange;


    void Start()
    {
        _gunAudioCreator = GetComponentInChildren<AudioSource>();
        WeaponsPoolInitializer();
        ActivateWeapon(_currentWeaponIndex);
    }

    private void Update()
    {
        _animator = _currentWeapon.GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        PickableObject.OnPickUpPistolAmmo += AmmoPickUp;
        PickableObject.OnPickUpSMGAmmo += AmmoPickUp;
        PickableObject.OnPickUpRifleAmmo += AmmoPickUp;
        PickableObject.OnPickUpShotgunAmmo += AmmoPickUp;
        PlayerMovement.PlayerSpeed += CurrentPlayerSpeed;
    }

    private void OnDisable()
    {
        PickableObject.OnPickUpPistolAmmo -= AmmoPickUp;
        PickableObject.OnPickUpSMGAmmo -= AmmoPickUp;
        PickableObject.OnPickUpRifleAmmo -= AmmoPickUp;
        PickableObject.OnPickUpShotgunAmmo -= AmmoPickUp;
        PlayerMovement.PlayerSpeed -= CurrentPlayerSpeed;
    }

    //Инициализирует и деактивирует список оружия игрока
    private void WeaponsPoolInitializer() 
    {
        foreach (GameObject weapon in _playerWeaponsPool)
        {
            GameObject _weapon = Instantiate(weapon, _weaponHolder.transform);
            _playerWeapons.Add(_weapon);
            _weapon.gameObject.SetActive(false);
        }
    }

    //Устанавливает активным из списка оружия то оружие, индекс которого пришел методу в качестве параметра.
    //А так же устанавливает активированое оружие в качестве текущего для последущих взаимодействий.
    private void ActivateWeapon(int _weaponIndex)
    {
        if (!_playerWeapons[_weaponIndex].activeInHierarchy)
        {
            _playerWeapons[_currentWeaponIndex].SetActive(false);
            _playerWeapons[_weaponIndex].SetActive(true);
            BaseWeapon _wpn = _playerWeapons[_weaponIndex].GetComponent<BaseWeapon>();
            _currentWeapon = _wpn;
            OnStartBullets(_currentWeapon.WeaponType);
            _currentWeaponIndex = _weaponIndex;
            OnWeaponChange?.Invoke(_currentWeapon);
        }
    }

//TODO:Дописать метод который будет добавлять патроны в магазин на старте сцены. Сделать флаги активации оружия, для проверки первая активация или нет.
    private void OnStartBullets(Weapons weaponType)
    {
        _ammoInMag[weaponType] = SetBulletsInMag();
    }

    private void AmmoPickUp(int amount, Weapons weaponType)
    {
        if(_currentAmmo.ContainsKey(weaponType))
        {
            if (_currentAmmo[weaponType] + amount < _maxAmmo[weaponType]) { 
                _currentAmmo[weaponType] += amount;
                Debug.Log("обычное добавление");
                return;
            }
            if(_currentAmmo[weaponType] + amount > _maxAmmo[weaponType])
            {
                Debug.Log($"{_currentWeapon.WeaponType} / {_currentAmmo[weaponType]} + {amount}");  
                Debug.Log("необычное добавление");
                _currentAmmo[weaponType] = _maxAmmo[weaponType];
                return;
            }
        }
        else Debug.Log($"Unable to add ammo to {weaponType}.");
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                {
                    if (_currentWeapon.IsFullAuto)
                    {
                        StartCoroutine(ShootAuto());
                    }
                    else
                    {
                        Shoot();
                    }
                    break;
                }
            case InputActionPhase.Performed:
                if (_currentWeapon.IsFullAuto && Time.time >= _nextFireTime)
                {
                    Shoot();
                    _nextFireTime = Time.time + 1f / _currentWeapon.RateOfFire;
                }
                break;
            case InputActionPhase.Canceled:
                {
                    StopAllCoroutines();
                    break;
                }
        }
    }

    private void Shoot()
    {
        if (isReadyToShoot)
        {
            _currentWeapon.ShootProjectile();
            _gunAudioCreator.PlayOneShot(_currentWeapon.GunSound);
            DecriseAmmoInMag(_currentWeapon.WeaponType);
        }
        else Debug.Log("Ne gotov strelyat");
    }

    private IEnumerator ShootAuto()
    {
        while (true)
        {
            if (Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + 1f / _currentWeapon.RateOfFire;
            }
            yield return null;
        }
    }

    //Уменьшает на 1 количество патронов в магазине в зависимости от типа текущего оружия.
    private void DecriseAmmoInMag(Weapons weaponType)
    {
        if (_ammoInMag.ContainsKey(weaponType))
        {
            _ammoInMag[weaponType]--;
        }
        else Debug.Log("Can't decriese ammo, weapon type is unknown.");
    }

    private void CurrentPlayerSpeed(float speed)
    {
        _currentPlayerSpeed = speed;
        //_animator.SetFloat("Speed", _currentPlayerSpeed);
    }


    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (context.control.name)
            {
                case "1": { ActivateWeapon(0); _animator = GetComponentInChildren<Animator>(); Debug.Log("Нажата клавиша 1"); break; }
                case "2": { ActivateWeapon(1); _animator = GetComponentInChildren<Animator>(); Debug.Log("Нажата клавиша 2"); break; }
                case "3": { ActivateWeapon(2); _animator = GetComponentInChildren<Animator>(); Debug.Log("Нажата клавиша 3"); break; }
                case "4": { ActivateWeapon(3); _animator = GetComponentInChildren<Animator>(); Debug.Log("Нажата клавиша 4"); break; }
            }
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Reload(_currentWeapon.WeaponType);
        }
    }

    private void Reload(Weapons weaponType)
    {
        if (HasAmmo(weaponType))
        {
            _isReloading = true;
            _ammoInMag[_currentWeapon.WeaponType] = SetBulletsInMag();
            Debug.Log("Перезарядка");
            _isReloading = false;
        }
        else Debug.Log("Перезарядка невозможна. Закончились патроны.");
    }

    private int SetBulletsInMag()
    {
        int toReturn;
        if (_currentAmmo.ContainsKey(_currentWeapon.WeaponType))
        {
            if (CurrentAmmoAmount(_currentWeapon.WeaponType) >= _currentWeapon.MagCapacity)
            {
                if (BulletsInMagLeft(_currentWeapon.WeaponType) > 0)
                {
                    _currentAmmo[_currentWeapon.WeaponType] += _ammoInMag[_currentWeapon.WeaponType];
                    _ammoInMag[_currentWeapon.WeaponType] = 0;
                }
                _currentAmmo[_currentWeapon.WeaponType] -= _currentWeapon.MagCapacity;
                return _currentWeapon.MagCapacity;
            }
            else if (CurrentAmmoAmount(_currentWeapon.WeaponType) < _currentWeapon.MagCapacity)
            {
                if (BulletsInMagLeft(_currentWeapon.WeaponType) > 0)
                {
                    _currentAmmo[_currentWeapon.WeaponType] += _ammoInMag[_currentWeapon.WeaponType];
                    _ammoInMag[_currentWeapon.WeaponType] = 0;
                }
                if (CurrentAmmoAmount(_currentWeapon.WeaponType) > _currentWeapon.MagCapacity)
                {
                    _currentAmmo[_currentWeapon.WeaponType] -= _currentWeapon.MagCapacity;
                    return _currentWeapon.MagCapacity;
                }
                toReturn = _currentAmmo[_currentWeapon.WeaponType];
                _currentAmmo[_currentWeapon.WeaponType] = 0;
                return toReturn;
            }
            else return 0;
        }
        else
        {
            Debug.Log("Reload is unavalible.");
            return 0;
        }
    }

    private bool HasAmmo(Weapons weaponType)
    {
        if (_currentAmmo.ContainsKey(weaponType))
        {
            return _currentAmmo[weaponType] > 0;
        }
        else
        {
            Debug.Log("Can't check ammo, weapon type is unknown.");
            return false;
        }
        
    }
    private bool HasAmmoInMag(Weapons weaponType)
    {
        if (_ammoInMag.ContainsKey(weaponType))
        {
            return _ammoInMag[weaponType] > 0;
        }
        else
        {
            Debug.Log("Can't check ammo in mag, weapon type is unknown.");
            return false;
        }
        
    }

    private bool isReadyToShoot => !_isReloading && HasAmmoInMag(_currentWeapon.WeaponType);

    public int BulletsInMagLeft(Weapons weaponType)
    {
        if (_ammoInMag.ContainsKey(weaponType))
        {
            return _ammoInMag[weaponType];
        }
        else
        {
            Debug.Log("Can't return ammo in mag, weapon type is unknown.");
            return 0;
        }
    }

    public int CurrentAmmoAmount(Weapons weaponType)
    {
        if (_currentAmmo.ContainsKey(weaponType))
        {
            return _currentAmmo[weaponType];
        }
        else
        {
            Debug.Log("Can't return ammo, weapon type is unknown.");
            return 0;
        }
        
    }
}
