# Project

This repo is the central location of Microsoft HID Tools.  Currently, the only tool available is Waratah.

# Waratah

> What A Really Awesome Tool for Authoring HIDs

Waratah is a HID descriptor composition tool.  It offers a high-level of abstraction, eliminates common errors (by design), and optimizes the descriptor to reduce byte size.  It implements the [HID 1.11](https://www.usb.org/sites/default/files/hid1_11.pdf) specification so developers don't have to.

It is expected to be used by device firmware authors during device bring-up.

## Overview

Waratah uses a [TOML-like](https://toml.io/en/) hierarchical language of sections and keys to represent a HID Report Descriptor  (*Note: There is currently no support for HID Physical Descriptors*).  This can then be compiled to to either a simple plain-text format, or a C++ header file suitable for ingestion into device firmware.

> Waratah is NOT a direct `dt.exe` replacement.  `dt.exe` permits the use of specialized items (e.g. Push/Pop) and non-optimal practices (e.g. ReportSize larger than LogicalMinimum/Maximum).  There are also known bugs in `dt.exe`, that have not been replicated in Waratah.  It is reasonable to think of Waratah as high-level compiler like `C` and `dt.exe` as an assembler.  *No further development of `dt.exe` is planned.*

### Features:
- Human-readable text for easy composition and meaningful source-control management.
- Inbuilt with all public Usages from [HID Usage Table](https://www.usb.org/sites/default/files/hut1_22.pdf)
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

### Limitations/Differences to standard TOML:
- Only decimal/float number format supported (not hex/octal/binary)

### Example: Simple mouse with 3 buttons.

```toml
[[applicationCollection]]
usage = ['Generic Desktop', 'Mouse']

    [[applicationCollection.inputReport]]
    id = 1

        [[applicationCollection.inputReport.physicalCollection]]
        usage = ['Generic Desktop', 'Pointer']

            [[applicationCollection.inputReport.physicalCollection.variableItem]]
            usage = ['Generic Desktop', 'X']
            logicalValueRange = [-128, 127]
            reportFlags = ['relative']

            [[applicationCollection.inputReport.physicalCollection.variableItem]]
            usage = ['Generic Desktop', 'Y']
            logicalValueRange = [-128, 127]
            reportFlags = ['relative']

            [[applicationCollection.inputReport.physicalCollection.variableItem]]
            usageRange = ['Button', 'Button 1', 'Button 3']
            logicalValueRange = [0, 1]
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
