using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking2._0;

namespace PragueParking.Tests
{
    [TestClass]
    public class ParkingTests
    {
        [TestMethod]
        public void TestVehicleCheckIn_ReturnTrue()
        {
            // Arrange
            ParkingGarage garage = new ParkingGarage(10);
            Car car = new Car("TEST123");

            // Act
            bool result = garage.CheckInVehicle(car);

            // Assert
            Assert.IsTrue(result, "Vehicle should be checked in successfully");
            Assert.IsTrue(garage.VehicleExists("TEST123"), "Vehicle should exist in garage");
        }

        [TestMethod]
        public void TestVehicleCheckIn_Duplicate_ReturnFalse()
        {
            // Arrange
            ParkingGarage garage = new ParkingGarage(10);
            Car car1 = new Car("TEST123");
            Car car2 = new Car("TEST123");

            // Act
            garage.CheckInVehicle(car1);
            bool result = garage.CheckInVehicle(car2);

            // Assert
            Assert.IsFalse(result, "Duplicate vehicle should not be checked in");
        }
    }
}