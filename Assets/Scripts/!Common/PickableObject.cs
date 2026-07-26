using System;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    [SerializeField] private Pickable _type;
    [SerializeField] private int _howMuchToAdd;
    [SerializeField] private GameObject _visual;
    [SerializeField] private GameObject _lightObject;
    private Light _light;

    public static event Action<bool> OnKeyPickUp;
    public static event Action<int> OnPickUpHealth; 
    public static event Action<int> OnPickUpArmor; 
    public static event Action<int, Weapons> OnPickUpPistolAmmo; 
    public static event Action<int, Weapons> OnPickUpSMGAmmo; 
    public static event Action<int, Weapons> OnPickUpRifleAmmo; 
    public static event Action<int, Weapons> OnPickUpShotgunAmmo; 
    
    private void Start()
    {
        _light = _lightObject.GetComponent<Light>();
    }

    private void Update()
    {
        _visual.transform.Rotate(0f, 100 * Time.deltaTime, 0f);
        _visual.transform.Translate(0f, Mathf.PingPong(Time.time, 3f), 0f);
        _visual.transform.Translate(0f, -Mathf.PingPong(Time.time, 3f), 0f);
        _light.intensity = Mathf.PingPong(Time.time, 3);
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
                        OnKeyPickUp?.Invoke(true);
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
            Destroy(gameObject);
        }
    }
}
