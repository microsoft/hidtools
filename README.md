# Project

This is a fork of Waratah that can be built using `mono`

## Build Dependencies

You'll need the following tools

- [mono](https://www.mono-project.com/download/stable/)
- [nuget](https://www.nuget.org/downloads)
    - You'll want to download the `Windows x86 Commandline` version
    - Make note of the path to `nuget.exe`, as you'll need it later in the build
    - Attribution [to this article](https://strongbox.github.io/user-guide/tool-integration/nuget-mono-example.html) that covers using `nuget` on Linux.


## Building the project

### Getting `nuget` packages

First, you'll need to download the nuget packages:

```shell
mono --runtime=v4.0 [path-to-nuget]/nuget.exe ./Waratah/Waratah.sln
```

Or, if you've created a `nuget` alias, you can just use `nuget`

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

NOTE: you may get an error about missing `dll`s. If that's the case, you can add the `--sdk` option. [Source](https://stackoverflow.com/a/42201087)

```shell
mkbundle -o WaratahCmd ./Waratah/WarataCmd/bin/Release/WaratahCmd.exe --sdk [path-to-sdk]
```

The path to mono's `SDK` is different across different linux distributions. For `debian-12`, it's actually just `/`:

```shell
mkbundle -o WaratahCmd ./Waratah/WarataCmd/bin/Release/WaratahCmd.exe --sdk /
```

For help on finding where the mono `sdk` is in your linux distribution, the [arch linux `mkbundle` man page](https://man.archlinux.org/man/extra/mono/mkbundle.1.en#sdk) says the following:

```plaintext
--sdk SDK_PATH
    Use this flag to specify a path from which mkbundle will resolve the Mono
    SDK from. The SDK path should be the prefix path that you used to configure
    a Mono installation. And would typically contain files like
    SDK_PATH/bin/mono, SDK_PATH/lib/mono/4.5 and so on. 
```

# Original README

This repo is the central location of Microsoft HID Tools.  Currently, the only tool available is Waratah.  The underlying HidSpecification and HidEngine libaries are available via Nuget

https://www.nuget.org/packages/Microsoft.HidTools.HidSpecification

https://www.nuget.org/packages/Microsoft.HidTools.HidEngine

Or from the package manager console:

    > Install-Package Microsoft.HidTools.HidEngine -Version 1.3.0

# Waratah

> What A Really Awesome Tool for Authoring HIDs

Waratah is a HID descriptor composition tool.  It offers a high-level of abstraction, eliminates common errors (by design), and optimizes the descriptor to reduce byte size.  It implements the [HID 1.11](https://www.usb.org/sites/default/files/hid1_11.pdf) specification so developers don't have to.

It is expected to be used by device firmware authors during device bring-up.

See [Wiki](https://github.com/microsoft/hidtools/wiki) for more details 

## Overview

Waratah uses a [TOML-like](https://toml.io/en/) hierarchical language of sections and keys to represent a HID Report Descriptor  (*Note: There is currently no support for HID Physical Descriptors*).  This can then be compiled to to either a simple plain-text format, or a C++ header file suitable for ingestion into device firmware.

> Waratah is NOT a direct `dt.exe` replacement.  `dt.exe` permits the use of specialized items (e.g. Push/Pop) and non-optimal practices (e.g. ReportSize larger than LogicalMinimum/Maximum).  There are also known bugs in `dt.exe`, that have not been replicated in Waratah.  It is reasonable to think of Waratah as high-level compiler like `C` and `dt.exe` as an assembler.  *No further development of `dt.exe` is planned.*

### Features:
- Human-readable text for easy composition and meaningful source-control management.
- Inbuilt with all public Usages from [HID Usage Table](https://www.usb.org/sites/default/files/hut1_5.pdf)
- Composition of custom Vendor Usages.
- Inbuilt with all defined Units.
- Support for composition of new named Units.
- Comprehensive error messaging with line-level blame.
- C++ struct generation with context-aware variable-name generation.
- Optimistic descriptor optimization (redundant global items removed, and items combined).
- Report summary with itemized Id/Type/Size.
- Context-aware integer types bounds validation (ReportId, UsageId,...).
- Context-aware Usage type validation.
- Automatic generation of ReportIds.
- Automatic 8bit report alignment (padding inserted as needed).
- Automatic collection termination.
- Report Item size inferred from LogicalMin/Max (or vice-versa).
- Overspanning preventation, guaranteeing no item spans more than 4bytes (padding inserted as needed).
- Validate conditionally invalid Report Flags.
- Inline Usage transformation.
- Usage Range validation.

### Example: Simple mouse with 3 buttons.

```toml
[[applicationCollection]]
usage = ['Generic Desktop', 'Mouse']

    [[applicationCollection.inputReport]]

        [[applicationCollection.inputReport.physicalCollection]]
        usage = ['Generic Desktop', 'Pointer']

            [[applicationCollection.inputReport.physicalCollection.variableItem]]
            usage = ['Generic Desktop', 'X']
            sizeInBits = 8
            logicalValueRange = 'maxSignedSizeRange'
            reportFlags = ['relative']

            [[applicationCollection.inputReport.physicalCollection.variableItem]]
            usage = ['Generic Desktop', 'Y']
            sizeInBits = 8
            logicalValueRange = 'maxSignedSizeRange'
            reportFlags = ['relative']

            [[applicationCollection.inputReport.physicalCollection.variableItem]]
            usageRange = ['Button', 'Button 1', 'Button 3']
            logicalValueRange = [0, 1]
```

#### Plain-text output
```
05-01....UsagePage(Generic Desktop[1])
09-02....UsageId(Mouse[2])
A1-01....Collection(Application)
85-01........ReportId(1)
09-01........UsageId(Pointer[1])
A1-00........Collection(Physical)
09-30............UsageId(X[48])
09-31............UsageId(Y[49])
15-80............LogicalMinimum(-128)
25-7F............LogicalMaximum(127)
95-02............ReportCount(2)
75-08............ReportSize(8)
81-06............Input(Data, Variable, Relative, NoWrap, Linear, PreferredState, NoNullPosition, BitField)
05-09............UsagePage(Button[9])
19-01............UsageIdMin(Button 1[1])
29-03............UsageIdMax(Button 3[3])
15-00............LogicalMinimum(0)
25-01............LogicalMaximum(1)
95-03............ReportCount(3)
75-01............ReportSize(1)
81-02............Input(Data, Variable, Absolute, NoWrap, Linear, PreferredState, NoNullPosition, BitField)
C0...........EndCollection()
95-01........ReportCount(1)
75-05........ReportSize(5)
81-03........Input(Constant, Variable, Absolute, NoWrap, Linear, PreferredState, NoNullPosition, BitField)
C0.......EndCollection()
```

#### C++ output
```C++
// AUTO-GENERATED by WaratahCmd.exe

#include <memory>

// HID Usage Tables: 1.3.0
// Descriptor size: 50 (bytes)
// +----------+-------+------------------+
// | ReportId | Kind  | ReportSizeInBits |
// +----------+-------+------------------+
// |        1 | Input |               24 |
// +----------+-------+------------------+
static const uint8_t hidReportDescriptor [] = 
{
    0x05, 0x01,    // UsagePage(Generic Desktop[1])
    0x09, 0x02,    // UsageId(Mouse[2])
    0xA1, 0x01,    // Collection(Application)
    0x85, 0x01,    //     ReportId(1)
    0x09, 0x01,    //     UsageId(Pointer[1])
    0xA1, 0x00,    //     Collection(Physical)
    0x09, 0x30,    //         UsageId(X[48])
    0x09, 0x31,    //         UsageId(Y[49])
    0x15, 0x80,    //         LogicalMinimum(-128)
    0x25, 0x7F,    //         LogicalMaximum(127)
    0x95, 0x02,    //         ReportCount(2)
    0x75, 0x08,    //         ReportSize(8)
    0x81, 0x06,    //         Input(Data, Variable, Relative, NoWrap, Linear, PreferredState, NoNullPosition, BitField)
    0x05, 0x09,    //         UsagePage(Button[9])
    0x19, 0x01,    //         UsageIdMin(Button 1[1])
    0x29, 0x03,    //         UsageIdMax(Button 3[3])
    0x15, 0x00,    //         LogicalMinimum(0)
    0x25, 0x01,    //         LogicalMaximum(1)
    0x95, 0x03,    //         ReportCount(3)
    0x75, 0x01,    //         ReportSize(1)
    0x81, 0x02,    //         Input(Data, Variable, Absolute, NoWrap, Linear, PreferredState, NoNullPosition, BitField)
    0xC0,          //     EndCollection()
    0x95, 0x01,    //     ReportCount(1)
    0x75, 0x05,    //     ReportSize(5)
    0x81, 0x03,    //     Input(Constant, Variable, Absolute, NoWrap, Linear, PreferredState, NoNullPosition, BitField)
    0xC0,          // EndCollection()
};
```

# Legal

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
