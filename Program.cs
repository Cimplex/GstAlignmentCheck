
// Use Command-Line tool to get a list of video sources:
//    gst-device-monitor-1.0 Video/Source

Application.Init();

string description =
    "avfvideosrc device-index=0 ! " +
    "videoconvert ! " +
    "video/x-raw,format=BGRA ! " +
    "appsink name=myAppSink";

var MainLoop = new GLib.MainLoop();
Pipeline Pipeline = (Pipeline)Parse.LaunchFull(description, ParseFlags.None);
Element appSinkElement = Pipeline.GetByName("myAppSink");
AppSink appSink = new(appSinkElement.Handle)
{
    EmitSignals = true
};

appSink.NewSample += (object sender, NewSampleArgs args) =>
{
    using Sample sample = appSink.PullSample();

    if (sample.Handle != nint.Zero)
    {
        if (sample.Buffer.Map(out MapInfo map, MapFlags.Read))
        {
            Console.WriteLine($"Is sample aligned to 4096-byte blocks? {(map.DataPtr % 4096 == 0 ? "Yes" : "No")}");
            sample.Buffer.Unmap(map);
        }
        sample.Dispose();
        MainLoop.Quit();
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
