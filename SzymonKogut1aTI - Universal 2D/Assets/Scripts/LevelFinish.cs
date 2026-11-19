using UnityEngine;

/// <summary>
/// Meta poziomu. Gdy żywy gracz wejdzie w trigger tego obiektu, poziom zostaje ukończony.
/// Wymaga Collider2D z włączonym Is Trigger (np. Stone of Recall ma go już w prefabie).
/// </summary>
public class LevelFinish : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null || player.IsDead) return;

        if (LevelManager.Instance != null) LevelManager.Instance.CompleteLevel();
    }
}
