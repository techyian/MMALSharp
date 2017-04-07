// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties

let mmalSharpCommonReleasedir = "./MMALSharp.Common/bin/Release"
let mmalSharpCommonDebugdir = "./MMALSharp.Common/bin/Release"

let mmalSharpReleaseDir = "./MMALSharp/bin/Release"
let mmalSharpDebugDir = "./MMALSharp/bin/Debug"

let mmalSharpFFmpegReleasedir = "./MMALSharp.FFmpeg/bin/Release"
let mmalSharpFFmpegDebugdir = "./MMALSharp.FFmpeg/bin/Release"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [mmalSharpCommonReleasedir; mmalSharpCommonDebugdir; mmalSharpReleaseDir; mmalSharpDebugDir; mmalSharpFFmpegReleasedir; mmalSharpFFmpegDebugdir]
)

Target "MMALSharpCommonReleaseApp" (fun _ ->
    !! "MMALSharp.Common/*.csproj"
    |> MSBuildRelease mmalSharpCommonReleasedir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpCommonDebugApp" (fun _ ->
    !! "MMALSharp.Common/*.csproj"
    |> MSBuildDebug mmalSharpCommonDebugdir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpReleaseApp" (fun _ ->
    !! "MMALSharp/*.csproj"
    |> MSBuildRelease mmalSharpReleaseDir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpDebugApp" (fun _ ->
    !! "MMALSharp/*.csproj"
    |> MSBuildDebug mmalSharpDebugDir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpFFmpegReleaseApp" (fun _ ->
    !! "MMALSharp.FFmpeg/*.csproj"
    |> MSBuildRelease mmalSharpFFmpegReleasedir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpFFmpegDebugApp" (fun _ ->
    !! "MMALSharp.FFmpeg/*.csproj"
    |> MSBuildDebug mmalSharpFFmpegDebugdir "Build"
    |> Log "AppBuild-Output: "
)


Target "Default" (fun _ ->
    trace "Default target executed"
)

// Dependencies
"Clean"
  ==> "MMALSharpCommonReleaseApp"
  ==> "MMALSharpCommonDebugApp"
  ==> "MMALSharpReleaseApp"
  ==> "MMALSharpDebugApp"
  ==> "MMALSharpFFmpegReleaseApp"
  ==> "MMALSharpFFmpegDebugApp"

// start build
RunTargetOrDefault "Default"