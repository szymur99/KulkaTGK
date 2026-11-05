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
        if (health == null) return;

        touching = health;
        touching.TakeDamage(damage);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (touching == null) return;
        if (collision.collider.GetComponentInParent<PlayerHealth>() == touching) touching = null;
    }

    // Update zamiast OnCollisionStay2D: uśpione Rigidbody2D (kulka leżąca na bloku) nie wysyła zdarzeń Stay.
    void Update()
    {
        if (touching != null) touching.TakeDamage(damage);
    }

    void OnDisable()
    {
        touching = null;
    }
}
