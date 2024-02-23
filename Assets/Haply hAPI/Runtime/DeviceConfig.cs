using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DeviceConfig", menuName = "ScriptableObjects/DeviceConfig", order = 1)]
public class DeviceConfig : ScriptableObject
{

    public EncoderRotations encoderRotations;
    public ActuatorRotations actuatorRotations;
    public Offset offset;
    public int resolution;
}

[Serializable]
public enum Rotation { CW = 0, CCW = 1 }

[Serializable]
public class EncoderRotations
{
    public Rotation rotation1;
    public Rotation rotation2;
}

[Serializable]
public class ActuatorRotations
{
    public Rotation rotation1;
    public Rotation rotation2;
}

[Serializable]
public class Offset
{
    public int left;
    public int right;
}