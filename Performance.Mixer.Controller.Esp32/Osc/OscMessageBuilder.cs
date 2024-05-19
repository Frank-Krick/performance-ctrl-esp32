using System;
using System.Buffers.Binary;
using System.Collections;
using System.IO;
using System.Text;

namespace Performance.Mixer.Controller.Esp32.Osc;

class OscMessageBuilder
{
    private string _address = string.Empty;
    private ArrayList _parameters = [];

    public OscMessageBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public OscMessageBuilder WithParameter(int parameter)
    {
        _parameters.Add(new IntegerMessageParameter(parameter));
        return this;
    }

    public OscMessageBuilder WithParameter(float parameter)
    {
        _parameters.Add(new FloatMessageParameter(parameter));
        return this;
    }

    public byte[] Build()
    {
        using var stream = new MemoryStream(new byte[Length()]);
        var addressBytes = Encoding.UTF8.GetBytes(_address);
        stream.Write(addressBytes, 0, addressBytes.Length);
        stream.WriteByte(0);
        WritePadding(stream);
        stream.WriteByte((byte)',');
        foreach (var parameter in _parameters)
        {
            stream.WriteByte(((IMessageParameter)parameter).TypeTag);
        }
        stream.WriteByte(0);
        WritePadding(stream);
        if (_parameters.Count == 0) return stream.ToArray();
        foreach (IMessageParameter parameter in _parameters)
        {
            var data = parameter.Encode();
            stream.Write(data, 0, data.Length);
        }
        return stream.ToArray();
    }

    private int Length()
    {
        var addressLength = _address.Length + 1;
        if (4 - addressLength % 4 < 4)
        {
            addressLength += 4 - addressLength % 4;
        }

        var typeTagSize = _parameters.Count + 2;
        var currentSize = addressLength + typeTagSize;
        if (4 - currentSize % 4 < 4)
        {
            typeTagSize += 4 - currentSize % 4;
        }


        return typeTagSize + CalculateArrayLength() + addressLength;
    }

    private int CalculateArrayLength()
    {
        int length = 0;
        foreach (var parameter in _parameters)
        {
            length += ((IMessageParameter)parameter).Length;
        }
        return length;
    }

    private static readonly byte[] NullBytes = [0, 0, 0, 0];
    private static void WritePadding(MemoryStream stream)
    {
        int count = 4 - (int)(stream.Position % 4);
        if (count < 4)
        {
            stream.Write(NullBytes, 0, count);
        }
    }
}

