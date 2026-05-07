using UnityEngine;

public class BossDoor : MonoBehaviour
{

    [SerializeField] private GameObject greyCube;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Boss");

        if (enemies.Length == 0)
        {
            Destroy(gameObject);
            Destroy(greyCube);
        }
    }
}
