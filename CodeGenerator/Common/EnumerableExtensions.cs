using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CodeGenerator.Common
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 文字列を結合する
        /// </summary>
        /// <typeparam name="T">IFormattableを継承している型</typeparam>
        /// <param name="source">拡張メソッド本体の値</param>
        /// <param name="separator">区切り文字</param>
        /// <param name="format">フォーマット文字列</param>
        /// <param name="provider">フォーマットプロバイダー</param>
        /// <returns>結合した文字</returns>
        public static string ConcatWith<T>(this IEnumerable<T> source,
                                           string separator,
                                           string format,
                                           IFormatProvider provider = null) where T : IFormattable
        {
            return source.Select(x => x.ToString(format, provider))
                .Aggregate((a, b) => a + separator + b);
        }

        /// <summary>
        /// 文字列を結合する
        /// </summary>
        /// <param name="source">拡張メソッド本体の値</param>
        /// <param name="separator">区切り文字</param>
        /// <returns>結合した文字</returns>
        public static string ConcatWith(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }

        /// <summary>
        /// 文字列を結合する(Htmlの箇条書きにする）
        /// </summary>
        /// <param name="source">拡張メソッド本体の値</param>
        /// <returns>結合した文字</returns>
        public static string ConcatWithHtmlItemize(this IEnumerable<string> source)
        {
            return "<ul><li>" + string.Join("</li><li>", source) + "</li></ul>";
        }

        public static DataTable ConvertDataTable<T>(this IEnumerable<T> items)
        {
            var properties = typeof(T).GetProperties();
            var result = new DataTable();

            // テーブルレイアウトの作成
            foreach (var prop in properties)
            {
                result.Columns.Add(prop.Name, prop.PropertyType);
            }

            // 値の投げ込み
            foreach (var item in items)
            {
                var row = result.NewRow();
                foreach (var prop in properties)
                {
                    var itemValue = prop.GetValue(item, new object[] { });
                    row[prop.Name] = itemValue;
                }
                result.Rows.Add(row);
            }
            return result;
        }
    }
}
