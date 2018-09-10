using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using NBitcoin;
using NBitcoin.Altcoins;

namespace BitcoinLibrary {
    public static class Base58 {
        /// <summary>
        /// Checks to see if a given string (bitcoin address) is base58 encoded or not.
        /// </summary>
        /// <param name="btcAddress">Bitcoin address to check</param>
        /// <returns>True if base58 encoded</returns>
        public static VerificationResult Verify(string btcAddress) {
            VerificationResult result = new VerificationResult() { IsVerified = false };
            if (string.IsNullOrWhiteSpace(btcAddress)) {
                result.Error = "Address can not be empty!";
                return result;
            }
            if (!btcAddress.StartsWith("1") && !btcAddress.StartsWith("3")) {
                result.Error = "Base58 address starts with 1 or 3!";
                return result;
            }
            //todo: Revisit specific groestl512 validation
            result.IsVerified = true;
            return result;

            //var network = NBitcoin.Altcoins.Groestlcoin.Instance.GetNetwork(NetworkType.Mainnet);

            //            var poo = Groestlcoin.GroestlEncoder.Instance;

            //          poo.EncodeData(ghash);



            //        var encode = checker.EncodeData(ghash);

            //int lengthWithoutChecksum = dataAsByte.Length - 4;
            //byte[] bytesWithoutChecksum = new byte[lengthWithoutChecksum];
            //Array.Copy(dataAsByte, bytesWithoutChecksum, lengthWithoutChecksum);

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
