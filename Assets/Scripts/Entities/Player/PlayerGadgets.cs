using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGadgets : MonoBehaviour
{
    public static PlayerGadgets gadgets;
    PlayerController movement;

    [Header("Light Source")]
    [SerializeField] private Light2D userLight;
    private bool lightOn;
    [SerializeField] private float intensity;
    [SerializeField] private float radiusInner;
    [SerializeField] private float radiusOuter;
    [SerializeField] private float falloff;

    [Header("Charge Settings")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private float rechargeTime = 20f; // seconds to regain one charge
    private int currentCharges;
    private float rechargeTimer;
    private bool isCranking;
    private bool isDimming; 

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

        currentCharges = maxCharges;
        isDimming = false;
    }

    void Update()
    {
      
    }

    public void OnCrankInput()
    {
        if (currentCharges <= 0) return; 

        currentCharges--;
        StartCoroutine(CrankLight());
        Debug.Log($"Current Charges: {currentCharges}");
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

    private IEnumerator CrankLight()
    {
        isCranking = true; 

        StartCoroutine(LerpFloat(userLight.intensity, userLight.intensity + 0.3f, 2f, value => userLight.intensity = value));
        yield return StartCoroutine(LerpFloat(userLight.pointLightOuterRadius, userLight.pointLightOuterRadius + 1f, 2f, value => userLight.pointLightOuterRadius = value));

        isCranking = false;

        if (currentCharges < maxCharges)
        {
            if(!isDimming)
                StartCoroutine(DimLight());
        }
    }

    private IEnumerator DimLight()
    {
        isDimming = true;

        StartCoroutine(LerpFloat(userLight.intensity, intensity, 2f, value => userLight.intensity = value));
        yield return StartCoroutine(LerpFloat(userLight.pointLightOuterRadius, radiusOuter, 2f, value => userLight.pointLightOuterRadius = value));

        isDimming = false;
        Debug.Log($"Current Charges: {currentCharges}");

    }

    public int CurrentCharges => currentCharges;
    public int MaxCharges => maxCharges;
    public float RechargeProgress => currentCharges >= maxCharges ? 1f : rechargeTimer / rechargeTime;
}