using System;
using System.Collections.Generic;
using System.Text;

namespace Codes
{
    public abstract class Code
    {
        public abstract string Encode(string plaintext);
        public abstract string Decode(string code);

        public static IEnumerable<string> Normalize(string text)
        {
            var words = new LinkedList<string>();
            var word = new StringBuilder();
            foreach (var c in text?.ToUpper() ?? "")
            {
                if (char.IsLetterOrDigit(c) && (c >= 65 && c <= 90))
                {
                    word.Append(c);
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (!string.IsNullOrWhiteSpace(word.ToString()))
                    {
                        words.AddLast(word.ToString());
                    }
                    word = new StringBuilder();
                }
            }

            if (!string.IsNullOrWhiteSpace(word.ToString()))
            {
                words.AddLast(word.ToString());
            }
            return words;
        }

    }
}
