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
    public class WhenAttemptToUseWrongKeyToGet
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
            BinaryStorage = new BinaryStorage(new StorageConfiguration(), IndexStorage, StreamStorage, new Compressor());

            Random random = new Random();

            // write first 128 bytes before new file to test non-zero index
            Bytes = new byte[128];
            random.NextBytes(Bytes);
            BinaryStorage.Add("fakekey", new MemoryStream(Bytes), new StreamInfo());
            // ------------------------------------------------------------

            Bytes = new byte[1024];
            random.NextBytes(Bytes);
            TestStream = new MemoryStream(Bytes);

            using (MD5 md5 = MD5.Create())
            {
                BinaryStorage.Add(TestKey, TestStream, new StreamInfo() { Hash = md5.ComputeHash(Bytes) });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownKeyException))]
        public void ThenExceptionIsThrown()
        {
            BinaryStorage.Get("unknown");
        }
    }
}
