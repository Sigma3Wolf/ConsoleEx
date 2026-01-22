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
//      Usage: Allow saving and restoring of console Theme, Color and Box for Menu.                 //
// Dependency: NuGet Newtonsoft.Json                                                                //
//             vcxAppExConsole_v2.02.cs                                                             //
//**************************************************************************************************//
// v2.00 - 2026-01-14:	** INITIAL RELEASE **;
// v2.01 - 2026-01-16:	Cosmetic;
// v2.02 - 2026-01-18:	Removing parameterless SetPosition();
//						Removing Console.CursorTop and Console.CursorLeft reference (ANSI issue);
// v2.03 - 2026-01-19:	Implementing List<Form>

#region PrototypeOmega namespace
#pragma warning disable IDE0130
using static PrototypeOmega.ConsoleAppEx;

namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

public partial class ConsoleAppEx : TextWriter {
	private const string strSYMBOLS = "╚╝╔╗╦╬╤╩╧╠╣╞╡║═ ┌┐└┘┬┼╥╨┴╟╢├┤│─";
	private FormData _FormData = null;
	
	public void FormBegin(string pstrFormName, int plngStartX, int plngStartY) {
		string strFormName = ConsoleAppEx.ToPascalCase(pstrFormName);
		if (string.IsNullOrEmpty(strFormName)) {
			throw new Exception("FormName must be set");
		}

		this._FormData = new FormData(this, strFormName);
		this._FormData.StartX = plngStartX;
		this._FormData.StartY = plngStartY;
	}

    public void FormEnd(bool pblnShowMessage = true) {
        if (this._FormData != null) {
            if (pblnShowMessage) {
                this.SetPosition(0, 0);
                Console.Write("Loading necessary modules...");
                this._FormData.FormConvert();
                this.SetPosition(0, 0);
                Console.Write("                            ");
            } else {
                this._FormData.FormConvert();
            }
        }
    }

    public void FormAddLine(string pstrLine) {
		if (this._FormData != null) {
			//Add ending CR2
			//string sCR2 = sCR + "2";
			//if (!pstrLine.EndsWith(sCR2)) {
			//	pstrLine= pstrLine + sCR2;
			//}
			this._FormData.FormAddLine(pstrLine, sCR, sCF, sCB);
		}
	}

	public void FormShow(int plngMoveToX = 0, int plngMoveToY = 0, int plngDebugOnly = 0) {
		//DesignGet(plngDesign)
		// 0: this._lstDesign
		// 1: this._lstDesignFlat
		// 2: this._lstDesignField
		if (this._FormData != null) {
			if (ScreenPos.ValidateCursorPosition(plngMoveToX, plngMoveToY)) {
				string sCR2 = sCR + "2";
				ResetActiveColor();

				//First we save current position
				this.ScreenPos.SavedX = this._FormData.StartX;
				this.ScreenPos.SavedY = this._FormData.StartY;
				this.SetPosition(this.ScreenPos.SavedX, this.ScreenPos.SavedY);

				List<string> lstDesign = this._FormData.DesignGet(plngDebugOnly);
				for (int i = 0; i < lstDesign.Count; i++) {
					//Debug: Console.WriteLine("//" + lstDesign[i]);
					string strData = lstDesign[i];
					//System.Diagnostics.Debug.WriteLine($"[{strData}]");
					Console.Write(strData + $"{sCR2}");
				}
				this.SetPosition(this._FormData.StartX + plngMoveToX, this._FormData.StartY + plngMoveToY);
			}
		}
	}

	public FormData GetActiveForm() {
		return this._FormData;
	}

	public void SetActiveForm(FormData pobjForm) {
		if (pobjForm != null) {
			this.FormBegin(pobjForm.FormName, pobjForm.StartX, pobjForm.StartY);
			if (this._FormData != null) {
				List<string> objDesign = pobjForm.Design;

				for (int i = 0; i < objDesign.Count; i++) {
					string strLine = objDesign[i];
					this._FormData.FormAddLine(strLine, sCR, sCF, sCB);
				}
				this.FormEnd();
			}
		}
	}

	public class FormData {
		private readonly ConsoleAppEx? _parent = null;
		private List<string> _lstDesign = new List<string>();
		private List<string> _lstDesignFlat = new List<string>();
		private List<string> _lstDesignField = new List<string>();
		private Dictionary<string, FormField> _FieldSha = new Dictionary<string, FormField>();
		private Dictionary<FormField, string> _FieldPos = new Dictionary<FormField, string>();

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

		[Newtonsoft.Json.JsonConstructor] // Aka [Json Constructor] is optional, Newtonsoft.Json will use private too
		private FormData() {
			//private because a FormData CANNOT go without FormName, you need to use the constructor
		}

		//public FormData(ConsoleAppEx pobjParent, string pstrFormName) {
		public FormData(string pstrFormName) {
			InitDesign(pstrFormName);
		}

		public FormData(ConsoleAppEx pobjParent, string pstrFormName) {
			InitDesign(pstrFormName);

			//This is unused for now
			this._parent = pobjParent;
		}

