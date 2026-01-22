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
//      Usage: PInvoke API32 definition (and Vb6 subclassing)                                       //
// Dependency:                                                                                      //
//**************************************************************************************************//
// v1.00 - 2026-01-16:	** INITIAL RELEASE **;
// v1.01 - 2026-01-19:	Add options for Caret modification;

using System.Runtime.InteropServices;

#region PrototypeOmega namespace
#pragma warning disable IDE0130
namespace PrototypeOmega;
#pragma warning restore IDE0130
#endregion PrototypeOmega namespace

public static class PInvokeEx {
	public const int STD_INPUT_HANDLE = -10;
	public const int STD_OUTPUT_HANDLE = -11;
	public const int ENABLE_MOUSE_INPUT = 0x0010;
	public const int ENABLE_WINDOW_INPUT = 0x0008;
	public const int ENABLE_EXTENDED_FLAGS = 0x0080;
	public const int ENABLE_PROCESSED_INPUT = 0x0001;
	public const int ENABLE_LINE_INPUT = 0x0002;
	public const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

	[DllImport("kernel32.dll")]
	public static extern IntPtr GetStdHandle(int nStdHandle);
	
	[DllImport("kernel32.dll")]
	public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

	[DllImport("kernel32.dll")]
	public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);


	//byte[] buffer = new byte[20];
	[DllImport("kernel32.dll")]
	public static extern bool ReadConsoleInput(
		IntPtr hConsoleInput,
		[Out] byte[] lpBuffer,
		uint nLength,
		out uint lpNumberOfEventsRead
	);

	[DllImport("kernel32.dll")]
	public static extern short GetAsyncKeyState(int vKey);

	[StructLayout(LayoutKind.Explicit)]
	public struct INPUT_MOUSE_Type {
		[FieldOffset(0)]
		//public short EventType;
		public WindowsEventTypeEnum EventType;      //(short)2 + 2 padding

		[FieldOffset(4)]
		public MOUSE_EVENT_Type MouseEvent;   //16
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct INPUT_KEYBOARD_Type {
		[FieldOffset(0)]
		//public short EventType;
		public WindowsEventTypeEnum EventType;      //(short)2 + 2 padding

		[FieldOffset(4)]
		public KEYB_EVENT_Type KeybEvent;   //16
	}

	//[StructLayout(LayoutKind.Sequential, Pack = 1)]
	//struct INPUT_RECORD_RAW {
	//	public WindowsEventType EventType;						//(short)2
	//	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
	//	public byte[] Data;										//2 + 16
	//}

	//https://learn.microsoft.com/en-us/windows/console/key-event-record-str
	[StructLayout(LayoutKind.Sequential)]
	public struct KEYB_EVENT_Type {
		public bool bKeyDown;        // TRUE si touche pressée, FALSE si relâchée
		public short wRepeatCount;
		public short wVirtualKeyCode;
		public short wVirtualScanCode;
		public char UnicodeChar;
		public int dwControlKeyState;
	}

	//https://learn.microsoft.com/en-us/windows/console/mouse-event-record-str
	[StructLayout(LayoutKind.Sequential)]
	public struct MOUSE_EVENT_Type {
		public COORD_Type dwMousePosition;
		public int dwButtonState;
		public int dwControlKeyState;
		public int dwEventFlags;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct COORD_Type {
		public short X;
		public short Y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct FOCUS_EVENT_Type {
		public bool bSetFocus;
	}

	public static class EventMouseClickClass {
		public const int L = 0x0001;
		public const int R = 0x0002;
		public const int M = 0x0004;
	}

	public const int KEY_EVENT = 0x0001;
	public const int MOUSE_EVENT = 0x0002;
	public const int MOUSE_MOVED = 0x0001;
	
	//public const int DOUBLE_CLICK = 0x0002;

	public enum WindowsEventTypeEnum : short {
		KEY_EVENT = 0x0001,
		MOUSE_EVENT = 0x0002,
		WINDOW_BUFFER_SIZE_EVENT = 0x0004,
		MENU_EVENT = 0x0008,
		FOCUS_EVENT = 0x0010,
		UNKNOWN_EVENT = 0x0020
	}

	public static T BytesToStruct<T>(byte[] bytes) where T : struct {
		GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		try {
			IntPtr ptr = handle.AddrOfPinnedObject();
			return Marshal.PtrToStructure<T>(ptr);
		} finally {
			handle.Free();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CONSOLE_CURSOR_INFO {
		public uint dwSize;     // 1–100 (% of cell height)
		[MarshalAs(UnmanagedType.Bool)]
		public bool bVisible;
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	public  static extern bool SetConsoleCursorInfo(
		IntPtr hConsoleOutput,
		ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo
	);

	public enum CaretSize {
		Thin,    // dwSize = 1
		Medium,  // dwSize = 50
		Block    // dwSize = 100
	}

	//typedef struct _INPUT_RECORD {
	//  WORD  EventType;
	//  union {
	//    KEY_EVENT_RECORD    KeyEvent;
	//    MOUSE_EVENT_RECORD  MouseEvent;
	//    WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
	//    MENU_EVENT_RECORD   MenuEvent;
	//    FOCUS_EVENT_RECORD  FocusEvent;
	//  } Event;
	//} INPUT_RECORD;
}