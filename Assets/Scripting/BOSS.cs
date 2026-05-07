
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BOSS : MonoBehaviour
{
    [Header("Shooting")]
    [Tooltip("Bullet prefab (should have a Bullet or BossBullet component)")]
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("Left eye / muzzle transform")]
    [SerializeField] private Transform leftEye;
    [Tooltip("Right eye / muzzle transform")]
    [SerializeField] private Transform rightEye;
    [Tooltip("Seconds between shots (both eyes fired each time)")]
    [SerializeField] private float fireInterval = 1.5f;
    [Tooltip("Optional delay between left and right eye when firing (0 = simultaneous)")]
    [SerializeField] private float eyeStagger = 0f;

    [Header("Optional")]
    [Tooltip("If true boss will aim at player position; otherwise fires horizontally toward player sign.")]
    [SerializeField] private bool aimAtPlayer = true;

    [Header("Eye targets (assign or auto-find)")]
    [Tooltip("Target component representing the left eye target object")]
    [SerializeField] private Target leftEyeTarget;
    [Tooltip("Target component representing the right eye target object")]
    [SerializeField] private Target rightEyeTarget;

    [Header("Spawn")]
    [Tooltip("Small offset to spawn bullets outside of the boss collider to avoid immediate self-collisions")]
    [SerializeField] private float spawnOffset = 0.2f;

    private Transform player;
    private Coroutine firingRoutine;

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Auto-find Target components if not assigned
        if (leftEyeTarget == null && leftEye != null)
            leftEyeTarget = leftEye.GetComponentInChildren<Target>();
        if (rightEyeTarget == null && rightEye != null)
            rightEyeTarget = rightEye.GetComponentInChildren<Target>();

        // If still null, try to find any child Targets (useful if eyes are children)
        if (leftEyeTarget == null || rightEyeTarget == null)
        {
            var childTargets = GetComponentsInChildren<Target>();
            foreach (var t in childTargets)
            {
                if (leftEyeTarget == null) leftEyeTarget = t;
                else if (rightEyeTarget == null && t != leftEyeTarget) rightEyeTarget = t;
                if (leftEyeTarget != null && rightEyeTarget != null) break;
            }
        }

        // Subscribe to health change events so we can react as soon as an eye dies
        if (leftEyeTarget != null)
            leftEyeTarget.OnHealthChanged.AddListener(OnEyeHealthChanged);
        if (rightEyeTarget != null)
            rightEyeTarget.OnHealthChanged.AddListener(OnEyeHealthChanged);

        // If both already missing (edge case), destroy boss immediately
        if (AreBothEyesDestroyed())
        {
            DestroyBoss();
            return;
        }

        if (bulletPrefab != null && (leftEye != null || rightEye != null))
            firingRoutine = StartCoroutine(FireRoutine());
        else if (bulletPrefab == null)
            Debug.LogWarning("BOSS: bulletPrefab not assigned.");
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            if (player != null)
            {
                // Left eye
                if (leftEye != null)
                    FireFromEye(leftEye);

                // optional stagger
                if (eyeStagger > 0f && rightEye != null)
                {
                    yield return new WaitForSeconds(eyeStagger);
                    FireFromEye(rightEye);
                }
                else
                {
                    // right eye immediately (or if left null, attempt right)
                    if (rightEye != null)
                        FireFromEye(rightEye);
                }
            }
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void FireFromEye(Transform eye)
    {
        if (bulletPrefab == null || eye == null) return;

        Vector2 dir;
        if (aimAtPlayer && player != null)
        {
            dir = (player.position - eye.position).normalized;
        }
        else if (player != null)
        {
            float sign = Mathf.Sign(player.position.x - transform.position.x);
            dir = new Vector2(sign != 0 ? sign : 1f, 0f);
        }
        else
        {
            dir = Vector2.right;
        }

        // spawn slightly outside to avoid immediate overlap with boss collider
        Vector3 spawnPos = eye.position + (Vector3)dir * spawnOffset;
        GameObject instance = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        // prefer BossBullet if present (allows passing owner), otherwise fallback to Bullet
        if (instance.TryGetComponent(out BossBullet bossBullet))
        {
            bossBullet.Initialize(dir, gameObject);
        }
        else if (instance.TryGetComponent(out Bullet bulletComponent))
        {
            bulletComponent.Initialize(dir);
        }
    }

    private void OnEyeHealthChanged(int currentHealth)
    {
        // Wait one frame so destroyed Targets evaluate as null and child lists update.
        // This avoids the race where the event is invoked before Unity marks the object destroyed.
        StartCoroutine(DelayedEyeCheck());
    }

    private IEnumerator DelayedEyeCheck()
    {
        yield return null; // wait one frame

        // If explicit references show both dead OR no Target children remain, destroy boss.
        if (AreBothEyesDestroyed() || GetComponentsInChildren<Target>().Length == 0)
            DestroyBoss();
    }

    private bool AreBothEyesDestroyed()
    {
        // Consider an eye destroyed if its Target reference is null (Unity's == null works after object destroyed)
        bool leftDead = leftEyeTarget == null;
        bool rightDead = rightEyeTarget == null;
        return leftDead && rightDead;
    }

    private void DestroyBoss()
    {
        // cleanup subscriptions (defensive)
        if (leftEyeTarget != null) leftEyeTarget.OnHealthChanged.RemoveListener(OnEyeHealthChanged);
        if (rightEyeTarget != null) rightEyeTarget.OnHealthChanged.RemoveListener(OnEyeHealthChanged);

        // optional: play death VFX / sound here

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (firingRoutine != null)
        {
            StopCoroutine(firingRoutine);
            firingRoutine = null;
        }

        if (leftEyeTarget != null) leftEyeTarget.OnHealthChanged.RemoveListener(OnEyeHealthChanged);
        if (rightEyeTarget != null) rightEyeTarget.OnHealthChanged.RemoveListener(OnEyeHealthChanged);
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}