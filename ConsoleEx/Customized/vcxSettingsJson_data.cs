//**************************************************************************************************//
//      Usage: THIS IS A CUSTOM [PROJECT FILE] Settings, see dependency.                            //
// Dependency: vcxSettingsJson_v2.04.cs                                                             //
//**************************************************************************************************//
#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega.Sealed;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

// Usage example for [Windows Form] application:
//		using PrototypeOmega;
//		public frmMain() {
//			AppPathEx.AppDataPath = AppPathEx.enmAppDataPath.AppDataEx;	// optional, this is default
//			AppDomain.CurrentDomain.ProcessExit += OnConsoleAppClosing;
//			PrototypeOmega.JsonSettings.Read();
//
//			//Use: JsonSettings.data.YourSettings = newvalue;
//			PrototypeOmega.JsonSettings.data.DataVersion = "1.0.0";
//		}
//		private static void OnConsoleAppClosing(object? sender, EventArgs e) {
//			PrototypeOmega.JsonSettings.Save();
//		}

// Usage example for [Console] application:

//AppDomain.CurrentDomain.ProcessExit += ConsoleEvent.OnConsoleAppClosing;
//		public static void OnConsoleAppClosing(object? sender, EventArgs e) {
//			PrototypeOmega.JsonSettings.Save();
//		}

internal sealed class JsonSettingsData_RESTRICTED {
	#region mandatory
	public string DataVersion { get; set; } = "1.0.0";
	#endregion mandatory

	//Add your Settings variable BELOW
	// ********************************************************************************************* //
	public ScriptCollection ScriptCollection {
		get; set;
	} = new ScriptCollection();

	public FormCollection FormCollection {
		get; set;
	} = new FormCollection();
}
