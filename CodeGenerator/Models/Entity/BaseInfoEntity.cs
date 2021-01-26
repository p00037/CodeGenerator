using CodeGenerator.Common;
using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Models.Entity
{
    public class BaseInfoEntity : InfoBaseEntity
    {
        [ExcelColumn("検索比較方法")]
        public string ComparisonMethod { get; set; }

        [ExcelColumn("検索用グリッド表示")]
        public bool DisplayGridForSearch { get; set; }

        public string GetXamlCode()
        {
            switch (GetXamlTypeName())
            {
                case XamlControlType.TextBox:
                    return GetXamlCodeTextBox();
                case XamlControlType.Number:
                    return GetXamlCodeNumber();
                case XamlControlType.ComboBox:
                    return GetXamlCodeComboBox();
                case XamlControlType.DatePicker:
                    return GetXamlCodeDatePicker();
                case XamlControlType.CheckBox:
                    return GetXamlCodeCheckBox();
            }

            return "";
        }

        public string GetXamlCodeSearch()
        {
            switch (GetXamlTypeName())
            {
                case XamlControlType.TextBox:
                    return GetXamlCodeSearchTextBox();
                case XamlControlType.Number:
                    if (IsRangeSearch() == true)
                    {
                        return GetXamlCodeSearchNumberRange();
                    }
                    else
                    {
                        return GetXamlCodeSearchNumber();
                    }
                case XamlControlType.ComboBox:
                    if (IsRangeSearch() == true)
                    {
                        return GetXamlCodeSearchComboBoxRange();
                    }
                    else
                    {
                        return GetXamlCodeSearchComboBox();
                    }
                case XamlControlType.DatePicker:
                    if (IsRangeSearch() == true)
                    {
                        return GetXamlCodeSearchDatePickerRange();
                    }
                    else
                    {
                        return GetXamlCodeSearchDatePicker();
                    }
                case XamlControlType.CheckBox:
                    return GetXamlCodeSearchCheckBox();
            }

            return "";
        }

        public string GetXamlCodeSearchGrid()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                            <DataGridTextColumn"));
            sb.AppendLine(string.Format("                                Width=\"200\""));
            sb.AppendLine(string.Format("                                Binding=\"{{Binding {0}}}\"", ColumnName));
            sb.AppendLine(string.Format("                                Header=\"{0}\" />", LabelName));
            return sb.ToString();
        }

        public string GetComboProperty()
        {
            return string.Format("        public ReactiveProperty<List<{0}Entity>> {1} {{ get; set; }} = new ReactiveProperty<List<{0}Entity>>(new List<{0}Entity>());", ComboBoxDataAcquisitionTable, ComboBoxItemsSource);
        }

        public string GetViewModelCodeLoad()
        {
            var list = new List<string>();
            list.Add(string.Format("            var dao{0} = new Dao{0}();", ComboBoxDataAcquisitionTable));
            list.Add(string.Format("            {0}.Value = dao{1}.Get{1}EntityList();", ComboBoxItemsSource, ComboBoxDataAcquisitionTable));
            return list.ConcatWith(Environment.NewLine);

        }    

        public bool IsRangeSearch()
        {
            if (this.ComparisonMethod == ">= && <=" || this.ComparisonMethod == "> && <")
            {
                return true;
            }

            return false;
        }

        public string GetSearchEntitiyCode()
        {
            var sb = new StringBuilder();
            if (IsRangeSearch() == true)
            {
                sb.AppendLine(string.Format("        public {0} {1}開始 {{ get; set; }}", GetEntityTypeName(), ColumnName));
                sb.AppendLine(string.Format(""));
                sb.AppendLine(string.Format("        public {0} {1}終了 {{ get; set; }}", GetEntityTypeName(), ColumnName));
            }
            else
            {
                sb.AppendLine(string.Format("        public {0} {1} {{ get; set; }}", GetEntityTypeName(), ColumnName));
            }

            return sb.ToString();
        }

        public string GetSearchWherePartCode()
        {
            if (this.ComparisonMethod == "Like")
            {
                return GetSearchWherePartCodeLike();
            }

            if (IsRangeSearch() == true)
            {
                return GetSearchWherePartCodeRanget();
            }

            return GetSearchWherePartCodeDefault();
        }

        private string GetXamlCodeTextBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(string.Format("                    <Label Content=\"{0}\" Style=\"{{StaticResource label-default}}\" />", LabelName));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("                    <TextBox"));
            sb.AppendLine(string.Format("                        Width=\"{0}\"", TextBoxWidth));
            if (PrimaryKey == true)
            {
                sb.AppendLine(string.Format("                        IsEnabled=\"{{Binding IsPrimaryKeyEnabled.Value}}\""));
            }
            if (GetMaxLength() != 0)
            {
                sb.AppendLine(string.Format("                        MaxLength=\"{0}\"", GetMaxLength()));
            }
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource {0}}}\"", GetStyle()));
            sb.AppendLine(string.Format("                        Text=\"{{Binding EditData.Value.{0}}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetStyle()
        {
            switch (CharacterType)
            {
                case "半角英字":
                case "半角英数字":
                case "半角英数字記号":
                    return "textbox-imedisable";
                case "半角カタカナ":
                    return "textbox-harfkana";
                case "全角カタカナ":
                    return "textbox-fullkana";
            }

            return "textbox-default";
        }

        private string GetXamlCodeNumber()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(string.Format("                    <Label Content=\"{0}\" Style=\"{{StaticResource label-default}}\" />", LabelName));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("                    <CustomContorol:NumberTextBox"));
            sb.AppendLine(string.Format("                        Width=\"{0}\"", TextBoxWidth));
            if (PrimaryKey == true)
            {
                sb.AppendLine(string.Format("                        IsEnabled=\"{{Binding IsPrimaryKeyEnabled.Value}}\""));
            }
            sb.AppendLine(string.Format("                        MaxLength=\"{0}\"", GetMaxLength()));
            sb.AppendLine(string.Format("                        MinValue=\"{0}\"", MinValue));
            sb.AppendLine(string.Format("                        MaxValue=\"{0}\"", MaxValue));
            sb.AppendLine(string.Format("                        DecimalPart=\"{0}\"", GetSizeDecimalPart()));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource textbox-number}}\""));
            sb.AppendLine(string.Format("                        Text=\"{{Binding EditData.Value.{0}}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeComboBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(string.Format("                    <Label Content=\"{0}\" Style=\"{{StaticResource label-default}}\" />", LabelName));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("                    <ComboBox"));
            if (PrimaryKey == true)
            {
                sb.AppendLine(string.Format("                        IsEnabled=\"{{Binding IsPrimaryKeyEnabled.Value}}\""));
            }
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource combobox-default}}\""));
            sb.AppendLine(string.Format("                        DisplayMemberPath=\"{0}\"", ComboBoxMemberPath));
            sb.AppendLine(string.Format("                        ItemsSource=\"{{Binding {0}.Value}}\"", ComboBoxItemsSource));
            sb.AppendLine(string.Format("                        SelectedValue=\"{{Binding EditData.Value.{0}}}\"", ColumnName));
            sb.AppendLine(string.Format("                        SelectedValuePath=\"{0}\" />", ComboBoxSelectedValuePath));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeDatePicker()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(string.Format("                    <Label Content=\"{0}\" Style=\"{{StaticResource label-default}}\" />", LabelName));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("                    <DatePicker"));
            if (PrimaryKey == true)
            {
                sb.AppendLine(string.Format("                        IsEnabled=\"{{Binding IsPrimaryKeyEnabled.Value}}\""));
            }
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource datepicker-default}}\""));
            sb.AppendLine(string.Format("                        Text=\"{{Binding EditData.Value.{0},Mode=TwoWay}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeCheckBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(string.Format("                    <CheckBox"));
            if (PrimaryKey == true)
            {
                sb.AppendLine(string.Format("                        IsEnabled=\"{{Binding IsPrimaryKeyEnabled.Value}}\""));
            }
            sb.AppendLine(string.Format("                        Content=\"{0}\"", LabelName));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource checkbox-default}}\""));
            sb.AppendLine(string.Format("                        IsChecked=\"{{Binding EditData.Value.{0}}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeSearchTextBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName));
            sb.AppendLine(string.Format("                    <TextBox"));
            sb.AppendLine(string.Format("                        Width=\"120\""));
            if (GetMaxLength() != 0)
            {
                sb.AppendLine(string.Format("                        MaxLength=\"{0}\"", GetMaxLength()));
            }
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource {0}}}\"", GetStyle()));
            sb.AppendLine(string.Format("                        Text=\"{{Binding SearchOptionEntity.Value.{0}}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeSearchNumber()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName));
            sb.AppendLine(string.Format("                    <CustomContorol:NumberTextBox"));
            sb.AppendLine(string.Format("                        Width=\"120\""));
            sb.AppendLine(string.Format("                        MaxLength=\"{0}\"", GetMaxLength()));
            sb.AppendLine(string.Format("                        MinValue=\"{0}\"", MinValue));
            sb.AppendLine(string.Format("                        MaxValue=\"{0}\"", MaxValue));
            sb.AppendLine(string.Format("                        DecimalPart=\"{0}\"", GetSizeDecimalPart()));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource textbox-number}}\""));
            sb.AppendLine(string.Format("                        Text=\"{{Binding SearchOptionEntity.Value.{0}}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeSearchNumberRange()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName + "開始"));
            sb.AppendLine(string.Format("                    <TextBox"));
            sb.AppendLine(string.Format("                        Width=\"120\""));
            sb.AppendLine(string.Format("                        Behavior:TextBoxBehaviors.NumConfig=\"{0}, {1}, {2}\"", MinValue, MaxValue, GetSizeDecimalPart()));
            sb.AppendLine(string.Format("                        MaxLength=\"{0}\"", GetMaxLength()));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource textbox-number}}\""));
            sb.AppendLine(string.Format("                        Text=\"{{Binding SearchOptionEntity.Value.{0}開始}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName + "終了"));
            sb.AppendLine(string.Format("                    <TextBox"));
            sb.AppendLine(string.Format("                        Width=\"120\""));
            sb.AppendLine(string.Format("                        Behavior:TextBoxBehaviors.NumConfig=\"{0}, {1}, {2}\"", MinValue, MaxValue, GetSizeDecimalPart()));
            sb.AppendLine(string.Format("                        MaxLength=\"{0}\"", GetMaxLength()));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource textbox-number}}\""));
            sb.AppendLine(string.Format("                        Text=\"{{Binding SearchOptionEntity.Value.{0}終了}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));

            return sb.ToString();
        }

        private string GetXamlCodeSearchComboBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName));
            sb.AppendLine(string.Format("                    <ComboBox"));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource combobox-default}}\""));
            sb.AppendLine(string.Format("                        DisplayMemberPath=\"{0}\"", ComboBoxMemberPath));
            sb.AppendLine(string.Format("                        ItemsSource=\"{{Binding {0}.Value}}\"", ComboBoxItemsSource));
            sb.AppendLine(string.Format("                        SelectedValue=\"{{Binding SearchOptionEntity.Value.{0}}}\"", ColumnName));
            sb.AppendLine(string.Format("                        SelectedValuePath=\"{0}\" />", ComboBoxSelectedValuePath));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeSearchComboBoxRange()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName + "開始"));
            sb.AppendLine(string.Format("                    <ComboBox"));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource combobox-default}}\""));
            sb.AppendLine(string.Format("                        DisplayMemberPath=\"{0}\"", ComboBoxMemberPath));
            sb.AppendLine(string.Format("                        ItemsSource=\"{{Binding {0}.Value}}\"", ComboBoxItemsSource));
            sb.AppendLine(string.Format("                        SelectedValue=\"{{Binding SearchOptionEntity.Value.{0}}開始}\"", ColumnName));
            sb.AppendLine(string.Format("                        SelectedValuePath=\"{0}\" />", ComboBoxSelectedValuePath));
            sb.AppendLine(string.Format("                </StackPanel>"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName + "終了"));
            sb.AppendLine(string.Format("                    <ComboBox"));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource combobox-default}}\""));
            sb.AppendLine(string.Format("                        DisplayMemberPath=\"{0}\"", ComboBoxMemberPath));
            sb.AppendLine(string.Format("                        ItemsSource=\"{{Binding {0}.Value}}\"", ComboBoxItemsSource));
            sb.AppendLine(string.Format("                        SelectedValue=\"{{Binding SearchOptionEntity.Value.{0}}終了}\"", ColumnName));
            sb.AppendLine(string.Format("                        SelectedValuePath=\"{0}\" />", ComboBoxSelectedValuePath));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeSearchDatePicker()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName));
            sb.AppendLine(string.Format("                    <DatePicker"));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource datepicker-default}}\""));
            sb.AppendLine(string.Format("                        Text=\"{{Binding SearchOptionEntity.Value.{0}}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeSearchDatePickerRange()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName + "開始"));
            sb.AppendLine(string.Format("                    <DatePicker"));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource datepicker-default}}\""));
            sb.AppendLine(string.Format("                        Text=\"{{Binding SearchOptionEntity.Value.{0}開始}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(GetXamlCodeSearchLabelPart(LabelName + "終了"));
            sb.AppendLine(string.Format("                    <DatePicker"));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource datepicker-default}}\""));
            sb.AppendLine(string.Format("                        Text=\"{{Binding SearchOptionEntity.Value.{0}終了}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeSearchCheckBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">"));
            sb.AppendLine(string.Format("                    <CheckBox"));
            sb.AppendLine(string.Format("                        Content=\"{0}\"", LabelName));
            sb.AppendLine(string.Format("                        Style=\"{{StaticResource checkbox-default}}\""));
            sb.AppendLine(string.Format("                        IsChecked=\"{{Binding SearchOptionEntity.Value.{0}}}\" />", ColumnName));
            sb.AppendLine(string.Format("                </StackPanel>"));
            return sb.ToString();
        }

        private string GetXamlCodeSearchLabelPart(string labelName)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                    <Label"));
            sb.AppendLine(string.Format("                        Width=\"80\""));
            sb.AppendLine(string.Format("                        Content=\"{0}\"", labelName));
            sb.AppendLine(string.Format("                        Style=\"{{ StaticResource label-default}}\" />"));
            return sb.ToString();
        }

        private string GetSearchWherePartCodeLike()
        {
            return string.Format("                .Where(e => e.{0}.Contains(searchData.{0}.NullToValue(\"\")))", ColumnName);
        }

        private string GetSearchWherePartCodeDefault()
        {
            return string.Format("                .WhereIf(searchData.{0} != null && searchData.{0}.ToString() != \"\", e => e.{0} {1} searchData.{0})", ColumnName, ComparisonMethod);
        }

        private string GetSearchWherePartCodeRanget()
        {
            string[] separator = { " && " };
            var array = ComparisonMethod.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            var list = new List<string>();
            list.Add(string.Format("                .WhereIf(searchData.{0}開始 != null && searchData.{0}開始.ToString() != \"\", e => e.{0} {1} searchData.{0}開始)", ColumnName, array[0]));
            list.Add(string.Format("                .WhereIf(searchData.{0}終了 != null && searchData.{0}終了.ToString() != \"\", e => e.{0} {1} searchData.{0}終了)", ColumnName, array[1]));
            return list.ConcatWith(Environment.NewLine);
        }


    }
}
