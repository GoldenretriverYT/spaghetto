using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _keys = new List<KeyMapping>(101);
            _keys.Add(new KeyMapping(0, ConsoleKeyEx.NoName));
            _keys.Add(new KeyMapping(1, ConsoleKeyEx.Escape));
            _keys.Add(new KeyMapping(0x29, '^', '°', '^', '^', '°', '^', ConsoleKeyEx.Backquote));
            _keys.Add(new KeyMapping(0x2, '1', '!', '1', '1', '!', '1', ConsoleKeyEx.D1));
            _keys.Add(new KeyMapping(0x3, '2', '"', '2', '2', '"', '2', '²', ConsoleKeyEx.D2));
            _keys.Add(new KeyMapping(0x4, '3', '§', '3', '3', '§', '3', '³', ConsoleKeyEx.D3));
            _keys.Add(new KeyMapping(0x5, '4', '$', '4', '4', '$', '4', '£', '£', ConsoleKeyEx.D4));
            _keys.Add(new KeyMapping(0x6, '5', '%', '5', '5', '%', '5', ConsoleKeyEx.D5));
            _keys.Add(new KeyMapping(0x7, '6', '&', '6', '6', '&', '6', ConsoleKeyEx.D6));
            _keys.Add(new KeyMapping(0x8, '7', '/', '7', '7', '/', '7', '{', ConsoleKeyEx.D7));
            _keys.Add(new KeyMapping(0x9, '8', '(', '8', '8', '(', '8', '[', ConsoleKeyEx.D8));
            _keys.Add(new KeyMapping(0xA, '9', ')', '9', '9', ')', '9', ']', ConsoleKeyEx.D9));
            _keys.Add(new KeyMapping(0xB, '0', '=', '0', '0', '=', '0', '}', ConsoleKeyEx.D0));
            _keys.Add(new KeyMapping(12, 'ß', '?', 'ß', 'ß', '?', 'ß', '\\', ConsoleKeyEx.Minus));
            _keys.Add(new KeyMapping(13, '\u00b4', '`', '\u00b4', '\u00b4', '`', '\u00b4', ConsoleKeyEx.Equal));
            _keys.Add(new KeyMapping(14, ConsoleKeyEx.Backspace));
            _keys.Add(new KeyMapping(15, ConsoleKeyEx.Tab));
            _keys.Add(new KeyMapping(16, 'q', 'Q', 'q', 'Q', 'q', 'Q', '@', ConsoleKeyEx.Q));
            _keys.Add(new KeyMapping(17, 'w', 'W', 'w', 'W', 'w', 'W', ConsoleKeyEx.W));
            _keys.Add(new KeyMapping(18, 'e', 'E', 'e', 'E', 'e', 'E', '€', ConsoleKeyEx.E));
            _keys.Add(new KeyMapping(19, 'r', 'R', 'r', 'R', 'r', 'R', ConsoleKeyEx.R));
            _keys.Add(new KeyMapping(20, 't', 'T', 't', 'T', 't', 'T', ConsoleKeyEx.T));
            _keys.Add(new KeyMapping(21, 'z', 'Z', 'z', 'Z', 'z', 'Z', ConsoleKeyEx.Y));
            _keys.Add(new KeyMapping(22, 'u', 'U', 'u', 'U', 'u', 'U', ConsoleKeyEx.U));
            _keys.Add(new KeyMapping(23, 'i', 'I', 'i', 'I', 'i', 'I', ConsoleKeyEx.I));
            _keys.Add(new KeyMapping(24, 'o', 'O', 'o', 'O', 'o', 'O', ConsoleKeyEx.O));
            _keys.Add(new KeyMapping(25, 'p', 'P', 'p', 'P', 'p', 'P', ConsoleKeyEx.P));
            _keys.Add(new KeyMapping(26, 'ü', 'Ü', 'ü', 'Ü', 'ü', 'Ü', ConsoleKeyEx.LBracket));
            _keys.Add(new KeyMapping(0x1b, '+', '*', '+', '*', '+', '*', '~', ConsoleKeyEx.RBracket));
            _keys.Add(new KeyMapping(0x1c, ConsoleKeyEx.Enter));
            _keys.Add(new KeyMapping(29, ConsoleKeyEx.LCtrl));
            _keys.Add(new KeyMapping(30, 'a', 'A', 'a', 'A', 'a', 'A', ConsoleKeyEx.A));
            _keys.Add(new KeyMapping(31, 's', 'S', 's', 'S', 's', 'S', ConsoleKeyEx.S));
            _keys.Add(new KeyMapping(32, 'd', 'D', 'd', 'D', 'd', 'D', ConsoleKeyEx.D));
            _keys.Add(new KeyMapping(33, 'f', 'F', 'f', 'F', 'f', 'F', ConsoleKeyEx.F));
            _keys.Add(new KeyMapping(34, 'g', 'G', 'g', 'G', 'g', 'G', ConsoleKeyEx.G));
            _keys.Add(new KeyMapping(35, 'h', 'H', 'h', 'H', 'h', 'H', ConsoleKeyEx.H));
            _keys.Add(new KeyMapping(36, 'j', 'J', 'j', 'J', 'j', 'J', ConsoleKeyEx.J));
            _keys.Add(new KeyMapping(37, 'k', 'K', 'k', 'K', 'k', 'K', ConsoleKeyEx.K));
            _keys.Add(new KeyMapping(38, 'l', 'L', 'l', 'L', 'l', 'L', ConsoleKeyEx.L));
            _keys.Add(new KeyMapping(39, 'ö', 'Ö', 'ö', 'Ö', 'ö', 'Ö', ConsoleKeyEx.Semicolon));
            _keys.Add(new KeyMapping(40, 'ä', 'Ä', 'ä', 'Ä', 'ä', 'Ä', ConsoleKeyEx.Apostrophe));
            _keys.Add(new KeyMapping(42, ConsoleKeyEx.LShift));
            _keys.Add(new KeyMapping(43, '#', '\'', '#', '\'', '#', '\'', ConsoleKeyEx.OEM102));
            _keys.Add(new KeyMapping(44, 'y', 'Y', 'y', 'Y', 'y', 'Y', ConsoleKeyEx.Z));
            _keys.Add(new KeyMapping(45, 'x', 'X', 'x', 'X', 'x', 'X', ConsoleKeyEx.X));
            _keys.Add(new KeyMapping(46, 'c', 'C', 'c', 'C', 'c', 'C', ConsoleKeyEx.C));
            _keys.Add(new KeyMapping(47, 'v', 'V', 'v', 'V', 'v', 'V', ConsoleKeyEx.V));
            _keys.Add(new KeyMapping(48, 'b', 'B', 'b', 'B', 'b', 'B', ConsoleKeyEx.B));
            _keys.Add(new KeyMapping(49, 'n', 'N', 'n', 'N', 'n', 'N', ConsoleKeyEx.N));
            _keys.Add(new KeyMapping(50, 'm', 'M', 'm', 'M', 'm', 'M', 'µ', ConsoleKeyEx.M));
            _keys.Add(new KeyMapping(51, ',', ';', ',', ';', ',', ';', ConsoleKeyEx.Comma));
            _keys.Add(new KeyMapping(52, '.', ':', '.', ':', '.', ':', ConsoleKeyEx.Period));
            _keys.Add(new KeyMapping(53, '-', '_', '-', '_', '-', '_', ConsoleKeyEx.Slash));
            _keys.Add(new KeyMapping(54, ConsoleKeyEx.RShift));
            _keys.Add(new KeyMapping(55, '*', '*', '*', '*', '*', '*', ConsoleKeyEx.NumMultiply));
            _keys.Add(new KeyMapping(56, ConsoleKeyEx.LAlt));
            _keys.Add(new KeyMapping(57, ' ', ConsoleKeyEx.Spacebar));
            _keys.Add(new KeyMapping(58, ConsoleKeyEx.CapsLock));
            _keys.Add(new KeyMapping(59, ConsoleKeyEx.F1));
            _keys.Add(new KeyMapping(60, ConsoleKeyEx.F2));
            _keys.Add(new KeyMapping(61, ConsoleKeyEx.F3));
            _keys.Add(new KeyMapping(62, ConsoleKeyEx.F4));
            _keys.Add(new KeyMapping(63, ConsoleKeyEx.F5));
            _keys.Add(new KeyMapping(64, ConsoleKeyEx.F6));
            _keys.Add(new KeyMapping(65, ConsoleKeyEx.F7));
            _keys.Add(new KeyMapping(66, ConsoleKeyEx.F8));
            _keys.Add(new KeyMapping(67, ConsoleKeyEx.F9));
            _keys.Add(new KeyMapping(68, ConsoleKeyEx.F10));
            _keys.Add(new KeyMapping(87, ConsoleKeyEx.F11));
            _keys.Add(new KeyMapping(88, ConsoleKeyEx.F12));
            _keys.Add(new KeyMapping(69, ConsoleKeyEx.NumLock));
            _keys.Add(new KeyMapping(70, ConsoleKeyEx.ScrollLock));
            _keys.Add(new KeyMapping(74, '-', '-', '-', '-', '-', '-', ConsoleKeyEx.NumMinus));
            _keys.Add(new KeyMapping(78, '+', '+', '+', '+', '+', '+', ConsoleKeyEx.NumPlus));
            _keys.Add(new KeyMapping(71, '\0', '\0', '7', '\0', '\0', '\0', ConsoleKeyEx.Home, ConsoleKeyEx.Num7));
            _keys.Add(new KeyMapping(72, '\0', '\0', '8', '\0', '\0', '\0', ConsoleKeyEx.UpArrow, ConsoleKeyEx.Num8));
            _keys.Add(new KeyMapping(73, '\0', '\0', '9', '\0', '\0', '\0', ConsoleKeyEx.PageUp, ConsoleKeyEx.Num9));
            _keys.Add(new KeyMapping(75, '\0', '\0', '4', '\0', '\0', '\0', ConsoleKeyEx.LeftArrow, ConsoleKeyEx.Num4));
            _keys.Add(new KeyMapping(76, '\0', '\0', '5', '\0', '\0', '\0', ConsoleKeyEx.Num5));
            _keys.Add(new KeyMapping(77, '\0', '\0', '6', '\0', '\0', '\0', ConsoleKeyEx.RightArrow, ConsoleKeyEx.Num6));
            _keys.Add(new KeyMapping(79, '\0', '\0', '1', '\0', '\0', '\0', ConsoleKeyEx.End, ConsoleKeyEx.Num1));
            _keys.Add(new KeyMapping(80, '\0', '\0', '2', '\0', '\0', '\0', ConsoleKeyEx.DownArrow, ConsoleKeyEx.Num2));
            _keys.Add(new KeyMapping(81, '\0', '\0', '3', '\0', '\0', '\0', ConsoleKeyEx.PageDown, ConsoleKeyEx.Num3));
            _keys.Add(new KeyMapping(82, '\0', '\0', '0', '\0', '\0', '\0', ConsoleKeyEx.Insert, ConsoleKeyEx.Num0));
            _keys.Add(new KeyMapping(83, '\b', '\b', ',', '\b', '\b', '\b', ConsoleKeyEx.Delete, ConsoleKeyEx.NumPeriod));
            _keys.Add(new KeyMapping(86, '<', '>', '<', '<', '>', '>', '|', ConsoleKeyEx.OEM102));
            _keys.Add(new KeyMapping(91, ConsoleKeyEx.LWin));
            _keys.Add(new KeyMapping(92, ConsoleKeyEx.RWin));
        }
    }
}
