using System.Buffers.Binary;

namespace Performance.Mixer.Controller.Esp32.Osc;

internal class IntegerMessageParameter(int value) : IMessageParameter
{
    public byte TypeTag => (byte)'i';
    public int Length => 4;
    public byte[] Encode()
    {
        var result = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(result, value);
        return result;
    }
}

