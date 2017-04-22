# Installation

MMALSharp has support for Mono 4.x, and experimental support for .NET Core 2.0 Preview (using .NET Standard 1.6). Installation differs between both runtimes, and the 
source code is shared between both runtimes.

## Mono

Installation for Mono differs between the original Model A/B/B+/Zero boards and the newer Pi Model B 2/3 boards running the ARMV7/8 chipsets.

### Model A/B/B+/Zero

The version of Mono currently available in the Raspbian repositories is 3.2.8 and isn't compatible with this library. Therefore, we need to do a few
extra steps to get a compatible version installed. Luckily, member 'plugwash' from the Raspberry Pi forums has built a version of Mono and provided a
repository from which we can install.

In order to install the required version, please open a console window and follow the below steps:

1. Run `sudo nano /etc/apt/sources.list`
2. On a new line, enter `deb http://plugwash.raspbian.org/mono4 wheezy-mono4 main`
3. Run `sudo apt-get update && sudo apt-get upgrade`
4. Run `sudo apt-get install mono-complete`

Once completed, if you run `mono --version` from your command window, you should see the mono version 4.0.2 returned.

### Model B 2/3

Using a later model of the Raspberry Pi allows you to install the latest Mono version from the Mono repositories without issue. To do so, please follow the below steps:

```
sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update && sudo apt-get upgrade
sudo apt-get install mono-complete
```

### Continued

Once Mono is installed, a build script is available in the MMALSharp repository which will download all required NuGet packages and then subsequently build the project
for you and output a NuGet .pkg file which you can use in your application.

**Pre-release builds are available from [Myget](https://www.myget.org/gallery/mmalsharp)**

## .NET Core

.NET Core is currently available on the Raspberry Pi 2 & 3 boards, using an Ubuntu flavoured distribution, e.g. Ubuntu MATE 16.04 (LTS).

### Installation
 
1. Download the .NET Core SDK v2.0 from [here](https://github.com/dotnet/cli) - scroll down to the 'Installers and Binaries' section and download & install the appropriate binaries.
2. Install the following packages on your Raspberry Pi: `sudo apt-get install libunwind8 libunwind8-dev gettext libicu-dev liblttng-ust-dev libcurl4-openssl-dev libssl-dev uuid-dev unzip`
3. Clone MMALSharp by running `git clone https://github.com/techyian/MMALSharp.git`
4. Enter the `.paket` directory and run paket.bootstrapper.exe
5. When complete, a new executable `paket.exe` can be found in the same directory - run `paket install` which will install all dependencies required for MMALSharp.
6. Change directory back to the root solution level `cd ..`
7. Run `dotnet restore` which will configure the .NET Core projects.
8. Run `dotnet publish -r ubuntu.16.04-arm` - this will create a new directory called `publish` within the `/src/MMALSharpCoreExample/bin/Debug/netcoreapp2.0/ directory.
9. Copy the contents of that folder over to your Raspberry Pi
10. Download and extract the .NET Core runtime on your Pi from [here](https://github.com/dotnet/core-setup#daily-builds), ensuring you choose the correct distribution ([Ubuntu 16.04 download location](https://dotnetcli.blob.core.windows.net/dotnet/master/Binaries/Latest/dotnet-ubuntu.16.04-arm.latest.tar.gz) )
11. Within the extracted directory will be an application called `dotnet`, run `sudo chmod +x ./dotnet` to make it executable, then run `dotnet LOCATION OF YOUR MMALSharpCoreExample.dll`.

## Building from Source

** These instructions apply to Mono 4.x - for .NET Core build instructions, please see above.**

If you wish to build from source, follow the below steps:

1. Clone the repository by running `git clone https://github.com/techyian/MMALSharp.git`
2. Enter the `.paket` directory, and run `paket.bootstrapper.exe` - this will download an executable `paket.exe` within the same directory
3. Run `paket.exe install` to download all NuGet packages required by MMALSharp.
4. Open the solution in Visual Studio
5. Build the `MMALSharp`, this will subsequently build `MMALSharp.Common` - if you require the FFmpeg helper methods, build the `MMALSharp.FFmpeg` project too. This will
then output the relevant .dll files you need to reference in your application.


 