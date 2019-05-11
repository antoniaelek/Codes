using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codes
{
    public class PoemCode : Code
    {
        public string Poem { get; }
        public string[] PoemWords { get; }
        public int TotalPoemWords { get; }

        private static readonly Random _random;

        static PoemCode()
        {
            _random = _random ?? new Random();
        }

        public PoemCode(string poem, ILogger logger)
        {
            Log.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            PoemWords = Normalize(poem).ToArray();
            TotalPoemWords = PoemWords.Length;
            Poem = string.Join(" ", PoemWords);
            if (string.IsNullOrWhiteSpace(Poem))
            {
                throw new Exception("Empty poem! Choose another one.");
            }
        }

        public override string Decode(string code)
        {
            throw new NotImplementedException();
        }

        public override string Encode(string plaintext)
        {
            plaintext = string.Join("", Normalize(plaintext));
            Log.Debug("Message: {msg}", plaintext);

            Log.Debug("Poem: {poem}", Poem);

            var tk = RandomTranspositionKey();

            var tkStr = TranspositionKeyStr(tk);
            Log.Debug("Transposition key: {key}", tkStr);

            var ig = IndicatorGroup(tk);
            Log.Debug("Indicator group: {ind}", ig);

            var numberedTK = NumberTranspositionKey(tk);
            Log.Debug("Numbering transposition key...\n{tkNum}\n{tk}", numberedTK.Print(), tkStr.ToCharArray().Print());

            // Construct message matrix
            var matrix = ConstructMessageMatrix(plaintext, numberedTK);
            Log.Debug("Constructing message matrix...\n{tkNum}\n{matrix}", numberedTK.Print(), matrix.Print());

            // Transpose message matrix
            var transposed = TransposeMatrix(matrix);
            Log.Debug("Transposing message matrix...\n{matrix}", transposed.Print());

            // Order by transposition key
            var result = OrderRowsByTranspositionKey(transposed, numberedTK);
            Log.Debug("Encoded message: {res}", result);

            return $"{ig} {result}";
        }

        private int[] RandomTranspositionKey()
        {
            var noWords = 5;
            var indices = new int[noWords];
            for (int i = 0; i < noWords; i++)
            {
                indices[i] = _random.Next(0, Math.Min(TotalPoemWords, 25) - 1);
            }
            return indices;
        }

        private string IndicatorGroup(int[] transpositionKey)
        {
            var ig = new StringBuilder();
            foreach (var index in transpositionKey)
            {
                ig.Append((char)('A' + index));
            }
            return ig.ToString();
        }

        private string TranspositionKeyStr(int[] transpositionKey)
        {
            var key = new StringBuilder();
            foreach (var index in transpositionKey)
            {
                key.Append(PoemWords[index]);
            }
            return key.ToString();
        }

        private int[] NumberTranspositionKey(int[] transpositionKey)
        {
            var key = TranspositionKeyStr(transpositionKey);
            var idx = 1;
            var numbered = new int[key.Length];
            for (int i = 0; i < 25; i++)
            {
                var letter = (char)('A' + i);
                for (int j = 0; j < key.Length; j++)
                {
                    char keyLetter = (char)key[j];
                    if (keyLetter == letter)
                    {
                        numbered[j] = idx;
                        idx++;
                    }
                }
            }

            return numbered;
        }

        private static char[,] ConstructMessageMatrix(string plaintext, int[] numberedTK)
        {
            // Split message
            var rows = new List<string>();
            var noRows = (int)(plaintext.Length / numberedTK.Length);
            for (int i = 0; i < noRows; i++)
            {
                rows.Add(plaintext.Substring(i * numberedTK.Length, numberedTK.Length));
            }
            rows.Add(plaintext.Substring(noRows * numberedTK.Length));

            // Construct message matrix
            char[,] matrix = new char[rows.Count, numberedTK.Length];
            for (int i = 0; i < rows.Count; i++)
            {
                string row = rows[i];
                for (int j = 0; j < row.Length; j++)
                {
                    matrix[i, j] = row[j];
                }
            }

            return matrix;
        }

        private char[,] TransposeMatrix(char[,] matrix)
        {
            var transposed = new char[matrix.GetLength(1), matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    transposed[j, i] = matrix[i, j];
                }
            }

            return transposed;
        }

        private string OrderRowsByTranspositionKey(char[,] transposed, int[] numberedTK)
        {
            var encoded = new Dictionary<int, string>();
            for (int i = 0; i < transposed.GetLength(0); i++)
            {
                encoded[numberedTK[i]] = "";
                for (int j = 0; j < transposed.GetLength(1); j++)
                {
                    if (transposed[i, j] != '\0')
                    {
                        encoded[numberedTK[i]] += transposed[i, j];
                    }
                }
            }

            var result = new StringBuilder();
            foreach (var key in encoded.Keys.OrderBy(k => k))
            {
                result.Append(encoded[key].Trim());
            }

            return result.ToString();
        }
    }
}
