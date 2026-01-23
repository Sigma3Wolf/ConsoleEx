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
// Dependency: vcxAppExConsole_v2.02.cs                                                             //
//**************************************************************************************************//
// v2.00 - 2026-01-16:	** INITIAL RELEASE **;
// v2.01 - 2026-01-16:	Cosmetic;
// v2.02 - 2026-01-18:	Removing parameterless SetPosition();
//						Removing Console.CursorTop and Console.CursorLeft reference (ANSI issue);
// v2.03 - 2026-01-19:	Cosmetic;

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

public partial class ConsoleAppEx : TextWriter {
	private static bool IsKeyDown(ConsoleKey key) {
		return (PInvokeEx.GetAsyncKeyState((int)key) & 0x8000) != 0;
	}

	public void WaitForEvent(string pstrFormName) {
		if (this._FormEx != null) {
			List<string> lstDesign = this._FormEx.DesignGet(pstrFormName, 2);

			EnableMouse();
			byte[] buffer = new byte[20];

			if (_InputHandle != IntPtr.Zero) {
				//string strMsg = "";
				int lngEventCount = 0;
				while (ConsoleAppEx._RunningProgram) {
					PInvokeEx.ReadConsoleInput(_InputHandle, buffer, 1, out uint read);
					short eventType = BitConverter.ToInt16(buffer, 0);

					if (eventType == PInvokeEx.MOUSE_EVENT) {
						PInvokeEx.INPUT_MOUSE_Type rec = PInvokeEx.BytesToStruct<PInvokeEx.INPUT_MOUSE_Type>(buffer);
						int X = rec.MouseEvent.dwMousePosition.X - this.ScreenPos.DeltaX - this._FormEx.FormData.StartX;
						int Y = rec.MouseEvent.dwMousePosition.Y - this.ScreenPos.DeltaY - this._FormEx.FormData.StartY;

						//this.ScreenPos.UpdateXY(0, 0, false);
						if (rec.MouseEvent.dwEventFlags == PInvokeEx.MOUSE_MOVED) {
							//Mouse HOVER !!!!
							//b13
							//WHY THIS DOESN'T WORK ???
							//this.SetCursorPosition(0, 0);

							//this._original.Write("odaio");
							//this.Write("odaio");

							
						} else if (rec.MouseEvent.dwEventFlags == 0) {
							// Button Up/Dn
							lngEventCount++;

							//if (lngReferenceTick == 0) {
							if (rec.MouseEvent.dwButtonState == 1) {
								//MouseDown
							} else {
								//MouseUp
								//Y[3] : 1 @ 5
								//Y[3] : 13 @ 17

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

								//string strField = $"({X:D2}x{Y:D2})";
								PrototypeOmega.ConsoleAppEx.FormEx.FormField objButton = new PrototypeOmega.ConsoleAppEx.FormEx.FormField {
									FormName = pstrFormName.ToUpper(),
									PosX = X,
									PosY = Y
								};

								string strFieldKeySha = this._FormEx.FieldSearch(ref objButton);  //will complete [objButton] data
								if (objButton.Length > 0) {
									if (objButton.FieldType == 0) {
										this.FlashButtonOn(objButton);

										// Call the method defined in Theme.cs
										OnConsoleEvent(1, strFieldKeySha);
									} else {
										//this is debug
										//this.SetPosition(0, 0);
										//this.Write("                                                             ");

										//This is a COPY/PASTE field
										//User must handle theses we send the event
										OnConsoleEvent(2, strFieldKeySha);
									}
								} else {
									//this is debug
									//this.SetPosition(0, 0);
									//this.Write("                                                             ");
								}
								//this._FieldPos.Add(strField, );
							}
						}
						//}
					} else if (eventType == PInvokeEx.KEY_EVENT) {
						//this.ScreenPos.UpdateXY(0, 0, false);
						PInvokeEx.INPUT_KEYBOARD_Type rec = PInvokeEx.BytesToStruct<PInvokeEx.INPUT_KEYBOARD_Type>(buffer);
						if (rec.KeybEvent.bKeyDown) {
							//This is a KeyDn
							//this.Write($"DN {rec.KeybEvent.bKeyDown} {rec.KeybEvent.wVirtualKeyCode} {rec.KeybEvent.UnicodeChar} {rec.KeybEvent.dwControlKeyState}                               ");
						} else {
							//This is a KeyUp

							//rec.KeybEvent.dwControlKeyState seem unreliable
							//rec.KeybEvent.UnicodeChar seem very good
							//this.SetPosition(0, 0);
							//this.Write($"                                                                                      ");
							//this.SetPosition(0, 0);
							//this.Write($"UP {rec.KeybEvent.bKeyDown} {rec.KeybEvent.wVirtualKeyCode} >[{rec.KeybEvent.UnicodeChar}]< [{rec.KeybEvent.dwControlKeyState}]                               ");

							char chrChar = rec.KeybEvent.UnicodeChar;
							string strChar = chrChar.ToString();
							if (char.IsLetter(strChar[0])) {
								OnConsoleEvent(3, strChar);
							} else if (char.IsDigit(strChar[0])) {
								OnConsoleEvent(4, strChar);
							}
						}

						//https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
						// see vcxPInvoke:
						// short wRepeatCount;
						// short wVirtualKeyCode;
						if (rec.KeybEvent.wVirtualKeyCode == 27) {
							break;
						}
					}
				}
			}
		}
	}

