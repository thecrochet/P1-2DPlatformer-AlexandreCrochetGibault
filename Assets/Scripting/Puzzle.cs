
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [Tooltip("1-based index for this piece (set in Inspector)")]
    public int order = 1;

    [Tooltip("Door to open when this puzzle group is completed")]
    [SerializeField] private GameObject door;
    [Tooltip("Optional extra object to remove when puzzle completes (e.g. a green cube)")]
    [SerializeField] private GameObject greenCube;

    [Header("Collect SFX (optional)")]
    [Tooltip("Audio clip played when collecting the correct piece")]
    [SerializeField] private AudioClip collectClip;
    [Tooltip("Base pitch for the first collected piece")]
    [SerializeField] private float collectBasePitch = 1f;
    [Tooltip("Pitch step added for each subsequent correct piece")]
    [SerializeField] private float collectPitchStep = 0.12f;
    [Tooltip("Playback volume (0..1)")]
    [SerializeField][Range(0f, 1f)] private float collectVolume = 1f;

    // groups keyed by a group root (explicit groupRoot -> door -> parent -> this object)
    [SerializeField] private GameObject groupRoot;
    private static readonly Dictionary<GameObject, List<Puzzle>> groups = new Dictionary<GameObject, List<Puzzle>>();
    private static readonly Dictionary<GameObject, int> groupSteps = new Dictionary<GameObject, int>();

    private GameObject groupKey;

    private void Start()
    {
        // determine group key
        groupKey = groupRoot != null ? groupRoot
                 : (door != null ? door
                 : (transform.parent != null ? transform.parent.gameObject
                 : gameObject));

        // build group list from children under groupKey (include inactive)
        Puzzle[] found;
        if (groupKey == gameObject)
            found = GetComponentsInChildren<Puzzle>(true);
        else
            found = groupKey.GetComponentsInChildren<Puzzle>(true);

        // ensure containers exist and populate
        groups[groupKey] = new List<Puzzle>(found);
        if (!groupSteps.ContainsKey(groupKey))
            groupSteps[groupKey] = 1;

        // Validation: ensure orders are sensible
        var orders = groups[groupKey].Select(p => p.order).ToList();
        if (orders.Count != orders.Distinct().Count())
        {
            Debug.LogWarning($"Puzzle group '{groupKey.name}': duplicate piece 'order' values detected. Orders must be unique within a group.");
        }
        if (orders.Any(o => o <= 0))
        {
            Debug.LogWarning($"Puzzle group '{groupKey.name}': piece 'order' values must be >= 1.");
        }
    }

    private void OnDestroy()
    {
        if (groupKey != null && groups.ContainsKey(groupKey))
        {
            groups[groupKey].Remove(this);
            if (groups[groupKey].Count == 0)
            {
                groups.Remove(groupKey);
                groupSteps.Remove(groupKey);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (other.CompareTag("Player") || other.TryGetComponent(out Bullet _))
            TryCollect();
    }

    public void TakeDamage(int amount)
    {
        TryCollect();
    }

    private void TryCollect()
    {
        if (groupKey == null || !groups.ContainsKey(groupKey)) return;

        int currentStep = groupSteps.ContainsKey(groupKey) ? groupSteps[groupKey] : 1;
        // determine total required pieces by highest order value in the group
        int totalPieces = groups[groupKey].Where(p => p != null).Select(p => p.order).DefaultIfEmpty(0).Max();

        // Debug
        Debug.Log($"Puzzle: trying collect piece '{name}' order={order} currentStep={currentStep} totalRequired={totalPieces}");

        if (order == currentStep)
        {
            // Play progression tone
            if (collectClip != null)
            {
                float pitch = collectBasePitch + (currentStep - 1) * collectPitchStep;
                PlayClipWithPitchAtPoint(collectClip, pitch, collectVolume, transform.position);
            }

            // correct item collected
            groupSteps[groupKey] = currentStep + 1;
            gameObject.SetActive(false);

            // completed when we've collected up to the highest order
            if (groupSteps[groupKey] > totalPieces)
            {
                if (door != null) Destroy(door);
                if (greenCube != null) Destroy(greenCube);

                groups.Remove(groupKey);
                groupSteps.Remove(groupKey);
            }
        }
        else
        {
            // Wrong item collected -> immediate reset
            ResetGroup();
        }
    }

    private void ResetGroup()
    {
        if (groupKey == null) return;

        groupSteps[groupKey] = 1;

        if (groups.ContainsKey(groupKey))
        {
            foreach (var piece in groups[groupKey])
            {
                if (piece != null)
                    piece.gameObject.SetActive(true);
            }
        }

        Debug.Log("Wrong order. Puzzle reset for group: " + (groupKey != null ? groupKey.name : "null"));
    }

    // small helper: create a temporary AudioSource at position so we can set pitch per-play
    private static void PlayClipWithPitchAtPoint(AudioClip clip, float pitch, float volume, Vector3 position)
    {
        if (clip == null) return;

        GameObject tmp = new GameObject("SFXTemp");
        tmp.transform.position = position;
        var src = tmp.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = Mathf.Clamp01(volume);
        src.pitch = Mathf.Max(0.01f, pitch);
        src.spatialBlend = 0f; // 2D sound
        src.Play();
        float duration = clip.length / Mathf.Abs(src.pitch);
        Object.Destroy(tmp, duration + 0.05f);
    }
}


/*
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [Tooltip("1-based index for this piece (set in Inspector)")]
    public int order = 1;

    [Tooltip("Door to open when this puzzle group is completed")]
    [SerializeField] private GameObject door;
    [Tooltip("Optional extra object to remove when puzzle completes (e.g. a green cube)")]
    [SerializeField] private GameObject greenCube;

    [Header("Collect SFX (optional)")]
    [Tooltip("Audio clip played when collecting the correct piece")]
    [SerializeField] private AudioClip collectClip;
    [Tooltip("Base pitch for the first collected piece")]
    [SerializeField] private float collectBasePitch = 1f;
    [Tooltip("Pitch step added for each subsequent correct piece")]
    [SerializeField] private float collectPitchStep = 0.12f;
    [Tooltip("Playback volume (0..1)")]
    [SerializeField][Range(0f, 1f)] private float collectVolume = 1f;

    // groups keyed by a group root (door if assigned, otherwise parent, otherwise this object)
    private static readonly Dictionary<GameObject, List<Puzzle>> groups = new Dictionary<GameObject, List<Puzzle>>();
    private static readonly Dictionary<GameObject, int> groupSteps = new Dictionary<GameObject, int>();

    private GameObject groupKey;

    private void Awake()
    {
        // choose a grouping key so multiple independent puzzles can coexist:
        // priority: explicit door -> parent -> this object
        groupKey = door != null ? door
                 : (transform.parent != null ? transform.parent.gameObject
                 : gameObject);

        if (!groups.ContainsKey(groupKey))
        {
            groups[groupKey] = new List<Puzzle>();
            groupSteps[groupKey] = 1; // start at step 1
        }

        if (!groups[groupKey].Contains(this))
            groups[groupKey].Add(this);
    }

    private void OnDestroy()
    {
        if (groupKey != null && groups.ContainsKey(groupKey))
        {
            groups[groupKey].Remove(this);
            if (groups[groupKey].Count == 0)
            {
                groups.Remove(groupKey);
                groupSteps.Remove(groupKey);
            }
        }
    }

    // allow collection by Player touching or by bullets (bullet collision may call TakeDamage elsewhere)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.TryGetComponent(out Bullet _))
        {
            TryCollect();
        }
    }

    // public API if other systems call it (e.g. Bullet uses TakeDamage)
    public void TakeDamage(int amount)
    {
        TryCollect();
    }

    private void TryCollect()
    {
        if (groupKey == null) return;

        int currentStep = groupSteps.ContainsKey(groupKey) ? groupSteps[groupKey] : 1;
        int totalPieces = groups.ContainsKey(groupKey) ? groups[groupKey].Count : 1;

        if (order == currentStep)
        {
            // play progression tone: basePitch + (currentStep - 1) * pitchStep
            if (collectClip != null)
            {
                float pitch = collectBasePitch + (currentStep - 1) * collectPitchStep;
                PlayClipWithPitchAtPoint(collectClip, pitch, collectVolume, transform.position);
            }

            // correct item collected
            groupSteps[groupKey] = currentStep + 1;
            gameObject.SetActive(false);

            if (groupSteps[groupKey] > totalPieces)
            {
                // puzzle complete for this group
                if (door != null) Destroy(door);
                if (greenCube != null) Destroy(greenCube);

                // cleanup group state
                groups.Remove(groupKey);
                groupSteps.Remove(groupKey);
            }
        }
        else
        {
            // wrong item: reset this group
            ResetGroup();
        }
    }

    private void ResetGroup()
    {
        if (groupKey == null) return;

        groupSteps[groupKey] = 1;

        if (groups.ContainsKey(groupKey))
        {
            foreach (var piece in groups[groupKey])
            {
                if (piece != null)
                    piece.gameObject.SetActive(true);
            }
        }

        Debug.Log("Wrong order. Puzzle reset for group: " + (groupKey != null ? groupKey.name : "null"));
    }

    // small helper: create a temporary AudioSource at position so we can set pitch per-play
    private static void PlayClipWithPitchAtPoint(AudioClip clip, float pitch, float volume, Vector3 position)
    {
        if (clip == null) return;

        GameObject tmp = new GameObject("SFXTemp");
        tmp.transform.position = position;
        var src = tmp.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = Mathf.Clamp01(volume);
        src.pitch = Mathf.Max(0.01f, pitch);
        src.spatialBlend = 0f; // 2D sound
        src.Play();
        // destroy after playback; account for pitch affecting playback length
        float duration = clip.length / Mathf.Abs(src.pitch);
        Object.Destroy(tmp, duration + 0.05f);
    }
}*/