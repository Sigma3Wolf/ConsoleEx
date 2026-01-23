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
// Dependency: vcxPInvoke                                                                           //
//**************************************************************************************************//
// v2.00 - 2026-01-13:	** INITIAL RELEASE **;
// v2.01 - 2026-01-16:	Cosmetic;
// v2.02 - 2026-01-18:	Removing parameterless SetPosition();
//						Removing Console.CursorTop and Console.CursorLeft reference (ANSI issue);
// v2.03 - 2026-01-19:	Fix Saving; Add Carret modification;
// v2.04 - 2026-01-23:	Preparing structure for multiple form;

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

//idea to add
//define variable that you can get/set
//example: Define("X => int = 6");
//         Define("Position => (x,y) = (12,2)")

//Add Cr and Lf Capability
// Cr Y+1, Y-1

//		1	2	4	8	16	32
//		A	B	C	D	E	F
// X-z   X-D = X-8
// X+z   X+D = X+8

// TODO <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
// NOTE: The DisableScroll not working yet  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

// Thing to check and validate if it still work after I have removed SetPosition();
// 1. MoveXY
// 2. WordWrap
// 3. OverflowX


// Usage example: SEE END OF FILE
public partial class ConsoleAppEx : TextWriter {
	private bool _disposed;
	private static int _isActive;
	private readonly TextWriter _original;
	private readonly System.Text.Encoding _originalEncoding;
	private static IntPtr _InputHandle = IntPtr.Zero;
	private readonly bool _ansiEnabled;
	private bool _ShouldProbablyAlwaysBeFalse = false;  //This only say if we should keep track for Height before exiting. I don't see WHEN it should be true except when Forcing a CrLf, we should revisit the coding on this.
	public event EventHandler<ConsoleEventArgs>? ConsoleEvent = null;

	//Const and alike
	public const char sCR = 'ª';       //NewLine		=> char c = '\u00AA'; [ª0] don't change CursorPos, [ª1] CrLf
	public const char sCF = '®';    //Foreground	=> char c = '\u00AE';
	public const char sCB = '©';    //Background	=> char c = '\u00A9';
	private readonly string CrLf = Environment.NewLine;

	public static bool _RunningProgram = true;
	public LastErrorEx LastError;
	public ScreenPosEx ScreenPos;
	private (int r, int g, int b) ForegroundColorRgb;
	private (int r, int g, int b) BackgroundColorRgb;
	private Dictionary<char, (int r, int g, int b)> ColorList;

	//Constructor
	public ConsoleAppEx() {
		// Enforce single active instance by avoiding race condition under different thread;
		if (Interlocked.Exchange(ref _isActive, 1) == 1) {
			throw new InvalidOperationException("ConsoleAppEx is already active and has not been disposed.");
		}

		try {
			// Save original encoding BEFORE changing it;
			_originalEncoding = Console.OutputEncoding;

			// Modify original OutputEncoding;
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.Clear();

			// this must come last in constructor;
			// Save original writer (now UTF8);
			_original = Console.Out;
			Console.SetOut(this);
		} catch {
			// Roll back activation on constructor failure;
			Interlocked.Exchange(ref _isActive, 0);
			throw;
		}

		this._ansiEnabled = TryEnableAnsi();
		this.ForegroundColorRgb = GetConsoleColorRgb(Console.ForegroundColor);
		this.BackgroundColorRgb = GetConsoleColorRgb(Console.BackgroundColor);

		this.LastError = new LastErrorEx();
		this.ScreenPos = new ScreenPosEx(this);
		this.ColorList = this.DefineColorList();    //execute this.ResetColor();
	}
	//Mandatory
	public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;

	protected virtual void OnConsoleEvent(int plngEventNo, string pstrEventData) {
		// Use the null-conditional operator to fire the event safely
		ConsoleEvent?.Invoke(this, new ConsoleEventArgs { EventNo = plngEventNo, EventData = pstrEventData });
	}

	public bool CheckInterval(ref long plngReference, int plngElapsed) {
		bool blnRet = false;

		if (plngReference != 0) {
			// Check if the difference is 125ms or more
			long lngCurrentTime = Environment.TickCount64;
			if (lngCurrentTime - plngReference >= plngElapsed) {
				plngReference = 0;
				blnRet = true;
			}
		}

		return blnRet;
	}

