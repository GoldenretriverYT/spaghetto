using Cosmos.System;
using System.Collections.Generic;

namespace EtorumOS.Utils.KeyboardLayouts
{
    public class DE_Fixed : ScanMapBase
    {
        //
        // Zusammenfassung:
        //     Create new instance of the Cosmos.System.ScanMaps.DE_Fixed class.
        public DE_Fixed()
        {
        }

        //
        // Zusammenfassung:
        //     Init key list.
        protected override void InitKeys()
        {
            Keys = new List<KeyMapping>(101);
            Keys.Add(new KeyMapping(0, ConsoleKeyEx.NoName));
            Keys.Add(new KeyMapping(1, ConsoleKeyEx.Escape));
            Keys.Add(new KeyMapping(0x29, '^', '°', '^', '^', '°', '^', ConsoleKeyEx.Backquote));
            Keys.Add(new KeyMapping(0x2, '1', '!', '1', '1', '!', '1', ConsoleKeyEx.D1));
            Keys.Add(new KeyMapping(0x3, '2', '"', '2', '2', '"', '2', '²', ConsoleKeyEx.D2));
            Keys.Add(new KeyMapping(0x4, '3', '§', '3', '3', '§', '3', '³', ConsoleKeyEx.D3));
            Keys.Add(new KeyMapping(0x5, '4', '$', '4', '4', '$', '4', '£', '£', ConsoleKeyEx.D4));
            Keys.Add(new KeyMapping(0x6, '5', '%', '5', '5', '%', '5', ConsoleKeyEx.D5));
            Keys.Add(new KeyMapping(0x7, '6', '&', '6', '6', '&', '6', ConsoleKeyEx.D6));
            Keys.Add(new KeyMapping(0x8, '7', '/', '7', '7', '/', '7', '{', ConsoleKeyEx.D7));
            Keys.Add(new KeyMapping(0x9, '8', '(', '8', '8', '(', '8', '[', ConsoleKeyEx.D8));
            Keys.Add(new KeyMapping(0xA, '9', ')', '9', '9', ')', '9', ']', ConsoleKeyEx.D9));
            Keys.Add(new KeyMapping(0xB, '0', '=', '0', '0', '=', '0', '}', ConsoleKeyEx.D0));
            Keys.Add(new KeyMapping(12, 'ß', '?', 'ß', 'ß', '?', 'ß', '\\', ConsoleKeyEx.Minus));
            Keys.Add(new KeyMapping(13, '\u00b4', '`', '\u00b4', '\u00b4', '`', '\u00b4', ConsoleKeyEx.Equal));
            Keys.Add(new KeyMapping(14, ConsoleKeyEx.Backspace));
            Keys.Add(new KeyMapping(15, ConsoleKeyEx.Tab));
            Keys.Add(new KeyMapping(16, 'q', 'Q', 'q', 'Q', 'q', 'Q', '@', ConsoleKeyEx.Q));
            Keys.Add(new KeyMapping(17, 'w', 'W', 'w', 'W', 'w', 'W', ConsoleKeyEx.W));
            Keys.Add(new KeyMapping(18, 'e', 'E', 'e', 'E', 'e', 'E', '€', ConsoleKeyEx.E));
            Keys.Add(new KeyMapping(19, 'r', 'R', 'r', 'R', 'r', 'R', ConsoleKeyEx.R));
            Keys.Add(new KeyMapping(20, 't', 'T', 't', 'T', 't', 'T', ConsoleKeyEx.T));
            Keys.Add(new KeyMapping(21, 'z', 'Z', 'z', 'Z', 'z', 'Z', ConsoleKeyEx.Y));
            Keys.Add(new KeyMapping(22, 'u', 'U', 'u', 'U', 'u', 'U', ConsoleKeyEx.U));
            Keys.Add(new KeyMapping(23, 'i', 'I', 'i', 'I', 'i', 'I', ConsoleKeyEx.I));
            Keys.Add(new KeyMapping(24, 'o', 'O', 'o', 'O', 'o', 'O', ConsoleKeyEx.O));
            Keys.Add(new KeyMapping(25, 'p', 'P', 'p', 'P', 'p', 'P', ConsoleKeyEx.P));
            Keys.Add(new KeyMapping(26, 'ü', 'Ü', 'ü', 'Ü', 'ü', 'Ü', ConsoleKeyEx.LBracket));
            Keys.Add(new KeyMapping(0x1b, '+', '*', '+', '*', '+', '*', '~', ConsoleKeyEx.RBracket));
            Keys.Add(new KeyMapping(0x1c, ConsoleKeyEx.Enter));
            Keys.Add(new KeyMapping(29, ConsoleKeyEx.LCtrl));
            Keys.Add(new KeyMapping(30, 'a', 'A', 'a', 'A', 'a', 'A', ConsoleKeyEx.A));
            Keys.Add(new KeyMapping(31, 's', 'S', 's', 'S', 's', 'S', ConsoleKeyEx.S));
            Keys.Add(new KeyMapping(32, 'd', 'D', 'd', 'D', 'd', 'D', ConsoleKeyEx.D));
            Keys.Add(new KeyMapping(33, 'f', 'F', 'f', 'F', 'f', 'F', ConsoleKeyEx.F));
            Keys.Add(new KeyMapping(34, 'g', 'G', 'g', 'G', 'g', 'G', ConsoleKeyEx.G));
            Keys.Add(new KeyMapping(35, 'h', 'H', 'h', 'H', 'h', 'H', ConsoleKeyEx.H));
            Keys.Add(new KeyMapping(36, 'j', 'J', 'j', 'J', 'j', 'J', ConsoleKeyEx.J));
            Keys.Add(new KeyMapping(37, 'k', 'K', 'k', 'K', 'k', 'K', ConsoleKeyEx.K));
            Keys.Add(new KeyMapping(38, 'l', 'L', 'l', 'L', 'l', 'L', ConsoleKeyEx.L));
            Keys.Add(new KeyMapping(39, 'ö', 'Ö', 'ö', 'Ö', 'ö', 'Ö', ConsoleKeyEx.Semicolon));
            Keys.Add(new KeyMapping(40, 'ä', 'Ä', 'ä', 'Ä', 'ä', 'Ä', ConsoleKeyEx.Apostrophe));
            Keys.Add(new KeyMapping(42, ConsoleKeyEx.LShift));
            Keys.Add(new KeyMapping(43, '#', '\'', '#', '\'', '#', '\'', ConsoleKeyEx.OEM102));
            Keys.Add(new KeyMapping(44, 'y', 'Y', 'y', 'Y', 'y', 'Y', ConsoleKeyEx.Z));
            Keys.Add(new KeyMapping(45, 'x', 'X', 'x', 'X', 'x', 'X', ConsoleKeyEx.X));
            Keys.Add(new KeyMapping(46, 'c', 'C', 'c', 'C', 'c', 'C', ConsoleKeyEx.C));
            Keys.Add(new KeyMapping(47, 'v', 'V', 'v', 'V', 'v', 'V', ConsoleKeyEx.V));
            Keys.Add(new KeyMapping(48, 'b', 'B', 'b', 'B', 'b', 'B', ConsoleKeyEx.B));
            Keys.Add(new KeyMapping(49, 'n', 'N', 'n', 'N', 'n', 'N', ConsoleKeyEx.N));
            Keys.Add(new KeyMapping(50, 'm', 'M', 'm', 'M', 'm', 'M', 'µ', ConsoleKeyEx.M));
            Keys.Add(new KeyMapping(51, ',', ';', ',', ';', ',', ';', ConsoleKeyEx.Comma));
            Keys.Add(new KeyMapping(52, '.', ':', '.', ':', '.', ':', ConsoleKeyEx.Period));
            Keys.Add(new KeyMapping(53, '-', '_', '-', '_', '-', '_', ConsoleKeyEx.Slash));
            Keys.Add(new KeyMapping(54, ConsoleKeyEx.RShift));
            Keys.Add(new KeyMapping(55, '*', '*', '*', '*', '*', '*', ConsoleKeyEx.NumMultiply));
            Keys.Add(new KeyMapping(56, ConsoleKeyEx.LAlt));
            Keys.Add(new KeyMapping(57, ' ', ConsoleKeyEx.Spacebar));
            Keys.Add(new KeyMapping(58, ConsoleKeyEx.CapsLock));
            Keys.Add(new KeyMapping(59, ConsoleKeyEx.F1));
            Keys.Add(new KeyMapping(60, ConsoleKeyEx.F2));
            Keys.Add(new KeyMapping(61, ConsoleKeyEx.F3));
            Keys.Add(new KeyMapping(62, ConsoleKeyEx.F4));
            Keys.Add(new KeyMapping(63, ConsoleKeyEx.F5));
            Keys.Add(new KeyMapping(64, ConsoleKeyEx.F6));
            Keys.Add(new KeyMapping(65, ConsoleKeyEx.F7));
            Keys.Add(new KeyMapping(66, ConsoleKeyEx.F8));
            Keys.Add(new KeyMapping(67, ConsoleKeyEx.F9));
            Keys.Add(new KeyMapping(68, ConsoleKeyEx.F10));
            Keys.Add(new KeyMapping(87, ConsoleKeyEx.F11));
            Keys.Add(new KeyMapping(88, ConsoleKeyEx.F12));
            Keys.Add(new KeyMapping(69, ConsoleKeyEx.NumLock));
            Keys.Add(new KeyMapping(70, ConsoleKeyEx.ScrollLock));
            Keys.Add(new KeyMapping(74, '-', '-', '-', '-', '-', '-', ConsoleKeyEx.NumMinus));
            Keys.Add(new KeyMapping(78, '+', '+', '+', '+', '+', '+', ConsoleKeyEx.NumPlus));
            Keys.Add(new KeyMapping(71, '\0', '\0', '7', '\0', '\0', '\0', ConsoleKeyEx.Home, ConsoleKeyEx.Num7));
            Keys.Add(new KeyMapping(72, '\0', '\0', '8', '\0', '\0', '\0', ConsoleKeyEx.UpArrow, ConsoleKeyEx.Num8));
            Keys.Add(new KeyMapping(73, '\0', '\0', '9', '\0', '\0', '\0', ConsoleKeyEx.PageUp, ConsoleKeyEx.Num9));
            Keys.Add(new KeyMapping(75, '\0', '\0', '4', '\0', '\0', '\0', ConsoleKeyEx.LeftArrow, ConsoleKeyEx.Num4));
            Keys.Add(new KeyMapping(76, '\0', '\0', '5', '\0', '\0', '\0', ConsoleKeyEx.Num5));
            Keys.Add(new KeyMapping(77, '\0', '\0', '6', '\0', '\0', '\0', ConsoleKeyEx.RightArrow, ConsoleKeyEx.Num6));
            Keys.Add(new KeyMapping(79, '\0', '\0', '1', '\0', '\0', '\0', ConsoleKeyEx.End, ConsoleKeyEx.Num1));
            Keys.Add(new KeyMapping(80, '\0', '\0', '2', '\0', '\0', '\0', ConsoleKeyEx.DownArrow, ConsoleKeyEx.Num2));
            Keys.Add(new KeyMapping(81, '\0', '\0', '3', '\0', '\0', '\0', ConsoleKeyEx.PageDown, ConsoleKeyEx.Num3));
            Keys.Add(new KeyMapping(82, '\0', '\0', '0', '\0', '\0', '\0', ConsoleKeyEx.Insert, ConsoleKeyEx.Num0));
            Keys.Add(new KeyMapping(83, '\b', '\b', ',', '\b', '\b', '\b', ConsoleKeyEx.Delete, ConsoleKeyEx.NumPeriod));
            Keys.Add(new KeyMapping(86, '<', '>', '<', '<', '>', '>', '|', ConsoleKeyEx.OEM102));
            Keys.Add(new KeyMapping(91, ConsoleKeyEx.LWin));
            Keys.Add(new KeyMapping(92, ConsoleKeyEx.RWin));
        }
    }
}
