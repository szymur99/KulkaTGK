using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lista poziomów oraz zapis postępu gracza (PlayerPrefs): gwiazdki, najlepszy wynik, odblokowania.
/// Aby dodać nowy poziom: dopisz scenę i nazwę poniżej oraz dodaj scenę do Build Settings.
/// </summary>
public static class GameProgress
{
    public const string MenuScene = "MainMenu";

    // Indeks poziomu = indeks w tych tablicach (od 0).
    public static readonly string[] LevelScenes = { "Level1", "Level2" };
    public static readonly string[] LevelNames = { "Poziom 1", "Poziom 2" };

    // Progi punktowe (w % zebranych punktów) dla 1, 2 i 3 gwiazdek.
    public const int OneStarPercent = 50;
    public const int TwoStarPercent = 80;
    public const int ThreeStarPercent = 100;

    public static int LevelCount { get { return LevelScenes.Length; } }

    private static string Key(int level, string what) { return "level" + level + "_" + what; }

    /// <summary>0 gwiazdek (&lt;50%), 1 gwiazdka (&gt;=50%), 2 gwiazdki (&gt;=80%), 3 gwiazdki (100%).</summary>
    public static int CalculateStars(int collected, int total)
    {
        if (total <= 0) return 3; // nie ma nic do zebrania - ukończenie = komplet
        if (collected * 100 >= total * ThreeStarPercent) return 3;
        if (collected * 100 >= total * TwoStarPercent) return 2;
        if (collected * 100 >= total * OneStarPercent) return 1;
        return 0;
    }

    /// <summary>Procent zebranych punktów (zaokrąglony w dół, żeby nie pokazywać 80% przy 79,6%).</summary>
    public static int Percent(int collected, int total)
    {
        if (total <= 0) return 100;
        return Mathf.FloorToInt(100f * collected / total);
    }

    /// <summary>Ile punktów trzeba mieć łącznie, żeby dostać daną liczbę gwiazdek.</summary>
    public static int PointsNeededForStars(int stars, int total)
    {
        int pct = stars <= 1 ? OneStarPercent : (stars == 2 ? TwoStarPercent : ThreeStarPercent);
        return (total * pct + 99) / 100; // sufit
    }

    public static bool IsCompleted(int level) { return PlayerPrefs.GetInt(Key(level, "done"), 0) == 1; }
    public static int GetStars(int level) { return PlayerPrefs.GetInt(Key(level, "stars"), 0); }
    public static int GetBestScore(int level) { return PlayerPrefs.GetInt(Key(level, "score"), 0); }
    public static int GetBestTotal(int level) { return PlayerPrefs.GetInt(Key(level, "total"), 0); }

    /// <summary>Pierwszy poziom jest zawsze odblokowany, kolejne po ukończeniu poprzedniego.</summary>
    public static bool IsUnlocked(int level) { return level <= 0 || IsCompleted(level - 1); }

    public static int TotalStars()
    {
        int sum = 0;
        for (int i = 0; i < LevelCount; i++) sum += GetStars(i);
        return sum;
    }

    /// <summary>Zapisuje wynik, ale tylko jeśli jest lepszy od dotychczasowego.</summary>
    public static void SaveResult(int level, int collected, int total)
    {
        int stars = CalculateStars(collected, total);
        bool firstTime = !IsCompleted(level);

        if (firstTime || stars > GetStars(level)) PlayerPrefs.SetInt(Key(level, "stars"), stars);
        if (firstTime || collected > GetBestScore(level))
        {
            PlayerPrefs.SetInt(Key(level, "score"), collected);
            PlayerPrefs.SetInt(Key(level, "total"), total);
        }
        PlayerPrefs.SetInt(Key(level, "done"), 1);
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        for (int i = 0; i < LevelCount; i++)
        {
            PlayerPrefs.DeleteKey(Key(i, "done"));
            PlayerPrefs.DeleteKey(Key(i, "stars"));
            PlayerPrefs.DeleteKey(Key(i, "score"));
            PlayerPrefs.DeleteKey(Key(i, "total"));
        }
        PlayerPrefs.Save();
    }

    public static void LoadLevel(int level)
    {
        if (level < 0 || level >= LevelCount) { LoadMenu(); return; }
        Time.timeScale = 1f;
        SceneManager.LoadScene(LevelScenes[level]);
    }

    public static void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MenuScene);
    }
}