	public override void Flush() {
		_original.Flush(); // force l'affichage réel
	}

	public bool DisableScroll {
		get; set;
	} = false;

	public bool WordWrap {
		get; set;
	} = false;

	private void WriteAndMove(string strData) {
		this._original.Write(strData);
		this.Flush();

		//should be reliable if you put multiple CrLf on same line
		if (strData.EndsWith(CrLf)) {
			this._ShouldProbablyAlwaysBeFalse = true;
		} else {
			this._ShouldProbablyAlwaysBeFalse = false;
		}

		if (strData == CrLf) {
			ScreenPos.UpdateX(0);
			ScreenPos.UpdateY(this.ScreenPos.Y + 1, this._ShouldProbablyAlwaysBeFalse);
		} else {
			ScreenPos.UpdateX(this.ScreenPos.X + strData.Length);
		}
	}

	public override void WriteLine() {
		this.LastError.LastFunction = "WriteLine";
		this.WriteAndMove(CrLf);
	}

	// this method modify Console.WriteLine(string) behavior;
	public override void WriteLine(string? value) {
		this.LastError.LastFunction = "WriteLine";
		if (!string.IsNullOrEmpty(value)) {
			this.Write(value);
		}
		this.WriteAndMove(CrLf);
	}

	// this method modify Console.Write(string) behavior;
	public override void Write(string? value) {
		this.LastError.LastFunction = "Write";

		if (!string.IsNullOrEmpty(value)) {
			string strText = value;

			//Here we process strText with Code (Color and CrLf)
			this.ColorWrite(strText);
		}
	}

	//public void ShowField(string pstrData, int plngFieldLen = 0) {
	//	int lngMaxSpace;
	//	int lngSpace;

	//	string strData = "";
	//	int lngScreenPosX = ScreenPos.X + ScreenPos.DeltaX;
	//	if (lngScreenPosX > 0) {
	//		if (ScreenPos.X == 0) {
	//			this.ScreenPos.DeltaX = this.ScreenPos.DeltaX - 1;
	//			this.SetXPosition();

	//			lngMaxSpace = Console.BufferWidth - 2 - this.ScreenPos.DeltaX - this.ScreenPos.X;
	//			lngSpace = (plngFieldLen < 1) ? lngMaxSpace : plngFieldLen;
	//			if (lngSpace > lngMaxSpace) {
	//				lngSpace = lngMaxSpace;
	//			}
	//			strData = "[" + new string(' ', lngSpace) + "]";

	//			this._original.Write(strData);
	//			this.Flush();

	//			this.ScreenPos.DeltaX = this.ScreenPos.DeltaX + 1;
	//			this.SetXPosition();
	//			this.WriteAndMove(pstrData);
	//		} else {
	//			this.ScreenPos.UpdateX(this.ScreenPos.X - 1);
	//			this.SetXPosition();
	//			lngMaxSpace = Console.BufferWidth - 2 - this.ScreenPos.DeltaX - this.ScreenPos.X;
	//			lngSpace = (plngFieldLen < 1) ? lngMaxSpace : plngFieldLen;
	//			if (lngSpace > lngMaxSpace) {
	//				lngSpace = lngMaxSpace;
	//			}
	//			strData = "[" + new string(' ', lngSpace) + "]";

	//			this._original.Write(strData);
	//			this.Flush();

	//			this.ScreenPos.UpdateX(this.ScreenPos.X + 1);
	//			this.SetXPosition();
	//			this.WriteAndMove(pstrData);
	//		}
	//	} else {
	//		lngMaxSpace = Console.BufferWidth - 2 - ScreenPos.DeltaX - ScreenPos.X;
	//		lngSpace = (plngFieldLen < 1) ? lngMaxSpace : plngFieldLen;
	//		if (lngSpace > lngMaxSpace) {
	//			lngSpace = lngMaxSpace;
	//		}
	//		strData = "[" + new string(' ', lngSpace) + "]";

	//		this._original.Write(strData);
	//		this.Flush();

	//		this.ScreenPos.UpdateX(this.ScreenPos.X + 1);
	//		this.SetXPosition();
	//		this.WriteAndMove(pstrData);
	//	}
	//}

