using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu główne z wyborem poziomów. Przyciski poziomów (LevelButton) same się odświeżają;
/// tutaj obsługiwane są Wyjście i Reset postępu (reset wymaga potwierdzenia drugim kliknięciem).
/// </summary>
public class MainMenu : MonoBehaviour
{
    public LevelButton[] levelButtons;
    public Text totalStarsText;
    public Button quitButton;
    public Button resetButton;
    public Text resetLabel;

    private bool confirmReset;
    private Coroutine cancelRoutine;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void OnEnable()
    {
        if (quitButton != null) quitButton.onClick.AddListener(Quit);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);
        Refresh();
    }

    void OnDisable()
    {
        if (quitButton != null) quitButton.onClick.RemoveListener(Quit);
        if (resetButton != null) resetButton.onClick.RemoveListener(OnResetClicked);
    }

    public void Refresh()
    {
        if (levelButtons != null)
        {
            for (int i = 0; i < levelButtons.Length; i++)
                if (levelButtons[i] != null) levelButtons[i].Refresh();
        }
        if (totalStarsText != null)
            totalStarsText.text = "Gwiazdki: " + GameProgress.TotalStars() + " / " + (GameProgress.LevelCount * 3);
    }

    private void OnResetClicked()
    {
        if (!confirmReset)
        {
            confirmReset = true;
            if (resetLabel != null) resetLabel.text = "Na pewno? Kliknij ponownie";
            if (cancelRoutine != null) StopCoroutine(cancelRoutine);
            cancelRoutine = StartCoroutine(CancelConfirmAfterDelay());
            return;
        }

        GameProgress.ResetAll();
        ResetConfirmState();
        Refresh();
    }

    private IEnumerator CancelConfirmAfterDelay()
    {
        yield return new WaitForSecondsRealtime(3f);
        ResetConfirmState();
    }

    private void ResetConfirmState()
    {
        confirmReset = false;
        if (resetLabel != null) resetLabel.text = "Resetuj postęp";
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
