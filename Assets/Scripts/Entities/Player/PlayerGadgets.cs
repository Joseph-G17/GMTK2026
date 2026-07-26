using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGadgets : MonoBehaviour
{
    public static PlayerGadgets gadgets;
    PlayerController movement;

    [Header("Light Source")]
    [SerializeField] public Light2D userLight;

    [Header("Base Light Values")]
    [SerializeField] private float baseIntensity;
    [SerializeField] private float baseRadiusOuter;

    [Header("Crank Boost Settings")]
    [SerializeField] private float boostIntensityAdd = 0.7f;
    [SerializeField] private float boostRadiusAdd = 6f;    
    [SerializeField] private float dimDuration = 15f;         

    [Header("Charge Settings")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private float rechargeTime = 10f;
    private int currentCharges;
    private float rechargeTimer;

    private void Awake()
    {
        gadgets = this;
        if (movement == null)
            movement = GetComponent<PlayerController>();
        if (userLight == null)
            userLight = GetComponentInChildren<Light2D>();

        baseIntensity = userLight.intensity;
        baseRadiusOuter = userLight.pointLightOuterRadius;

        currentCharges = maxCharges;
        rechargeTimer = 0f;
    }

    void Update()
    {
        DimTowardsBase();
        HandleRecharge();
    }

    public void OnCrankInput()
    {
        if (currentCharges <= 0) return;

        currentCharges--;
        Debug.Log($"Current Charges: {currentCharges}");

        userLight.intensity += boostIntensityAdd;
        userLight.pointLightOuterRadius += boostRadiusAdd;

        if (currentCharges == 0)
            rechargeTimer = rechargeTime;
    }

    private void DimTowardsBase()
    {
        float maxBoostIntensity = boostIntensityAdd * maxCharges;
        float maxBoostRadius = boostRadiusAdd * maxCharges;

        float intensityRate = maxBoostIntensity / dimDuration;
        float radiusRate = maxBoostRadius / dimDuration;

        if (userLight.intensity > baseIntensity)
            userLight.intensity = Mathf.MoveTowards(userLight.intensity, baseIntensity, intensityRate * Time.deltaTime);

        if (userLight.pointLightOuterRadius > baseRadiusOuter)
            userLight.pointLightOuterRadius = Mathf.MoveTowards(userLight.pointLightOuterRadius, baseRadiusOuter, radiusRate * Time.deltaTime);
    }

    private void HandleRecharge()
    {
        if (currentCharges >= maxCharges) return;
        if (rechargeTimer <= 0f) return;

        rechargeTimer -= Time.deltaTime;
        if (rechargeTimer <= 0f)
        {
            currentCharges = maxCharges;
            Debug.Log("Charges full");
        }
    }
}