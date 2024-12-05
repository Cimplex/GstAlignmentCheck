# GstAlignmentCheck
Checks the alignment of the given Source element (Uses AppSink)


### Getting Started

Run the following commands:
  - `git clone git@github.com:Cimplex/GstAlignmentCheck.git`
  - `cd GstAlignmentCheck`
  - `dotnet restore`
  - Adjust the Gst description at the start of "Program.cs" to whatever device you have:
  - `code .`
  - `dotnet run`
  - Give me the output of a single line

It should print the same thing for each frame. Theoretically you could use this to see how gstreamer adjusts on changes, e.g. unplug the HDMI or change resolutions from the source device.
