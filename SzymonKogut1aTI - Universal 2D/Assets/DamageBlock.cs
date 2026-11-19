using UnityEngine;

/// <summary>
/// Solidny blok zadający obrażenia graczowi przy dotknięciu.
/// Dopóki gracz dotyka bloku, obrażenia są ponawiane - tempo ogranicza damageCooldown w PlayerHealth.
/// </summary>
public class DamageBlock : MonoBehaviour
{
    [Tooltip("Ile punktów życia traci gracz po dotknięciu bloku.")]
    public int damage = 25;

    private PlayerHealth touching;

void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth health = collision.collider.GetComponentInParent<PlayerHealth>();
        if (health == null || health.IsDead) return;

        SetTouching(health);
        touching.TakeDamage(damage);
    }

void OnCollisionExit2D(Collision2D collision)
    {
        if (touching == null) return;
        if (collision.collider.GetComponentInParent<PlayerHealth>() == touching) SetTouching(null);
    }

    // Update zamiast OnCollisionStay2D: uśpione Rigidbody2D (kulka leżąca na bloku) nie wysyła zdarzeń Stay.
void Update()
    {
        if (touching == null) return;
        if (touching.IsDead) { SetTouching(null); return; }
        touching.TakeDamage(damage);
    }

void OnDisable()
    {
        SetTouching(null);
    }

// Po śmierci gracza zapominamy o nim, żeby po respawnie nie dostał obrażeń od starego kontaktu.
    private void SetTouching(PlayerHealth health)
    {
        if (touching != null) touching.OnDied -= ClearTouching;
        touching = health;
        if (touching != null) touching.OnDied += ClearTouching;
    }

    private void ClearTouching()
    {
        SetTouching(null);
    }

}
