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
//      Usage: Allow positionning of console output and adding console color capabilities.          //
// Dependency: vcxAppExConsole_v2.04.cs                                                             //
//**************************************************************************************************//
// v2.04 - 2026-01-23:	** INITIAL RELEASE **;

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

//The idea is this class to hold all Form, 
//right now it hold only 1
public class FormEx {
    private static char sCR;  //NewLine
    private static char sCF;  //Foreground
    private static char sCB;  //Background
    private static FormEx? _tmpFormEx = null;

    private const string strSYMBOLS = "╚╝╔╗╦╬╤╩╧╠╣╞╡║═ ┌┐└┘┬┼╥╨┴╟╢├┤│─";
    public FormDataEx FormData = new FormDataEx();

    [Newtonsoft.Json.JsonConstructor] // Aka [Json Constructor] is optional, Newtonsoft.Json will use private too
    private FormEx() {
        //private because a FormData CANNOT go without FormName, you need to use the constructor
    }

    public FormEx(string pstrFormName) {
        sCR = PrototypeOmega.ConsoleAppEx.sCR;
        sCF = PrototypeOmega.ConsoleAppEx.sCF;
        sCB = PrototypeOmega.ConsoleAppEx.sCB;

        // b13: the [FormName] is mandatory for event CLSID, yet I don't validate it.
        this.FormData.FormName = ToPascalCase(pstrFormName);
        this.FormData.MaxX = Console.BufferWidth;
        this.FormData.MaxY = Console.BufferHeight;
    }

    public static FormEx? GetActiveForm() {
        return _tmpFormEx;
    }

    public static void SetActiveForm(FormEx pobjForm) {
        if (pobjForm != null) {
            NewFormBegin(pobjForm.FormData.FormName, pobjForm.FormData.StartX, pobjForm.FormData.StartY);
            if (_tmpFormEx != null) {
                List<string> objDesign = pobjForm.FormData.Design;

                for (int i = 0; i < objDesign.Count; i++) {
                    string strLine = objDesign[i];
                    NewFormAddLine(strLine);
                }
                NewFormEnd();
            }
        }
    }

    public static void NewFormBegin(string pstrFormName, int plngStartX, int plngStartY) {
        string strFormName = FormEx.ToPascalCase(pstrFormName);
        if (string.IsNullOrEmpty(strFormName)) {
            throw new Exception("FormName must be set");
        }

        _tmpFormEx = new FormEx(strFormName);
        _tmpFormEx.FormData.StartX = plngStartX;
        _tmpFormEx.FormData.StartY = plngStartY;
    }

    public static void NewFormEnd(bool pblnShowMessage = true) {
        if (_tmpFormEx != null) {
            if (pblnShowMessage) {
                //this.SetPosition(0, 0);
                //Console.Write("Loading necessary modules...");
                _tmpFormEx.FormConvert();
                //this.SetPosition(0, 0);
                //Console.Write("                            ");
            } else {
                _tmpFormEx.FormConvert();
            }
        }
    }

    public static void NewFormAddLine(string pstrLine) {
        if (_tmpFormEx != null) {
            string sCR0 = sCR + "0";
            string strLine = pstrLine.Trim().Replace(sCR0, "");
            if (!string.IsNullOrEmpty(strLine)) {
                _tmpFormEx.FormData.Design.Add(strLine);

                string strFixed = RemoveColorsAndFix(strLine, out string strFlat);
                if (_tmpFormEx.FormData._Design.Count < _tmpFormEx.FormData.MaxY) {
                    _tmpFormEx.FormData._Design.Add(strFixed);
                    _tmpFormEx.FormData._DesignFlat.Add(strFlat);
                }
            }
        }
    }

