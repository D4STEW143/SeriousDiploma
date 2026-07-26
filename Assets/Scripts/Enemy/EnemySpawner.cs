using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int _numberOfEnemies = 10;

    [SerializeField]private Vector2 minSpawnCoord = new Vector2(3,3);
    [SerializeField] private Vector2 maxSpawnCoord = new Vector2(13, 13);
    [SerializeField] private float spawnHeight = 1f;

    [SerializeField] private float _secPerEnemySpawn;
    private float _timer = 0f;

    [SerializeField]private float _groundDelta = 20f;

    [SerializeField]private GameObject[] _prefabs;
    private List<GameObject> EnemiesPool = new List<GameObject>();

    private Transform _player;

    private bool _activateSpawner = false;
    private int _enemySpawnCounter = 0;
    bool check;

    public static event Action<GameObject> OnEnemyCreation;

    private void Start()
    {
        CreatePool();
    }

    private void Update()
    {
        if (_activateSpawner)
        {
            _timer += Time.deltaTime;
            if (_timer >= _secPerEnemySpawn)
            {
                Spawn();
                _timer = 0f;
                _enemySpawnCounter++;
            }
            if (CheckPrefabsActive()) Destroy(gameObject);
        }
    }

    GameObject GetRandomEnemy()
    {
        if (_prefabs == null || _prefabs.Length == 0)
        {
            Debug.LogError("Массив префабов пуст или не инициализирован!");
            return null;
        }
        return _prefabs[UnityEngine.Random.Range(0, _prefabs.Length)];
    }

    void CreatePool()
    {
        for (int i = 0; i < _numberOfEnemies; i++)
        {
            GameObject enemy = Instantiate(GetRandomEnemy());
            enemy.SetActive(false);
            EnemiesPool.Add(enemy);
            OnEnemyCreation?.Invoke(enemy);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision work");
            _activateSpawner = true;
            _player = collision.gameObject.GetComponent<Transform>();
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < EnemiesPool.Count; i++)
        {
            GameObject enemy = EnemiesPool[i];

            // Сначала проверяем на null (покрывает и уничтоженные объекты)
            if (enemy == null)
            {
                // Удаляем из пула, чтобы не проверять дальше
                EnemiesPool.RemoveAt(i);
                i--;
                continue;
            }

            if (!enemy.activeInHierarchy)
            {
                Vector3 spawnPosition = GetSpawnPosition();
                Debug.Log($"Spawn position: {spawnPosition}");

                if (GroundCheck(spawnPosition, out Vector3 finalPosition))
                {
                    enemy.transform.position = finalPosition;
                    enemy.SetActive(true);
                    break;
                }
            }
        }
    }

    //private void Spawn()
    //{
    //    foreach(GameObject enemy in EnemiesPool)
    //    {
    //        if (!enemy.activeInHierarchy && !enemy.IsUnityNull())
    //        {
    //            Vector3 spawnPosition = GetSpawnPosition();
    //            Debug.Log($"Spawn position: {spawnPosition}");
    //            if (GroundCheck(spawnPosition, out Vector3 finalPosition))
    //            {
    //                enemy.transform.position = finalPosition;
    //                enemy.SetActive(true);
    //                break;
    //            }
    //        }
    //    }
    //}

    private bool GroundCheck(Vector3 spawnPosition, out Vector3 groundPosition)
    {
        //TODO: Make this method clear.
        RaycastHit hit;
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        bool rhit = Physics.Raycast(spawnPosition + Vector3.up * (_groundDelta - 1f), Vector3.down, out hit, _groundDelta, groundLayer);
        Debug.Log($"Hit point: {hit.point}");
        DebugRaycast(spawnPosition, _groundDelta, groundLayer);
        //if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, _groundDelta, groundLayer))
        if(rhit)
        {
            groundPosition = hit.point + Vector3.up * 0.1f;
            Debug.Log("Ground Check success");
            return true;
        }
        groundPosition = spawnPosition;
        Debug.Log($"Ground position: {groundPosition}; Ground Check failed");
        return false;
    }


    /*
     Данный метод берет позицию тригера спавна, добавляет к ней заданые координаты и спавнит противников в этом квадрате
     */
    private Vector3 GetSpawnPosition()
    {
        return new Vector3(UnityEngine.Random.Range(transform.position.x + minSpawnCoord.x, transform.position.x + maxSpawnCoord.x), spawnHeight, UnityEngine.Random.Range(transform.position.z + minSpawnCoord.y, transform.position.z + maxSpawnCoord.y));
        //return new Vector3(UnityEngine.Random.Range(_player.position.x + minSpawnCoord.x, _player.position.x + maxSpawnCoord.x), 1f, UnityEngine.Random.Range(_player.position.z + minSpawnCoord.y, _player.position.z + maxSpawnCoord.y));
    }

    private void OnDrawGizmos()
    {
        Vector3 minXminY = new Vector3(transform.position.x + minSpawnCoord.x, spawnHeight, transform.position.z + minSpawnCoord.y);
        Gizmos.DrawSphere(minXminY, 0.5f);
        Vector3 minXmaxY = new Vector3(transform.position.x + minSpawnCoord.x, spawnHeight, transform.position.z + maxSpawnCoord.y);
        //Gizmos.DrawSphere(new Vector3(transform.position.x + minSpawnCoord.x, 1f, transform.position.z + minSpawnCoord.y), 1f);
        Gizmos.DrawSphere(minXmaxY, 0.5f);
        //Gizmos.DrawSphere(new Vector3(transform.position.x + minSpawnCoord.x, 1f, transform.position.z + maxSpawnCoord.y), 1f);
        Vector3 maxXmaxY = new Vector3(transform.position.x + maxSpawnCoord.x, spawnHeight, transform.position.z + maxSpawnCoord.y);
        Gizmos.DrawSphere(maxXmaxY, 0.5f);
        //Gizmos.DrawSphere(new Vector3(transform.position.x + maxSpawnCoord.x, 1f, transform.position.z + maxSpawnCoord.y), 1f);
        Vector3 maxXminY = new Vector3(transform.position.x + maxSpawnCoord.x, spawnHeight, transform.position.z + minSpawnCoord.y);
        Gizmos.DrawSphere(maxXminY, 0.5f);
        //Gizmos.DrawSphere(new Vector3(transform.position.x + maxSpawnCoord.x, 1f, transform.position.z + minSpawnCoord.y), 1f);
        Gizmos.DrawLine(minXminY, minXmaxY); //левая вертикаль
        Gizmos.DrawLine(minXmaxY, maxXmaxY); //левая вертикаль
        Gizmos.DrawLine(maxXmaxY, maxXminY); //левая вертикаль
        Gizmos.DrawLine(maxXminY, minXminY); //левая вертикаль
    }

    //private bool CheckPrefabsActive()
    //{
    //    foreach(GameObject enemy in EnemiesPool)
    //    {
    //        if (enemy.activeInHierarchy) check = true;
    //        if (!enemy.activeInHierarchy)
    //        {
    //            check = false;
    //            break;
    //        }
    //    }
    //    return check;
    //}

    private bool CheckPrefabsActive()
    {
        foreach (var enemy in EnemiesPool)
        {
            // Сначала проверяем на null — это ловит и уничтоженные объекты
            if (enemy == null)
            {
                // Если в пуле есть уничтоженный объект, считаем проверку проваленной
                return false;
            }

            if (!enemy.activeInHierarchy)
            {
                return false;
            }
        }

        // Если дошли сюда — все объекты валидны и активны
        return true;
    }

    void DebugRaycast(Vector3 spawnPosition, float groundDelta, LayerMask groundLayer)
    {
        Vector3 rayStart = spawnPosition + Vector3.up * 10f;
        RaycastHit hit;

        // Визуализация луча в редакторе
        Debug.DrawRay(rayStart, Vector3.down * groundDelta, Color.red, 5f);

        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundDelta, groundLayer))
        {
            Debug.Log($"HIT: точка столкновения = {hit.point}, объект = {hit.collider.name}");
        }
        else
        {
            Debug.Log("RAYCAST: луч не нашёл коллайдер");
        }

        Debug.Log($"Параметры: старт = {rayStart}, дистанция = {groundDelta}, маска слоёв = {groundLayer.ToString()}");
    }
}
