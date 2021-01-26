using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// nullのとき引数の文字列に変換する
        /// </summary>
        /// <param name="source">拡張メソッド本体の値</param>
        /// <param name="value">nullから変換する値</param>
        /// <returns>変換後の値</returns>
        public static string NullToValue(this string source, string value)
        {
            if (source == null)
            {
                return value;
            }

            return source;
        }

        /// <summary>
        /// int型に変換する（数値に変換できないときは0を返す）
        /// </summary>
        /// <param name="source">拡張メソッド本体の値</param>
        /// <returns>int型に変換した値</returns>
        public static int ToInt(this string source)
        {
            int returnvalue;
            if (int.TryParse(source, out returnvalue) == false)
            {
                return 0;
            }

            return returnvalue;
        }

        /// <summary>
        /// Date?型に変換する（数値に変換できないときは0を返す）
        /// </summary>
        /// <param name="source">拡張メソッド本体の値</param>
        /// <returns>Date?型に変換した値</returns>
        public static DateTime? ToNullableDateTime(this string source)
        {
            DateTime returnvalue;
            if (DateTime.TryParse(source, out returnvalue) == false)
            {
                return null;
            }

            return returnvalue;
        }

        /// <summary>
        /// バイト数を取得する
        /// </summary>
        /// <param name="source">拡張メソッド本体の値</param>
        /// <returns></returns>
        public static int GetByteCount(this string source)
        {
            return  Encoding.GetEncoding("Shift_JIS").GetByteCount(source);
        }
    }
}
