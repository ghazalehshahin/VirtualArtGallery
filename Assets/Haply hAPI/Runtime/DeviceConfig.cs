using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DeviceConfig", menuName = "ScriptableObjects/DeviceConfig", order = 1)]
public class DeviceConfig : ScriptableObject
{
    public EncoderRotations EncoderRotations;
    public ActuatorRotations ActuatorRotations;
    public Offset Offset;
    public int Resolution;
    public bool FlippedStylusButton;
}

[Serializable]
public enum Rotation { CW = 0, CCW = 1 }

[Serializable]
public class EncoderRotations
{
    public Rotation Rotation1;
    public Rotation Rotation2;
}

[Serializable]
public class ActuatorRotations
{
    public Rotation Rotation1;
    public Rotation Rotation2;
}

[Serializable]
public class Offset
{
    public int Left;
    public int Right;
}