	private void ColorWrite(string pstrData) {
		bool blnDebug = false;

		(int r, int g, int b) colTemp = (0, 0, 0);
		string[] DataArray = SplitData(pstrData, out int lngPosX, out int lngPosY);
		for (int i = 0; i < DataArray.Length; i++) {
			string strPart = DataArray[i];
			if (blnDebug) {
				//This is DEBUG, DO NOT MODIFY
				this._original.Write(i + " : [" + strPart + "]" + CrLf);
				this.Flush();
			} else {
				switch (strPart[0]) {
					case sCR:
						if (strPart.Length == 2) {
							if (strPart[1] == '1') {
								this.WriteAndMove(CrLf);
							} else if (strPart[1] == '2') {
								//This should Lf but move to SavedX, not Cr
								//this.WriteAndMove(CrLf);
								//ScreenPos.UpdateX(ScreenPos.SavedX);
								ScreenPos.UpdateXY(this.ScreenPos.SavedX, this.ScreenPos.Y + 1, this._ShouldProbablyAlwaysBeFalse);
							}
						} else {
							throw new Exception("Invalid SplitData");
						}
						break;

					case sCF:
						if (strPart.Length == 2) {
							colTemp = this.GetDicColor(strPart[1]);
							SetForegroundColor(colTemp);
						} else {
							throw new Exception("Invalid SplitData");
						}
						break;

					case sCB:
						if (strPart.Length == 2) {
							colTemp = this.GetDicColor(strPart[1]);
							SetBackgroundColor(colTemp);
						} else {
							throw new Exception("Invalid SplitData");
						}
						break;

					default:
						if (ScreenPos.ValidateCursorPosition(this.ScreenPos.X + strPart.Length, this.ScreenPos.Y)) {
							this.WriteAndMove(strPart);
						} else {
							if (this.WordWrap) {
								//WARNING: We are not handling a string larger then the whole screen
								ScreenPos.UpdateX(0);
								ScreenPos.UpdateY(this.ScreenPos.Y + 1, false);
								//this.SetXPosition();
								this.WriteAndMove(strPart);
							} else {
								//OverflowX
								bool blnSuccess = false;
								do {
									int lngRemaining = (this.ScreenPos.X + strPart.Length) - Console.BufferWidth;
									string strPart1 = strPart.Substring(0, strPart.Length - lngRemaining);
									this._original.Write(strPart1);
									this.Flush();

									ScreenPos.UpdateX(0);
									ScreenPos.UpdateY(this.ScreenPos.Y + 1, false);
									//this.SetXPosition();

									strPart = strPart.Substring(strPart.Length - lngRemaining, lngRemaining);
									blnSuccess = ScreenPos.ValidateCursorPosition(this.ScreenPos.X + strPart.Length, this.ScreenPos.Y);
								} while (!blnSuccess);

								if (strPart.Length > 0) {
									this.WriteAndMove(strPart);
								}
							}
						}
						break;
				}
			}
		}
	}

	static string[] SplitData(string input, out int plngPosX, out int plngPosY) {
		string unixData = input.Replace("\n\r", "\r\n").Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", sCR + "1");
		plngPosX = 0;
		plngPosY = 0;

		int i = 0;
		List<string> result = new List<string>();
		while (i < unixData.Length) {
			if (unixData[i] == sCR || unixData[i] == sCF || unixData[i] == sCB) {
				// Handle special codes
				char code = unixData[i];
				i++;

				// Grab next character if it exists
				string nextChar = (i < unixData.Length) ? unixData[i].ToString() : "A"; //FALL BACK to Color A when COLOR CODE MISSING
				i++; // Skip the next character because we already included it

				if ((code == sCR) && (nextChar == "1")) {
					plngPosX = 0;
					plngPosY++;
				}

				result.Add(code + nextChar);
			} else {
				// Collect normal text until next separator
				int start = i;
				while (i < unixData.Length &&
					   !(i + 1 < unixData.Length && unixData[i] == '\r' && unixData[i + 1] == '\n') &&
					   unixData[i] != sCR &&
					   unixData[i] != sCF &&
					   unixData[i] != sCB) {
					i++;
				}
				string strRemain = unixData.Substring(start, i - start);
				plngPosX += strRemain.Length;

				result.Add(strRemain);
			}
		}

		return result.ToArray();
	}

