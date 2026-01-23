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
// Dependency:                                                                                      //
//**************************************************************************************************//
// v2.04 - 2026-01-23:	** INITIAL RELEASE **;

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

internal class FieldEx {
    //private static char sCR;  //NewLine
    //private static char sCF;  //Foreground
    //private static char sCB;  //Background

    public FieldEx() {
        //sCR = PrototypeOmega.ConsoleAppEx.sCR;
        //sCF = PrototypeOmega.ConsoleAppEx.sCF;
        //sCB = PrototypeOmega.ConsoleAppEx.sCB;
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