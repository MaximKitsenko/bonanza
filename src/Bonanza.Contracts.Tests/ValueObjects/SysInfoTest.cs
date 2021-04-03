using System;
using Bonanza.Contracts.ValueObjects;
using Bonanza.Contracts.ValueObjects.Tenant;
using Bonanza.Contracts.ValueObjects.User;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Bonanza.Contracts.Tests.ValueObjects
{
    [TestFixture]
    public class SysInfoTest
    {
        [Test]
        public void GivenNotNullSysInfo_WhenCompareWithAnotherObjectOfSysInfoWithTheSameData_ThenObjectsAreEqual()
        {
            // arrange
            var dateTime = DateTime.UtcNow;
            var sysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);
            var otherSysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);

            // act
            var objectsAreEqual = sysInfo.Equals(otherSysInfo);

            // assert
            Assert.IsTrue(objectsAreEqual);
        }

        [Test]
        public void GivenNotNullSysInfo_WhenCompareWithAnotherObjectOfSysInfoWithDifferentTenantId_ThenObjectsAreNotEqual()
        {
            // arrange
            var dateTime = DateTime.UtcNow;
            var sysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);
            var otherSysInfo = SysInfo.CreateSysInfo(new TenantId(333), new UserId(1), dateTime);

            // act
            var objectsAreEqual = sysInfo.Equals(otherSysInfo);

            // assert
            Assert.IsFalse(objectsAreEqual);
        }

        [Test]
        public void GivenNotNullSysInfo_WhenCompareWithAnotherObjectOfSysInfoWithDifferentUserId_ThenObjectsAreNotEqual()
        {
            // arrange
            var dateTime = DateTime.UtcNow;
            var sysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);
            var otherSysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(333), dateTime);

            // act
            var objectsAreEqual = sysInfo.Equals(otherSysInfo);

            // assert
            Assert.IsFalse(objectsAreEqual);
        }

        [Test]
        public void GivenNotNullSysInfo_WhenCompareWithAnotherObjectOfSysInfoWithDifferentDateTime_ThenObjectsAreNotEqual()
        {
            // arrange
            var dateTime = DateTime.UtcNow;
            var sysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);
            var otherSysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime.AddMilliseconds(1));

            // act
            var objectsAreEqual = sysInfo.Equals(otherSysInfo);

            // assert
            Assert.IsFalse(objectsAreEqual);
        }
    }
}
