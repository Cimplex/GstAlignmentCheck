//gst_macos_main_simple

using System.Runtime.InteropServices;

namespace GstAlignmentCheck;



internal static partial class NativeAPI
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int GstMacosMainFunction(int argc, nint argv, nint user_data);

    [LibraryImport("libgstreamer-1.0.0.dylib", EntryPoint = "gst_macos_main_simple")]
    public static partial int gst_macos_main_simple(nint main_func, nint user_data);
}