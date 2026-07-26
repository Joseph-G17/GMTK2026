using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/Library/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public EnvironmentSounds world;
    public PlayerSounds player;
    public SpiderSounds spider;
}

[System.Serializable]
public class EnvironmentSounds {
    public SoundEffect pickupItem;
    public SoundEffect lightSwitch;
}

[System.Serializable]
public class PlayerSounds
{
    public SoundEffect footsteps;
    public SoundEffect crankLight;
    public SoundEffect afterGlow;
}

[System.Serializable]
public class SpiderSounds 
{
    public SoundEffect spiderRoam;
    public SoundEffect spiderLooking;
    public SoundEffect spiderWarning;
    public SoundEffect spiderStopping;
    public SoundEffect spiderChasing;
}


