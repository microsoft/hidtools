// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngineTest
{
    using Microsoft.HidTools.HidSpecification;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class HidUnitTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Utils.GlobalReset();
        }

        [TestMethod]
        public void SimpleUnitCreation()
        {
            HidUnit testUnit = new HidUnit("testUnit");
            testUnit.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("centimeter"), dimensionMultiplier:100, dimensionPowerExponent:1);

            Assert.AreEqual(100, testUnit.Multiplier);
            Assert.AreEqual(1, testUnit.LengthExponent);
        }

        [TestMethod]
        public void InvalidDimensionBaseUnit()
        {
            // Dimension BaseUnit cannot be null
            {
                HidUnit testUnit = new HidUnit("testUnit");
                Assert.ThrowsException<HidSpecificationException>(() => testUnit.TryAddDimension(null, 1, 1));
            }

            // Dimension BaseUnit must have a least 1 valid exponent.
            {
                HidUnit testUnit = new HidUnit("testUnit");
                HidUnit testUnit2 = new HidUnit("testUnit2");

                Assert.ThrowsException<HidSpecificationException>(() => testUnit.TryAddDimension(testUnit2, 1, 1));
            }
        }

        [TestMethod]
        public void InvalidDimensionMultiplier()
        {
            HidUnit centimeter = HidUnitDefinitions.GetInstance().TryFindUnitByName("centimeter");

            // Cannot be 0.
            {
                HidUnit testUnit = new HidUnit("testUnit");
                Assert.ThrowsException<HidSpecificationException>(() => testUnit.TryAddDimension(centimeter, 0, 1));
            }

            // Must be a factor of 10 (e.g. 1, 10, 0.01).
            {
                HidUnit testUnit = new HidUnit("testUnit");
                Assert.ThrowsException<HidSpecificationException>(() => testUnit.TryAddDimension(centimeter, 2, 1));
            }

            {
                HidUnit testUnit = new HidUnit("testUnit");
                Assert.ThrowsException<HidSpecificationException>(() => testUnit.TryAddDimension(centimeter, 0.2, 1));
            }

            // Cannot be negative
            {
                HidUnit testUnit = new HidUnit("testUnit");
                Assert.ThrowsException<HidSpecificationException>(() => testUnit.TryAddDimension(centimeter, -1, 1));
            }
        }

        [TestMethod]
        public void InvalidDimensionExponent()
        {
            HidUnit centimeter = HidUnitDefinitions.GetInstance().TryFindUnitByName("centimeter");

            // Cannot be 0.
            {
                HidUnit testUnit = new HidUnit("testUnit");
                Assert.ThrowsException<HidSpecificationException>(() => testUnit.TryAddDimension(centimeter, 1, 0));
            }
        }

        [TestMethod]
        public void AddedDimensionsDontOverlapSystemKinds()
        {
            // All added dimensions must overlap at least 1 SystemKind.
            HidUnit centimeter = HidUnitDefinitions.GetInstance().TryFindUnitByName("centimeter");
            HidUnit inch = HidUnitDefinitions.GetInstance().TryFindUnitByName("inch");

            HidUnit testUnit = new HidUnit("testUnit");

            testUnit.TryAddDimension(centimeter, 1, 1);
            Assert.ThrowsException<HidSpecificationException>(() => testUnit.TryAddDimension(inch, 1, 1));
        }

        [TestMethod]
        public void AddedDimensionsOverlapSystemKinds()
        {
            // All added dimensions must overlap at least 1 SystemKind.
            HidUnit second = HidUnitDefinitions.GetInstance().TryFindUnitByName("second");
            HidUnit kelvin = HidUnitDefinitions.GetInstance().TryFindUnitByName("kelvin");

            HidUnit testUnit = new HidUnit("testUnit");

            testUnit.TryAddDimension(second, 1, 1);
            testUnit.TryAddDimension(kelvin, 1, 1);

            Assert.AreEqual(HidConstants.UnitItemSystemKind.SiLinear, testUnit.SystemKind);
        }

        [TestMethod]
        public void ValidDerivedUnit()
        {
            HidUnit centimeter = HidUnitDefinitions.GetInstance().TryFindUnitByName("centimeter");

            // Adding multiple dimensions of the same parameters will multiply them together.
            HidUnit testUnit = new HidUnit("testUnit");

            // New Unit is 100 centimeters.
            testUnit.TryAddDimension(centimeter, 100, 1);

            // New Unit is 100 centimeters * 100 centimeters
            // ==> 10_000 centimeters^2 (i.e. 1 square meter)
            testUnit.TryAddDimension(centimeter, 100, 1);
            Assert.AreEqual(10_000, testUnit.Multiplier);
            Assert.AreEqual(2, testUnit.LengthExponent);

            // New Unit is 10_000 centimeters^2 * 0.01 centimeters
            // ==> 100 centimeters^3
            testUnit.TryAddDimension(centimeter, 0.01, 1);
            Assert.AreEqual(100, testUnit.Multiplier);
            Assert.AreEqual(3, testUnit.LengthExponent);

            // New Unit is 100 centimeters^3 * 0.01 centimeters^-2
            // ==> 1 centimeters^1
            testUnit.TryAddDimension(centimeter, 0.01, -2);
            Assert.AreEqual(1, testUnit.Multiplier);
            Assert.AreEqual(1, testUnit.LengthExponent);

            // New Unit is 1 centimeters^0
            // ==> 1 (nothing)
            testUnit.TryAddDimension(centimeter, 1, -1);
            Assert.AreEqual(1, testUnit.Multiplier);
            Assert.AreEqual(0, testUnit.LengthExponent);
        }

        [TestMethod]
        public void SIUnitDerivedCombinations()
        {
            {
                HidUnit meter = new HidUnit("meter");
                meter.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("centimeter"), 100, 1);

                Assert.AreEqual(100, meter.Multiplier);
                Assert.AreEqual(1, meter.LengthExponent);

                HidUnitDefinitions.GetInstance().TryAddUnit(meter);
            }

            {
                HidUnit metersquared = new HidUnit("meter^2");
                metersquared.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("meter"), 1, 2);

                Assert.AreEqual(10_000, metersquared.Multiplier);
                Assert.AreEqual(2, metersquared.LengthExponent);

                HidUnitDefinitions.GetInstance().TryAddUnit(metersquared);
            }

            {
                HidUnit kg = new HidUnit("kg");
                kg.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("gram"), 1_000, 1);

                Assert.AreEqual(1_000, kg.Multiplier);
                Assert.AreEqual(1, kg.MassExponent);

                HidUnitDefinitions.GetInstance().TryAddUnit(kg);
            }

            {
                HidUnit newton = new HidUnit("newton");
                newton.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("meter"), 1, 1);
                newton.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("kg"), 1, 1);
                newton.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("second"), 1, -2);

                Assert.AreEqual(100_000, newton.Multiplier);
                Assert.AreEqual(1, newton.LengthExponent);
                Assert.AreEqual(1, newton.MassExponent);
                Assert.AreEqual(-2, newton.TimeExponent);

                HidUnitDefinitions.GetInstance().TryAddUnit(newton);
            }

            {
                HidUnit newtonMeterSecond = new HidUnit("newtonMeterSecond");
                newtonMeterSecond.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("newton"), 1, 1);
                newtonMeterSecond.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("meter"), 1, 1);
                newtonMeterSecond.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("second"), 1, 1);

                Assert.AreEqual(10_000_000, newtonMeterSecond.Multiplier);
                Assert.AreEqual(2, newtonMeterSecond.LengthExponent);
                Assert.AreEqual(1, newtonMeterSecond.MassExponent);
                Assert.AreEqual(-1, newtonMeterSecond.TimeExponent);

                HidUnitDefinitions.GetInstance().TryAddUnit(newtonMeterSecond);
            }

            {
                HidUnit farad = new HidUnit("farad");
                farad.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("meter"), 1, -2);
                farad.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("kg"), 1, -1);
                farad.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("second"), 1, 4);
                farad.TryAddDimension(HidUnitDefinitions.GetInstance().TryFindUnitByName("ampere"), 1, 2);

                Assert.AreEqual(10_000_000, farad.Multiplier);
                Assert.AreEqual(-2, farad.LengthExponent);
                Assert.AreEqual(-1, farad.MassExponent);
                Assert.AreEqual(4, farad.TimeExponent);
                Assert.AreEqual(2, farad.CurrentExponent);

                HidUnitDefinitions.GetInstance().TryAddUnit(farad);
            }
        }
    }
}
