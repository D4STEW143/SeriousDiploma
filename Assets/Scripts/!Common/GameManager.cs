using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    private int _gameScore;

    [SerializeField] private PlayerHUD _playerHUD;
    private ScoreCounterScreen _scoreCounter;

    private List<GameObject> _enemies = new List<GameObject>();

    private bool isLevelKeyPickedUp = false;

    public static event Action<int> OnScoreChanged;
    public static event Action ActivateExitPortal;

    public enum GameState { Playing, GameOver, Paused }
    public static GameState State { get; set; } = GameState.Playing;
    [field: SerializeField] public AudioClip[] LevelTracks { get; private set; }

    private void Start()
    {
        Time.timeScale = 1.0f;
        _scoreCounter = GetComponent<ScoreCounterScreen>();
        BackGroundMusic.Instance.SetPlaylist(LevelTracks);
        BackGroundMusic.Instance.PlayFirst();
    }

    private void Update()
    {
        if (_enemies != null)
        {
            if (_enemies.Count == 0)
            {
                Debug.Log("WIN");
            }
        }
        if (State == GameState.Playing)
        {
            BackGroundMusic.Instance.CheckForTrackEnd();
        }
    }

    private void OnEnable()
    {
        BaseEnemy.OnEnemyDestroyed += EnemyKilled;
        PlayerHealthManager.OnPlayerDead += PlayerDeath;
        EnemySpawner.OnEnemyCreation += EnemyCreated;
        PickableObject.OnKeyPickUp += LevelKeyPickedUp;
        PortalScript.OnLevelEnd += EndViaPortal;
        ScoreCounterScreen.ContinueButtonClick += LoadNextLevel;
    }
    private void OnDisable()
    {
        BaseEnemy.OnEnemyDestroyed -= EnemyKilled;
        PlayerHealthManager.OnPlayerDead += PlayerDeath;
        EnemySpawner.OnEnemyCreation -= EnemyCreated;
        PickableObject.OnKeyPickUp -= LevelKeyPickedUp;
        PortalScript.OnLevelEnd -= EndViaPortal;
        ScoreCounterScreen.ContinueButtonClick -= LoadNextLevel;
    }

    private void EnemyKilled(GameObject thisScore)
    {
        _enemies.RemoveAt(_enemies.Count - 1);
        _gameScore += thisScore.GetComponent<BaseEnemy>().Score;
        Debug.Log($"Противников осталось: {_enemies.Count}");
        OnScoreChanged?.Invoke(_gameScore);
    }

    private void PlayerDeath(string _string)
    {
        SceneManager.LoadScene("GameOver");
    }

    private void EnemyCreated(GameObject enemy)
    {
        _enemies.Add(enemy);
        Debug.Log($"Противников : {_enemies.Count}");
    }

    private void LevelKeyPickedUp()
    {
        isLevelKeyPickedUp = true;
        ActivateExitPortal?.Invoke();
    }

    private void EndViaPortal()
    {
        _scoreCounter.ManageScore(_gameScore, _playerHUD.Timer);
        //TODO:Сделать здесь заггрузку экрана подсчета очков и перехода на следующий уровень
    }

    private void LoadNextLevel()
    {
        if(Time.timeScale == 0) Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
