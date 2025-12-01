using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RippleEffectController : MonoBehaviour
{
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private FullScreenPassRendererFeature rippleFeature;

    [Header("Ripple Settings")]
    [SerializeField] private float rippleDuration = 5f;
    [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Material rippleMaterial;

    void Start()
    {
        // Récupérer le material du shader
        if (rippleFeature != null && rippleFeature.passMaterial != null)
        {
            rippleMaterial = rippleFeature.passMaterial;
        }

        // Désactiver l'effet au départ
        SetRippleActive(false);
    }

    private void Update()
    {
        rippleMaterial.SetFloat("_UnscaledTime", Time.unscaledDeltaTime);
    }

    public void TriggerRipple()
    {
        StartCoroutine(RippleCoroutine());
    }

    private IEnumerator RippleCoroutine()
    {
        SetRippleActive(true);

        float elapsedTime = 0f;
        float initialAmplitude = rippleMaterial.GetFloat("_Amplitude");

        while (elapsedTime < rippleDuration)
        {
            float t = elapsedTime / rippleDuration;
            float intensity = intensityCurve.Evaluate(t);

            // Modifier l'amplitude au fil du temps
            rippleMaterial.SetFloat("_Amplitude", initialAmplitude * intensity);

            elapsedTime += Time.unscaledTime;
            yield return null;
        }

        SetRippleActive(false);
    }

    private void SetRippleActive(bool active)
    {
        if (rippleFeature != null)
        {
            rippleFeature.SetActive(active);
        }
    }
}