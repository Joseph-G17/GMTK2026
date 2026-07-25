using System.Globalization;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathscreenManager : MonoBehaviour
{
    public static DeathscreenManager instance;

    [Header("Components")]
    public GameObject deathscreen;
    [HideInInspector] public bool showingDeathscreen;

    void Start()
    {
        instance = this;

        if (deathscreen == null)
            deathscreen = HUD.instance.deathscreen;
    }

    public void ShowDeathscreen()
    {
        if (showingDeathscreen)
            return;

        deathscreen.SetActive(true);
        showingDeathscreen = true;
    }

    public void CloseDeathscreen()
    {
        deathscreen.SetActive(false);
        showingDeathscreen = false; 
    }
    
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
