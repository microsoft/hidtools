# Building for Operating systems other than Windows using Mono

While it is possible to build `Waratah` on systems other than Windows, it is not supported.

For anyone who would like to build `Waratah` on a non-Windows system, this guide is provided as a convenience.
Please do not open any issues related to non-Windows or `mono` builds of the project.

## Build Dependencies

You'll need the following tools

- [mono](https://www.mono-project.com/download/stable/)
- [nuget](https://www.nuget.org/downloads)
    - You'll want to download the `Windows x86 Commandline` version
    - Make note of the path to `nuget.exe`, as you'll need it later in the build


## Building the project

### Getting `nuget` packages

First, you'll need to download the nuget packages:

```shell
mono --runtime=v4.0 [path-to-nuget]/nuget.exe ./Waratah/Waratah.sln
```

### Building the project

Use `msbuild` to make the project

```shell
msbuild Waratah/Waratah.sln /p:Configuration=Release
```

That command creates the file `./Waratah/WaratahCmd/bin/Release/WaratahCmd.exe` that can be run like so:

```shell
mono ./Waratah/WaratahCmd/bin/Release/WaratahCmd.exe --source [path to config file]
```

### Making a stand-alone application

If you'd like to make a stand-alone version of the application, you can do so.


```shell
mkbundle -o WaratahCmd ./Waratah/WarataCmd/bin/Release/WaratahCmd.exe
```

NOTE: you may get an error about missing `dll`s. If that's the case, you can specify `mono`'s `sdk` path by adding the `--sdk` option.

```shell
mkbundle -o WaratahCmd ./Waratah/WarataCmd/bin/Release/WaratahCmd.exe --sdk [path-to-sdk]
```

The path to mono's `SDK` is different across different operating systems and different installation methods.

For help on finding where the mono `sdk` is in your linux distribution, the [arch linux `mkbundle` man page](https://man.archlinux.org/man/extra/mono/mkbundle.1.en#sdk) says the following:

```plaintext
--sdk SDK_PATH
    Use this flag to specify a path from which mkbundle will resolve the Mono
    SDK from. The SDK path should be the prefix path that you used to configure
    a Mono installation. And would typically contain files like
    SDK_PATH/bin/mono, SDK_PATH/lib/mono/4.5 and so on. 
```
