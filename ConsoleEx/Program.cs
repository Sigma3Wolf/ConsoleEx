using PrototypeOmega;

namespace ConsoleEx {
    internal class Program {
        //NOTE: I'm using .Net Core 8.0 because it is a LTS and .Net Core 9 is now expired.
        //      but it work on .Net Core 10
        static void Main(string[] args) {
            //I have tried to make a simple example of what it can do
            //but it is way more powerfull then this example.
            //I'll add more example and documentation later...
            //If you want to join in, just contact me I'll be glad to help you out.

            //First we create the MANDATORY Console object
            using ConsoleAppEx objConsole = new ConsoleAppEx();

            //Not at all mandatory but just for the example...
            Console.Clear();

            //This is equivalent of Margin in html
            //it move EVERYTHING you Console.Write() in X and Y
            objConsole.ScreenPos.DeltaX = 5;
            objConsole.ScreenPos.DeltaY = 0;
            //So in our case, if you think you'll write at (0, 0), you'll in fact write at (5, 0)
            // >>>>>> Ok, so Last update just broke that...  I'll fix it soon.

            // ** From HERE, you can now use 16 million color with Console.Write and Console.WriteLine
            Console.WriteLine("®CWelcome ®Eto a ®Fnew ®Htype of ®IConsoleApp®A. ®PType®A any Key to continue");

            //OPTIONAL
            // The default Directory where the app save data in [{appname}.json] is:
            //		AppData		=> C:\ProgramData\PrototypeOmega  (it's a shared location among all your program)
            // other settings are
            //		AppDataEx	=> C:\ProgramData\PrototypeOmega\{appname}
            //		AppDataPath	=> {where your app is located}\AppData
            AppExPath.AppDataPath = AppExPath.enmAppDataPath.AppData;

            //Let's define some CustomSettings
            AddCustomSettings(objConsole);

            //Let's customize our Color, thoses Microsoft have provided are hard to read
            AddCustomColors(objConsole);

            //lets test our NEW color:
            Console.WriteLine("®CWelcome ®Eto a ®Fnew ®Htype of ®IConsoleApp®A. ®PType®A any Key to continue");

            //Yes that's allowed now :) It's basiquely just a CrLf
            Console.WriteLine();

            Console.WriteLine("®OThoses two lines have the same code but it produce a different");
            Console.WriteLine("result because we have customized the color. You can now use");
            Console.WriteLine("more vibrant color, you are not limited to 16 defined one®A");
            System.Console.ReadKey();

            //We need to declare the name we give to a particular Form.
            //This name is stored in PascalCase because it's a dictionary Key
            string strFormName = ConsoleAppEx.ToPascalCase("Calculator");

            //Lets read our saved form if it exist
            PrototypeOmega.ConsoleAppEx.FormData? _objFormData = null;
            bool blnSuccess = JsonSettings.data.FormCollection.Forms.TryGetValue(strFormName, out _objFormData);
            if (blnSuccess && _objFormData != null) {
                //Ok it exist, let's restore it's definition
                objConsole.SetActiveForm(_objFormData);
            } else {
                //Well, you never created that form or it was not saved on your disk,
                //so here is the definition let's recreate from scratch
                CreateCalculatorForm(objConsole, strFormName);

                //Add our object to be saved
                PrototypeOmega.ConsoleAppEx.FormData objFormData;
                objFormData = objConsole.GetActiveForm();

                //Because we want to update our change, we need to remove/add from our collection
                JsonSettings.data.FormCollection.Remove(strFormName);
                JsonSettings.data.FormCollection.Add(objFormData, false);
            }

            //Before showing our form, let's erase the screen
            Console.Clear();

            //see vcxAppExConsole_v2.03.cs
            //sCR = 'ª'
            //string sCR2 = sCR + "2";
            //FormShow always add sCR2 at the end of each Form Line.
            //when it print the first line of your form on Screen, it first save the Position(x, y)
            //when you do a sCR2, it only do a X = SavedX; Y = Y +1;
            //Meaning the whole form is align on the initial PosX

            //some note...
            objConsole.SetPosition(0, 17);
            Console.Write("Please note, this is not a real calculator, you'll need to add codeª2");
            Console.Write("in Event ®PSetEventData() ®Aand ®PHandleConsoleButtonEvent() ®Ato make aª2");
            Console.Write("full working program. Use the mouse to click button. Exit with Escª2");
            Console.Write("or clicking form X button");

            //Interestly, I use the next line because it move the Final cursor 1 line more down.
            //when you exit the app you normally see a "Press any key to close this windows . . ."
            //Normally, if your cursor is up, the message will end up being over something else.
            //I always keep track of the further written line and I'll position there when the 
            //dispose() is hit.  Try removing this line, you'll see.
            Console.WriteLine();

            //This is the Initial position of the cursor in the Calculator, set it to a Field object
            int LedPosX = 19;
            int LedPosY = 2;
            objConsole.FormShow(LedPosX, LedPosY);

            //and finally, we wait for Event and you'll handle the rest of program inside thoses event
            //SetEventData() and HandleConsoleButtonEvent()

            //Ok, I just found a bug... I don't clear buffer here so if last key hit was ESC, it exit...
            //I'll fix that. Beside that, it's now time to use mouse and check the event firing in console output
            objConsole.WaitForEvent(strFormName);


            // ==== THE END. ====
            //Here the program exit...
            //If you press ESC, it will also exit
            //If you Mouse Click the App X, it will also exit
            //Soon I'll add that you'll not have to define Event Handling for Clicking the X => Exit
        }

