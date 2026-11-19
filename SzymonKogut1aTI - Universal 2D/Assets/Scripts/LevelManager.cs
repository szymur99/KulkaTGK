using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Zarządza jednym poziomem: liczy punkty, pokazuje je w HUD, a po dotarciu do mety
/// ocenia wynik (50% = 1 gwiazdka, 80% = 2, 100% = 3), zapisuje go i pokazuje ekran końca poziomu.
/// Umieść ten skrypt na obiekcie HUD (lub dowolnym aktywnym obiekcie) w każdej scenie poziomu.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Poziom")]
    [Tooltip("Numer poziomu liczony od 0, zgodny z GameProgress.LevelScenes (Level1 = 0, Level2 = 1...).")]
    public int levelIndex;

    [Header("HUD")]
    public Text scoreText;
    public Image[] hudStars;
    public Button menuButton;

    [Header("Ekran końca poziomu")]
    public GameObject resultPanel;
    public Text resultTitleText;
    public Text resultScoreText;
    public Text resultNoteText;
    public Image[] resultStars;
    public Button nextButton;
    public Button retryButton;
    public Button resultMenuButton;

    [Header("Kolory gwiazdek")]
    public Color starOnColor = new Color(1f, 0.82f, 0.1f, 1f);
    public Color starOffColor = new Color(0.22f, 0.22f, 0.22f, 0.85f);

    public int TotalPoints { get; private set; }
    public int CollectedPoints { get; private set; }
    public bool IsFinished { get; private set; }

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        // Suma punktów do zdobycia na poziomie (łącznie z już wyłączonymi przedmiotami).
        TotalPoints = 0;
        Collectible[] all = FindObjectsByType<Collectible>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++) TotalPoints += all[i].points;
    }

    void OnEnable()
    {
        if (menuButton != null) menuButton.onClick.AddListener(GoToMenu);
        if (nextButton != null) nextButton.onClick.AddListener(GoToNext);
        if (retryButton != null) retryButton.onClick.AddListener(Retry);
        if (resultMenuButton != null) resultMenuButton.onClick.AddListener(GoToMenu);
    }

    void OnDisable()
    {
        if (menuButton != null) menuButton.onClick.RemoveListener(GoToMenu);
        if (nextButton != null) nextButton.onClick.RemoveListener(GoToNext);
        if (retryButton != null) retryButton.onClick.RemoveListener(Retry);
        if (resultMenuButton != null) resultMenuButton.onClick.RemoveListener(GoToMenu);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        UpdateHud();
    }

    public void AddPoints(int amount)
    {
        if (IsFinished) return;
        CollectedPoints += amount;
        UpdateHud();
    }

    private void UpdateHud()
    {
        if (scoreText != null) scoreText.text = "Punkty: " + CollectedPoints + " / " + TotalPoints;
        SetStars(hudStars, GameProgress.CalculateStars(CollectedPoints, TotalPoints), CollectedPoints > 0 || TotalPoints == 0);
    }

    private void SetStars(Image[] images, int lit, bool anyProgress)
    {
        if (images == null) return;
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] == null) continue;
            images[i].color = (i < lit && anyProgress) ? starOnColor : starOffColor;
        }
    }

    /// <summary>Wywoływane przez LevelFinish po dotknięciu mety.</summary>
    public void CompleteLevel()
    {
        if (IsFinished) return;
        IsFinished = true;

        int stars = GameProgress.CalculateStars(CollectedPoints, TotalPoints);
        int percent = GameProgress.Percent(CollectedPoints, TotalPoints);

        bool wasCompleted = GameProgress.IsCompleted(levelIndex);
        bool beatBest = wasCompleted && (stars > GameProgress.GetStars(levelIndex) || CollectedPoints > GameProgress.GetBestScore(levelIndex));
        GameProgress.SaveResult(levelIndex, CollectedPoints, TotalPoints);

        // Wyłączamy sterowanie kulką i zatrzymujemy grę pod ekranem wyniku.
        BallController ball = FindFirstObjectByType<BallController>();
        if (ball != null) ball.enabled = false;

        if (resultTitleText != null) resultTitleText.text = "Poziom ukończony!";
        if (resultScoreText != null) resultScoreText.text = "Punkty: " + CollectedPoints + " / " + TotalPoints + "  (" + percent + "%)";
        if (resultNoteText != null)
        {
            string note;
            if (stars >= 3) note = "Perfekcyjnie! Zebrano wszystkie punkty.";
            else
            {
                int missing = GameProgress.PointsNeededForStars(stars + 1, TotalPoints) - CollectedPoints;
                note = "Do następnej gwiazdki brakuje " + missing + " pkt.";
            }
            if (beatBest) note = "Nowy rekord! " + note;
            resultNoteText.text = note;
        }

        bool hasNext = levelIndex + 1 < GameProgress.LevelCount;
        if (nextButton != null) nextButton.gameObject.SetActive(hasNext);

        if (resultPanel != null) resultPanel.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(RevealStars(stars));
    }

    // Gwiazdki pojawiają się po kolei z małą animacją; używamy czasu rzeczywistego, bo gra jest zatrzymana.
    private IEnumerator RevealStars(int lit)
    {
        if (resultStars == null) yield break;

        for (int i = 0; i < resultStars.Length; i++)
        {
            if (resultStars[i] == null) continue;
            resultStars[i].color = starOffColor;
            resultStars[i].rectTransform.localScale = Vector3.one;
        }

        yield return new WaitForSecondsRealtime(0.35f);

        for (int i = 0; i < lit && i < resultStars.Length; i++)
        {
            Image star = resultStars[i];
            if (star == null) continue;
            star.color = starOnColor;

            const float duration = 0.3f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / duration);
                float s = k < 0.6f ? Mathf.Lerp(0.2f, 1.35f, k / 0.6f) : Mathf.Lerp(1.35f, 1f, (k - 0.6f) / 0.4f);
                star.rectTransform.localScale = Vector3.one * s;
                yield return null;
            }
            star.rectTransform.localScale = Vector3.one;
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private void GoToNext() { GameProgress.LoadLevel(levelIndex + 1); }
    private void Retry() { GameProgress.LoadLevel(levelIndex); }
    private void GoToMenu() { GameProgress.LoadMenu(); }
}
