using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Przycisk jednego poziomu w menu: pokazuje nazwę, gwiazdki i najlepszy wynik,
/// a zablokowany poziom jest wyszarzony i nieklikalny.
/// </summary>
public class LevelButton : MonoBehaviour
{
    [Tooltip("Numer poziomu liczony od 0, zgodny z GameProgress.LevelScenes.")]
    public int levelIndex;

    public Button button;
    public Text titleText;
    public Text statusText;
    public Image[] stars;
    public GameObject lockedOverlay;

    public Color starOnColor = new Color(1f, 0.82f, 0.1f, 1f);
    public Color starOffColor = new Color(0.22f, 0.22f, 0.22f, 0.85f);

    void Awake()
    {
        if (button != null) button.onClick.AddListener(OnClick);
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool unlocked = GameProgress.IsUnlocked(levelIndex);
        bool completed = GameProgress.IsCompleted(levelIndex);
        int earned = completed ? GameProgress.GetStars(levelIndex) : 0;

        if (button != null) button.interactable = unlocked;
        if (lockedOverlay != null) lockedOverlay.SetActive(!unlocked);

        if (titleText != null)
            titleText.text = levelIndex < GameProgress.LevelNames.Length ? GameProgress.LevelNames[levelIndex] : "Poziom " + (levelIndex + 1);

        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null) stars[i].color = (unlocked && i < earned) ? starOnColor : starOffColor;
            }
        }

        if (statusText != null)
        {
            if (!unlocked) statusText.text = "Zablokowany - ukończ poprzedni poziom";
            else if (!completed) statusText.text = "Nie ukończono";
            else statusText.text = "Rekord: " + GameProgress.GetBestScore(levelIndex) + " / " + GameProgress.GetBestTotal(levelIndex) + " pkt";
        }
    }

    private void OnClick()
    {
        if (GameProgress.IsUnlocked(levelIndex)) GameProgress.LoadLevel(levelIndex);
    }
}
