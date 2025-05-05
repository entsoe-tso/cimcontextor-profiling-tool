using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CimContextor.Utilities
{
    internal class CeasarCipher
    {
        private char Cipher(char ch, int key)
        {
            if (char.IsControl(ch))
                return ch;

            char offset = ' ';
            return (char)((((ch + key) - offset) % 95) + offset);
        }
        public string Encipher(string input, int key)
        {
            string output = string.Empty;

            foreach (char ch in input)
                output += Cipher(ch, key);

            return output;
        }

        public string Decipher(string input, int key)
        {
            return Encipher(input, 95 - key);
        }
    }
}
