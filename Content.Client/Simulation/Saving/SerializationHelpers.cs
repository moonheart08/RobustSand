using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Content.Client.Simulation.ParticleKinds.Abstract;
using JetBrains.Annotations;
using Robust.Shared.Utility;

namespace Content.Client.Simulation.Saving;

public static class SerializationHelpers
{
    [Pure]
    public static uint CalcCRC32(IEnumerable<byte> input) {
        var crc = 0xFFFFFFFF;
	
        foreach (var t in input)
        {
            var byt = t;
            
            for(var j = 0; j < 8; j++) {
                var b = (byt ^ crc) & 1;
                
                crc >>= 1;
                
                if (b != 0) 
                    crc ^= 0xEDB88320;
                
                byt >>= 1;
            }
        }
	
        return ~crc;
    }

    [Pure]
    public static int ExpectedStringByteLength(string input)
    {
        return sizeof(int) + input.Length * sizeof(char);
    }

    public static void SerializeString(Span<byte> span, string input)
    {
        DebugTools.Assert(span.Length >= ExpectedStringByteLength(input));
        var pos = 0;
        BitConverter.TryWriteBytes(span.Slice(pos, sizeof(int)), input.Length * sizeof(char));
        pos += sizeof(int);
        foreach (var c in input)
        {
            BitConverter.TryWriteBytes(span.Slice(pos, sizeof(char)), c);
            pos += sizeof(char);
        }
    }

    [Pure]
    public static string DeserializeString(ReadOnlySpan<byte> span)
    {
        
        var pos = 0;
        var size = BitConverter.ToInt32(span.Slice(pos, sizeof(int)));
        pos += sizeof(int);
        var builder = new StringBuilder(size / sizeof(char));
        DebugTools.Assert(span.Length-pos >= size);

        for (var idx = 0; idx < size / sizeof(char); idx++)
        {
            builder.Append(BitConverter.ToChar(span.Slice(pos, sizeof(char))));
            pos += sizeof(char);
        }

        return builder.ToString();
    }

    [Pure]
    public static int ExpectedTypeDictLength(Dictionary<string, ushort> dict)
    {
        return dict.Keys.Sum(ExpectedStringByteLength) + dict.Values.Count * sizeof(ushort) + sizeof(int);
    }
    
    public static void SerializeTypeDict(Span<byte> span, Dictionary<string, ushort> dict)
    {
        DebugTools.Assert(ExpectedTypeDictLength(dict) >= span.Length);
        
        var pos = 0;
        foreach (var (key, value) in dict)
        {
            var len = ExpectedStringByteLength(key);
            SerializeString(span.Slice(pos, len), key);
            pos += len;
            BitConverter.TryWriteBytes(span.Slice(pos, sizeof(ushort)), value);
            pos += sizeof(ushort);
        }
        // Write terminator.
        BitConverter.TryWriteBytes(span.Slice(pos, sizeof(int)), (int) 0);
    }

    [Pure]
    public static (Dictionary<string, ushort> dict, int len) DeserializeTypeDict(ReadOnlySpan<byte> span)
    {
        var pos = 0;
        var dict = new Dictionary<string, ushort>(Enum.GetValues<ParticleType>().Length);
        while (true)
        {
            if (BitConverter.ToInt32(span.Slice(pos, sizeof(int))) == 0)
                break;
            
            var str = DeserializeString(span.Slice(pos));
            pos += ExpectedStringByteLength(str);
            var id = BitConverter.ToUInt16(span.Slice(pos, sizeof(ushort)));
            pos += sizeof(ushort);
            dict.Add(str, id);
        }

        pos += sizeof(int);
        
        Logger.Debug($"{string.Join(",", dict)}");

        return (dict, pos);
    }
}