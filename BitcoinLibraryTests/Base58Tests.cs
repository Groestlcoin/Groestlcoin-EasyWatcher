using GroestlcoinLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroestlcoinLibraryTests
{
    [TestClass]
    public class Base58Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Examples are from https://chainz.cryptoid.info/grs/#!rich
            string[] correctAddresses = 
            {
                "3PzGYz5R9JPtUAVRP996dVn2iSugy9gsMU",
                "36SDhzC2o12VFUsoGdCMfzVrdN8SzGyncY"
            };
            string[] incorrectAddresses = 
            {
                "3BSDhzC2o12VFUsoGdCMfzVrdN8SzGyncY", 
                "3A92t1WpEZ73CNmQviecrnyiWrnqRhWNLy", 
                "5BvBMSEYStWetqTFn5Au4m4GFg7xJaNVN2", 
                " ", 
                "" 
            };

            foreach (var addr in correctAddresses)
            {
                Assert.AreEqual(true, Base58.Verify(addr).IsVerified);
            }

            foreach (var addr in incorrectAddresses)
            {
                Assert.AreEqual(false, Base58.Verify(addr).IsVerified);
            }
        }

    }
}