    //Remove special Code for [DesignFlat] and fix the special Code if odd
    private static string RemoveColorsAndFix(string pstrData, out string pstrFlat) {
        string strRet = pstrData;
        int i = 0;

        pstrFlat = "";
        while (i < pstrData.Length) {
            // Detect code: X + alphanumeric
            if (pstrData[i] == sCR || pstrData[i] == sCF || pstrData[i] == sCB) {
                i++;

                if (i < pstrData.Length && char.IsLetterOrDigit(pstrData[i])) {
                    i++;

                    // skip the rest of code
                    continue;
                } else {
                    strRet = pstrData.Substring(0, pstrData.Length - 1);
                    break;
                }
            }

            pstrFlat += pstrData[i];
            i++;
        }

        return strRet;
    }

    public string FieldSearch(ref FormField pobjSearch) {
        string strRet = "";

        bool blnSuccess = this.FormData._DesignFieldPos.TryGetValue(pobjSearch, out string? strSha);
        if (blnSuccess) {
            strRet = strSha + "";
            FormField? objField = new FormField();
            blnSuccess = this.FormData._DesignFieldSha.TryGetValue(strRet, out objField);
            if (!blnSuccess || objField == null) {
                //if [this.FormData._DesignFieldPos] is set, then [this.FormData._DesignFieldSha] has to be set
                throw new Exception("Programmer Error!");
            }

            pobjSearch = objField;
        }

        return strRet;
    }

    private void GetRealFormMaxXY() {
        this.FormData.MaxX = 0;
        this.FormData.MaxY = this.FormData._DesignFlat.Count;
        for (int i = 0; i < this.FormData.MaxY; i++) {
            if (this.FormData._DesignFlat[i].Length > this.FormData.MaxX) {
                this.FormData.MaxX = this.FormData._DesignFlat[i].Length;
            }
        }
    }

    public void FormConvert() {
        List<string> lstFormFlat = new List<string>();
        List<string> lstFormFields = new List<string>();

        // Prepare to create Field
        this.FormData._DesignFieldSha.Clear();
        this.FormData._DesignFieldPos.Clear();
        string pstrFormName = this.FormData.FormName;

        //First we set MaxX
        this.GetRealFormMaxXY();

        //b13 work: _lstDesign must not contain Field Tag
        // DEBUG ONLY
        if ((this.FormData.MaxX > 0) && (this.FormData.MaxY > 0)) {
            for (int lngPosY = 0; lngPosY < this.FormData.MaxY; lngPosY++) {
                string strDataFlat = this.FormData._DesignFlat[lngPosY];
                string strDataDesign = this.ConvertRow(lngPosY, strDataFlat);
                lstFormFlat.Add(strDataDesign);

                // ABCD     
                //   | H  V 
                //   |------
                // ╬ | X  X 
                // ┼ |      
                // ╫ |    X 
                // ╪ | X    
                strDataFlat = strDataFlat.Replace('╬', ' ');
                strDataFlat = strDataFlat.Replace('┼', ' ');
                strDataFlat = strDataFlat.Replace('╫', ' ');
                strDataFlat = strDataFlat.Replace('╪', ' ');

                //Now Create Field data
                if (strDataFlat.Trim().Length > 0) {
                    //System.Diagnostics.Debug.WriteLine($"{lngPosY:D2} " + strDataFlat + ": " + strDataFlat.Length);
                    this.ExtractFields(strDataFlat, pstrFormName, lngPosY);
                }

                //DesignGet(plngDesign)
                // 0: this.FormData._lstDesign		: the one that show on screen in final
                // 1: this.FormData._lstDesignFlat	:
                // 2: this.FormData._lstDesignField

                //Finally, remove all button spacer section code
                // EFGH
                // ▞ | {START} of [COPY/PASTE] region
                // ▚ |  {END}  of [COPY/PASTE] region
                // ▌ | {START} of [BUTTON] region
                // ▐ |  {END}  of [BUTTON] region
                strDataFlat = FlushFieldTag(strDataFlat);
                lstFormFields.Add(strDataFlat);
            }

            this.FormData._DesignFlat = lstFormFlat;
            this.FormData._DesignField = lstFormFields;
            this.FormMergeBack();
        } else {
            //Let's reset instead of ruining the design process
            this.FormData.MaxX = Console.BufferWidth;
            this.FormData.MaxY = Console.BufferHeight;
        }
    }

