using UnityEngine;
using UnityEngine.UI;

/// <summary>Zielony pasek życia + tekst z procentem. Pasek skaluje się przez anchorMax, więc nie wymaga sprite'a.</summary>
public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth health;
    public RectTransform fillRect;
    public Text percentText;

    void OnEnable()
    {
        if (health != null) health.OnHealthChanged += Refresh;
    }

    void OnDisable()
    {
        if (health != null) health.OnHealthChanged -= Refresh;
    }

    void Start()
    {
        if (health != null) Refresh(health.CurrentHealth, health.maxHealth);
    }

    private void Refresh(int current, int max)
    {
        float p = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;

        if (fillRect != null) fillRect.anchorMax = new Vector2(p, 1f);
        if (percentText != null) percentText.text = Mathf.RoundToInt(p * 100f) + "%";
    }
}