	protected override void Dispose(bool disposing) {
		if (_disposed) {
			return;
		}

		if (disposing) {
			// Restore console state
			this.ResetActiveColor();
			this.EmdOfPage();
			Console.SetOut(_original);
			Console.OutputEncoding = _originalEncoding;

			// Release single-instance lock
			Interlocked.Exchange(ref _isActive, 0);
		}

		_disposed = true;
		base.Dispose(disposing);
	}

	private void EmdOfPage() {
		Console.SetCursorPosition(0, ScreenPos.Height);
	}

	public bool SetCursorPosition(int plngPosX = 0, int plngPosY = 0) {
		bool blnCheck = this.ScreenPos.ValidateCursorPosition(plngPosX, plngPosY);
		if (blnCheck) {
			Console.SetCursorPosition(plngPosX, plngPosY);
		}

		return blnCheck;
	}

	public void SetPosition(int plngPosX, int plngPosY) {
		int lngPosX = plngPosX + this.ScreenPos.DeltaX;
		int lngPosY = plngPosY + this.ScreenPos.DeltaY;
		bool blnSuccess = this.SetCursorPosition(lngPosX, lngPosY);
		if (blnSuccess) {
			//Never do a UpdateXY() here [CRC]
			this.ScreenPos.UpdateX(plngPosX);
			this.ScreenPos.UpdateY(plngPosY, false);
		}
	}

	private (int r, int g, int b) GetDicColor(char pchrColorCode) {
		char chrColorCode = char.ToUpper(pchrColorCode);

		if (this.ColorList.ContainsKey(chrColorCode)) {
			return this.ColorList[chrColorCode];
		} else {
			throw new Exception("invalid code in GetDicColor");
		}
	}

	public bool ChangeCustomColor(char pchrPosition, System.Drawing.Color color) {
		bool blnSuccess = false;

		blnSuccess = ChangeCustomColor(pchrPosition, (color.R, color.G, color.B));
		return blnSuccess;
	}

	//A-Z
	public bool ChangeCustomColor(char pchrPosition, (int r, int g, int b) color) {
		bool blnSuccess = false;
		RgbColorEx c = new RgbColorEx(color.r, color.g, color.b);

		if ((pchrPosition >= 'A') && (pchrPosition <= 'Z')) {
			this.ColorList[pchrPosition] = (c.R, c.G, c.B);
			blnSuccess = true;
		}

		return blnSuccess;
	}

