using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;

public class BackGroundMusic : MonoBehaviour
{
    public static BackGroundMusic Instance { get; private set; }

    [Header("Настройки")]
    [SerializeField] private AudioSource _sourceA;
    [SerializeField] private AudioSource _sourceB;
    [SerializeField] private GameManager _gameManager;
    [Range(0.5f, 5f)] public float fadeDuration = 2f;
    [Range(0f, 2f)] public float endTolerance = 0.2f; 

    private AudioClip[] _playlist;
    private int _currentIndex = -1;
    private bool _isPaused = false;
    private Coroutine _fadeCoroutine;
    private float _trackStartTime;
    private AudioSource _activeSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        GamePauseEvents.OnGamePaused += PauseMusic;
        GamePauseEvents.OnGameResumed += ResumeMusic;

        ValidateSources();
    }

    private void OnDestroy()
    {
        GamePauseEvents.OnGamePaused -= PauseMusic;
        GamePauseEvents.OnGameResumed -= ResumeMusic;
    }

    public void SetPlaylist(AudioClip[] clips)
    {
        _playlist = clips;
        _currentIndex = -1;
        StopAll();
    }
    public void PlayFirst()
    {
        if (_playlist == null || _playlist.Length == 0) return;
        _currentIndex = 0;
        PlayClipWithCrossfade(_playlist[_currentIndex]);
        _trackStartTime = Time.time;
    }

    public void PlayNext()
    {
        if (_playlist == null || _playlist.Length <= 1) return;

        _currentIndex++;
        if (_currentIndex >= _playlist.Length)
            _currentIndex = 0;

        PlayClipWithCrossfade(GetRandomSong());
        _trackStartTime = Time.time;
    }

    private AudioClip GetRandomSong()
    {
        return _playlist[UnityEngine.Random.Range(0, _gameManager.LevelTracks.Length)];
    }

    public void PlayAtIndex(int index)
    {
        if (_playlist == null || index < 0 || index >= _playlist.Length) return;
        _currentIndex = index;
        PlayClipWithCrossfade(_playlist[_currentIndex]);
        _trackStartTime = Time.time;
    }

    public void CheckForTrackEnd()
    {
        if (_playlist == null || _currentIndex < 0) return;

        AudioClip currentClip = _playlist[_currentIndex];
        float elapsed = Time.time - _trackStartTime;

        if (elapsed >= currentClip.length - endTolerance && !_isPaused)
        {
            PlayNext();
        }
    }

    private void PlayClipWithCrossfade(AudioClip clip)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        var targetSource = (_sourceA.isPlaying) ? _sourceB : _sourceA;
        var otherSource = (targetSource == _sourceA) ? _sourceB : _sourceA;

        targetSource.clip = clip;
        targetSource.volume = 0f;
        targetSource.Play();

        _activeSource = targetSource;
        _fadeCoroutine = StartCoroutine(CrossfadeRoutine(otherSource, targetSource));
    }

    private IEnumerator CrossfadeRoutine(AudioSource fadeOut, AudioSource fadeIn)
    {
        float t = 0f;
        while (t < 1f)
        {
            if (_isPaused)
            {
                yield return null;
                continue;
            }

            t += Time.deltaTime / fadeDuration;
            if (t > 1f) t = 1f;

            fadeOut.volume = Mathf.Lerp(fadeOut.volume, 0f, t);
            fadeIn.volume = Mathf.Lerp(fadeIn.volume, 1f, t);

            yield return null;
        }

        fadeOut.Stop();
        fadeOut.volume = 0f;
    }

    private void PauseMusic()
    {
        _isPaused = true;
        _sourceA.Pause();
        _sourceB.Pause();
    }

    private void ResumeMusic()
    {
        _isPaused = false;
        _sourceA.UnPause();
        _sourceB.UnPause();
    }

    private void StopAll()
    {
        _sourceA.Stop(); _sourceA.volume = 0f;
        _sourceB.Stop(); _sourceB.volume = 0f;
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
    }

    private void ValidateSources()
    {
        if (_sourceA == null) _sourceA = gameObject.AddComponent<AudioSource>();
        if (_sourceB == null) _sourceB = gameObject.AddComponent<AudioSource>();

        _sourceA.playOnAwake = false;
        _sourceB.playOnAwake = false;
        _sourceA.loop = false;
        _sourceB.loop = false;
    }
}