    public void DebugForm() {
        Console.WriteLine("\r\nlstDesignFlat");
        for (int o = 0; o < this.FormData._DesignFlat.Count; o++) {
            Console.WriteLine($"{o:D2}: {this.FormData._DesignFlat[o]}");
        }
        Console.WriteLine("------------------------------------------------------\r\n");

        Console.WriteLine("_lstDesign");
        for (int o = 0; o < this.FormData._Design.Count; o++) {
            Console.Write($"{o:D2}: {this.FormData._Design[o]}");
        }
        Console.WriteLine("------------------------------------------------------\r\n");

        Console.WriteLine("lstDesignField");
        for (int o = 0; o < this.FormData._DesignField.Count; o++) {
            Console.WriteLine($"{o:D2}: {this.FormData._DesignField[o]}");
        }
        Console.WriteLine("------------------------------------------------------\r\n");
    }

    public string FlushFieldTag(string pstrData) {
        string strRet = pstrData;
        strRet = strRet.Replace('▞', ' ');
        strRet = strRet.Replace('▚', ' ');
        strRet = strRet.Replace('▌', ' ');
        strRet = strRet.Replace('▐', ' ');

        return strRet;
    }

    public void ExtractFields(string pstrData, string pstrFormName, int plngPosY) {
        if (string.IsNullOrEmpty(pstrData)) {
            return;
        }

        // Extraction type 0 - BUTTON region
        // Note : S'il peut y avoir plusieurs blocs B, on pourrait boucler, 
        // ici on illustre la détection des délimiteurs.
        this.DoFindRegion(pstrData, pstrFormName, plngPosY, 0);

        // Extraction type 1 - COPY/PASTE field region
        this.DoFindRegion(pstrData, pstrFormName, plngPosY, 1);
    }

    private void DoFindRegion(string pstrData, string pstrFormName, int plngPosY, int plngFieldType) {
        // EFGH
        // ▞ | {START} of [COPY/PASTE] region	U+259E
        // ▚ |  {END}  of [COPY/PASTE] region	U+259A
        // ▌ | {START} of [BUTTON] region		U+258C
        // ▐ |  {END}  of [BUTTON] region		U+2590

        // Définition des balises de debut et de fin
        string pstrTagBegin;
        string pstrTagEnd;
        if (plngFieldType == 0) {
            pstrTagBegin = "▌";
            pstrTagEnd = "▐";
        } else {
            pstrTagBegin = "▞";
            pstrTagEnd = "▚";
        }

        // On boucle au cas où la balise apparaît plusieurs fois
        int lngStart = pstrData.IndexOf(pstrTagBegin);
        while (lngStart != -1) {
            int lngEnd = pstrData.IndexOf(pstrTagEnd, lngStart + pstrTagBegin.Length);
            // The size we return exclude the [begin] and [end] tag.
            // Only what is between the tag is considered a button.
            int lngFieldLength = lngEnd - lngStart + 1;

            if (lngEnd != -1) {
                // Affichage des positions X (Index)
                string strRegionSha = $"[{pstrFormName.ToUpper()}: {plngPosY:D2}=>{lngStart:D2}x{lngEnd:D2}]=={lngFieldLength}";
                string strSha = ShaEx.ComputeSha384(strRegionSha);
                if (!this.FormData._DesignFieldSha.ContainsKey(strSha)) {
                    //Now we add child Key (position)
                    for (int lngPosX = lngStart; lngPosX <= lngEnd; lngPosX++) {
                        FormField fieldPos = new FormField {
                            FormName = pstrFormName.ToUpper(),
                            PosX = lngPosX,
                            PosY = plngPosY
                        };
                        //we keep default value except theses 3, this will allow research by position

                        this.FormData._DesignFieldPos.Add(fieldPos, strSha);
                    }

                    //Field signature and length
                    FormField fieldSign = new FormField {
                        FormName = pstrFormName,
                        FieldType = plngFieldType,
                        PosX = lngStart,
                        PosY = plngPosY,
                        StartX = lngStart,
                        EndX = lngEnd,
                        Length = lngFieldLength
                    };

                    this.FormData._DesignFieldSha.Add(strSha, fieldSign);
                    //Debug.WriteLine($"//Y[{plngPosY}] : {lngStart} @ {lngEnd}");
                }

                // Optionnel : Extraire le contenu entre les balises
                //string content = pstrData.Substring(lngStart + pstrTagB.Length, lngFieldLength);
                //Debug.WriteLine($"  Contenu : {content}");
            } else {
                //Debug.WriteLine($"Balise de fin manquante après l'indice {lngStart}");
            }

            // Chercher la prochaine occurrence après l'index de fin actuel
            lngStart = (lngEnd != -1) ? pstrData.IndexOf(pstrTagBegin, lngEnd + 1) : -1;
        }
    }