	//Color A-Z
	private Dictionary<char, (int r, int g, int b)> DefineColorList() {
		this.LastError.LastFunction = "DefineColorList";

		//Reset console color in case of STOP while debugging
		this.ResetActiveColor();

		Dictionary<char, (int r, int g, int b)> objDic = new Dictionary<char, (int r, int g, int b)>();
		objDic.Add('A', GetConsoleColorRgb(ConsoleColor.Gray));     // MANDATORY (FALL BACK to Color A when COLOR CODE MISSING)
		objDic.Add('B', GetConsoleColorRgb(ConsoleColor.Black));    // MANDATORY

		int lngLetter = 67; //C
		char chrLetter;
		(int r, int g, int b) objColor;
		foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor))) {
			if ((color != ConsoleColor.Black) && (color != ConsoleColor.Gray)) {
				chrLetter = (char)lngLetter;
				lngLetter++;
				objColor = GetConsoleColorRgb(color);
				objDic.Add(chrLetter, objColor);
			}
		}

		//Add 'Q' to 'Y'
		objColor = (0, 0, 255);
		for (int i = lngLetter; i <= 89; i++) {
			chrLetter = (char)i;
			objDic.Add(chrLetter, objColor);
		}

		//Add 'Z'
		objColor = (0, 0, 0);   //PureBlack
		objDic.Add('Z', objColor);

		//Foreground ANSI Color
		//Console.Write("\x1b[38;2;255;128;0m"); // RGB foreground (orange)
		//Console.WriteLine("Hello RGB");

		//Console.Write("\x1b[38;2;255;100;0m");
		//Console.WriteLine("Orange text");

		//========================================================================

		//Background ANSI Color
		//Console.Write("\x1b[48;2;0;128;255m");

		//Console.Write("\x1b[48;2;30;30;30m");
		//Console.WriteLine("Dark background");

		//========================================================================

		//Foreground & Background ANSI Color
		//Console.Write("\x1b[38;2;0;200;255;48;2;20;20;20m");
		//Console.WriteLine("Cyan on dark gray");

		//========================================================================

		return objDic;
	}

	//see : DefineColorList
	private static (int r, int g, int b) GetConsoleColorRgb(ConsoleColor color) {
		return color switch {
			ConsoleColor.Black => (12, 12, 12), //this is the REAL console Black. Use PureBlack 'Z' for special effect
			ConsoleColor.DarkBlue => (0, 0, 128),
			ConsoleColor.DarkGreen => (0, 128, 0),
			ConsoleColor.DarkCyan => (0, 128, 128),
			ConsoleColor.DarkRed => (128, 0, 0),
			ConsoleColor.DarkMagenta => (128, 0, 128),
			ConsoleColor.DarkYellow => (128, 128, 0),
			ConsoleColor.Gray => (192, 192, 192),
			ConsoleColor.DarkGray => (128, 128, 128),
			ConsoleColor.Blue => (0, 0, 255),
			ConsoleColor.Green => (0, 255, 0),
			ConsoleColor.Cyan => (0, 255, 255),
			ConsoleColor.Red => (255, 0, 0),
			ConsoleColor.Magenta => (255, 0, 255),
			ConsoleColor.Yellow => (255, 255, 0),
			//ConsoleColor.White => (255, 255, 255),
			_ => (255, 255, 255)
		};
	}

	private static bool TryEnableAnsi() {
		IntPtr handle = PInvokeEx.GetStdHandle(PInvokeEx.STD_OUTPUT_HANDLE); // STD_OUTPUT_HANDLE
		if (handle == IntPtr.Zero) {
			return false;
		}

		if (!PInvokeEx.GetConsoleMode(handle, out int mode)) {
			return false;
		}

		if ((mode & PInvokeEx.ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0) {
			return true;
		}

		return PInvokeEx.SetConsoleMode(handle, mode | PInvokeEx.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
	}

	//Helper for ANSI color
	public readonly struct RgbColorEx {
		public int R {
			get;
		}
		public int G {
			get;
		}
		public int B {
			get;
		}

		public RgbColorEx(ConsoleColor color) {
			(int r, int g, int b) = GetConsoleColorRgb(color);
			R = r;
			G = g;
			B = b;
		}

		public RgbColorEx(int r, int g, int b) {
			R = Clamp(r);
			G = Clamp(g);
			B = Clamp(b);
		}

		public static int Clamp(int value, int MinValue = 0, int MaxValue = 255) {
			if (value < MinValue) {
				return MinValue;
			} else if (value > MaxValue) {
				return MaxValue;
			}

			return value;
		}
	}

	public void SetForegroundColor(ConsoleColor color) {
		(int cr, int cg, int cb) = GetConsoleColorRgb(color);
		SetForegroundColor(cr, cg, cb);
	}

	public void SetForegroundColor((int r, int g, int b) color) {
		SetForegroundColor(color.r, color.g, color.b);
	}

	public void SetForegroundColor(int r, int g, int b) {
		RgbColorEx c = new RgbColorEx(r, g, b);

		if (this._ansiEnabled) {
			this._original.Write($"\x1b[38;2;{c.R};{c.G};{c.B}m");
			this.Flush();
		} else {
			Console.ForegroundColor = ToNearestConsoleColor(c.R, c.G, c.B);
		}

		this.ForegroundColorRgb = (c.R, c.G, c.B);
	}

	public void SetBackgroundColor(ConsoleColor color) {
		(int cr, int cg, int cb) = GetConsoleColorRgb(color);
		SetBackgroundColor(cr, cg, cb);
	}

	public void SetBackgroundColor((int r, int g, int b) color) {
		SetBackgroundColor(color.r, color.g, color.b);
	}

	public void SetBackgroundColor(int r, int g, int b) {
		RgbColorEx c = new RgbColorEx(r, g, b);

		if (this._ansiEnabled) {
			this._original.Write($"\x1b[48;2;{c.R};{c.G};{c.B}m");
			this.Flush();
		} else {
			Console.BackgroundColor = ToNearestConsoleColor(c.R, c.G, c.B);
		}

		this.BackgroundColorRgb = (c.R, c.G, c.B);
	}

	private static ConsoleColor ToNearestConsoleColor(int r, int g, int b) {
		RgbColorEx c = new RgbColorEx(r, g, b);

		ConsoleColor bestColor = ConsoleColor.White;
		double bestDistance = double.MaxValue;

		foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor))) {
			(int cr, int cg, int cb) = GetConsoleColorRgb(color);

			double distance =
				(c.R - cr) * (c.R - cr) +
				(c.G - cg) * (c.G - cg) +
				(c.B - cb) * (c.B - cb);

			if (distance < bestDistance) {
				bestDistance = distance;
				bestColor = color;
			}
		}

		return bestColor;
	}

	public void ClearScreen() {
		this.ResetActiveColor();
		this.ScreenPos.ClearScreen();
	}

	// Original method is ResetColor, I prefer ResetActiveColor because since I can customize color,
	// just ResetColor could mean something else.
	public void ResetActiveColor() {
		this.LastError.LastFunction = "ResetColor";

		if (this._ansiEnabled) {
			//Do not use Console.ForegroundColor when using ANSI
			//Do not use Console.BackgroundColor when using ANSI
			this._original.Write("\x1b[0m");
			this.Flush();
		} else {
			Console.ResetColor();
		}
	}

	//Size is 1-100 but it only work for 1, 50, 100
	public static void ChangeCaret(PrototypeOmega.PInvokeEx.CaretSize penmSize) {
		PInvokeEx.CONSOLE_CURSOR_INFO info;

		uint dwSize = penmSize switch {
            PrototypeOmega.PInvokeEx.CaretSize.Block => 100,
            PrototypeOmega.PInvokeEx.CaretSize.Medium => 50,
			_ => 1
		};
		info.dwSize = dwSize;
		info.bVisible = true;

		IntPtr handle = PInvokeEx.GetStdHandle(PInvokeEx.STD_OUTPUT_HANDLE);
		if (handle == IntPtr.Zero || handle == new IntPtr(-1)) {
			System.Diagnostics.Debug.WriteLine("Invalid console handle");
		}
		PInvokeEx.SetConsoleCursorInfo(handle, ref info);
	}





























	// Helper Class
	public class ScreenPosEx {
		private readonly ConsoleAppEx? _parent = null;

		public int X { get; private set; } = 0;
		public int Y { get; private set; } = 0;

		public int DeltaX { get; set; } = 1;
		public int DeltaY { get; set; } = 1;

		public int SavedX { get; set; } = 0;
		public int SavedY { get; set; } = 0;

		public int Height { get; private set; } = 0;

		[Newtonsoft.Json.JsonConstructor] // Aka [Json Constructor] is optional, Newtonsoft.Json will use private too
		private ScreenPosEx() {
			// This is a parameterless constructor for Json [using Newtonsoft.Json;]
			// You can mark it with [JsonConstructor] before private but it's not mandatory
			// It is made private so user can't use it.
		}
		
		public ScreenPosEx(ConsoleAppEx pobjParent, int plngDeltaX = 1, int plngDeltaY = 1) {
			this.X = 0;
			this.Y = 0;
			this.DeltaX = plngDeltaX;
			this.DeltaY = plngDeltaY;
			this._parent = pobjParent;
		}

		//UpdateX is relative to Monitor position
		public void UpdateX(int plngX) {
			if (ValidateCursorPosition(plngX, Y)) {
				this.X = plngX;
			} else {
				throw new Exception("Overflow UpdateX!");
			}
		}

		//UpdateY is relative to Monitor position
		public void UpdateY(int plngY, bool pblnMoveHeight) {
			if (ValidateCursorPosition(X, plngY)) {
				this.Y = plngY;
				if (this.Y + this.DeltaY > this.Height) {
					if (pblnMoveHeight) {
						this.Height = this.Y + this.DeltaY;
					} else {
						this.Height = this.Y + this.DeltaY + 1;
					}
				}
			} else {
				throw new Exception("Overflow UpdateY!");
			}
		}

		public void UpdateXY(int plngX, int plngY, bool pblnMoveHeight) {
			UpdateX(plngX);
			UpdateY(plngY, pblnMoveHeight);
			this._parent?.SetPosition(plngX, plngY);
		}

		//MoveX is relative to current Cursor position
		public void MoveX(int lngAddX) {
			int lngNewX = this.X + lngAddX;
			if (ValidateCursorPosition(lngNewX, this.Y)) {
				this.X = lngNewX;
			}
		}

		//MoveY is relative to current Cursor position
		public void MoveY(int lngAddY) {
			int lngNewY = this.Y + lngAddY;
			if (ValidateCursorPosition(this.X, lngNewY)) {
				this.Y = lngNewY;
			}
		}

		public void MoveXY(int lngDeltaX, int lngDeltaY) {
			this.MoveX(lngDeltaX);
			this.MoveY(lngDeltaY);
			//this._parent?.SetPosition();
		}

		public void CrLf() {
			this.X = 0;
			this.Y = this.Y + 1;
		}

		public void ClearScreen() {
			Console.Clear();
			this.X = 0;
			this.Y = 0;
			this.Height = 0;
		}

		public bool ValidateCursorPosition(int plngPosX, int plngPosY) {
			bool blnSuccess = false;

			if (plngPosX + this.DeltaX  < Console.BufferWidth) {
				if (this._parent?.DisableScroll == true) {
					if (plngPosY + this.DeltaY < Console.BufferHeight) {
						blnSuccess = true;
					}
				} else {
					blnSuccess = true;
				}
			}

			if (plngPosX + this.DeltaX < 0) {
				blnSuccess = false;
			}

			if (plngPosY + this.DeltaY < 0) {
				blnSuccess = false;
			}

			return blnSuccess;
		}

		//for intellisense debuging only because it was previously a pain to debug without knowing X and Y
		public override string ToString() {
			string strRet = this.X.ToString() + " x " + this.Y.ToString();
			return strRet;
		}
	}

	public class LastErrorEx {
		public LastErrorEx() {
			HexCode = "";
			Message = "";
			LastFunction = "";
		}

		public string HexCode {
			get; set;
		} = "";

		public string Message {
			get; set;
		} = "";

		private string _LastFunction = "";
		public string LastFunction {
			get {
				return _LastFunction;
			}

			set {
				_LastFunction = value;
				Message = "";
				HexCode = "";
			}
		}

		public void Clear() {
			this.HexCode = "";
			this.Message = "";
			this.LastFunction = "";
		}
	}
}

