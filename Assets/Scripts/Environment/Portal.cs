using System;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    [SerializeField] private GameObject _trigger;
    [SerializeField] private bool _isBidirectional;
    [SerializeField] private GameObject _exitSide;
    private bool _isReadyToUse;
    private SphereCollider _collider;

    public static event Action OnLevelEnd;

    private void OnEnable()
    {
        GameManager.ActivateExitPortal += ThisPortalActive;
    }

    private void OnDisable()
    {
        GameManager.ActivateExitPortal -= ThisPortalActive;
        
    }

    private void Start()
    {
        _collider = _trigger.GetComponent<SphereCollider>();
        if(_isBidirectional)_isReadyToUse = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Teleport(other.gameObject);
        }
    }

    private void Teleport(GameObject player)
    {
            if (_isBidirectional)
            {
                //TODO:Сделать тут перемещение персонажа между порталами. Можно сделать через передачу координат в класс PlayerMovement
            }
            else if(!_isBidirectional)
            {
                if (_isReadyToUse)
                {
                    Debug.Log("Сработало");
                    OnLevelEnd?.Invoke();
                }
            }
    }

    private void ThisPortalActive()
    {
        _isReadyToUse = true;
    }
}
