using System;
using System.Collections;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    [SerializeField] private Pickable _type;
    [SerializeField] private int _howMuchToAdd;
    [SerializeField] private GameObject _visual;
    [SerializeField] private GameObject _lightObject;
    [SerializeField] private AudioClip _puckUpSound;
    private AudioSource _puckUpAudioSource;
    private Light _light;
    private Collider _collider;

    public static event Action<int> OnPickUpHealth; 
    public static event Action<int> OnPickUpArmor; 
    public static event Action<int, Weapons> OnPickUpPistolAmmo; 
    public static event Action<int, Weapons> OnPickUpSMGAmmo; 
    public static event Action<int, Weapons> OnPickUpRifleAmmo; 
    public static event Action<int, Weapons> OnPickUpShotgunAmmo;
    public static event Action OnKeyPickUp;
    public static event Action<Pickable> OnPistolWeaponPickUp;
    public static event Action<Pickable> OnSMGWeaponPickUp;
    public static event Action<Pickable> OnRifleWeaponPickUp;
    public static event Action<Pickable> OnShotgunWeaponPickUp;
    
    private void Start()
    {
        _light = _lightObject.GetComponent<Light>();
        _puckUpAudioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider>(); 
    }

    private void Update()
    {
        _visual.transform.Rotate(0f, 100 * Time.deltaTime, 0f);
        _visual.transform.Translate(0f, Mathf.PingPong(Time.time, 3f), 0f);
        _visual.transform.Translate(0f, -Mathf.PingPong(Time.time, 3f), 0f);
        _light.intensity = Mathf.PingPong(Time.time, 3);
    }

    private IEnumerator DisablePickableObject()
    {
        _collider.enabled = false;
        _lightObject.SetActive(false);
        _visual.SetActive(false);
        _puckUpAudioSource.PlayOneShot(_puckUpSound, 1f);
        Debug.Log("Звук поднятия пикапа проигрался");
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (_type)
            {
                case Pickable.Health:
                    {
                        OnPickUpHealth?.Invoke(_howMuchToAdd);
                        break;
                    }
                case Pickable.Armor:
                    {
                        OnPickUpArmor?.Invoke(_howMuchToAdd); 
                        break;
                    }
                case Pickable.PistolAmmo:
                    {
                        OnPickUpPistolAmmo?.Invoke(_howMuchToAdd, Weapons.Pistol);
                        break;
                    }
                case Pickable.SMGAmmo:
                    {
                        OnPickUpSMGAmmo?.Invoke(_howMuchToAdd, Weapons.SMG);
                        break;
                    }
                case Pickable.RifleAmmo:
                    {
                        OnPickUpRifleAmmo?.Invoke(_howMuchToAdd, Weapons.Rifle);
                        break;
                    }
                case Pickable.ShotgunAmmo:
                    {
                        OnPickUpShotgunAmmo?.Invoke(_howMuchToAdd, Weapons.Shotgun);
                        break;
                    }
                case Pickable.LevelKey:
                    {
                        OnKeyPickUp?.Invoke();
                        break;
                    }
                case Pickable.PistolWeapon:
                    {
                        OnPistolWeaponPickUp?.Invoke(Pickable.PistolWeapon);
                        break;
                    }
                case Pickable.SMGWeapon:
                    {
                        OnSMGWeaponPickUp?.Invoke(Pickable.SMGWeapon);
                        break;
                    }
                case Pickable.RifleWeapon:
                    {
                        OnRifleWeaponPickUp?.Invoke(Pickable.RifleWeapon);
                        break;
                    }
                case Pickable.ShotgunWeapon:
                    {
                        OnShotgunWeaponPickUp?.Invoke(Pickable.ShotgunWeapon);
                        break;
                    }
                default:
                    {
                        Debug.Log("Тип объекта не определен, изменений внесено не будет.");
                        break;
                    }
                        //TODO:Defalult сделать
                    }
            Debug.Log($"{_type} поднят");
            StartCoroutine(DisablePickableObject());
        }
    }
}
