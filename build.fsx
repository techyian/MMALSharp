// include Fake lib
#r "packages/netframework/FAKE/tools/FakeLib.dll"
open Fake

// Properties

let mmalSharpCommonReleasedir = "./src/MMALSharp.Common/bin/Release"
let mmalSharpCommonDebugdir = "./src/MMALSharp.Common/bin/Release"

let mmalSharpReleaseDir = "./src/MMALSharp/bin/Release"
let mmalSharpDebugDir = "./src/MMALSharp/bin/Debug"

let mmalSharpFFmpegReleasedir = "./src/MMALSharp.FFmpeg/bin/Release"
let mmalSharpFFmpegDebugdir = "./src/MMALSharp.FFmpeg/bin/Release"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [mmalSharpCommonReleasedir; mmalSharpCommonDebugdir; mmalSharpReleaseDir; mmalSharpDebugDir; mmalSharpFFmpegReleasedir; mmalSharpFFmpegDebugdir]
)

Target "MMALSharpCommonReleaseApp" (fun _ ->
    !! "src/MMALSharp.Common/*.csproj"
    |> MSBuildRelease mmalSharpCommonReleasedir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpCommonDebugApp" (fun _ ->
    !! "src/MMALSharp.Common/*.csproj"
    |> MSBuildDebug mmalSharpCommonDebugdir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpReleaseApp" (fun _ ->
    !! "src/MMALSharp/*.csproj"
    |> MSBuildRelease mmalSharpReleaseDir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpDebugApp" (fun _ ->
    !! "src/MMALSharp/*.csproj"
    |> MSBuildDebug mmalSharpDebugDir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpFFmpegReleaseApp" (fun _ ->
    !! "src/MMALSharp.FFmpeg/*.csproj"
    |> MSBuildRelease mmalSharpFFmpegReleasedir "Build"
    |> Log "AppBuild-Output: "
)

Target "MMALSharpFFmpegDebugApp" (fun _ ->
    !! "src/MMALSharp.FFmpeg/*.csproj"
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