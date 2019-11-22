using System;

namespace WasmLib.Utils
{
    internal static class StringHelper
    {
        public static string SafeFormat<T>(this string format, T[]? parameters)
        {
            if (parameters is null || parameters.Length == 0) {
                return format;
            }

            string s = format;
            
            for (int j = 0; j < parameters.Length; j++) {
                T param = parameters[j];
                
                if (param is null) {
                    throw new NullReferenceException($"Passed a null parameter to {nameof(SafeFormat)}");
                }
                
                s = s.Replace($"{{{j}}}", param.ToString());
            }

            return s;
        }
    }
}