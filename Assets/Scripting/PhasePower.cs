using UnityEngine;

public class PhasePower : MonoBehaviour
{
    [SerializeField] private float duration = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            StartCoroutine(PhaseRoutine(other.gameObject));
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator PhaseRoutine(GameObject player)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wall"), true);

        yield return new WaitForSeconds(duration);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wall"), false);
    }
}