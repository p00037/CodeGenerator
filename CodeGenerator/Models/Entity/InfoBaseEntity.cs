using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using CodeGenerator.Common;

namespace CodeGenerator.Models.Entity
{
    public class InfoBaseEntity
    {
        [ExcelColumn("カラム名")]
        public string ColumnName { get; set; }

        [ExcelColumn("型")]
        public string DBType { get; set; }

        [ExcelColumn("必須")]
        public bool Required { get; set; }

        [ExcelColumn("主キー")]
        public bool PrimaryKey { get; set; }

        [ExcelColumn("最小値")]
        public string MinValue { get; set; }

        [ExcelColumn("最大値")]
        public string MaxValue { get; set; }

        [ExcelColumn("文字種")]
        public string CharacterType { get; set; }

        [ExcelColumn("並び順")]
        public int? Order { get; set; }

        [ExcelColumn("ラベル名")]
        public string LabelName { get; set; }

        [ExcelColumn("ComboBoxフラグ")]
        public bool ComboBoxFlg { get; set; }

        [ExcelColumn("ComboBoxItemsSource")]
        public string ComboBoxItemsSource { get; set; }

        [ExcelColumn("ComboBoxMemberPath")]
        public string ComboBoxMemberPath { get; set; }

        [ExcelColumn("ComboBoxSelectedValuePath")]
        public string ComboBoxSelectedValuePath { get; set; }

        [ExcelColumn("ComboBoxデータ取得テーブル")]
        public string ComboBoxDataAcquisitionTable { get; set; }

        [ExcelColumn("コントロール作成なし")]
        public bool NothingCreateControlFlg { get; set; }

        [ExcelColumn("DBなし")]
        public bool NothingDbColumn { get; set; }

        public int? TextBoxWidth { get; set; }

        public string XamlName { get; set; }

        public enum XamlControlType { なし, TextBox, Number, ComboBox, DatePicker, CheckBox }

        public string GetEntitiyCode()
        {
            var entitiyCode = new List<string>();

            if (NothingDbColumn == true) entitiyCode.Add(string.Format("[NotMapped]"));

            var str = "        [Required(ErrorMessage = \"{0}は必須です。\")]";
            if (Required == true) entitiyCode.Add(string.Format(str, LabelName));

            if (MinValue != null) entitiyCode.Add(string.Format("        [Range({1}, {2}, ErrorMessage = \"{0}の範囲は{1}～{2}です\")]", LabelName, MinValue, MaxValue));

            if (CharacterType != null) entitiyCode.Add(GetRegularExpressionEntitiyCode());

            if (GetEntityTypeName() == "string" && GetMaxLength() != 0) entitiyCode.Add(string.Format("        [StringLength({1}, ErrorMessage = \"{0}の最大文字数は{1}です。\")]", LabelName, GetSize()));

            entitiyCode.Add(string.Format("        public {0} {1} {{ get; set; }}", GetEntityTypeName(), ColumnName));

            return entitiyCode.ConcatWith(Environment.NewLine);
        }
        
        public string GetEntityTypeName()
        {
            var returnValue = "";
            switch (GetNoSizeDBType())
            {
                case "varchar":
                case "nvarchar":
                    returnValue = "string";
                    break;
                case "int":
                    returnValue = "int";
                    break;
                case "numeric":
                    returnValue = "decimal";
                    break;
                case "datetime":
                    returnValue = "DateTime";
                    break;
                case "time":
                    returnValue = "TimeSpan";
                    break;
                case "bit":
                    returnValue = "bool";
                    break;
            }

            if (returnValue != "string" && Required == false) returnValue += "?";

            return returnValue;
        }

        public XamlControlType GetXamlTypeName()
        {
            if (ComboBoxFlg == true) return XamlControlType.ComboBox;

            switch (GetEntityTypeName().Replace("?", ""))
            {
                case "string":
                case "TimeSpan":
                    return XamlControlType.TextBox;
                case "int":
                case "decimal":
                    return XamlControlType.Number;
                case "DateTime":
                    return XamlControlType.DatePicker;
                case "bool":
                    return XamlControlType.CheckBox;
            }
            return XamlControlType.なし;
        }

        public string GetNoSizeDBType()
        {
            var strArray = DBType.Split('(');
            return strArray[0];
        }

        public string GetSize()
        {
            var strArray = DBType.Split('(');
            if (strArray.Length <= 1) return "";
            var size = strArray[1].Replace(")", "");
            return size;
        }

        public int GetMaxLength()
        {
            if (GetXamlTypeName() == XamlControlType.TextBox)
            {
                if (GetNoSizeDBType() == "varchar" && IsHalfSize() == false) return GetSize().ToInt() / 2;

                return GetSize().ToInt();
            }

            if (GetXamlTypeName() == XamlControlType.Number)
            {
                //マイナス表記のために+1桁
                var maxLength = GetSizeIntegerPart().ToInt() + 1;

                if (GetSizeDecimalPart() != 0) maxLength += GetSizeDecimalPart() + 1;

                return maxLength;
            }

            return 0;
        }

        public bool IsHalfSize()
        {
            if (CharacterType.NullToValue("").Contains("半角")) return true;

            if (GetXamlTypeName() == XamlControlType.Number) return true;

            return false;
        }

        protected string GetSizeIntegerPart()
        {
            var strArray = GetSize().Split(',');
            return strArray[0];
        }

        protected int GetSizeDecimalPart()
        {
            var strArray = GetSize().Split(',');
            if (strArray.Length <= 1) return 0;

            return strArray[1].Trim().ToInt();
        }

        protected string GetRegularExpressionEntitiyCode()
        {
            switch (CharacterType)
            {
                case "半角英字":
                    return string.Format("        [RegularExpression(@\"[a-zA-Z]+\", ErrorMessage = \"{0}は半角英字のみ入力できます\")]", LabelName);
                case "半角英数字":
                    return string.Format("        [RegularExpression(@\"[a-zA-Z0-9]+\", ErrorMessage = \"{0}は半角英数字のみ入力できます\")]", LabelName);
                case "半角英数字記号":
                    return string.Format("        [RegularExpression(@\"[a-zA-Z0-9- /:-@\\[-~]+\", ErrorMessage = \"{0}は半角英数字記号のみ入力できます\")]", LabelName);
                case "半角カタカナ":
                    return string.Format("        [RegularExpression(@\"[｡-ﾟ+]+\", ErrorMessage = \"{0}は半角カタカナのみ入力できます\")]", LabelName);
                case "全角カタカナ":
                    return string.Format("        [RegularExpression(@\"[ァ-ヶ]+\", ErrorMessage = \"{0}は半角カタカナのみ入力できます\")]", LabelName);
            }

            return "";
        }
    }
}
