using UnityEngine;
using UnityEngine.Rendering;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [Header("Components")]
    public GameObject deathscreen;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
}
