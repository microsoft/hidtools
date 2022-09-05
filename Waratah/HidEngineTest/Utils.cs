﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest
{
    using Microsoft.HidTools.HidEngine;
    using Microsoft.HidTools.HidEngine.CppGenerator;
    using Microsoft.HidTools.HidSpecification;

    public class Utils
    {
        public static void GlobalReset()
        {
            HidUsageTableDefinitions.GetInstance(true);

            HidUnitDefinitions.GetInstance(true);

            // Cleans-up any modification made to the global settings.
            // Parsing settings will pollute the global settings.
            Settings.GetInstance(true);

            // Cleans-up any modification made to the global UniqueNameCache.
            UniqueMemberNameCache.Reset();
            UniqueStructNameCache.Reset();

            CppEnum.Reset();
        }
    }
}