	private void FlashButtonOn(PrototypeOmega.ConsoleAppEx.FormEx.FormField pobjButton) {
		int lngOldX = this.ScreenPos.X;
		int lngOldY = this.ScreenPos.Y;
		int lngNewX = pobjButton.StartX;
		int lngNewY = pobjButton.PosY;
		string strLabel = this._FormEx.ExtractButton(lngNewX, lngNewY, pobjButton.Length);
		PrototypeOmega.ConsoleAppEx.FormEx.FormField objButton = pobjButton with {
			PosX = lngOldX,
			PosY = lngOldY,
			StartX = lngNewX,
			EndX = lngNewY,      //NOT AN ERROR, it's only used in FlashButtonOff()
			Label = strLabel
		};

		this.ScreenPos.UpdateXY(lngNewX + this._FormEx.FormData.StartX, lngNewY + this._FormEx.FormData.StartY, false);
		this.Write($"{sCB}N{strLabel}");
		this.ScreenPos.UpdateXY(lngOldX, lngOldY, false);

		Thread.Sleep(100);
		this.FlashButtonOff(objButton);
	}

	private long FlashButtonOff(PrototypeOmega.ConsoleAppEx.FormEx.FormField pobjButton) {
		int lngOldX = pobjButton.PosX;
		int lngOldY = pobjButton.PosY;
		int lngNewX = pobjButton.StartX;
		int lngNewY = pobjButton.EndX;
		string strLabel = pobjButton.Label;

		this.ScreenPos.UpdateXY(lngNewX + this._FormEx.FormData.StartX, lngNewY + this._FormEx.FormData.StartY, false);
		this.Write($"{sCB}B{strLabel}");
		this.ScreenPos.UpdateXY(lngOldX, lngOldY, false);

		return 0;
	}

	public static void EnableMouse() {
		if (_InputHandle == IntPtr.Zero) {
			_InputHandle = PInvokeEx.GetStdHandle(PInvokeEx.STD_INPUT_HANDLE);
			PInvokeEx.GetConsoleMode(_InputHandle, out int mode);

			mode &= ~0x0040; // désactiver QUICK_EDIT_MODE
			mode |= PInvokeEx.ENABLE_MOUSE_INPUT;
			mode |= PInvokeEx.ENABLE_EXTENDED_FLAGS;
			PInvokeEx.SetConsoleMode(_InputHandle, mode);
		}
	}
	//ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT // optional
	//ENABLE_PROCESSED_INPUT | ENABLE_LINE_INPUT | ENABLE_ECHO_INPUT
}
