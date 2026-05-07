using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    private int targetCount =0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void NotifyTargetDestroyed()
    {
        targetCount--;
        Debug.Log("Target destroyed. Remaining targets: " + targetCount);
    }
}
