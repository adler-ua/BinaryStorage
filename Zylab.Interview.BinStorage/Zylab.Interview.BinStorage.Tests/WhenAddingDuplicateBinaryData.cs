using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage.Tests
{
    [TestClass]
    public class WhenAddingDuplicateBinaryData
    {
        protected static IPersistentIndexStorage PersistentIndexStorage;
        protected static IPersistentStreamStorage PersistentStreamStorage;
        protected static IndexStorage IndexStorage;
        protected static StreamStorage StreamStorage;
        protected static IBinaryStorage BinaryStorage;
        protected const string TestKey1 = "testkey1";
        protected const string TestKey2 = "testkey2";
        protected static Stream TestStream1;
        protected static Stream TestStream2;
        protected static byte[] Data1;
        protected static byte[] Data2;


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
            var bytes = new byte[128];
            random.NextBytes(bytes);
            BinaryStorage.Add("fakekey", new MemoryStream(bytes), new StreamInfo());
            // ------------------------------------------------------------
            

            // write first 128 bytes before new file to test non-zero index
            Data1 = new byte[1024];
            random.NextBytes(Data1);
            TestStream1 = new MemoryStream(Data1);
            using (MD5 md5 = MD5.Create())
            {
                BinaryStorage.Add(TestKey1, TestStream1, new StreamInfo() {Hash = md5.ComputeHash(Data1)});
            }
            // ------------------------------------------------------------

            Data2 = new byte[1024];
            Data1.CopyTo(Data2, 0);
            TestStream2 = new MemoryStream(Data2);

            using (MD5 md5 = MD5.Create())
            {
                BinaryStorage.Add(TestKey2, TestStream2, new StreamInfo() { Hash = md5.ComputeHash(Data2) });
            }
        }

        [TestMethod]
        public void ThenSecondIndexHasSameOffset()
        {
            Index index1 = IndexStorage.Get(TestKey1);
            Index index2 = IndexStorage.Get(TestKey2);
            Assert.AreEqual(128, index1.Offset);
            Assert.AreEqual(128, index2.Offset);
        }


        [TestMethod]
        public void AndIndexHasSameSize()
        {
            Index index1 = IndexStorage.Get(TestKey1);
            Index index2 = IndexStorage.Get(TestKey2);
            Assert.AreEqual(1024, index1.Size);
            Assert.AreEqual(1024, index2.Size);
        }


        [TestMethod]
        public void AndBothHaveTheSameContent()
        {
            Stream stream1 = BinaryStorage.Get(TestKey1);
            Stream stream2 = BinaryStorage.Get(TestKey2);
            for (int i = 0; i < Data1.Length; i++)
            {
                int actual = stream1.ReadByte();
                Assert.AreEqual(Data1[i], actual);
                actual = stream2.ReadByte();
                Assert.AreEqual(Data1[i], actual);
            }
        }
    }
}