		private void InitDesign(string pstrFormName) {
			this._lstDesign.Clear();
			this._lstDesignFlat.Clear();
			this._lstDesignField.Clear();
			this._FieldSha.Clear();
			this._FieldPos.Clear();

			// b13: the [FormName] is mandatory for event CLSID, yet I don't validate it.
			this.FormName = ConsoleAppEx.ToPascalCase(pstrFormName);
			this.MaxX = Console.BufferWidth;
			this.MaxY = Console.BufferHeight;
		}

		public void FormAddLine(string pstrLine, char sCR, char sCF, char sCB) {
			string sCR0 = sCR + "0";
			string strLine = pstrLine.Trim().Replace(sCR0, "");
			if (!string.IsNullOrEmpty(strLine)) {
				this.Design.Add(strLine);

				string strFixed = RemoveColorsAndFix(strLine, sCR, sCF, sCB, out string strFlat);
				if (this._lstDesign.Count < this.MaxY) {
					this._lstDesign.Add(strFixed);
					this._lstDesignFlat.Add(strFlat);
				}
			}
		}

		//Remove special Code for [DesignFlat] and fix the special Code if odd
		private string RemoveColorsAndFix(string pstrData, char sCR, char sCF, char sCB, out string pstrFlat) {
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

			bool blnSuccess = this._FieldPos.TryGetValue(pobjSearch, out string? strSha);
			if (blnSuccess) {
				strRet = strSha + "";
				FormField? objField = new FormField();
				blnSuccess = this._FieldSha.TryGetValue(strRet, out objField);
				if (!blnSuccess || objField == null) {
					//if [this._FieldPos] is set, then [this._FieldSha] has to be set
					throw new Exception("Programmer Error!");
				}

				pobjSearch = objField;
			}
			
			return strRet;
		}

		private void GetRealFormMaxXY() {
			this.MaxX = 0;
			this.MaxY = this._lstDesignFlat.Count;
			for (int i = 0; i < this.MaxY; i++) {
				if (this._lstDesignFlat[i].Length > this.MaxX) {
					this.MaxX = this._lstDesignFlat[i].Length;
				}
			}
		}

		public void FormConvert() {
			List<string> lstFormFlat = new List<string>();
			List<string> lstFormFields = new List<string>();

			// Prepare to create Field
			this._FieldSha.Clear();
			this._FieldPos.Clear();
			string pstrFormName = this.FormName;

			//First we set MaxX
			this.GetRealFormMaxXY();

			//b13 work: _lstDesign must not contain Field Tag
			// DEBUG ONLY
			if ((this.MaxX > 0) && (this.MaxY > 0)) {
				for (int lngPosY = 0; lngPosY < this.MaxY; lngPosY++) {
					string strDataFlat = this._lstDesignFlat[lngPosY];
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
					// 0: this._lstDesign		: the one that show on screen in final
					// 1: this._lstDesignFlat	:
					// 2: this._lstDesignField

					//Finally, remove all button spacer section code
					// EFGH
					// ▞ | {START} of [COPY/PASTE] region
					// ▚ |  {END}  of [COPY/PASTE] region
					// ▌ | {START} of [BUTTON] region
					// ▐ |  {END}  of [BUTTON] region
					strDataFlat = FlushFieldTag(strDataFlat);
					lstFormFields.Add(strDataFlat);
				}

				this._lstDesignFlat = lstFormFlat;
				this._lstDesignField = lstFormFields;
				this.FormMergeBack();
			} else {
				//Let's reset instead of ruining the design process
				this.MaxX = Console.BufferWidth;
				this.MaxY = Console.BufferHeight;
			}
		}

