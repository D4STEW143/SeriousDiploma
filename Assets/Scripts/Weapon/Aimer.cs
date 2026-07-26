using UnityEngine;

public class Aimer : MonoBehaviour
{
    private Transform _camTransform;
    private Transform _bulletSpawnPoint;
    [SerializeField]private LayerMask _layerMask = Physics.DefaultRaycastLayers;
    [SerializeField]private bool _drawGizmos;
    private Vector3 _targetPoint;

    private void OnValidate()
    {
        if( _camTransform == null)
        {
            _camTransform = transform;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_camTransform.position, _targetPoint);
        Gizmos.DrawSphere(_targetPoint, 0.1f);
    }

    private void Update()
    {
        Ray ray = new Ray(_camTransform.position, _camTransform.forward);
        _targetPoint = ray.GetPoint(50f);
        if(Physics.Raycast(ray, out RaycastHit hit, 1000f, _layerMask, QueryTriggerInteraction.Ignore))
        {
            if(hit.distance >= 0.01f)
            {
                _targetPoint = hit.point;
            }
        }
        GetComponentInChildren<BaseWeapon>().MuzzleEnd.LookAt(_targetPoint);
    }
}