//this is for the event: protected virtual void OnConsoleEvent()
public class ConsoleEventArgs : EventArgs {
	public int EventNo {
		get; set;
	} = 0;

	public string EventData {
		get; set;
	} = "";
}

//Example 1:
//Script script = new Script("Demo");

//// Ajout chaîné via ScriptInstruction
//script.Instruction
//	.Add("MOVE X")
//	.Add("TURN Y")
//	.Add("WAIT 100")
//	.Add("END");

//// Lecture via InstructionsCopy
//foreach (var instr in script.InstructionsCopy)
//{
//	System.Diagnostics.Debug.WriteLine(instr);
//}

#region Child ScriptData
public sealed class ScriptData {
	public string Name {
		get; set;
	} = "";

	private readonly List<string> _Instructions = new List<string>();
	public List<string> Instructions {
		set {
			this._Instructions.Clear();

			if (value == null) {
				return;
			}

			foreach (string data in value) {
				string strPlain = data.Trim();
				if (!string.IsNullOrEmpty(strPlain)) {
					this._Instructions.Add(strPlain);
				}
			}
		}
	}

	//public List<string> InstructionsCopy {
	//	get {
	//		return new List<string>(this._Instructions);
	//	}
	//}

	public ScriptInstruction Instruction {
		get;
	}