    // A [class] cannot be used as a [Dictionary] Key because it used the 'Reference Equality' for comparaison;
    // A [struct] would work but then it would be slow and you would need to implement constructor;
    // The [record] is the right type in that case.

    //b13:
    //I think I got overwhelmed here and forgot it was initially a dictionary Key...
    //I'll need to add a dictionnary and keep only (X, Y) here
    public record FormField {
        public string FormName { get; init; } = "";
        public int FieldType { get; init; } = 0;    //0: Button, 1: COPY/PASTE
        public int PosX { get; init; } = 0;         //The first PosX
        public int PosY { get; init; } = 0;         // temporary variable used in the search dictionary
        public int StartX { get; init; } = 0;       //The Last PosX
        public int EndX { get; init; } = 0;         //The Last PosX
        public int Length { get; init; } = 0;       //The Length of the Button from PosX1 to PosX2 without counting Color Code, mainly [PosX2 - PosX1 + 1]
        public int Length2 { get; init; } = 0;      //unused for now, same as Length but counting Color Code, this would help for ExtractButton()
        public string Label { get; init; } = "";

        //This is for Debug only
        public override string ToString() {
            string strField = $"[FormName: {this.FormName} => [{FieldType}]:{PosX}x{PosY}, Start: {this.StartX:D2}, End: {this.EndX:D2}, Length: {this.Length}]";
            return strField;
        }
    }

