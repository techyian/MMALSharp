// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let releaseDir = "./MMALSharp/bin/Release"
let debugDir = "./MMALSharp/bin/Debug"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [releaseDir; debugDir]
)



Target "ReleaseApp" (fun _ ->
    !! "MMALSharp/*.csproj"
    |> MSBuildRelease releaseDir "Build"
    |> Log "AppBuild-Output: "
)

Target "DebugApp" (fun _ ->
    !! "MMALSharp/*.csproj"
    |> MSBuildDebug debugDir "Build"
    |> Log "AppBuild-Output: "
)


Target "Default" (fun _ ->
    trace "Default target executed"
)

// Dependencies
"Clean"
  ==> "ReleaseApp"
  ==> "DebugApp"

// start build
RunTargetOrDefault "Default"