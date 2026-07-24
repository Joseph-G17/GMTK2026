using System;
using System.Collections;
using System.Timers;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGadgets : MonoBehaviour
{
    public static PlayerGadgets gadgets;
    PlayerController movement; 

    [Header("Light Source")]
    [SerializeField]private Light2D userLight;
    private bool lightOn;
    [SerializeField] private float intensity;
    [SerializeField] private float radiusInner;
    [SerializeField] private float radiusOuter;
    [SerializeField] private float falloff;
    int count;

    private void Awake()
    {
        gadgets = this;
        if (movement == null)
            movement = GetComponent<PlayerController>();
        if (userLight == null)
            userLight = GetComponentInChildren<Light2D>();

        intensity = userLight.intensity;
        radiusInner = userLight.pointLightInnerRadius;
        radiusOuter = userLight.pointLightOuterRadius;
        falloff = userLight.falloffIntensity;

        count = 0;
    }

    void Update()
    {
        
    }

    private IEnumerator LerpFloat(float startValue, float endValue, float duration, Action<float> onValueUpdate)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float currentValue = Mathf.Lerp(startValue, endValue, t);
            onValueUpdate(currentValue);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        onValueUpdate(endValue);
    }

    public IEnumerator CrankLight()
    {   if (count < 4)
        {
            StartCoroutine(LerpFloat(userLight.intensity, userLight.intensity + 0.3f, 0.8f, value => userLight.intensity = value));
            StartCoroutine(LerpFloat(userLight.pointLightOuterRadius, userLight.pointLightOuterRadius + 1.5f, 1f, value => userLight.pointLightOuterRadius = value));
        }
        yield return null;
    }

}
