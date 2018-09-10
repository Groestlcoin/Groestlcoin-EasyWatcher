using GroestlcoinLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroestlcoinLibraryTests
{
    [TestClass]
    public class SegWitAddressTests
    {
        [TestMethod]
        public void VerifyTest()
        {
            string[] correctAddresses = 
            {
                "grs1q7y35lw3fgev6fhgep4d8fqtj8cuvxdcwmfpqhx",
                "grs1qgyl6qr6jwj92j49ceskp089taqd3vsms5g9508", 
            };
            string[] incorrectAddresses = 
            { 
                "grs117y35lw3fgev6fhgep4d8fqtj8cuvxdcwmfpqhx",
                "grs10ody35lw3fgev6fhgep4d8fqtj8cuvxdcwmfpqhx", 
                "bc1rw5uspcuh", 
                "bc10w508d6qejxtdg4y5r3zarvary0c5xw7kw508d6qejxtdg4y5r3zarvary0c5xw7kw5rljs90",
                "BC1QR508D6QEJXTDG4Y5R3ZARVARYV98GJ9P",
                "bc1zw508d6qejxtdg4y5r3zarvaryvqyzf3du",
                "bc1gmk9yu"
            };

            foreach (var addr in correctAddresses)
            {
                Assert.AreEqual(true, SegWitAddress.Verify(addr, SegWitAddress.NetworkType.MainNet).IsVerified);
            }

            foreach (var addr in incorrectAddresses)
            {
                Assert.AreEqual(false, SegWitAddress.Verify(addr, SegWitAddress.NetworkType.MainNet).IsVerified);
            }
        }

    }
}