	public ScriptData() {
		this.Instruction = new ScriptInstruction(this);
	}

	public ScriptData(string pstrName) {
		this.Name = pstrName;
		this.Instruction = new ScriptInstruction(this);
	}

	public ScriptData(string pstrName, List<string> pstrInstructions) {
		this.Name = pstrName;
		this.Instructions = pstrInstructions;
		this.Instruction = new ScriptInstruction(this);
	}

	public sealed class ScriptInstruction {
		private readonly ScriptData _parent;

		internal ScriptInstruction(ScriptData parent) {
			_parent = parent;
		}

		public ScriptInstruction Add(string pstrInstruction) {
			if (!string.IsNullOrEmpty(pstrInstruction)) {
				_parent._Instructions.Add(pstrInstruction);
			}
			return this;    //permet le chainage .Add().Add();
		}
	}
}
#endregion Child ScriptData

#region Parent ScriptCollection
public sealed class ScriptCollection {
	public Dictionary<string, ScriptData> Scripts { get; set; } = new Dictionary<string, ScriptData>();

	public void Add(ScriptData pstrScript, bool pblnThrow = true) {
		if (pstrScript != null) {
			if (string.IsNullOrEmpty(pstrScript.Name)) {
				pstrScript.Name = "default";
			}

			if (this.Scripts.ContainsKey(pstrScript.Name)) {
				if (pblnThrow) {
					throw new InvalidOperationException($"A Script with the Name [{pstrScript.Name}] already exists");
				}
			} else {
				this.Scripts.Add(pstrScript.Name, pstrScript);
			}
		}
	}
}
#endregion Parent ScriptCollection

