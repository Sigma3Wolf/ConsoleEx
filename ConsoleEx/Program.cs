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
            ConsoleHelper.InitConsoleHelper(objConsole);

            //Let's customize our Color, thoses Microsoft have provided are hard to read
            ConsoleHelper.AddCustomColors();

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
            string strFormName = FormEx.ToPascalCase("Calculator");

            //Lets read our saved form if it exist
            PrototypeOmega.FormEx? _objFormData = null;
            bool blnSuccess = JsonSettings.data.FormCollection.Forms.TryGetValue(strFormName, out _objFormData);
            if (blnSuccess && _objFormData != null) {
                //Ok it exist, let's restore it's definition
                objConsole.SetActiveForm(_objFormData);
            } else {
                //Well, you never created that form or it was not saved on your disk,
                //so here is the definition let's recreate from scratch
                ConsoleHelper.CreateCalculatorForm(strFormName);

                //Add our object to be saved
                PrototypeOmega.FormEx objFormData;
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
    }
}
