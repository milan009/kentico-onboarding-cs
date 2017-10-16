using System;
using ListApp.Contracts.Interfaces;
using ListApp.Services.Helpers;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ListApp.Services.Tests.Helpers
{
    [TestFixture]
    internal class TimeHelperTests
    {
        private ITimeService _timeService;

        [SetUp]
        public void Setup()
        {
            _timeService = new TimeService();
        }

        [Test]
        public void GetCurrentTime_DoesNotReturnDefaultValue()
        {
            var dateTime = _timeService.GetCurrentTime();

            Assert.That(dateTime, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public async Task GetCurrentTime_IsNotCounter()
        {
            var tolerance = TimeSpan.FromMilliseconds(2);
            var millisToWait = TimeSpan.FromMilliseconds(200);

            var dateTime1 = _timeService.GetCurrentTime();
            await Task.Delay(millisToWait);
            var dateTime2 = _timeService.GetCurrentTime();
            var difference = dateTime2 - dateTime1;

            Assert.That(dateTime2, Is.GreaterThan(dateTime1));
            Assert.That(difference, Is.InRange(millisToWait - tolerance, millisToWait + tolerance));
        }
    }
}
