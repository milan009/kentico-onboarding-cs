using System;
using System.Threading;
using ListApp.Contracts.Interfaces;
using NUnit.Framework;

namespace ListApp.Services.Tests
{
    [TestFixture]
    internal class TimeHelperTests
    {
        private ITimeHelper _timeHelper;

        [SetUp]
        public void Setup()
        {
            _timeHelper = new TimeHelper.TimeHelper();
        }

        [Test]
        public void GetCurrentTime_DoesNotReturnDefaultValue()
        {
            var dateTime = _timeHelper.GetCurrentTime();

            Assert.That(dateTime, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void GetCurrentTime_IsNotCounter()
        {
            var tolerance = TimeSpan.FromMilliseconds(2);
            var millisToWait = TimeSpan.FromMilliseconds(200);

            var dateTime1 = _timeHelper.GetCurrentTime();
            Thread.Sleep(millisToWait);
            var dateTime2 = _timeHelper.GetCurrentTime();
            var difference = dateTime2 - dateTime1;

            Assert.That(dateTime2, Is.GreaterThan(dateTime1));
            Assert.That(difference, Is.InRange(millisToWait - tolerance, millisToWait + tolerance));
        }
    }
}