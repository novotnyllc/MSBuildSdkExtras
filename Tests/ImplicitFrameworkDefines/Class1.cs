using System;

namespace ImplicitFrameworkDefines
{
    public class TestClass
    {
#if !NET8_0 && !NET8_0_OR_GREATER
#error NET8_0 and NET8_0_OR_GREATER must be defined
#endif

#if NET8_0_WINDOWS
        // This should only compile for net8.0-windows
        public const string Platform = "Windows";
#else
        public const string Platform = "CrossPlatform";
#endif
    }
}
