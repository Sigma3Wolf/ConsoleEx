//**************************************************************************************************//
// Copyright 2026, Patrice CHARBONNEAU                                                              //
//                 a.k.a. Sigma3Wolf                                                                //
//                 Iid: [80a11b77a52aa9eeed80c9d37dcb7878519289d40beeddb40bb23a60d2711963]          //
//                 All rights reserved.                                                             //
//                                                                                                  //
// This source code is licensed under the [BSD 3-Clause "New" or "Revised" License]                 //
// found in the LICENSE file in the root directory of this source tree.                             //
//**************************************************************************************************//
//** WARNING: If you modify this file, you MUST rename it to exclude the version number. :WARNING **//
//**************************************************************************************************//
//      Usage: SHA utilities function.                                                              //
// Dependency:                                                                                      //
//**************************************************************************************************//
// v3.00 - 2025-10-28:   SuperSalt is passed as parameter when needed;
// v3.01 - 2026-01-17:   Cosmetic 2026

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

//https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm.computehash?redirectedfrom=MSDN&view=netframework-4.8#System_Security_Cryptography_HashAlgorithm_ComputeHash_System_Byte___
internal static class ShaEx {
    public static string ComputeSha256(string pstrData, string pstrSuperSalt = "") {
        string strRetValue;
        //pstrSuperSalt = SecretsEx.SuperSalt;

        // Create a SHA256
        using (System.Security.Cryptography.SHA256 typSha = System.Security.Cryptography.SHA256.Create()) {
            pstrData = pstrData + pstrSuperSalt;
            strRetValue = BuildShaString(typSha, pstrData);
        }

        return strRetValue;
    }

    public static string ComputeSha384(string pstrData, string pstrSuperSalt = "") {
        string strRetValue;
        //pstrSuperSalt = SecretsEx.SuperSalt;

        // Create a SHA384
        using (System.Security.Cryptography.SHA384 typSha = System.Security.Cryptography.SHA384.Create()) {
            pstrData = pstrData + pstrSuperSalt;
            strRetValue = BuildShaString(typSha, pstrData);
        }

        return strRetValue;
    }

    public static string ComputeSha512(string pstrData, string pstrSuperSalt = "") {
        string strRetValue;
        //pstrSuperSalt = SecretsEx.SuperSalt;

        // Create a SHA512
        using (System.Security.Cryptography.SHA512 typSha = System.Security.Cryptography.SHA512.Create()) {
            pstrData = pstrData + pstrSuperSalt;
            strRetValue = BuildShaString(typSha, pstrData);
        }

        return strRetValue;
    }

    ///* *************************************************************************************************** */
    private static string BuildShaString(System.Security.Cryptography.HashAlgorithm hashAlgorithm, string pstrData) {
        // ComputeHash - returns byte array
        byte[] bytData = hashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pstrData));

		// Convert byte array to a string
		System.Text.StringBuilder typBuilder = new System.Text.StringBuilder();
        for (int i = 0; i < bytData.Length; i++) {
            _ = typBuilder.Append(bytData[i].ToString("x2"));
        }

        return typBuilder.ToString();
    }
}