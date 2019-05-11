using System;
using System.Collections.Generic;
using System.Text;

namespace Codes
{
    public static class Helpers
    {
        public static string Print(this char[,] matrix)
        {
            var m = new StringBuilder();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    m.Append($"{matrix[i, j].ToString(),-4}");
                }
                m.AppendLine();
            }
            return m.ToString().Trim();
        }

        public static string Print<T>(this T[] row)
        {
            var m = new StringBuilder();
            for (int i = 0; i < row.Length; i++)
            {
                m.Append($"{row[i].ToString(),-4}");
            }
            return m.ToString().Trim();
        }
    }
}
