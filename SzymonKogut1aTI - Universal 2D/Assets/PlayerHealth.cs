using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    [Tooltip("Czas (s) po otrzymaniu obrażeń, w którym kolejne obrażenia są ignorowane.")]
    public float damageCooldown = 0.5f;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    // (aktualne życie, maksymalne życie)
    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;
    public event Action OnRespawned;

    private float lastDamageTime = float.NegativeInfinity;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody2D rb;
    private BallController controller;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<BallController>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        CurrentHealth = maxHealth;
    }

    /// <summary>Zwraca true, jeśli obrażenia zostały zadane (nie trafiły w cooldown ani w martwego gracza).</summary>
    public bool TakeDamage(int amount)
    {
        if (IsDead || amount <= 0) return false;
        if (Time.time < lastDamageTime + damageCooldown) return false;

        lastDamageTime = Time.time;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth == 0) Die();
        return true;
    }

    private void Die()
    {
        IsDead = true;

        // Wyłączamy sterowanie, fizyka kulki (Rigidbody2D) zostaje nietknięta.
        if (controller != null) controller.enabled = false;
        StopMotion();

        OnDied?.Invoke();
    }

    public void Respawn()
    {
        transform.SetPositionAndRotation(startPosition, startRotation);
        if (rb != null)
        {
            rb.position = startPosition;
            rb.rotation = startRotation.eulerAngles.z;
        }
        StopMotion();

        CurrentHealth = maxHealth;
        IsDead = false;
        lastDamageTime = float.NegativeInfinity;

        if (controller != null) controller.enabled = true;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnRespawned?.Invoke();
    }

    private void StopMotion()
    {
        if (rb == null) return;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
