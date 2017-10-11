using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ListApp.Services.Tests
{
    [TestFixture]
    internal class GuidGeneratorTests
    {
        private IGuidGenerator _guidGenerator;

        [SetUp]
        public void Setup()
        {
            _guidGenerator = new GuidGenerator.GuidGenerator();
        }

        [Test]
        public void GenerateGuid_DoesNotReturnEmptyGuid()
        {
            var guid = _guidGenerator.GenerateGuid();
            
            Assert.That(guid, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void GenerateGuid_GeneratesEnoughUniqueGuids()
        {
            const int numberOfGuids = 4;

            var guids = Enumerable
                .Repeat<Func<Guid>>(() => _guidGenerator.GenerateGuid(), numberOfGuids)
                .Select(generator => generator()).ToArray();
            var distinctGuids = guids.Distinct().ToArray();

            Assert.That(guids, Is.EqualTo(distinctGuids));
        }
    }
}
