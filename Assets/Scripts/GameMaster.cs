using System.Collections;
using TMPro;
using UnityEngine;

public class GameMaster : Singleton<GameMaster>
{
    public enum GameStates
    {
        WaitingForPlayer,
        Playing,
        Finished
    }

    [Header("Game Settings")]
    public float gameDuration = 60f;
    public float easyModeSpawnInterval = 2f;
    public float easyModeTargetLifetime = 5f;
    public float mediumModeSpawnInterval = 1f;
    public float mediumModeTargetLifetime = 3f;
    public float hardModeSpawnInterval = 0.5f;
    public float hardModeTargetLifetime = 1f;


    [Header("Targets")]
    public Target easyTarget;
    public Target mediumTarget;
    public Target hardTarget;
    public Target targetPrefab;
    public Transform[] targetSpawns;
    public AudioClip targetHitSound;

    private AudioSource audioSource;
    private GameStates currentState;
    private float points;
    private Coroutine gameLoopCoroutine;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    float elapsed = 0f;
    private float spawnInterval = 2f;
    private float targetLifetime = 4f;

    private Target currentTarget;

    private void Start()
    {
        scoreText.text = "Score: 0";
        timerText.text = $"Time: {gameDuration:0}";
        currentState = GameStates.WaitingForPlayer;
        audioSource = GetComponent<AudioSource>();
        ActivateStartTarget();
    }

    public void HitTarget(Target target)
    {

        switch (currentState)
        {
            case GameStates.WaitingForPlayer:
                if (target == easyTarget || target == mediumTarget || target == hardTarget)
                {
                    audioSource.PlayOneShot(targetHitSound);
                    DeactivateStartTarget();
                    currentState = GameStates.Playing;
                    gameLoopCoroutine = StartCoroutine(GameLoop());

                    if (target == easyTarget)
                    {
                        spawnInterval = easyModeSpawnInterval;
                        targetLifetime = easyModeTargetLifetime;
                    }
                    else if (target == mediumTarget)
                    {
                        spawnInterval = mediumModeSpawnInterval;
                        targetLifetime = mediumModeTargetLifetime;
                    }
                    else if (target == hardTarget)
                    {
                        spawnInterval = hardModeSpawnInterval;
                        targetLifetime = hardModeTargetLifetime;
                    }
                }
                break;

            case GameStates.Playing:
                audioSource.PlayOneShot(targetHitSound);
                points += 10;
                scoreText.text = $"Score: {points}";
                Destroy(target.gameObject);
                if (target == currentTarget)
                    currentTarget = null;
                break;

            case GameStates.Finished:
                break;
        }
    }

    private void Update()
    {
        if (currentState == GameStates.Playing)
            timerText.text = $"Time: {60 - elapsed: 0}";
    }

    private IEnumerator GameLoop()
    {
        elapsed = 0f;

        while (elapsed < gameDuration && currentState == GameStates.Playing)
        {
            SpawnTarget();

            float lifetimeTimer = 0f;
            while (lifetimeTimer < targetLifetime && currentState == GameStates.Playing && currentTarget != null)
            {
                yield return null;
                lifetimeTimer += Time.deltaTime;
                elapsed += Time.deltaTime;
            }

            if (currentTarget != null && currentState == GameStates.Playing)
            {
                Destroy(currentTarget.gameObject);
                currentTarget = null;
            }

            float intervalTimer = 0f;
            while (intervalTimer < spawnInterval && currentState == GameStates.Playing)
            {
                yield return null;
                intervalTimer += Time.deltaTime;
                elapsed += Time.deltaTime;
            }
        }

        currentState = GameStates.Finished;
        yield return StartCoroutine(FinishGame());
    }

    private void SpawnTarget()
    {
        if (targetSpawns.Length == 0) return;

        Transform spawn = targetSpawns[Random.Range(0, targetSpawns.Length)];
        currentTarget = Instantiate(targetPrefab, spawn.position, spawn.rotation);
    }

    private void ActivateStartTarget()
    {
        easyTarget.gameObject.SetActive(true);
        mediumTarget.gameObject.SetActive(true);
        hardTarget.gameObject.SetActive(true);
    }

    private void DeactivateStartTarget()
    {
        easyTarget.gameObject.SetActive(false);
        mediumTarget.gameObject.SetActive(false);
        hardTarget.gameObject.SetActive(false);
    }

    private IEnumerator FinishGame()
    {
        Debug.Log($"Game finished! Score: {points}");
        scoreText.text = $"Score: {points}";
        timerText.text = $"Time: 0";

        yield return new WaitForSeconds(2f);

        if (currentTarget != null)
        {
            Destroy(currentTarget.gameObject);
            currentTarget = null;
        }

        points = 0f;

        currentState = GameStates.WaitingForPlayer;
        ActivateStartTarget();
    }

    public void ForceReset()
    {
        if (gameLoopCoroutine != null)
            StopCoroutine(gameLoopCoroutine);
        currentTarget = null;
        StartCoroutine(FinishGame());
    }
}
