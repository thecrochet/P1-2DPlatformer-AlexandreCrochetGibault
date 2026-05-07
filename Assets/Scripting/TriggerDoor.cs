using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    [SerializeField] private BaseTrigger[] triggers;
    [SerializeField] private GameObject redCube;

    private void Update()
    {
        foreach (var trigger in triggers)
        {
            if (!trigger.activated)
                return;
        }

        OpenDoor();
    }

    void OpenDoor()
    {
        Destroy(gameObject);
        Destroy(redCube);
    }
}