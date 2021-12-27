// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest
{
    using HidEngine.ReportDescriptorComposition;
    using HidEngine.ReportDescriptorComposition.Modules;
    using HidEngine.ReportDescriptorItems;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using HidEngine.TomlReportDescriptorParser;
    using HidSpecification;

    [TestClass]
    public class HidUnitDefinitionsTests
    {
        [TestInitialize]
        public void Initialize()
        {
            // Reset the Unit definitions after each test.
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleAdditionToDefinitions()
        {
            HidUnit testUnit = new HidUnit("testUnit");
            testUnit.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("centimeter"), dimensionMultiplier: 100, dimensionPowerExponent: 1);

            HidUnitDefinitions.GetInstance().TryAddUnit(testUnit);

            // New Unit can now be looked-up.
            HidUnitDefinitions.GetInstance().TryFindUnitByName("testUnit");
        }
        [TestMethod]
        public void NonExistingUnitCantBeFound()
        {
            Assert.IsNull(HidUnitDefinitions.GetInstance().TryFindUnitByName("invalidUnitName"));
        }

        [TestMethod]
        public void InvalidAdditionToDefinitions()
        {
            HidUnit testUnit = new HidUnit("testUnit");
            testUnit.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("centimeter"), dimensionMultiplier: 100, dimensionPowerExponent: 1);

            HidUnitDefinitions.GetInstance().TryAddUnit(testUnit);

            // Cannot add a Unit with the same name as an existing Unit.
            Assert.ThrowsException<HidSpecificationException>(() => HidUnitDefinitions.GetInstance().TryAddUnit(testUnit));
        }
    }
}
