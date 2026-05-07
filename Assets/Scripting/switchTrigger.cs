using UnityEngine;

public class switchTrigger : BaseTrigger
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            activated = true;
            Destroy(gameObject);
        }
    }
}