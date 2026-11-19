using UnityEngine;

/// <summary>
/// Zbieralny przedmiot (moneta). Po dotknięciu przez gracza dodaje punkty w LevelManager i znika.
/// Wymaga Collider2D (ustawiany automatycznie jako trigger).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [Tooltip("Ile punktów daje ten przedmiot.")]
    public int points = 10;

    [Header("Animacja")]
    public float bobHeight = 0.12f;
    public float bobSpeed = 2.5f;

    private Vector3 startPosition;
    private bool taken;

    void Awake()
    {
        startPosition = transform.position;
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Update()
    {
        // Delikatne unoszenie się góra-dół; przesunięcie fazy zależne od pozycji, żeby monety nie falowały równo.
        transform.position = startPosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed + startPosition.x) * bobHeight);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (taken) return;

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null || player.IsDead) return;

        taken = true;
        if (LevelManager.Instance != null) LevelManager.Instance.AddPoints(points);
        gameObject.SetActive(false);
    }
}
