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
//      Usage: Define all used Application Path.                                                    //
// Dependency:                                                                                      //
//**************************************************************************************************//
// v1.00 - 2026-01-16:  Initial release;
// v1.01 - 2026-01-16:	Cosmetic;
// v1.02 - 2026-01-19:	modify [AppData] for Shared settings between app (now default location)

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

internal sealed class AppExPath {
	public enum enmAppDataPath {
		AppData,
		AppDataEx,
		ProgramAppData
	}

	//https://stackoverflow.com/questions/6041332/best-way-to-get-application-folder-path
	public static enmAppDataPath AppDataPath { get; set; } = enmAppDataPath.AppData;	//Shared location by default
	private static readonly string _strAppPath;
	private static readonly string _strAppExeName;

	private static readonly string _strUserDesktopPath;
	private static readonly string _strUserDocumentPath;
	private static readonly string _strUserMusicPath;
	private static readonly string _strUserPicturesPath;
	private static readonly string _strUserVideospPath;

	static AppExPath() {
		_strAppPath = AppDomain.CurrentDomain.BaseDirectory; //this got EndBackSlash;
		_strAppExeName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;  //ProcessName is the AssemblyName in VS and ExeName in OS

		_strUserDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
		_strUserDesktopPath += Path.DirectorySeparatorChar;

		_strUserDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		_strUserDocumentPath += Path.DirectorySeparatorChar;

		_strUserPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
		_strUserPicturesPath += Path.DirectorySeparatorChar;

		_strUserMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
		_strUserMusicPath += Path.DirectorySeparatorChar;

		_strUserVideospPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
		_strUserVideospPath += Path.DirectorySeparatorChar;
	}

	private enum enmProgramPath {
		AppExAppPath,
		AppExExeName,
		AppExAppDataPath,
		AppExAppDataPathEx
	}

	internal static string GetAppPath {
		get {
			return _strAppPath;
		}

		//set {
		//	//check if new path value exist and if EndBackSlash is provided
		//	string strNewPath = DirectoryWE(value);
		//	bool IsValid = FileExist64(strNewPath, 16);

		//	if (IsValid == true) {
		//		_strAppPath = strNewPath + @"\";
		//	}
		//}
	} //Path

	internal static string GetAppRessourcesPath {
		get {
			string strRet = GetAppDataPath;
			strRet = Path.Combine(strRet, "ressources");
			strRet += Path.DirectorySeparatorChar;

			return strRet;
		}

		//set {
		//	//check if new path value exist and if EndBackSlash is provided
		//	string strNewPath = DirectoryWE(value);
		//	bool IsValid = FileExist64(strNewPath, 16);

		//	if (IsValid == true) {
		//		_strAppRessourcesPath = strNewPath + @"\";
		//	}
		//}
	}

	internal static string GetAppExeName {
		get {
			return _strAppExeName;
		}
	}

	internal static string GetAppDbName {
		get {
			string strAppDataPath = GetAppDataPath;
			string strRet = Path.Combine(strAppDataPath, _strAppExeName + ".accdb");
			return strRet;
		}
	}

	internal static string AppSettingsFileName {
		get {
			string strAppDataPath = GetAppDataPath;
			string strRet = Path.Combine(strAppDataPath, _strAppExeName + ".json");
			return strRet;
		}
	}

	internal static string GetAppDataPath {
		get {
			string strRet = "";
			switch (AppDataPath) {
				case enmAppDataPath.AppData:
					// for Shared settings between app
					// [C:\ProgramData\PrototypeOmega]
					strRet = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					strRet = Path.Combine(strRet, "PrototypeOmega");
					break;

				case enmAppDataPath.AppDataEx:
					// [C:\ProgramData\PrototypeOmega\AppExeName]
					strRet = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					strRet = Path.Combine(strRet, "PrototypeOmega", _strAppExeName);
					break;

				case enmAppDataPath.ProgramAppData:
					// [AppPath\AppData\]
					strRet = Path.Combine(_strAppPath, "AppData");
					break;
			}
			strRet += Path.DirectorySeparatorChar;

			return strRet;
		}
	}

	internal static string GetUserDesktopPath {
		get {
			return _strUserDesktopPath;
		}
	}

	internal static string GetUserDocumentPath {
		get {
			return _strUserDocumentPath;
		}
	}

	internal static string GetUserPicturesPath {
		get {
			return _strUserPicturesPath;
		}
	}

	internal static string GetUserMusicPath {
		get {
			return _strUserMusicPath;
		}
	}
	internal static string GetUserVideospPath {
		get {
			return _strUserVideospPath;
		}
	}

	internal static bool FileExist64(string pstrFilePath, int plngFileAttrib = 0) {
		bool blnRetValue = false;

		if (pstrFilePath.Length > 2) {
			if ((plngFileAttrib & 16) == 16) {
				blnRetValue = Directory.Exists(pstrFilePath);
			} else {
				blnRetValue = File.Exists(pstrFilePath);
			}
		}

		return blnRetValue;
	}
}
