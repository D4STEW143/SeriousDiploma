using UnityEngine;

public class AimerV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private bool _drawGizmos;

    private Transform _camTransform;
    private Vector3 _targetPoint;

    private void OnValidate()
    {
        if (_camTransform == null)
            _camTransform = transform;
    }

    public Vector3 GetShootPoint(out Ray ray)
    {
        ray = new Ray(_camTransform.position, _camTransform.forward);
        _targetPoint = ray.GetPoint(50f); // запасная точка, если нет препятствий

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.distance >= 0.01f)
                _targetPoint = hit.point;
        }

        return _targetPoint;
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        Gizmos.color = Color.red;
        Ray ray = new Ray(_camTransform.position, _camTransform.forward);
        Vector3 endPoint = Physics.Raycast(ray, out RaycastHit hit, 1000f, _layerMask, QueryTriggerInteraction.Ignore)
            ? hit.point
            : ray.GetPoint(100f);

        Gizmos.DrawLine(_camTransform.position, endPoint);
        Gizmos.DrawSphere(endPoint, 0.1f);
    }
}