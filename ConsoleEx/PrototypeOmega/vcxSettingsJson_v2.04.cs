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
//      Usage: Allow easy handling of program configuration.                                        //
// Dependency: vcxAppExPath.AppDataPath                                                             //
//**************************************************************************************************//
// v2.00 - 2025-10-17:  Microsoft json is proven unreliable for some type of class data, removing support;
// v2.01 - 2026-01-05:  Removing double Namespace;
// v2.02 - 2026-01-15:  No more dependency to vcxAppEx;
// v2.03 - 2026-01-16:  Now use vcxAppExPath; removed unused settings;
// v2.04 - 2026-01-19:	Modify [AppData] for Shared settings between app

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

// ** DON'T MESS WITH THIS FILE, THERE IS NOTHING HERE TO MODIFY      ** //
// ** only \Customized\vcxSettingsJson_data.cs should be modified     ** //
// ** If you find a bug related to the current file, please notify me ** //
#region DO NOT OPEN, SEE \Customized\vcxSettingsJson_data.cs
internal static class JsonSettings {
	public static PrototypeOmega.Sealed.JsonSettingsData_RESTRICTED data = new PrototypeOmega.Sealed.JsonSettingsData_RESTRICTED();

	public static void Read() {
		//Reading Settings
		PrototypeOmega.Sealed.JsonSettingsData_RESTRICTED? objSettings = JsonSettings.ReadEx();
		if (objSettings != null) {
			data = objSettings;
		}
	}

	private static PrototypeOmega.Sealed.JsonSettingsData_RESTRICTED ReadEx() {
		PrototypeOmega.Sealed.JsonSettingsData_RESTRICTED objRet = new Sealed.JsonSettingsData_RESTRICTED();

		//Will fix directory structure if not present
		string strAppSettingsPath = AppExPath.GetAppDataPath;
		_ = Directory.CreateDirectory(strAppSettingsPath);

		if (Directory.Exists(strAppSettingsPath)) {
			string strAppSettingsFileName = AppExPath.AppSettingsFileName;

			if (File.Exists(strAppSettingsFileName)) {
				string strJson = File.ReadAllText(strAppSettingsFileName);
				if (!string.IsNullOrWhiteSpace(strJson)) {
					try {
						//using NewtonSoft.Json
						PrototypeOmega.Sealed.JsonSettingsData_RESTRICTED? objSettings;
						objSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<PrototypeOmega.Sealed.JsonSettingsData_RESTRICTED>(strJson);
						if (objSettings != null) {
							objRet = objSettings;
						}
					} catch {
						//
					}
				}
			}
		}

		return objRet;
	}

	public static void Save() {
		//Saving Settings
		JsonSettings.SaveEx(data);
	}

	private static void SaveEx(PrototypeOmega.Sealed.JsonSettingsData_RESTRICTED pobjSettings) {
		//Saving Settings in files
		string strAppSettingsPath = AppExPath.GetAppDataPath;

		//We don't create directory on Save in case Read didn't succeed on it
		if (Directory.Exists(strAppSettingsPath)) {
			string strAppSettingsFileName = AppExPath.AppSettingsFileName;

			string strJSon = "";

			//using NewtonSoft.Json
			strJSon = Newtonsoft.Json.JsonConvert.SerializeObject(pobjSettings, Newtonsoft.Json.Formatting.Indented);

			File.WriteAllText(strAppSettingsFileName, strJSon, System.Text.Encoding.UTF8);
		}
	}
}
#endregion DO NOT OPEN, SEE \Customized\vcxSettingsJson_data.cs
