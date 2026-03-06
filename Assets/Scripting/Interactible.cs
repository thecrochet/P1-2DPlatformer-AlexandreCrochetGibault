
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Interactible : MonoBehaviour
{
    [Tooltip("Name of the scene to load when the player touches this object.")]
    [SerializeField] private string sceneName = "Alex_C";

  
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log($"Interactible triggered by {other.name}. Loading scene '{sceneName}'.");
        SceneManager.LoadScene(sceneName);
    }
}