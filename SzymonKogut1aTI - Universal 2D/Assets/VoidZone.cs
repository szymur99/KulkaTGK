using UnityEngine;

/// <summary>
/// Strefa pustki (void) pod mapą. Gracz, który w nią wpadnie, umiera natychmiast
/// i wraca na start przez standardowy mechanizm śmierci/respawnu w PlayerHealth.
/// Wymaga Collider2D z włączonym Is Trigger.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class VoidZone : MonoBehaviour
{
    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health != null) health.Kill();
    }
}
