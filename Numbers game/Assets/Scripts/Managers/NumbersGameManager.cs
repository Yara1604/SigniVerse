using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
public class NumbersGameManager : MonoBehaviour{
    public static NumbersGameManager Instance;

    [Header("UI References")]
    [SerializeField] private SignGameManager signGameManager;
    [SerializeField] private UnityEngine.Video.VideoClip[] numberSignVideos; [SerializeField] private TextMeshProUGUI panelStarText;
    [SerializeField] private GameObject[] fullStars;
    [SerializeField] private TextMeshProUGUI countText;
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

    [Header("Win Condition")]
    [SerializeField] private int starsToWin = 10;

    [Header("Win Effects")]
    [SerializeField] private ParticleSystem confetti_1;
    [SerializeField] private ParticleSystem confetti_2;

    private int currentStars = 0;
    private bool levelRewardSaved = false;
    private bool levelFinished = false;

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
        ResetStars();
        UpdateCurrentStarsUI();
        UpdateTotalStarsUI();
        SetupExitPanel();
        SetupWinPanel();
    }

    public void AddStar()
    {
        if (levelFinished) return;
        if (fullStars == null || fullStars.Length == 0) return;
        if (currentStars >= fullStars.Length) return;

        if (fullStars[currentStars] != null)
            fullStars[currentStars].SetActive(true);

        currentStars++;
        UpdateCurrentStarsUI();
        PlayStarSound();
        OpenSignLanguagePanel();

        if (currentStars >= starsToWin && !levelRewardSaved)
        {
            SaveLevelStarsToTotal();
            WinLevel();
        }
    }

    private void SaveLevelStarsToTotal()
    {
        GameData.AddStars(currentStars);
        levelRewardSaved = true;
        UpdateTotalStarsUI();

        TotalStarsDisplay display = FindObjectOfType<TotalStarsDisplay>();
        if (display != null)
        {
            display.RefreshDisplay();
        }

        Debug.Log("تم حفظ نجوم المرحلة في الرصيد الكلي");
    }

    private void ResetStars()
    {
        if (fullStars == null) return;

        foreach (GameObject star in fullStars)
        {
            if (star != null)
                star.SetActive(false);
        }

        currentStars = 0;
        levelRewardSaved = false;
        levelFinished = false;
    }

    private void UpdateCurrentStarsUI()
    {
        if (countText != null)
            countText.text = currentStars.ToString();

        if (panelStarText != null)
            panelStarText.text = currentStars.ToString();
    }

    private void UpdateTotalStarsUI()
    {
        if (totalStarsDisplay != null)
            totalStarsDisplay.text = GameData.GetTotalStars().ToString();
    }

    private void WinLevel()
    {
        levelFinished = true;
        Debug.Log("مبروك! اكتمل المستوى.");
        ShowWinPanel();
    }

    private void OpenSignLanguagePanel()
    {
        if (signGameManager != null && !levelFinished)
        {
            UnityEngine.Video.VideoClip currentVideo = null;
            if (currentStars - 1 >= 0 && currentStars - 1 < numberSignVideos.Length)
            {
                currentVideo = numberSignVideos[currentStars - 1];
            }

            // 1. What the AI looks for (e.g., "1", "2", "3")
            string aiTarget = currentStars.ToString();

            // 2. What the text box actually displays!
            string displayText = currentStars.ToString();

            // Send both to the AI Manager
            signGameManager.OpenPuzzle(aiTarget, displayText, currentVideo);

        }
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    private void PlayStarSound()
    {
        if (audioSource == null) return;

        if (currentStars >= starsToWin && winSound != null)
            audioSource.PlayOneShot(winSound);
        else
            audioSource.Play();
    }

    private void SetupExitPanel()
    {
        if (exitWarningPanel != null)
        {
            exitWarningPanel.SetActive(false);

            if (exitCanvasGroup != null)
                exitCanvasGroup.alpha = 0;
        }
    }

    private void SetupWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);

            if (winCanvasGroup != null)
                winCanvasGroup.alpha = 0;
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

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winCanvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(winCanvasGroup, 0f, 1f));
        }

        if (confetti_1 != null)
        {
            confetti_1.Clear();
            confetti_1.Play();
        }

        if (confetti_2 != null)
        {
            confetti_2.Clear();
            confetti_2.Play();
        }

        // لا تضعي Time.timeScale = 0 هنا الآن
        // حتى يظهر تأثير الفوز بشكل صحيح
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToPlanets()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("planetsScene");
    }

    public void ConfirmExitAndGoToPlanets()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("planetsScene");
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
    }}
 
 