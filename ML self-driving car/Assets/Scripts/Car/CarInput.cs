public interface CarInput
{
    public float SteerInput { get; }
    public float ThrottleInput { get; }
    public bool GearUp { get; }
    public bool GearDown { get; }
    public bool Reverse { get; }
}
