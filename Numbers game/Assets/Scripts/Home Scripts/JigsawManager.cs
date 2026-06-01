using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class JigsawManager : MonoBehaviour
{
    public static JigsawManager Instance;

    [Header("UI References")]
    [SerializeField] private SignGameManager signGameManager;
    [SerializeField] private TextMeshProUGUI totalStarsDisplay;
    [SerializeField] private GameObject exitWarningPanel;

    [Header("Win Panel")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private CanvasGroup winCanvasGroup;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup exitCanvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound;

    [Header("Win Effects")]
    [SerializeField] private ParticleSystem confetti_1;
    [SerializeField] private ParticleSystem confetti_2;

    [Header("Navigation Settings")]
    [Tooltip("The name of the planet scene to select minigame")]
    public string minigamesScene = "planetsScene";

    [Header("Puzzle Star Payout")]
    [Tooltip("How many stars should the player earn for completing this level?")]
    public int starsToReward = 3;

    [Header("Jigsaw Dynamic Target Data")]
    public string aiTargetWord;
    public string displayWord;
    public UnityEngine.Video.VideoClip instructionalVideo;

    [HideInInspector] public bool TileMovementEnabled = false;
    [HideInInspector] public int totalTilesInCorrectPosition = 0;
    [HideInInspector] public int totalTilesNeeded = 0;


    private bool levelRewardSaved = false;
    private bool levelFinished = false;

    [HideInInspector] public TilesSorting localSorting = new TilesSorting();
    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        levelRewardSaved = false;
        levelFinished = false;
        totalTilesInCorrectPosition = 0;

        // Cleaned up! No more reading from JigsawLevelButton static fields.
        // The variables (aiTargetWord, displayWord, instructionalVideo) are now 
        // typed out directly into this script's inspector for each level scene.

        UpdateTotalStarsUI();
        SetupExitPanel();
        SetupWinPanel();
    }


    // Triggered automatically by BoardGen when tile generation finishes
    public void StartGame(GameObject[,] tiles, int numX, int numY)
    {
        totalTilesNeeded = numX * numY;
        StartCoroutine(Coroutine_Shuffle(tiles, numX, numY));
    }

    // Triggered automatically by TileMovement when a piece snaps into place
    public void AddCorrectTile()
    {
        if (levelFinished) return;

        totalTilesInCorrectPosition++;

        // STEP 1: The entire jigsaw puzzle is solved
        if (totalTilesInCorrectPosition >= totalTilesNeeded)
        {
            TileMovementEnabled = false;
            OpenSignLanguagePanel(); // Open the teaching module silently
        }
    }

    // STEP 2: Hook this function up to your Sign Panel's Continue Button Click Event!
    public void CheckForWin()
    {
        if (!levelRewardSaved)
        {
            PlayStarSound();
            SaveLevelStarsToTotal();
            WinLevel(); // Shows the win panel and triggers confetti particles
            
        }
    }

    private void SaveLevelStarsToTotal()
    {
        GameData.AddStars(starsToReward);
        levelRewardSaved = true;
        UpdateTotalStarsUI();

        TotalStarsDisplay display = FindObjectOfType<TotalStarsDisplay>();
        if (display != null)
        {
            display.RefreshDisplay();
        }

        Debug.Log("Stars saved to total!");
    }

    private void UpdateTotalStarsUI()
    {
        if (totalStarsDisplay != null)
            totalStarsDisplay.text = GameData.GetTotalStars().ToString();
    }

    private void WinLevel()
    {
        levelFinished = true;
        Debug.Log("Level finished!");
        ShowWinPanel();
    }

    private void OpenSignLanguagePanel()
    {
        if (signGameManager != null && !levelFinished)
        {
            signGameManager.OpenPuzzle(aiTargetWord, displayWord, instructionalVideo);
        }
    }

    public void ResumeGame() => Time.timeScale = 1f;

    private void PlayStarSound()
    {
        if (audioSource == null) return;
        if (winSound != null) audioSource.PlayOneShot(winSound);
        else audioSource.Play();
    }

    // --- PAUSE, EXIT & PANEL ANIMATIONS ---

    private void SetupExitPanel()
    {
        if (exitWarningPanel != null)
        {
            exitWarningPanel.SetActive(false);
            if (exitCanvasGroup != null) exitCanvasGroup.alpha = 0;
        }
    }

    private void SetupWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
            if (winCanvasGroup != null) winCanvasGroup.alpha = 0;
        }
    }

    public void ShowExitWarning()
    {
        if (exitWarningPanel != null && exitCanvasGroup != null)
        {
            exitWarningPanel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(exitCanvasGroup, 0f, 1f));
            Time.timeScale = 0f;
        }
    }

    public void CancelExit()
    {
        if (exitWarningPanel != null && exitCanvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(exitCanvasGroup, 1f, 0f, () =>
            {
                exitWarningPanel.SetActive(false);
                Time.timeScale = 1f;
            }));
        }
    }

    private void ShowWinPanel()
    {
        if (winPanel != null) winPanel.SetActive(true);
        if (winCanvasGroup != null)
        {
            StartCoroutine(FadeCanvasGroup(winCanvasGroup, 0f, 1f));
        }

        if (confetti_1 != null) { confetti_1.Clear(); confetti_1.Play(); }
        if (confetti_2 != null) { confetti_2.Clear(); confetti_2.Play(); }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToPlanets()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(minigamesScene);
    }

    public void ConfirmExitAndGoToPlanets()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(minigamesScene);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, Action onComplete = null)
    {
        float elapsedTime = 0f;
        float startTime = Time.unscaledTime;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime = Time.unscaledTime - startTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        onComplete?.Invoke();
    }

    // --- SHUFFLING ENGINE ---
    private IEnumerator Coroutine_Shuffle(GameObject[,] mTileGameObjects, int numTileX, int numTileY)
    {
        float boardWidth = numTileX * 100f;
        float boardHeight = numTileY * 100f;

        List<Rect> regions = new List<Rect>
        {
            new Rect(-boardWidth * 0.8f, 0f, boardWidth * 0.5f, boardHeight),
            new Rect(boardWidth * 1.3f, 0f, boardWidth * 0.5f, boardHeight)
        };

        for (int i = 0; i < numTileX; ++i)
        {
            for (int j = 0; j < numTileY; ++j)
            {
                ShuffleTile(mTileGameObjects[i, j], regions);
                yield return null;
            }
        }

        foreach (var item in activeCoroutines) yield return item;
        activeCoroutines.Clear();
        TileMovementEnabled = true;
    }

    private void ShuffleTile(GameObject obj, List<Rect> regions)
    {
        int regionIndex = UnityEngine.Random.Range(0, regions.Count);
        float x = UnityEngine.Random.Range(regions[regionIndex].xMin, regions[regionIndex].xMax);
        float y = UnityEngine.Random.Range(regions[regionIndex].yMin, regions[regionIndex].yMax);

        Vector3 pos = new Vector3(x, y, 0.0f);
        Coroutine moveCoroutine = StartCoroutine(Coroutine_MoveOverSeconds(obj, pos, 1.0f));
        activeCoroutines.Add(moveCoroutine);
    }

    private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }
}