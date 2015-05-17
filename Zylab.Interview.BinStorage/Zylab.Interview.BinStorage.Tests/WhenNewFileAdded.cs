using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage.Tests
{
    [TestClass]
    public class WhenNewFileAdded
    {
        protected static IPersistentIndexStorage PersistentIndexStorage;
        protected static IPersistentStreamStorage PersistentStreamStorage;
        protected static IndexStorage IndexStorage;
        protected static StreamStorage StreamStorage;
        protected static IBinaryStorage BinaryStorage;
        protected const string TestKey = "testkey";
        protected static Stream TestStream;
        protected static byte[] Bytes;
        

        [ClassInitialize]
        public static void Given(TestContext context)
        {
            PersistentIndexStorage = new FakePersistentIndexStorage();
            IndexStorage = new IndexStorage(PersistentIndexStorage);
            PersistentStreamStorage = new FakePersistentStreamStorage();
            StreamStorage = new StreamStorage(PersistentStreamStorage);
            BinaryStorage = new BinaryStorage(new StorageConfiguration(), IndexStorage, StreamStorage);

            Random random = new Random();
            
            // write first 128 bytes before new file to test non-zero index
            Bytes = new byte[128];
            random.NextBytes(Bytes);
            BinaryStorage.Add("fakekey", new MemoryStream(Bytes), new StreamInfo());
            // ------------------------------------------------------------
            
            Bytes = new byte[1024];
            random.NextBytes(Bytes);
            TestStream = new MemoryStream(Bytes);
            BinaryStorage.Add(TestKey, TestStream, new StreamInfo());
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
        public void AndTheSameStreamContent()
        {
            Stream stream = BinaryStorage.Get(TestKey);
            for (int i = 0; i < Bytes.Length; i++)
            {
                int actual = stream.ReadByte();
                Assert.AreEqual(Bytes[i],actual);
            }
        }

        [TestMethod]
        public void IndexStoredPersistently()
        {
            Index index;
            Assert.IsTrue(PersistentIndexStorage.Restore().TryGetValue(TestKey, out index));
            Assert.IsNotNull(index);
        }

        [TestMethod]
        public void IndexHasCorrectOffset()
        {
            var index = PersistentIndexStorage.Restore()[TestKey];
            Assert.AreEqual(128, index.Offset);
        }

        [TestMethod]
        public void IndexHasCorrectSize()
        {
            var index = PersistentIndexStorage.Restore()[TestKey];
            Assert.AreEqual(1024, index.Size);
        }
    
        [ClassCleanup]
        public static void Cleanup()
        {
            TestStream.Dispose();
            BinaryStorage.Dispose();
        }
    }
}
