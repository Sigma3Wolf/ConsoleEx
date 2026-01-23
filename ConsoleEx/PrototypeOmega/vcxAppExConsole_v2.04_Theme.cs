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
//             vcxAppExConsole_v2.04.cs                                                             //
//**************************************************************************************************//
// v2.00 - 2026-01-14:	** INITIAL RELEASE **;
// v2.01 - 2026-01-16:	Cosmetic;
// v2.02 - 2026-01-18:	Removing parameterless SetPosition();
//						Removing Console.CursorTop and Console.CursorLeft reference (ANSI issue);
// v2.03 - 2026-01-19:	Implementing List<Form>;
// v2.04 - 2026-01-23:	Preparing structure for multiple form;

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

public partial class ConsoleAppEx : TextWriter {
	private FormEx _FormEx = null;

    //change name to something like Theme.AddFormBegin
    public void FormBegin(string pstrFormName, int plngStartX, int plngStartY) {
        string strFormName = FormEx.ToPascalCase(pstrFormName);
        if (string.IsNullOrEmpty(strFormName)) {
            throw new Exception("FormName must be set");
        }

        this._FormEx = new FormEx(this, strFormName);
        this._FormEx.FormData.StartX = plngStartX;
        this._FormEx.FormData.StartY = plngStartY;
    }

    //change name to something like Theme.AddFormEnd
    public void FormEnd(bool pblnShowMessage = true) {
        if (this._FormEx != null) {
            if (pblnShowMessage) {
                this.SetPosition(0, 0);
                Console.Write("Loading necessary modules...");
                this._FormEx.FormConvert();
                this.SetPosition(0, 0);
                Console.Write("                            ");
            } else {
                this._FormEx.FormConvert();
            }
        }
    }

    //change name to something like Theme.AddFormLine
    public void FormAddLine(string pstrLine) {
        if (this._FormEx != null) {
            //Add ending CR2
            //string sCR2 = sCR + "2";
            //if (!pstrLine.EndsWith(sCR2)) {
            //	pstrLine= pstrLine + sCR2;
            //}
            this._FormEx.FormAddLine(pstrLine, sCR, sCF, sCB);
        }
    }

    public void FormShow(int plngMoveToX = 0, int plngMoveToY = 0, int plngDebugOnly = 0) {
        //DesignGet(plngDesign)
        // 0: this.FormData._lstDesign
        // 1: this.FormData._lstDesignFlat
        // 2: this.FormData._lstDesignField
        if (this._FormEx != null) {
            if (ScreenPos.ValidateCursorPosition(plngMoveToX, plngMoveToY)) {
                string sCR2 = sCR + "2";
                ResetActiveColor();

                //First we save current position
                this.ScreenPos.SavedX = this._FormEx.FormData.StartX;
                this.ScreenPos.SavedY = this._FormEx.FormData.StartY;
                this.SetPosition(this.ScreenPos.SavedX, this.ScreenPos.SavedY);

                List<string> lstDesign = this._FormEx.DesignGet(this._FormEx.FormData.FormName, plngDebugOnly);
                for (int i = 0; i < lstDesign.Count; i++) {
                    //Debug: Console.WriteLine("//" + lstDesign[i]);
                    string strData = lstDesign[i];
                    //System.Diagnostics.Debug.WriteLine($"[{strData}]");
                    Console.Write(strData + $"{sCR2}");
                }
                this.SetPosition(this._FormEx.FormData.StartX + plngMoveToX, this._FormEx.FormData.StartY + plngMoveToY);
            }
        }
    }

    public FormEx GetActiveForm() {
        return this._FormEx;
    }

    public void SetActiveForm(FormEx pobjForm) {
        if (pobjForm != null) {
            this.FormBegin(pobjForm.FormData.FormName, pobjForm.FormData.StartX, pobjForm.FormData.StartY);
            if (this._FormEx != null) {
                List<string> objDesign = pobjForm.FormData.Design;

                for (int i = 0; i < objDesign.Count; i++) {
                    string strLine = objDesign[i];
                    this._FormEx.FormAddLine(strLine, sCR, sCF, sCB);
                }
                this.FormEnd();
            }
        }
    }
}

#region Parent FormCollection
public sealed class FormCollection {
	// It's [public] because [SettingsJson.Save] need to be able to save theses but you should use only the provided method
	public Dictionary<string, FormEx> Forms { get; set; } = new Dictionary<string, FormEx>();

	public void Add(FormEx pobjForm, bool pblnThrow = true) {
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