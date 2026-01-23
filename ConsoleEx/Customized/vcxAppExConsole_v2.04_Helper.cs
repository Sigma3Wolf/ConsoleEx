//**************************************************************************************************//
//      Usage: THIS IS A CUSTOM [PROJECT FILE] Events, see dependency.                              //
// Dependency: vcxAppExConsole_v2.03.cs                                                             //
//**************************************************************************************************//
#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

//This class is used for Events (Keyboard, Mouse and OnConsoleAppClosing)
public static class ConsoleHelper {
    private static ConsoleAppEx? _objConsole;

    //This is like a constructor for this Static class
    public static void InitConsoleHelper(ConsoleAppEx pobjConsole) {
        #region ** MANDATORY Console Event handling **
        if (pobjConsole == null) {
            throw new ArgumentNullException("ConsoleAppEx argument is null");
        }
        _objConsole = pobjConsole;
        _objConsole.ConsoleEvent += OnConsoleEvent;

        // This allow to save settings on exit
        AppDomain.CurrentDomain.ProcessExit += OnConsoleAppClosing;
        #endregion ** MANDATORY Console Event handling **

        // this one don't work completely for now
        // it's for preventing scrolling console. We want a 1 page application where you can use multiple form
        _objConsole.DisableScroll = true;

        //Change Cursor size
        ConsoleAppEx.ChangeCaret(PrototypeOmega.PInvokeEx.CaretSize.Block);

        //We read previously saved data if any
        PrototypeOmega.JsonSettings.Read();
    }

    public static void OnConsoleAppClosing(object? sender, EventArgs e) {
        // On App Closing, your settings are saved
        // It is possible to add more stuff to be saved by modifying the
        // class inside \Customized\vcxSettingsJson_data.cs

        //For saving purpose
        PrototypeOmega.JsonSettings.Save();
    }

    public static void AddCustomColors() {
        // we need to save that also in Json (not yet)
        // Customize the original console color
        _objConsole?.ChangeCustomColor('C', (59, 120, 255));
        _objConsole?.ChangeCustomColor('E', (97, 214, 214));
        _objConsole?.ChangeCustomColor('F', (231, 72, 86));
        _objConsole?.ChangeCustomColor('H', (192, 156, 0));
        _objConsole?.ChangeCustomColor('I', (118, 118, 118));
        _objConsole?.ChangeCustomColor('P', (225, 100, 15));
    }

