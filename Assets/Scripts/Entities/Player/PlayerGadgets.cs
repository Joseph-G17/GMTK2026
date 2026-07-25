using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGadgets : MonoBehaviour
{
    public static PlayerGadgets gadgets;
    PlayerController movement;

    [Header("Light Source")]
    [SerializeField] public Light2D userLight;
    private bool lightOn;
    [SerializeField] private float intensity;
    [SerializeField] private float radiusInner;
    [SerializeField] private float radiusOuter;
    [SerializeField] private float falloff;

    [Header("Charge Settings")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private float rechargeTime = 10f; // seconds to regain one charge
    private int currentCharges;
    private float rechargeTimer = 0f;
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
        rechargeTimer = 0f;
        currentCharges = maxCharges;
        isDimming = false;
    }
    void Update()
    {
     
    }
    public void OnCrankInput()
    {
        if (currentCharges <= 0) return; 

        StartCoroutine(CrankLight());
        currentCharges--;
        Debug.Log($"Current Charges: {currentCharges}");

        if (currentCharges == 0)
            StartCoroutine(ChargeLight());
    }
    private IEnumerator CrankLight()
    {
        isCranking = true; 
        StartCoroutine(LerpFloat(userLight.intensity, userLight.intensity + 0.7f, 1f, value => userLight.intensity = value));
        yield return StartCoroutine(LerpFloat(userLight.pointLightOuterRadius, userLight.pointLightOuterRadius + 6f, 2f, value => userLight.pointLightOuterRadius = value));
        isCranking = false;

    }

    private IEnumerator TryDimLight()
    {
        if (isCranking) yield break;

        isDimming = true;
        StartCoroutine(LerpFloat(userLight.intensity, intensity, 15f, value => userLight.intensity = value));
        StartCoroutine(LerpFloat(userLight.pointLightOuterRadius, radiusOuter, 15f, value => userLight.pointLightOuterRadius = value));
        isDimming = false;

        yield return null;
    }
    private IEnumerator ChargeLight()
    {
        yield return new WaitForSeconds(rechargeTime);
        currentCharges = 3;
        Debug.Log("Charges full");
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
}