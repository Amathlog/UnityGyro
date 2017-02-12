using UnityEngine;
using UnityEngine.Networking;


class RegisterHostMessage : MessageBase
{
    public static readonly short id = 898;
    public int deviceId;
    public string deviceName;
    public string version;
    public bool accelerometerCompatible;

    public override string ToString()
    {
        return "Device Id : " + deviceId + "\nDevice name : " + deviceName + "\nVersion : " + version + "\nAccelerometerCompatible : " + accelerometerCompatible.ToString();
    }
}

class ActionMessage : MessageBase
{
    public static readonly short id = 899;
    public int idDevice;
    public Vector3 position;
    public bool triggered;

    public override string ToString()
    {
        return "Device id : " + idDevice + "\nPosition : " + position + "\nTriggered : " + triggered;
    }
}

class CalibrationMessage : MessageBase {
    public static readonly short id = 897;
    public bool enable;
    public int idDevice;
    public float minX, minY, maxX, maxY;
}
