using UnityEngine;

public class EnemyDoor : MonoBehaviour
{

    [SerializeField] private GameObject blueCube;

    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            Destroy(gameObject);
            Destroy(blueCube);
        }
    }
}