using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using NBitcoin;
using NBitcoin.Altcoins;

namespace GroestlcoinLibrary {
    public static class Base58 {
        /// <summary>
        /// Checks to see if a given string (grs address) is base58 encoded or not.
        /// </summary>
        /// <param name="grsAddress">Groestlcoin address to check</param>
        /// <returns>True if base58 encoded</returns>
        public static VerificationResult Verify(string grsAddress) {
            VerificationResult result = new VerificationResult() { IsVerified = false };
            if (string.IsNullOrWhiteSpace(grsAddress)) {
                result.Error = "Address can not be empty!";
                return result;
            }
            if (!grsAddress.StartsWith("1") && !grsAddress.StartsWith("3")) {
                result.Error = "Base58 address starts with 1 or 3!";
                return result;
            }
            //todo: Revisit specific groestl512 validation
            result.IsVerified = true;
            return result;

            
            //// calculate the checksum
            //SHA256 sha = new SHA256Managed();
            //byte[] hash1 = sha.ComputeHash(bytesWithoutChecksum);
            //byte[] hash2 = sha.ComputeHash(hash1);

            //if (hash2[0] != dataAsByte[lengthWithoutChecksum] ||
            //    hash2[1] != dataAsByte[lengthWithoutChecksum + 1] ||
            //    hash2[2] != dataAsByte[lengthWithoutChecksum + 2] ||
            //    hash2[3] != dataAsByte[lengthWithoutChecksum + 3]) {
            //    result.Error = "Invalid Checksum!";
            //}
            //else {
            //result.IsVerified = true;
            //}
            //return result;
        }

    }
}
