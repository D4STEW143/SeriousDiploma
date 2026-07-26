using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyZone_DamageZone : MonoBehaviour
{
    public static event Action<int> OnPlayerTouch;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerTouch?.Invoke(0);
        }
    }
}
