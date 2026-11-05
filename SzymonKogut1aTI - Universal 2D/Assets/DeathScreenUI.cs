using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pokazuje panel „Umarłeś” z przyciskiem Respawn, gdy gracz zginie.
/// Ten skrypt ma być na aktywnym obiekcie (np. HUD), a nie na samym panelu, który bywa wyłączony.
/// </summary>
public class DeathScreenUI : MonoBehaviour
{
    public PlayerHealth health;
    public GameObject panel;
    public Button respawnButton;

    void OnEnable()
    {
        if (health != null)
        {
            health.OnDied += Show;
            health.OnRespawned += Hide;
        }
        if (respawnButton != null) respawnButton.onClick.AddListener(OnRespawnClicked);
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.OnDied -= Show;
            health.OnRespawned -= Hide;
        }
        if (respawnButton != null) respawnButton.onClick.RemoveListener(OnRespawnClicked);
    }

    void Start()
    {
        if (panel != null) panel.SetActive(health != null && health.IsDead);
    }

    private void Show()
    {
        if (panel != null) panel.SetActive(true);
    }

    private void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void OnRespawnClicked()
    {
        if (health != null) health.Respawn();
    }
}
