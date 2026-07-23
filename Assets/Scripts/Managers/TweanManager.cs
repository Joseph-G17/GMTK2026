using UnityEngine;

public class TweanManager : MonoBehaviour
{
    public static TweanManager instance;

    [Header("Animation Settings")]
    [SerializeField] float openDuration = 0.35f;
    [SerializeField] float closeDuration = 0.25f;
    [SerializeField] float buttonPunchScale = 1.15f;
    [SerializeField] float buttonPunchDuration = 0.12f;
    [SerializeField] LeanTweenType openEase = LeanTweenType.easeOutBack;
    [SerializeField] LeanTweenType closeEase = LeanTweenType.easeInBack;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    public void PunchButton(GameObject button)
    {
        if (button == null) return;
        LeanTween.cancel(button);

        LeanTween.scale(button, Vector3.one * buttonPunchScale, buttonPunchDuration)
                 .setEase(LeanTweenType.easeOutQuad)
                 .setIgnoreTimeScale(true)
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(button, Vector3.one, buttonPunchDuration)
                              .setEase(LeanTweenType.easeInQuad)
                              .setIgnoreTimeScale(true);
                 });
    }
}
