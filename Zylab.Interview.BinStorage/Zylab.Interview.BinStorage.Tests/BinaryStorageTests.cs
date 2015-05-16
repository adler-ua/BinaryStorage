using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage.Tests
{
    [TestClass]
    public class WhenNewItemAdded
    {
        protected static IPersistentIndexStorage PersistentIndexStorage;
        protected static IndexStorage IndexStorage;
        protected static IBinaryStorage BinaryStorage;
        protected const string TestKey = "testkey";
        protected static Stream TestStream;
        protected static byte[] Bytes;
        

        [ClassInitialize]
        public static void Given(TestContext context)
        {
            PersistentIndexStorage = new FakePersistentStorage();
            IndexStorage = new IndexStorage(PersistentIndexStorage);
            BinaryStorage = new BinaryStorage(new StorageConfiguration(), IndexStorage);
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

        [TestMethod]
        public void IndexStoredPersistently()
        {
            var index = PersistentIndexStorage.Restore().SingleOrDefault(item=>item.Key == TestKey);
            Assert.IsNotNull(index);
        }
    }
}
