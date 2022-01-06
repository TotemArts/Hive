using System;
using System.Text;

namespace Hive.Shared.Common.Extensions
{
    public static class Utf8Decoder
    {
        public static string ToLiteral(byte[] buffer)
        {
            Encoding utf8 = (Encoding)Encoding.UTF8.Clone();
            utf8.DecoderFallback = new Fallback();
            string input = utf8.GetString(buffer);

            return ToLiteral(input);
        }

        public static string ToLiteral(string input)
        {
            for (int i = input.Length - 1; i >= 0; i--)
            {
                char c = input[i];
                if (c >= ' ' && c != '\\' && c != '"' && (c <= '~' || c >= '¡') && c <= 'ÿ')
                    continue;
                string insert = c switch
                {
                    '\a' => "\\a",
                    '\b' => "\\b",
                    '\f' => "\\f",
                    '\n' => "\\n",
                    '\r' => "\\r",
                    '\t' => "\\t",
                    '\v' => "\\v",
                    '\\' => "\\\\",
                    '"' => "\\\"",
                    _ => $"\\x{(int)c:X4}"
                };
                input = input.Remove(i, 1).Insert(i, insert);
            }

            input = input.Insert(0, "\"") + "\"";
            return input;
        }

        public static byte[] GetBytes(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            int index = 0;
            foreach (char c in message)
            {
                if (!(c >= ' ' && c <= '~'))
                {
                    bytes[index] = Convert.ToByte(c);
                }

                index++;
            }

            return bytes;
        }
    }

    public class Fallback : DecoderFallback
    {
        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return new FallbackBuffer();
        }

        public override int MaxCharCount => 6;
    }

    public class FallbackBuffer : DecoderFallbackBuffer
    {
        private string _buffer = "";
        private int _bufferIndex;

        public override bool Fallback(byte[] bytesUnknown, int index)
        {
            _buffer = "";
            _bufferIndex = 0;
            foreach (byte c in bytesUnknown)
            {
                _buffer += $"\\x{(int)c:X4}";
            }

            return true;
        }

        public override char GetNextChar()
        {
            return Remaining != 0 ? _buffer[_bufferIndex++] : (char)0;
        }

        public override bool MovePrevious()
        {
            _bufferIndex--;
            return true;
        }

        public override int Remaining => _buffer.Length - _bufferIndex;
    }
}