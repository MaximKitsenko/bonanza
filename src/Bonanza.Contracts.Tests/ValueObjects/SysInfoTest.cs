using System;
using Bonanza.Contracts.ValueObjects;
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
            // a
            var dateTime = DateTime.UtcNow;
            var sysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);
            var otherSysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);

            // a
            var objectsAreEqual = sysInfo.Equals(otherSysInfo);

            // a
            Assert.IsTrue(objectsAreEqual);
        }

        [Test]
        public void GivenNotNullSysInfo_WhenCompareWithAnotherObjectOfSysInfoWithDifferentTenantId_ThenObjectsAreNotEqual()
        {
            // a
            var dateTime = DateTime.UtcNow;
            var sysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);
            var otherSysInfo = SysInfo.CreateSysInfo(new TenantId(333), new UserId(1), dateTime);

            // a
            var objectsAreEqual = sysInfo.Equals(otherSysInfo);

            // a
            Assert.IsFalse(objectsAreEqual);
        }

        [Test]
        public void GivenNotNullSysInfo_WhenCompareWithAnotherObjectOfSysInfoWithDifferentUserId_ThenObjectsAreNotEqual()
        {
            // a
            var dateTime = DateTime.UtcNow;
            var sysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);
            var otherSysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(333), dateTime);

            // a
            var objectsAreEqual = sysInfo.Equals(otherSysInfo);

            // a
            Assert.IsFalse(objectsAreEqual);
        }

        [Test]
        public void GivenNotNullSysInfo_WhenCompareWithAnotherObjectOfSysInfoWithDifferentDateTime_ThenObjectsAreNotEqual()
        {
            // a
            var dateTime = DateTime.UtcNow;
            var sysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime);
            var otherSysInfo = SysInfo.CreateSysInfo(new TenantId(2), new UserId(1), dateTime.AddMilliseconds(1));

            // a
            var objectsAreEqual = sysInfo.Equals(otherSysInfo);

            // a
            Assert.IsFalse(objectsAreEqual);
        }
    }
}