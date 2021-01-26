using CodeGenerator.Common;
using CodeGenerator.Models.Entity;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.ViewModel
{
    public class InfoBaseList<T> : List<T> where T : InfoBaseEntity
    {
        public string GetArgumentString()
        {
            return this
                .Where(e => e.PrimaryKey == true)
                .Select(e => e.GetEntityTypeName() + " " + e.ColumnName)
                .ConcatWith(",");
        }

        public string GetColumnName(string headerString)
        {
            return this
                .Where(e => e.PrimaryKey == true)
                .Select(e => headerString + e.ColumnName)
                .ConcatWith(",");
        }

        /// <summary>
        /// xxx.ColumnName == yyy.ColumnName && ...
        /// </summary>
        /// <param name="LeftSideHeaderString"></param>
        /// <param name="RightSideHeaderString"></param>
        /// <returns></returns>
        public string GetSameComparisonString(string LeftSideHeaderString,string RightSideHeaderString)
        {
            return this
                .Where(e => e.PrimaryKey == true)
                .Select(e => LeftSideHeaderString + e.ColumnName + "== " + RightSideHeaderString + e.ColumnName)
                .ConcatWith(" && ");
        }

        public string GetLabelName()
        {
            return this
                .Where(e => e.PrimaryKey == true)
                .Select(e => e.LabelName)
                .ConcatWith(",");
        }
    }
}
