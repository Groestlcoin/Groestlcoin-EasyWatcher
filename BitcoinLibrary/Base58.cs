using System;
using NBitcoin.Altcoins;

namespace GroestlcoinLibrary {
    public static class Base58 {
        /// <summary>
        /// Checks to see if a given string (groestlcoin address) is base58 encoded or not.
        /// </summary>
        /// <param name="grsAddress">Groestlcoin address to check</param>
        /// <returns>True if base58 encoded</returns>
        public static VerificationResult Verify(string grsAddress) {
            VerificationResult result = new VerificationResult { IsVerified = false };
            if (string.IsNullOrWhiteSpace(grsAddress)) {
                result.Error = "Address can not be empty!";
                return result;
            }
            if (!grsAddress.StartsWith("F") && !grsAddress.StartsWith("3")) {
                result.Error = "Groestlcoin Base58 address starts with 1 or 3!";
                return result;
            }
            try {
                Groestlcoin.GroestlEncoder.Instance.DecodeData(grsAddress);
                result.IsVerified = true;
            }
            catch (Exception ex) {
                result.Error = ex.Message;
            }
            return result;
        }
    }
}