    public static void CreateCalculatorForm(string pstrMainFormName) {
        // You only need to run that part to create the Json Settings on your computer
        // You can add as many form on your computer and just recall them in different program using 
        // bool blnSuccess = JsonSettings.data.FormCollection.Forms.TryGetValue($"{pstrMainFormName}", out _objFormData);
        // if (blnSuccess && _objFormData != null) {
        //   _objConsole.SetActiveForm(_objFormData);
        // } else {
        //   //Let's recreate from scratch
        //   CreateCalculatorForm(this._strMainFormName);
        // }

        // The color we will use locally to define our form
        const char sCR = ConsoleAppEx.sCR;
        const char sCF = ConsoleAppEx.sCF;
        //const char sCB = ConsoleAppEx.sCB;

        string sCR0 = sCR + "0";    //This do NOTHING, they are replaced immediately with Empty, used for code aligning only in code editor
        string sCFC = $"{sCF}C";    //Blue
        string sCFD = $"{sCF}D";    //Green LED digit
        string sCFE = $"{sCF}E";    //Teal
        string sCFF = $"{sCF}F";    //Red
        string sCFH = $"{sCF}H";    //Yellow
        string sCFI = $"{sCF}I";    //Gray (border)
        string sCFP = $"{sCF}P";    //Orange (Error)

        // Creating New Form
        FormEx.NewFormBegin(pstrMainFormName, 5, 0);
        //_objConsole.FormAddLine($"{sCFI}╬╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╪╬");

        // ABCD EFGH
        //   | H  V 
        //   |------
        // ╬ | X  X     HDouble linking with VDouble
        // ┼ |          HSingle linking with VSingle
        // ╫ |    X     HSingle linking with VDouble
        // ╪ | X        HDouble linking with VSingle

        // ▞ | {START} of [COPY/PASTE] region	U+259E
        // ▚ |  {END}  of [COPY/PASTE] region	U+259A
        // ▌ | {START} of [BUTTON] region		U+258C
        // ▐ |  {END}  of [BUTTON] region		U+2590

        //This is a Calculator Form
        FormEx.NewFormAddLine($"{sCFI}    {sCR0}    {sCR0}   {sCR0}   {sCR0}   {sCR0}   {sCR0}  ▌X▐");
        FormEx.NewFormAddLine($"╬{sCR0}╪╪╪{sCR0}╪╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╬");
        FormEx.NewFormAddLine($"╬{sCFP} E▞{sCFF}-{sCFD}500,000,000,000{sCFF}.{sCFD}00▚{sCFI}╬");
        FormEx.NewFormAddLine($"╬╬{sCR0}╬╬╬{sCR0}╬╪╬{sCR0}╬╬╬{sCR0}╬╪╬{sCR0}╬╬╬{sCR0}╬╪╬{sCR0}╬╬╬{sCR0}╬╬");
        FormEx.NewFormAddLine($"╬▌{sCFF} C {sCFI}▐┼▌{sCFC}MR {sCFI}▐┼▌{sCFC}M+ {sCFI}▐┼▌{sCFC}M- {sCFI}▐╬");
        FormEx.NewFormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
        FormEx.NewFormAddLine($"╬▌{sCFC}POW{sCFI}▐┼▌{sCFC}MOD{sCFI}▐┼ {sCR0}   {sCR0} ┼ {sCR0}   {sCR0} ╬");
        FormEx.NewFormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
        FormEx.NewFormAddLine($"╬▌{sCFH} 7 {sCFI}▐┼▌{sCFH} 8 {sCFI}▐┼▌{sCFH} 9 {sCFI}▐┼▌{sCFE} + {sCFI}▐╬");
        FormEx.NewFormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
        FormEx.NewFormAddLine($"╬▌{sCFH} 4 {sCFI}▐┼▌{sCFH} 5 {sCFI}▐┼▌{sCFH} 6 {sCFI}▐┼▌{sCFE} - {sCFI}▐╬");
        FormEx.NewFormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
        FormEx.NewFormAddLine($"╬▌{sCFH} 1 {sCFI}▐┼▌{sCFH} 2 {sCFI}▐┼▌{sCFH} 3 {sCFI}▐┼▌{sCFE} * {sCFI}▐╬");
        FormEx.NewFormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
        FormEx.NewFormAddLine($"╬▌{sCFH} 0 {sCFI}▐┼▌{sCFF} . {sCFI}▐┼▌{sCFC}+/-{sCFI}▐┼▌{sCFE} / {sCFI}▐╬");
        FormEx.NewFormAddLine($"╬╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╬");

        //Giving the command to create the real Form from the template we just give.
        //It will create the form with Color and Fields (Button)

        //a FormEnd() by default show the message "Loading necessary modules..." at (0, 0)
        //and erase it when the form is converted. We can disable this by argument false
        //the convert is time consuming and can take up to 1 second (very slow)
        FormEx.NewFormEnd(false);
    }

    public static void OnConsoleEvent(object? sender, ConsoleEventArgs e) {
        //_objConsole?.ClearScreen();

        //This is where we receive Keyboard and Mouse Events
        HandleConsoleButtonEvent objHandleConsoleButtonEvent = new HandleConsoleButtonEvent(e);

        switch (objHandleConsoleButtonEvent.EventType) {
            case 1:
                //This is Mouse click X button (Exit)
                //That will exit the WaitForEvent();
                ConsoleAppEx._RunningProgram = false;
                break;

            case 2:
                //Boutton Pressed
                //you need to take it from here now...
                System.Diagnostics.Debug.WriteLine($"Boutton : [{objHandleConsoleButtonEvent.BouttonPressed}]");
                break;

            case 3:
                //Digit Pressed
                //you need to take it from here now...
                System.Diagnostics.Debug.WriteLine($"Digit : [{objHandleConsoleButtonEvent.DigitPressed}]");
                break;

            case 4:
                //DEPRECATED, I'll just clean up when I replace number with Enum
                break;

            case 5:
                //There is nothing to handle that one, it already happen
                //It just mean it succeed
                System.Diagnostics.Debug.WriteLine($"A Copy/Paste opperation occured");
                break;

            case 6:
                //Letter Pressed
                System.Diagnostics.Debug.WriteLine($"Digit : [{objHandleConsoleButtonEvent.KeyPressed}]");
                break;

            default:
                //If an update occur and produce more event then you currently handle, 
                //you'll get a console message to add it...
                if (objHandleConsoleButtonEvent.EventType == 0) {
                    System.Diagnostics.Debug.WriteLine($"case \"{e.EventData}\":");
                    System.Diagnostics.Debug.WriteLine($"System.Diagnostics.Debug.WriteLine(\"\");");
                    System.Diagnostics.Debug.WriteLine($"\tthis.EventType > 0;");
                    System.Diagnostics.Debug.WriteLine($"\tbreak;");
                }
                break;
        }
    }

