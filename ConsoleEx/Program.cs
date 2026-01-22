using PrototypeOmega;

namespace ConsoleEx {
    internal class Program {
        static void Main(string[] args) {
            //First we create our Console object
            using ConsoleAppEx objConsole = new ConsoleAppEx();

            //OPTIONAL
            // The default Directory where the app save data in [{appname}.json] is:
            //		AppData		=> C:\ProgramData\PrototypeOmega  (it's a shared location among all your program)
            // other settings are
            //		AppDataEx	=> C:\ProgramData\PrototypeOmega\{appname}
            //		AppDataPath	=> {where your app is located}\AppData
            AppExPath.AppDataPath = AppExPath.enmAppDataPath.AppData;


        }
    }

    //This class is used for Events (Keyboard, Mouse and OnConsoleAppClosing)
    internal class ConsoleEvent {
        public static void OnConsoleAppClosing(object? sender, EventArgs e) {
            // On App Closing, your settings are saved
            // It is possible to add more stuff to be saved by modifying the
            // class inside \Customized\vcxSettingsJson_data.cs
            PrototypeOmega.JsonSettings.Save();
        }
    }
}