    //see _FieldSha and _FieldPos
    //string input1 = "[01x02]";
    //string input2 = "(05x10)";
    public static void ExtractXY(string pstrData) {
        // Pattern logic:
        // \d+ matches one or more digits
        // ( ) creates a "group" so we can extract the X and Y separately
        string pattern = @"(\d+)x(\d+)";

        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(pstrData, pattern);
        if (match.Success) {
            // Group 1 is the first (\d+), Group 2 is the second
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);
            System.Diagnostics.Debug.WriteLine($"Input: {pstrData} -> X: {x}, Y: {y}");
        } else {
            System.Diagnostics.Debug.WriteLine($"Format not recognized for: {pstrData}");
        }
    }

    public string ConvertRow(int plngPosY, string strData) {
        char[] chrData = strData.ToCharArray();
        string strBit = "";

        for (int lngPosX = 0; lngPosX < strData.Length; lngPosX++) {
            SetFormNeighbors(lngPosX, plngPosY);

            if (FormNeighbors.Me != ' ') {
                int lngBits = SetFlagOnMe();
                if (FormNeighbors.Top == ' ') {
                    lngBits = lngBits | (int)enmSideEffects.DontConnectToTop;
                }

                if (FormNeighbors.Left == ' ') {
                    lngBits = lngBits | (int)enmSideEffects.DontConnectToLeft;
                }

                if (FormNeighbors.Bottom == ' ') {
                    lngBits = lngBits | (int)enmSideEffects.DontConnectToDown;
                }

                if (FormNeighbors.Right == ' ') {
                    lngBits = lngBits | (int)enmSideEffects.DontConnectToRight;
                }

                strBit = EliminateSymbols(lngBits);
                if (strBit.Length != 1) {
                    //Error in User Design, show it
                    chrData[lngPosX] = 'X';
                } else {
                    chrData[lngPosX] = strBit[0];
                }
            }
        }

        string strDataRet = new string(chrData);
        return strDataRet;
    }

    private int SetFlagOnMe() {
        int lngBits = 0;
        // ABCD     
        //   | H  V 
        //   |------
        // ╬ | X  X 
        // ┼ |      
        // ╫ |    X 
        // ╪ | X    
        switch (FormNeighbors.Me) {
            case '╬':
                lngBits = lngBits | (int)enmSideEffects.A_HDouble_VDouble;
                break;

            case '┼':
                lngBits = lngBits | (int)enmSideEffects.B_HSingle_VSingle;
                break;

            case '╫':
                lngBits = lngBits | (int)enmSideEffects.C_HSingle_VDouble;
                break;

            case '╪':
                lngBits = lngBits | (int)enmSideEffects.D_HDouble_VSingle;
                break;
        }

        return lngBits;
    }

    //Strange
    //https://www.compart.com/en/unicode/U+219A

    // Box Drawing (Cadre)
    //https://www.compart.com/en/unicode/block/U+2500

    //Block Elements
    //https://www.compart.com/en/unicode/block/U+2580

    // ABCD EFGH
    //   | H  V 
    //   |------
    // ╬ | X  X 
    // ┼ |      
    // ╫ |    X 
    // ╪ | X    
    // ▞ | {START} of [COPY/PASTE] region	U+259E
    // ▚ |  {END}  of [COPY/PASTE] region	U+259A
    // ▌ | {START} of [BUTTON] region		U+258C
    // ▐ |  {END}  of [BUTTON] region		U+2590

    //╔═══════════════════════╗
    //║ E               div/0 ║
    //╠═════╤═════╤═════╤═════╣
    //║  C  │ MR  │ M-  │ M+  ║
    //╟─────┼─────┼─────┼─────╢
    //║ POW │ MOD │     │     ║
    //╟─────┼─────┼─────┼─────╢
    //║  7  │  8  │  9  │  +  ║
    //╟─────┼─────┼─────┼─────╢
    //║  4  │  5  │  6  │  -  ║
    //╟─────┼─────┼─────┼─────╢
    //║  1  │  2  │  3  │  *  ║
    //╟─────┼─────┼─────┼─────╢
    //║  0  │  .  │ +/- │  /  ║
    //╚═════╧═════╧═════╧═════╝

    // ABCD     
    //   | H  V 
    //   |------
    // ╬ | X  X 
    // ┼ |      
    // ╫ |    X 
    // ╪ | X    
    private enum enmSideEffects {
        DontConnectToTop = 1,
        DontConnectToLeft = 2,
        DontConnectToDown = 4,
        DontConnectToRight = 8,

        A_HDouble_VDouble = 16,
        B_HSingle_VSingle = 32,
        C_HSingle_VDouble = 64,
        D_HDouble_VSingle = 128
    }

    private static string EliminateSymbols(int plngBit, string pstrAllChar = strSYMBOLS) {
        string strFilterOut;
        string strFilterIn;
        string strAllChar = pstrAllChar;

        string strConnectToTop = "╠╣╬╟╢║╚╧╩╝├┤┼╞╡│└╨┴┘";
        string strConnectToLeft = "╦╬╤╗╧╩═╝╣╢┬┼╥┐╨┴─┤╡┘";
        string strConnectToDown = "╔╗┌┐╦╬╤║╠╣╟╢┬┼╥│├┤╞╡";
        string strConnectToRight = "╚╔┌└╦╬╤╧╩═╠╟┬┼╥╨┴─├╞";

        string strListA = "╚╝╔╗╦╬╩╠╣║═";
        string strListB = "┌┐└┘┬┼┴├┤│─";
        string strListC = "╟╢╥╨─║";
        string strListD = "╞╡╤╧═│";

        strFilterOut = strConnectToTop;
        if ((plngBit & (int)enmSideEffects.DontConnectToTop) == (int)enmSideEffects.DontConnectToTop) {
            //Don't Connect to Top
            strAllChar = new string(strAllChar.Where(c => !strFilterOut.Contains(c)).ToArray());
        } else {
            //Connect to Top
            strAllChar = new string(strAllChar.Where(c => strFilterOut.Contains(c)).ToArray());
        }

        strFilterOut = strConnectToLeft;
        if ((plngBit & (int)enmSideEffects.DontConnectToLeft) == (int)enmSideEffects.DontConnectToLeft) {
            //Don't Connect to Left
            strAllChar = new string(strAllChar.Where(c => !strFilterOut.Contains(c)).ToArray());
        } else {
            //Connect to Left
            strAllChar = new string(strAllChar.Where(c => strFilterOut.Contains(c)).ToArray());
        }

        strFilterOut = strConnectToDown;
        if ((plngBit & (int)enmSideEffects.DontConnectToDown) == (int)enmSideEffects.DontConnectToDown) {
            //Don't Connect to Down
            strAllChar = new string(strAllChar.Where(c => !strFilterOut.Contains(c)).ToArray());
        } else {
            //Connect to Down
            strAllChar = new string(strAllChar.Where(c => strFilterOut.Contains(c)).ToArray());
        }

        strFilterOut = strConnectToRight;
        if ((plngBit & (int)enmSideEffects.DontConnectToRight) == (int)enmSideEffects.DontConnectToRight) {
            //Don't Connect to Right
            strAllChar = new string(strAllChar.Where(c => !strFilterOut.Contains(c)).ToArray());
        } else {
            //Connect to Right
            strAllChar = new string(strAllChar.Where(c => strFilterOut.Contains(c)).ToArray());
        }

        if ((plngBit & (int)enmSideEffects.A_HDouble_VDouble) == (int)enmSideEffects.A_HDouble_VDouble) {
            //Need to be in strListA
            strFilterIn = strListA;
            strAllChar = new string(strAllChar.Where(c => strFilterIn.Contains(c)).ToArray());
        }

        if ((plngBit & (int)enmSideEffects.B_HSingle_VSingle) == (int)enmSideEffects.B_HSingle_VSingle) {
            //Need to be in strListB
            strFilterIn = strListB;
            strAllChar = new string(strAllChar.Where(c => strFilterIn.Contains(c)).ToArray());
        }

        if ((plngBit & (int)enmSideEffects.C_HSingle_VDouble) == (int)enmSideEffects.C_HSingle_VDouble) {
            //Need to be in strListC
            strFilterIn = strListC;
            strAllChar = new string(strAllChar.Where(c => strFilterIn.Contains(c)).ToArray());
        }

        if ((plngBit & (int)enmSideEffects.D_HDouble_VSingle) == (int)enmSideEffects.D_HDouble_VSingle) {
            //Need to be in strListD
            strFilterIn = strListD;
            strAllChar = new string(strAllChar.Where(c => strFilterIn.Contains(c)).ToArray());
        }

        //System.Diagnostics.Debug.WriteLine("bits [" + strAllChar + "]");
        return strAllChar;
    }

    private void SetFormNeighbors(int plngPosX, int plngPosY) {
        //Set our Data Lines [strBefore], [strCurrent], [strAfter]
        string strBefore;
        if (plngPosY - 1 >= 0) {
            strBefore = this.FormData._DesignFlat[plngPosY - 1];
        } else {
            strBefore = new string(' ', this.FormData.MaxX);
        }

        string strCurrent = this.FormData._DesignFlat[plngPosY];

        string strAfter;
        if (plngPosY + 1 < this.FormData.MaxY) {
            strAfter = this.FormData._DesignFlat[plngPosY + 1];
        } else {
            strAfter = new string(' ', this.FormData.MaxX);
        }

        //Let's get our Neighbors
        FormNeighbors.Left = GetCharOrSpace(strCurrent, plngPosX - 1);
        FormNeighbors.Me = GetCharOrSpace(strCurrent, plngPosX);
        FormNeighbors.Right = GetCharOrSpace(strCurrent, plngPosX + 1);

        FormNeighbors.Top = GetCharOrSpace(strBefore, plngPosX);
        FormNeighbors.Bottom = GetCharOrSpace(strAfter, plngPosX);
    }

    private char GetCharOrSpace(string pstrData, int plngPosX) {
        // ABCD     
        //   | H  V 
        //   |------
        // ╬ | X  X 
        // ┼ |      
        // ╫ |    X 
        // ╪ | X    
        string strSymbols = "╬┼╫╪";

        char chrRet = ' ';
        if (plngPosX >= 0) {
            if (plngPosX < pstrData.Length) {
                char chrX = pstrData[plngPosX];
                if (strSymbols.Contains(chrX)) {
                    chrRet = chrX;
                }
            }
        }

        return chrRet;
    }

    private static class FormNeighbors {
        public static char Left { get; set; } = ' ';
        public static char Me { get; set; } = ' ';
        public static char Right { get; set; } = ' ';

        public static char Top { get; set; } = ' ';
        public static char Bottom { get; set; } = ' ';
    }

    //public const char ColorCodeNewLine = 'ª';       //char c = '\u00AA'; [ª0] don't change CursorPos, [ª1] CrLf
    //public const char ColorCodeForeground = '®';    //char c = '\u00AE';
    //public const char ColorCodeBackground = '©';    //char c = '\u00A9';
    public void FormMergeBack() {
        List<string> pobjForm = this.FormData._Design;

        if (pobjForm.Count != this.FormData._DesignFlat.Count) {
            throw new ArgumentException("Original and plain lists must have the same number of elements.");
        }

        char[] codeLetters = new char[] { sCR, sCF, sCB };

        for (int i = 0; i < pobjForm.Count; i++) {
            string strFormY = pobjForm[i];
            string strPlainList = this.FormData._DesignFlat[i];
            char[] chrMerged = strFormY.ToCharArray();

            int plainIndex = 0;
            for (int j = 0; j < chrMerged.Length; j++) {
                // Check if current char is a code letter and next char exists
                if (Array.IndexOf(codeLetters, chrMerged[j]) >= 0 && j + 1 < chrMerged.Length) {
                    j++; // Skip the second char of the code
                    continue;
                }

                // Replace with plain character
                if (plainIndex < strPlainList.Length) {
                    chrMerged[j] = strPlainList[plainIndex];
                    plainIndex++;
                }
            }

            strFormY = FlushFieldTag(new string(chrMerged));
            this.FormData._Design[i] = strFormY;
        }
    }

    public List<string> DesignGet(string pstrFormName, int lngDesignType = 0) {
        List<string> lstRet = new List<string>();

        if (lngDesignType == 1) {
            lstRet = new List<string>(this.FormData._DesignFlat);
        } else if (lngDesignType == 2) {
            lstRet = new List<string>(this.FormData._DesignField);
        } else {
            lstRet = new List<string>(this.FormData._Design);
        }

        return lstRet;
    }

    public string ExtractButton(int plngPosX, int plngPosY, int plngLength) {
        if (plngPosY >= this.FormData._Design.Count) {
            return string.Empty;
        }

        string pstrData = this.FormData._Design[plngPosY];
        if (string.IsNullOrEmpty(pstrData)) {
            return string.Empty;
        }

        string strRet = "";
        int lngLogicalCount = 0;   // Compte uniquement les caractères "réels"
        int lngPhysicalCount = 0;  // Compte combien de caractères "réels" ont été ajoutés
        int i = 0;

        while (i < pstrData.Length && lngPhysicalCount < plngLength) {
            // Détection du code "Xx"
            if (pstrData[i] == sCF || pstrData[i] == sCB) {
                i++;
                if (i < pstrData.Length && char.IsLetter(pstrData[i])) {
                    // skip the rest of code
                    // on inclut le code sans l'ajouter au compte logique
                    if (lngLogicalCount >= plngPosX) {
                        strRet = strRet + pstrData[i - 1];
                        strRet = strRet + pstrData[i];
                    }
                    i++;

                    continue;
                } else {
                    break;
                }
            }

            // Traitement d'un caractère de contenu "réel"
            if (lngLogicalCount >= plngPosX) {
                strRet = strRet + pstrData[i];
                lngPhysicalCount++; // On a récolté 1 caractère réel
            }

            lngLogicalCount++;
            i++;
        }

        return strRet;
    }

    public static string ToPascalCase(string pstrData, bool pblnTrim = true) {
        string strRet = pstrData + "";
        if (pblnTrim) {
            strRet = strRet.Trim();
        }

        if (strRet.Length > 0) {
            strRet = strRet.ToLower();
            string strChar = strRet.Substring(0, 1).ToUpper();
            strRet = strChar + strRet.Substring(1);
        }

        return strRet;
    }

    public class FormDataEx {
        public List<string> _Design = new List<string>();
        public List<string> _DesignFlat = new List<string>();
        public List<string> _DesignField = new List<string>();
        public Dictionary<string, FormField> _DesignFieldSha = new Dictionary<string, FormField>();
        public Dictionary<FormField, string> _DesignFieldPos = new Dictionary<FormField, string>();

        public List<string> Design {
            get; set;
        } = new List<string>();

        public int StartX {
            get; set;
        } = 0;

        public int StartY {
            get; set;
        } = 0;

        //this need private set but json can't set it ?
        public string FormName {
            get; set;
        } = "";

        //this need private set but json can't set it ?
        public int MaxX {
            get; set;
        } = 0;

        //this need private set but json can't set it ?
        public int MaxY {
            get; set;
        } = 0;
    }
}