		public void DebugForm() {
			Console.WriteLine("\r\nlstDesignFlat");
			for (int o = 0; o < this._lstDesignFlat.Count; o++) {
				Console.WriteLine($"{o:D2}: {this._lstDesignFlat[o]}");
			}
			Console.WriteLine("------------------------------------------------------\r\n");

			Console.WriteLine("_lstDesign");
			for (int o = 0; o < this._lstDesign.Count; o++) {
				Console.Write($"{o:D2}: {this._lstDesign[o]}");
			}
			Console.WriteLine("------------------------------------------------------\r\n");

			Console.WriteLine("lstDesignField");
			for (int o = 0; o < this._lstDesignField.Count; o++) {
				Console.WriteLine($"{o:D2}: {this._lstDesignField[o]}");
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
					if (!this._FieldSha.ContainsKey(strSha)) {
						//Now we add child Key (position)
						for (int lngPosX = lngStart; lngPosX <= lngEnd; lngPosX++) {
							FormField fieldPos = new FormField {
								FormName = pstrFormName.ToUpper(),
								PosX = lngPosX,
								PosY = plngPosY
							};
							//we keep default value except theses 3, this will allow research by position
							
							this._FieldPos.Add(fieldPos, strSha);
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
						
						this._FieldSha.Add(strSha, fieldSign);
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
			public int FieldType { get; init; } = 0;	//0: Button, 1: COPY/PASTE
			public int PosX { get; init; } = 0;			//The first PosX
			public int PosY { get; init; } = 0;			// temporary variable used in the search dictionary
			public int StartX { get; init; } = 0;		//The Last PosX
			public int EndX { get; init; } = 0;			//The Last PosX
			public int Length { get; init; } = 0;		//The Length of the Button from PosX1 to PosX2 without counting Color Code, mainly [PosX2 - PosX1 + 1]
			public int Length2 { get; init; } = 0;		//unused for now, same as Length but counting Color Code, this would help for ExtractButton()
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
				strBefore = this._lstDesignFlat[plngPosY - 1];
			} else {
				strBefore = new string(' ', this.MaxX);
			}

			string strCurrent = this._lstDesignFlat[plngPosY];

			string strAfter;
			if (plngPosY + 1 < this.MaxY) {
				strAfter = this._lstDesignFlat[plngPosY + 1];
			} else {
				strAfter = new string(' ', this.MaxX);
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
			List<string> pobjForm = this._lstDesign;

			if (pobjForm.Count != this._lstDesignFlat.Count) {
				throw new ArgumentException("Original and plain lists must have the same number of elements.");
			}
			
			char[] codeLetters = new char[] { sCR, sCF, sCB };

			for (int i = 0; i < pobjForm.Count; i++) {
				string strFormY = pobjForm[i];
				string strPlainList = this._lstDesignFlat[i];
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
				this._lstDesign[i] = strFormY;
			}
		}

		public List<string> DesignGet(int lngDesignType = 0) {
			List<string> lstRet = new List<string>();

			if (lngDesignType == 1) {
				lstRet = new List<string>(this._lstDesignFlat);
			} else if (lngDesignType == 2) {
				lstRet = new List<string>(this._lstDesignField);
			} else {
				lstRet = new List<string>(this._lstDesign);
			}

			return lstRet;
		}

		public string ExtractButton(int plngPosX, int plngPosY, int plngLength) {
			if (plngPosY >= this._lstDesign.Count) {
				return string.Empty;
			}

			string pstrData = this._lstDesign[plngPosY];
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
}

#region Parent FormCollection
public sealed class FormCollection {
	// It's [public] because [SettingsJson.Save] need to be able to save theses but you should use only the provided method
	public Dictionary<string, FormData> Forms { get; set; } = new Dictionary<string, FormData>();

	public void Add(FormData pobjForm, bool pblnThrow = true) {
		if (pobjForm != null) {
			string strFormName = ConsoleAppEx.ToPascalCase(pobjForm.FormName);
			if (this.Forms.ContainsKey(strFormName)) {
				if (pblnThrow) {
					throw new InvalidOperationException($"A Script with the Name [{pobjForm.FormName}] already exists");
				}
			} else {
				this.Forms.Add(pobjForm.FormName, pobjForm);
			}
		}
	}

	public bool Exist(string pstrFormName) {
		string strFormName = ConsoleAppEx.ToPascalCase(pstrFormName);
		bool blnRet = false;

		if (this.Forms.ContainsKey(strFormName)) {
			blnRet = true;
		}

		return blnRet;
	}

	public bool Remove(string pstrFormName) {
		string strFormName = ConsoleAppEx.ToPascalCase(pstrFormName);
		bool blnRet = false;

		if (this.Forms.ContainsKey(strFormName)) {
			this.Forms.Remove(strFormName);
			blnRet = true;
		}

		return blnRet;
	}
}
#endregion Parent FormCollection


//Example 1:
////This is a FlatDesign
//string sCFE = $"{sCF}E";
//objConsole.DesignBegin("Calculator");
//objConsole.DesignAddLine($"{sCFE}ADDDDDDDDDDDDDDDDDDDDDDDA");
//objConsole.DesignAddLine($"{sCR0}A                       A");
//objConsole.DesignAddLine($"{sCR0}AAAAAADAAAAADAAAAADAAAAAA");
//objConsole.DesignAddLine($"{sCR0}A     B     B     B     A");
//objConsole.DesignAddLine($"{sCR0}CBBBBBBBBBBBBBBBBBBBBBBBC");
//objConsole.DesignAddLine($"{sCR0}A     B     B     B     A");
//objConsole.DesignAddLine($"{sCR0}CBBBBBBBBBBBBBBBBBBBBBBBC");
//objConsole.DesignAddLine($"{sCR0}A     B     B     B     A");
//objConsole.DesignAddLine($"{sCR0}CBBBBBBBBBBBBBBBBBBBBBBBC");
//objConsole.DesignAddLine($"{sCR0}A     B     B     B     A");
//objConsole.DesignAddLine($"{sCR0}CBBBBBBBBBBBBBBBBBBBBBBBC");
//objConsole.DesignAddLine($"{sCR0}A     B     B     B     A");
//objConsole.DesignAddLine($"{sCR0}CBBBBBBBBBBBBBBBBBBBBBBBC");
//objConsole.DesignAddLine($"{sCR0}A     B     B     B     A");
//objConsole.DesignAddLine($"{sCR0}ADDDDDDDDDDDDDDDDDDDDDDDA");
//objConsole.DesignEnd();
//objConsole.DesignShow();