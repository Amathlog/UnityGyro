
class DeviceInfo {

    // Class members
    private int m_id;
    public string name;
    public bool accelerometerCompatible;

    // Id Property
    public int id {
        get { return m_id; }
        private set { m_id = id; }
    }

    // Public constructor
    public DeviceInfo(int id, string name, bool accelerometerCompatible) {
        this.m_id = id;
        this.name = name;
        this.accelerometerCompatible = accelerometerCompatible;
    }

    // Overide operators
    public static bool operator ==(DeviceInfo d1, DeviceInfo d2) {
        return !(d1 != d2);
    }

    public static bool operator !=(DeviceInfo d1, DeviceInfo d2) {
        return d1.id != d2.id;
    }

    public override bool Equals(object o) {
        try {
            return (bool)(this == (DeviceInfo)o);
        } catch {
            return false;
        }
    }

    public override int GetHashCode() {
        return id.GetHashCode();
    }

}