#region Parent FormCollection
public sealed class FormCollection {
    // It's [public] because [SettingsJson.Save] need to be able to save theses but you should use only the provided method
    public Dictionary<string, FormEx> Forms { get; set; } = new Dictionary<string, FormEx>();

    public void Add(FormEx? pobjForm, bool pblnThrow = true) {
        if (pobjForm != null) {
            string strFormName = FormEx.ToPascalCase(pobjForm.FormData.FormName);
            if (this.Forms.ContainsKey(strFormName)) {
                if (pblnThrow) {
                    throw new InvalidOperationException($"A Script with the Name [{pobjForm.FormData.FormName}] already exists");
                }
            } else {
                this.Forms.Add(pobjForm.FormData.FormName, pobjForm);
            }
        }
    }

    public bool Exist(string pstrFormName) {
        string strFormName = FormEx.ToPascalCase(pstrFormName);
        bool blnRet = false;

        if (this.Forms.ContainsKey(strFormName)) {
            blnRet = true;
        }

        return blnRet;
    }

    public bool Remove(string pstrFormName) {
        string strFormName = FormEx.ToPascalCase(pstrFormName);
        bool blnRet = false;

        if (this.Forms.ContainsKey(strFormName)) {
            this.Forms.Remove(strFormName);
            blnRet = true;
        }

        return blnRet;
    }
}
#endregion Parent FormCollection