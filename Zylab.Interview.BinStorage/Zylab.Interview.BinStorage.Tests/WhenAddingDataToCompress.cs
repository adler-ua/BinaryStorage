using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zylab.Interview.BinStorage.Compressing;
using Zylab.Interview.BinStorage.FileStorage;
using Zylab.Interview.BinStorage.Indexing;

namespace Zylab.Interview.BinStorage.Tests
{
    [TestClass]
    public class WhenAddingDataToCompress
    {
        protected static IPersistentIndexStorage PersistentIndexStorage;
        protected static IPersistentStreamStorage PersistentStreamStorage;
        protected static IndexStorage IndexStorage;
        protected static StreamStorage StreamStorage;
        protected static IBinaryStorage BinaryStorage;
        protected const string TestKey1 = "testkey1";
        protected const string TestKey2 = "testkey2";
        protected const string TestKey3 = "testkey3";
        protected static Stream TestStream1;
        protected static Stream TestStream2;
        protected static Stream TestStream3;
        protected static byte[] Data1;
        protected static byte[] Data2;
        protected static byte[] Data3;


        [ClassInitialize]
        public static void Given(TestContext context)
        {
            PersistentIndexStorage = new FakePersistentIndexStorage();
            IndexStorage = new IndexStorage(PersistentIndexStorage);
            PersistentStreamStorage = new FakePersistentStreamStorage();
            StreamStorage = new StreamStorage(PersistentStreamStorage);
            BinaryStorage = new BinaryStorage(new StorageConfiguration() { CompressionThreshold = 1024 }, IndexStorage, StreamStorage, new Compressor());

            Random random = new Random();


            // write first 512 bytes before new file to test non-zero index
            Data1 = new byte[512];
            random.NextBytes(Data1);
            TestStream1 = new MemoryStream(Data1);
            using (MD5 md5 = MD5.Create())
            {
                BinaryStorage.Add(TestKey1, TestStream1, new StreamInfo() { Hash = md5.ComputeHash(Data1) });
            }
            // ------------------------------------------------------------

            Data2 = new byte[4096];
            random.NextBytes(Data2);
            TestStream2 = new MemoryStream(Data2);

            using (MD5 md5 = MD5.Create())
            {
                BinaryStorage.Add(TestKey2, TestStream2, new StreamInfo() { Hash = md5.ComputeHash(Data2) });
            }

            // write other 512 bytes after new file
            Data3 = new byte[512];
            random.NextBytes(Data3);
            TestStream3 = new MemoryStream(Data3);
            using (MD5 md5 = MD5.Create())
            {
                BinaryStorage.Add(TestKey3, TestStream3, new StreamInfo() { Hash = md5.ComputeHash(Data3) });
            }
            // ------------------------------------------------------------
        }

        [TestMethod]
        public void ThenFirstUncompressedFileContentCorrect()
        {
            Stream stream1 = BinaryStorage.Get(TestKey1);
            for (int i = 0; i < Data1.Length; i++)
            {
                int actual = stream1.ReadByte();
                Assert.AreEqual(Data1[i], actual);
            }
        }

        [TestMethod]
        public void CompressedFileContentCorrect()
        {
            Stream stream2 = BinaryStorage.Get(TestKey2);
            for (int i = 0; i < Data2.Length; i++)
            {
                int actual = stream2.ReadByte();
                Assert.AreEqual(Data2[i], actual);
            }
        }

        [TestMethod]
        public void ThirdUncompressedFileContentCorrect()
        {
            Stream stream3 = BinaryStorage.Get(TestKey3);
            for (int i = 0; i < Data3.Length; i++)
            {
                int actual = stream3.ReadByte();
                Assert.AreEqual(Data3[i], actual);
            }
        }

    }
}
