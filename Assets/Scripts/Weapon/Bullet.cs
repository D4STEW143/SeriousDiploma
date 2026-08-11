using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletHolePrefab; 
    [SerializeField] private float _decalDestroyDelay = 2f; 
    public int damage;
    [SerializeField]private float _bulletDestroyDelay = 2.5f;


    public void Update()
    {
        //Debug.Log("Letim");
        Destroy(this.gameObject, _bulletDestroyDelay);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;
            CreateBulletHole(hitPoint, hitNormal);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void CreateBulletHole(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (bulletHolePrefab == null)
        {
            Debug.LogError("Префаб отметины не назначен!");
            return;
        }
        GameObject bulletHole = Instantiate(bulletHolePrefab, hitPoint, Quaternion.LookRotation(hitNormal));
        bulletHole.transform.position += bulletHole.transform.forward * 0.001f;
        Destroy(bulletHole, _decalDestroyDelay);
    }
}