using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScoreCounterScreen : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _scoreScreen;
    [SerializeField] private TextMeshProUGUI _scoreView;
    [SerializeField] private TextMeshProUGUI _rankView;
    [SerializeField] private Button _btnContinue;
    [SerializeField] private GameObject[] _elements;
    private PlayerInput _playerInput;

    private int _score;
    private char _rank;

    public static event Action ContinueButtonClick;

    private void Awake()
    {
        if (_playerInput = _player.GetComponent<PlayerInput>())
        {
            Debug.Log("Player Input Success");
        }
    }

    public void ManageScore(int gameScore, float levelTime)
    {
        HideHudElements(_elements);
        _playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        _scoreScreen.SetActive(true);
        _scoreView.text = $"Score:{CalculateScore(gameScore, levelTime)}";
        _rankView.text = $"Rank:{CalculateRank(_score)}";
        Time.timeScale = 0;
    }

    private int CalculateScore(int gameScore, float levelTime)
    {
        _score = (int)(gameScore * levelTime) / 100;
        return _score;
    }

    private char CalculateRank(int levelScore)
    {
        if (levelScore == 0) _rank = 'F';
        if (levelScore < 0 && levelScore > 3) _rank = 'D';
        if (levelScore <= 3 && levelScore > 5) _rank = 'C';
        if (levelScore <= 5 && levelScore > 7) _rank = 'B';
        if (levelScore <= 7 && levelScore > 9) _rank = 'A';
        if (levelScore >= 10) _rank = 'S';
        return _rank;
    }

    public void OnContinueBtnClick()
    {
        ContinueButtonClick?.Invoke();
    }

    private void HideHudElements(GameObject[] Elements)
    {
        foreach (GameObject element in Elements)
        {
            element.SetActive(false);
        }
    }

    private void ShowHudElements(GameObject[] Elements)
    {
        foreach (GameObject element in Elements)
        {
            element.SetActive(true);
        }
    }
}
