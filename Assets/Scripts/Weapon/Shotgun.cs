using UnityEngine;

public class Shotgun : BaseWeapon
{
    [SerializeField] private int _projectilesInShell;
    [SerializeField] private float spreadAngle;
    public override void ShootProjectile()
    {
        for (int i = 0; i < _projectilesInShell; i++) { 
            Rigidbody _rb = Instantiate(_bulletPrefab, CreateStartSpread(MuzzleEnd.transform.position), Quaternion.identity);
            Bullet bullet = _rb.GetComponent<Bullet>();
            bullet.damage = this.Damage / _projectilesInShell;

            Vector3 baseDirection = MuzzleEnd.forward;
            Vector3 randomSpread = new Vector3(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle));

            Quaternion spreadRotation = Quaternion.Euler(randomSpread);
            Vector3 finalDirection = GetRandomSpreadDirection(MuzzleEnd.forward, spreadAngle);
            _rb.linearVelocity = finalDirection * _projectileSpeed;


            Debug.DrawRay(MuzzleEnd.position, finalDirection * 100f, Color.red, 5f);
        }
    }

    Vector3 GetRandomSpreadDirection(Vector3 baseDirection, float maxAngle)
    {
        Vector3 randomDirection = Random.insideUnitSphere * maxAngle;
        Quaternion spread = Quaternion.Euler(randomDirection);
        return spread * baseDirection;
    }

    Vector3 CreateStartSpread(Vector3 startPosition)//Добавляет небольшой разброс при спавне проджектьайлов, чтобы пофиксить баг с тупой стрельбой дробовика
    {
        return new Vector3(startPosition.x + Random.Range(0, 0.1f), startPosition.y + Random.Range(0, 0.1f), startPosition.z + Random.Range(0, 0.1f));
    }


}
