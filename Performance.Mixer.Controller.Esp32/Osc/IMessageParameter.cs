namespace Performance.Mixer.Controller.Esp32.Osc;

internal interface IMessageParameter
{
    public byte TypeTag { get; }
    public int Length { get; }
    public byte[] Encode();
}

