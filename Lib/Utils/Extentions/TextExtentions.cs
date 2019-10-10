using System;
using System.Text;

namespace Utils.Extentions
{
    public static class TextExtentions
    {
        public static byte[] ToBytes(this string str) => Encoding.UTF8.GetBytes(str);
    }
}