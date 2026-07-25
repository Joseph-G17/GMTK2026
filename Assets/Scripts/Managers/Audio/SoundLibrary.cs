using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/Library/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public EnvironmentSounds world;
    public PlayerSounds player;
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

