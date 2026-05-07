using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform m_FollowTarget;

    [SerializeField] private Vector3 m_Offset = new Vector3(0,1,-10);

    [Header("Boundaries")]
    [SerializeField] private bool useBound = true;
    [SerializeField] float minX = 0;
    [SerializeField] float maxX = 0;
    [SerializeField] float minY = 0;
    [SerializeField] float maxY = 0;


    float smoothTime = 0.25f;
    Vector3 velocity;

    private void LateUpdate()
    {
        if(m_FollowTarget == null) return;

        Vector3 desiredTarget = m_FollowTarget.position + m_Offset;
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, desiredTarget, ref velocity, smoothTime);

        if(useBound)
        {
            smoothPosition.x = Mathf.Clamp(smoothPosition.x, minX, maxX);
            smoothPosition.y = Mathf.Clamp(smoothPosition.y, minY, maxY);
        }


        transform.position = smoothPosition;
    }

}