// Usage example 1:
//using ConsoleAppEx objConsole = new ConsoleAppEx();
//char sCR = ConsoleAppEx.ColorCodeNewLine;
//char sCF = ConsoleAppEx.ColorCodeForeground;
//char sCB = ConsoleAppEx.ColorCodeBackground;
//string sC1 = sCR + "1";
//
////Get our Tab1 PositionX
//Console.Write($"{sCF}B" + "  \t");
//int lngCurrentX = objConsole.ScreenPos.SetRealX();
//
////Let show our example positioning
//objConsole.SetDPosition(0, 0);
//Console.Write($"{sCF}B0. \t[This Position X]{sC1}");
//
////Let show our Number
//for (int i = 1; i <= 26; i++) {
//    Console.WriteLine($"{sCF}B" + i + ".");
//}
//
////Let show our Colored label
//for (int i = 1; i <= 26; i++) {
//    string strMsg;
//    if (i == 1) {
//        strMsg = $"{sCF}B << FColorA This is Black on Black (invisible)";
//    } else if (i == 26) {
//        strMsg = $"{sCF}B << FColorZ This is PureBlack on Black (look carefully)";
//    } else {
//        strMsg = "";
//    }
//
//    objConsole.SetXPosition(lngCurrentX, i);
//    char chrLetter = (char)(64 + i);
//    Console.WriteLine($"{sCF}{chrLetter}FColor{chrLetter}{strMsg}");
//}
//
//objConsole.ScreenPos.CrLf();
//Console.WriteLine($"You can also change BackColor and ForeColor: {sCB}C {sCF}OBColorC with FColorO {sCB}A");
//objConsole.ResetColor();
//Console.WriteLine("Back to normal color");
//Console.WriteLine("End of DEMO");

// Usage example 2:
//using ConsoleAppEx objConsole = new ConsoleAppEx();
//
//int lngTest = 4;
//string data = "";
//switch (lngTest) {
//    case 0:
//        data = "1        2        3        4        5        6        7        8        9        0        A        B        C        ";
//        data = data + "E=+[+=+=+F        G        H        I        J        K        L        M        N        O        P        Q        ";
//        data = data + "R=+=+=[=+S        T        U        V        W        X        Y        Z";
//        Console.WriteLine(data);
//        break;
//
//    case 1:
//        for (int i = 1; i < 40; i++) {
//            data = "®BLine" + i + "\r\n";
//            Console.Write(data);
//        }
//
//        objConsole.SetXPosition(10, 0);
//        Console.WriteLine("Test1");
//
//        objConsole.SetXPosition(10, 38);
//        Console.WriteLine("Test2");
//        break;
//
//    case 2:
//        for (int i = 1; i < 40; i++) {
//            data = "®BLine" + i + "\r\n";
//            Console.Write(data);
//        }
//
//        objConsole.SetXPosition(10, 0);
//        Console.WriteLine("Test1");
//        break;
//
//    case 3:
//        data = "[" + new string('*', Console.BufferWidth -2) + "]";
//        Console.WriteLine(data);
//        Console.WriteLine(objConsole.ScreenPos.X + ", " + objConsole.ScreenPos.Y);
//        break;
//
//    case 4:
//        objConsole.SetDelta(10,10);
//
//        objConsole.ShowField("Hello", 10);
//        break;
//}

//We could add a fonction call ShowColorChart()
//Show All Color
//Console.WriteLine();
//for (int i = 0; i < 26; i++) {
//	char chrLetter = (char)(65 + i);
//	string strColor = $"{sCF}" + chrLetter;
//	Console.WriteLine($"{sCFA}[{chrLetter}] : {strColor}THIS IS COLORED");
//}