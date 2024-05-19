using System;

namespace Performance.Mixer.Controller.Esp32.Osc;

internal class FloatMessageParameter(float value) : IMessageParameter
{
    public byte TypeTag => (byte)'f';
    public int Length => 4;
    public byte[] Encode()
    {
        var result = BitConverter.GetBytes(value);
        return [result[3], result[2], result[1], result[0]];
    }
}

