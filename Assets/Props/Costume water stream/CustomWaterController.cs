using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class CustomWaterController : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float waveHeight = 0.15f;
    [SerializeField] private float waveFrequency = 0.8f;
    [SerializeField] private float waveSpeed = 1.0f;

    [Header("Optional Ripple Offset")]
    [SerializeField] private Vector2 rippleDirection = new Vector2(1f, 0.5f);
    [SerializeField] private float rippleScrollSpeed = 0.2f;

    private static readonly int WaveHeightID =
        Shader.PropertyToID("_Wave_Height");

    private static readonly int WaveFrequencyID =
        Shader.PropertyToID("_Wave_Frequency");

    private static readonly int WaveSpeedID =
        Shader.PropertyToID("_Wave_Speed");

    private static readonly int RippleOffsetID =
        Shader.PropertyToID("_RippleOffset");

    private Renderer waterRenderer;
    private Material waterMaterial;

    private void OnEnable()
    {
        SetupMaterial();
        ApplySettings();
    }

    private void Update()
    {
        if (waterMaterial == null)
        {
            SetupMaterial();
        }

        if (waterMaterial == null)
        {
            return;
        }

        ApplySettings();

        if (waterMaterial.HasProperty(RippleOffsetID))
        {
            float time = Application.isPlaying
                ? Time.time
                : Time.realtimeSinceStartup;

            Vector2 direction = rippleDirection.sqrMagnitude > 0.0001f
                ? rippleDirection.normalized
                : Vector2.right;

            Vector2 offset =
                direction *
                time *
                rippleScrollSpeed;

            waterMaterial.SetVector(
                RippleOffsetID,
                new Vector4(offset.x, offset.y, 0f, 0f)
            );
        }
    }

    private void SetupMaterial()
    {
        waterRenderer = GetComponent<Renderer>();

        if (waterRenderer == null)
        {
            return;
        }

        // Creates a unique material instance for this water object.
        waterMaterial = Application.isPlaying
            ? waterRenderer.material
            : waterRenderer.sharedMaterial;
    }

    private void ApplySettings()
    {
        if (waterMaterial == null)
        {
            return;
        }

        if (waterMaterial.HasProperty(WaveHeightID))
        {
            waterMaterial.SetFloat(WaveHeightID, waveHeight);
        }

        if (waterMaterial.HasProperty(WaveFrequencyID))
        {
            waterMaterial.SetFloat(WaveFrequencyID, waveFrequency);
        }

        if (waterMaterial.HasProperty(WaveSpeedID))
        {
            waterMaterial.SetFloat(WaveSpeedID, waveSpeed);
        }
    }
}