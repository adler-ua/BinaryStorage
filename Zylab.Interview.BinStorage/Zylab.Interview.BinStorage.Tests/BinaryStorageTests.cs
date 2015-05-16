using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zylab.Interview.BinStorage.Tests
{
    [TestClass]
    public class WhenNewItemAdded
    {
        protected static IBinaryStorage BinaryStorage;
        protected const string TestKey = "testkey";
        protected static Stream TestStream;
        protected static byte[] Bytes;

        [ClassInitialize]
        public static void Given(TestContext context)
        {
            BinaryStorage = new BinaryStorage(new StorageConfiguration());
            Bytes = new byte[1024];
            Random random = new Random();
            random.NextBytes(Bytes);
            TestStream = new MemoryStream(Bytes);
            BinaryStorage.Add(TestKey, TestStream, new StreamInfo());
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            TestStream.Dispose();
            BinaryStorage.Dispose();
        }

        [TestMethod]
        public void ThenStorageContainsKey()
        {
            Assert.IsTrue(BinaryStorage.Contains(TestKey));
        }

        [TestMethod]
        public void AndStorageReturnsStream()
        {
            Stream stream = BinaryStorage.Get(TestKey);
            Assert.IsNotNull(stream);
        }

        [TestMethod]
        public void WithTheSameLength()
        {
            Stream stream = BinaryStorage.Get(TestKey);
            Assert.AreEqual(Bytes.Length, stream.Length);
        }

        [TestMethod]
        public void AndTheSameContent()
        {
            Stream stream = BinaryStorage.Get(TestKey);
            for (int i = 0; i < Bytes.Length; i++)
            {
                Assert.AreEqual(Bytes[i],stream.ReadByte());
            }
        }
    }
}
