
// Use Command-Line tool to get a list of video sources:
//    gst-device-monitor-1.0 Video/Source

using System.Runtime.InteropServices;
using GstAlignmentCheck;

static int Main(int argc, nint argv, nint user_data)
{
    Application.Init();

    string description =
        "avfvideosrc device-index=0 ! video/x-raw, width=1552, height=1552, format=YUY2, framerate=30/1 ! videoconvert ! video/x-raw, format=UYVY ! glupload ! " +
        //"tee name=t t. ! queue ! appsink name=myAppSink " +
        //"t. ! queue ! autovideosink";
    //    "videoconvert ! " +
    //    "video/x-raw,format=BGRA ! " +
        "appsink name=myAppSink";




    var MainLoop = new GLib.MainLoop();
    Pipeline Pipeline = (Pipeline)Parse.LaunchFull(description, ParseFlags.None);
    Element appSinkElement = Pipeline.GetByName("myAppSink");
    AppSink appSink = new(appSinkElement.Handle)
    {
        EmitSignals = true
    };

    int samples = 0;

    appSink.NewSample += (object sender, NewSampleArgs args) =>
    {
        using Sample sample = appSink.PullSample();

        if (sample.Handle != nint.Zero)
        {
            if (sample.Buffer.Map(out MapInfo map, MapFlags.Read))
            {
                sample.Buffer.AllMemory.GetSizes(out ulong offset, out ulong maxsize);

                Console.WriteLine($"[ Sample #{++samples:D4} ] Aligned: {(map.DataPtr % 4096 == 0 ? "Yes" : "No")}, DataSize: {map.Size}, DataSize Alignable: {(map.Size % 4096 == 0 ? "Yes" : "No")} | Memory Max Size Alignable: {(sample.Buffer.AllMemory.Maxsize - map.Size >= 4096 ? "Yes" : "No")}, Diff: {sample.Buffer.AllMemory.Maxsize - map.Size}, Offset: {offset}, MaxSize: {maxsize}, Buffers: {sample.Buffer.Size}, Mod: {sample.Buffer.Size % 4096}");
                sample.Buffer.Unmap(map);
            }
            //MainLoop.Quit();
        }
    };

    StateChangeReturn state = Pipeline.SetState(State.Playing);

    switch(state)
    {
        case StateChangeReturn.Success:
        case StateChangeReturn.NoPreroll:
        case StateChangeReturn.Async:
            Console.WriteLine("Started gst pipeline");
            break;
        default:
            Console.WriteLine("Failed to start gst pipeline");
            appSink.Dispose();
            Pipeline.Dispose();
            return 0;
    }


    MainLoop.Run();
    Pipeline.SetState(State.Null);

    appSink.Dispose();
    Pipeline.Dispose();

    return 0;
}

if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    return NativeAPI.gst_macos_main_simple(Marshal.GetFunctionPointerForDelegate<NativeAPI.GstMacosMainFunction>(Main), IntPtr.Zero);
else
    return Main(0, IntPtr.Zero, IntPtr.Zero);