        private static void AddCustomSettings(ConsoleAppEx objConsole) {
            #region ** MANDATORY Console Event handling **
            //Ok, this doesn't seem right:
            //  We create object ConsoleEvent (non static) and we give him the ConsoleAppEx object but
            //  it's coming from static AddCustomSettings. How come ConsoleEvent consoleAppEx is not destroyed
            // when static void AddCustomSettings Exit ???
            ConsoleEvent consoleAppEx = new(objConsole);
            #endregion ** MANDATORY Console Event handling **

            // this one don't work completely for now
            // it's for preventing scrolling console. We want a 1 page application where you can use multiple form
            objConsole.DisableScroll = true;

            //Change Cursor size
            ConsoleAppEx.ChangeCaret(PrototypeOmega.PInvokeEx.CaretSize.Block);

            //We read previously saved data if any
            PrototypeOmega.JsonSettings.Read();
        }

        private static void AddCustomColors(ConsoleAppEx objConsole) {
            // we need to save that also in Json (not yet)
            // Customize the original console color
            objConsole.ChangeCustomColor('C', (59, 120, 255));
            objConsole.ChangeCustomColor('E', (97, 214, 214));
            objConsole.ChangeCustomColor('F', (231, 72, 86));
            objConsole.ChangeCustomColor('H', (192, 156, 0));
            objConsole.ChangeCustomColor('I', (118, 118, 118));
            objConsole.ChangeCustomColor('P', (225, 100, 15));
        }

        private static void CreateCalculatorForm(ConsoleAppEx objConsole, string pstrMainFormName) {
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
            objConsole.FormBegin(pstrMainFormName, 5, 0);
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
            objConsole.FormAddLine($"{sCFI}    {sCR0}    {sCR0}   {sCR0}   {sCR0}   {sCR0}   {sCR0}  ▌X▐");
            objConsole.FormAddLine($"╬{sCR0}╪╪╪{sCR0}╪╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╬");
            objConsole.FormAddLine($"╬{sCFP} E▞{sCFF}-{sCFD}500,000,000,000{sCFF}.{sCFD}00▚{sCFI}╬");
            objConsole.FormAddLine($"╬╬{sCR0}╬╬╬{sCR0}╬╪╬{sCR0}╬╬╬{sCR0}╬╪╬{sCR0}╬╬╬{sCR0}╬╪╬{sCR0}╬╬╬{sCR0}╬╬");
            objConsole.FormAddLine($"╬▌{sCFF} C {sCFI}▐┼▌{sCFC}MR {sCFI}▐┼▌{sCFC}M+ {sCFI}▐┼▌{sCFC}M- {sCFI}▐╬");
            objConsole.FormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
            objConsole.FormAddLine($"╬▌{sCFC}POW{sCFI}▐┼▌{sCFC}MOD{sCFI}▐┼ {sCR0}   {sCR0} ┼ {sCR0}   {sCR0} ╬");
            objConsole.FormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
            objConsole.FormAddLine($"╬▌{sCFH} 7 {sCFI}▐┼▌{sCFH} 8 {sCFI}▐┼▌{sCFH} 9 {sCFI}▐┼▌{sCFE} + {sCFI}▐╬");
            objConsole.FormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
            objConsole.FormAddLine($"╬▌{sCFH} 4 {sCFI}▐┼▌{sCFH} 5 {sCFI}▐┼▌{sCFH} 6 {sCFI}▐┼▌{sCFE} - {sCFI}▐╬");
            objConsole.FormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
            objConsole.FormAddLine($"╬▌{sCFH} 1 {sCFI}▐┼▌{sCFH} 2 {sCFI}▐┼▌{sCFH} 3 {sCFI}▐┼▌{sCFE} * {sCFI}▐╬");
            objConsole.FormAddLine($"╫┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼┼┼{sCR0}┼╫");
            objConsole.FormAddLine($"╬▌{sCFH} 0 {sCFI}▐┼▌{sCFF} . {sCFI}▐┼▌{sCFC}+/-{sCFI}▐┼▌{sCFE} / {sCFI}▐╬");
            objConsole.FormAddLine($"╬╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╪╪{sCR0}╪╬");

            //Giving the command to create the real Form from the template we just give.
            //It will create the form with Color and Fields (Button)

            //a FormEnd() by default show the message "Loading necessary modules..." at (0, 0)
            //and erase it when the form is converted. We can disable this by argument false
            //the convert is time consuming and can take up to 1 second (very slow)
            objConsole.FormEnd(false);
        }
    }

    //This class is used for Events (Keyboard, Mouse and OnConsoleAppClosing)
    internal class ConsoleEvent {
        #region DON'T TOUCH THAT PART
        //that's our parent object
        private ConsoleAppEx _objConsole;

        public ConsoleEvent(ConsoleAppEx pobjConsole) {
            _objConsole = pobjConsole;
            _objConsole.ConsoleEvent += this.OnConsoleEvent;

            // This allow to save settings on exit
            AppDomain.CurrentDomain.ProcessExit += this.OnConsoleAppClosing;
        }
        #endregion DON'T TOUCH THAT PART

        public void OnConsoleEvent(object? sender, ConsoleEventArgs e) {
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

        public void OnConsoleAppClosing(object? sender, EventArgs e) {
            // On App Closing, your settings are saved
            // It is possible to add more stuff to be saved by modifying the
            // class inside \Customized\vcxSettingsJson_data.cs

            //For saving purpose
            PrototypeOmega.JsonSettings.Save();
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