    public class HandleConsoleButtonEvent {
        //This where you decide what to do with Events for your particular program
        //In this calculator App, for example, we will convert a press on calculator button 1 to a real value
        public int EventType { get; set; } = 0;
        public string BouttonPressed { get; set; } = "";
        public int DigitPressed { get; set; } = 0;
        public string KeyPressed { get; set; } = "";

        //Constructor
        public HandleConsoleButtonEvent(ConsoleEventArgs e) {
            this.SetEventData(e);
        }

        private void SetEventData(ConsoleEventArgs e) {
            //e.EventData are SHA384 of Button, they are calculated as :
            //string strRegionSha = $"[{FormName.ToUpper()}: {ButtonPosY:D2}=>{ButtonStartX:D2}x{ButtonEndX:D2}]=={FieldLength}";
            //string strSha = ShaEx.ComputeSha384(strRegionSha);
            //see DoFindRegion() in vcxAppExConsole_vX.XX_Theme.cs

            //EventNo is defining wich kind of Event.
            // 1. This is a Mouse Button Click
            // 2. This is a Copy/Paste event (does not copy and paste yet, some code is missing)
            // 3. This is a Keyboard KeyUp on Letter event
            // 4. This is a Keyboard KeyUp on Digit event

            if (e.EventNo == 1) {
                //Mouse Event
                switch (e.EventData) {
                    case "1ebc487cee112c73934ef22d77f9905b05e5df4ddf58dc531c57cc6b7d79dc5403347ec2207d2be1ec83999c0cab67ab":
                        this.EventType = 1;
                        break;

                    case "2c166a36213c7e79702d335a0f4c4f5cc94f5daee7a9a97b0d149422e46a81bdd4623c25e73234f27db333eb249d2e75":
                        this.EventType = 2;
                        this.BouttonPressed = "C";
                        break;

                    case "5871d506e36e742be2bec6d3e967ddfc8086d368fb8e1a5cbc32935fc5db0b71c6f77a839b4e885b7baf7ef2cc9a9797":
                        this.EventType = 2;
                        this.BouttonPressed = "MR";
                        break;

                    case "a752e7451ed40657d7ba36f4512f1b5a74e87555ed5fe0479e63c344e1c73a24d1aa75b076bebe1ded4dd40af74d1571":
                        this.EventType = 2;
                        this.BouttonPressed = "M+";
                        break;

                    case "73d4921ff3876ee866cd5ec924bb385d4a2071b4b5f78df9ed963b0a06a24c377d2be6d85519c537461e1ee3af81a812":
                        this.EventType = 2;
                        this.BouttonPressed = "M-";
                        break;

                    case "a4cbdb3a67ee283fcbe03ffe53a082e6eb6bba32f5223b5f6107a18eef2632892a036f16f71a0f9d9b4473c3ba5c11dd":
                        this.EventType = 2;
                        this.BouttonPressed = "POW";
                        break;

                    case "ee42c3b4d246194825fce85836e7a413b23ce985f9375f7c1ef8cfeb58a0289b4871dd47d3769f47ffd9ae9c6425fcb8":
                        this.EventType = 2;
                        this.BouttonPressed = "MOD";
                        break;

                    case "baaa473c777921f1433cc290728d76bd4e1fb53cc8b70ee58de89550f694995f7cdce11694aad96aef650db2374b8232":
                        this.EventType = 3;
                        this.DigitPressed = 0;
                        break;

                    case "c9eab1833af3501d06289905d8474d56f252fd7803c21fffb740869ff8a5da32f0e386e5cbb372d677d87f374e7cc194":
                        this.EventType = 3;
                        this.DigitPressed = 1;
                        break;

                    case "9ae738267066ef8f0a9bb6246fde784e9d713d3a3389aea8772d2b00d1dd55361f396a052f189ddf5d7649886fd577c6":
                        this.EventType = 3;
                        this.DigitPressed = 2;
                        break;

                    case "5970d61dac88787e35240cad3551ca1fd51450be0398e43c8619da6991f8b4d714593d29208077dbf950e7f9420eaf35":
                        this.EventType = 3;
                        this.DigitPressed = 3;
                        break;

                    case "e09a4438e0434d98d2b08fd34e5e11b2164cfb3759e4256159b6d71fdf8999d52d88206da14a521654918e5ff0355a7f":
                        this.EventType = 3;
                        this.DigitPressed = 4;
                        break;

                    case "588640e81de0a74a2166c2514f146c4165464fc7770fb212db1d21e4b1af09af1dd9244fbc6bde143acd1ea62ad00580":
                        this.EventType = 3;
                        this.DigitPressed = 5;
                        break;

                    case "d8008558f67a9c42b63cbf45c28ca6e309a04b3ff048ea9e2322c2a270bacb80dfe32bcfb291137cc9dc38c55dca8094":
                        this.EventType = 3;
                        this.DigitPressed = 6;
                        break;

                    case "de9a34d9737edd77c8d66b5fb3ff0a803cec4fb92a686b03ab5947d187b9c387b27a8881a4b411bc3105246fdcd2fadf":
                        this.EventType = 3;
                        this.DigitPressed = 7;
                        break;

                    case "9bfa59221ed7ae4a594f7f266f098ac859c432c248b1639c9d0296e6e2f0daa529b4c216cbe944b50c934218ca8c0967":
                        this.EventType = 3;
                        this.DigitPressed = 8;
                        break;

                    case "9cf61ffc7ef59fb79378fd9b4bcd5d069f78fea8a0eb6a67b2d24cb90c1522e78a1710c57f0ddb85ec87bd43d9d8f3ae":
                        this.EventType = 3;
                        this.DigitPressed = 9;
                        break;

                    case "5bbb2b8aefba37bd693344190c940c1802890a11a180c5ea6ecc7f8159945d254b2168f59a868ab808661338ca103ab7":
                        this.EventType = 2;
                        this.BouttonPressed = ".";
                        break;

                    case "7a54e6af0b5bb0e97e37b4cc3098f828b6a8442d44c3ca2f80a18e03b2287b075596167cf278e30011fc770aa3c8e9c6":
                        this.EventType = 2;
                        this.BouttonPressed = "+/-";
                        break;

                    case "9d5dcbb0b55a495238f9c56023c7cbb42f8cfeea827e33f2a947893910c93c73041fc912b3311a4e059a06976467052f":
                        this.EventType = 2;
                        this.BouttonPressed = "/";
                        break;

                    case "994fbc38033307ad82cb9b3d174f77958fac98d4b999cb0f8e165cab93ade11360742d0735c4dbf9a546274647886fb9":
                        this.EventType = 2;
                        this.BouttonPressed = "+";
                        break;

                    case "c3f0cd578cd021e33b1c44baa2d747b0e8b0a980d52ded2b28e0b4925d713ced1b2aa70a83951e0b9413fa535d7a2f71":
                        this.EventType = 2;
                        this.BouttonPressed = "-";
                        break;

                    case "9c9e53ad47d5ae037ac529b787301cc4f96de0ae5797aa93bd0f1a5009e90a8dd40444aebeab10b9a69887cfc7727cf4":
                        this.EventType = 2;
                        this.BouttonPressed = "*";
                        break;
                }
            } else if (e.EventNo == 2) {
                //COPY/PASTE
                this.EventType = 5;

            } else if (e.EventNo == 3) {
                //Letter
                //I really need to reorganize theses value in order or prioritee
                //that will break compatibilitie though :( unless I use an enum... we will see how I fix this...
                this.EventType = 6;
                this.KeyPressed = e.EventData;

            } else if (e.EventNo == 4) {
                if (e.EventData.Length > 0) {
                    //Digit
                    this.EventType = 3;
                    this.DigitPressed = (e.EventData[0] - '0');
                }
            }
        }
    }
}