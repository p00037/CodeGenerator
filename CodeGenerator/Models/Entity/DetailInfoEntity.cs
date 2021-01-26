using CodeGenerator.Common;
using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Models.Entity
{
    public class DetailInfoEntity : InfoBaseEntity
    {
        [ExcelColumn("DB更新キー")]
        public bool DBUpdateKey { get; set; }

        [ExcelColumn("連番")]
        public bool SerialNumber { get; set; }

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

        private string GetXamlCodeTextBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                            <DataGridTemplateColumn Header=\"{0}\" Width=\"{1}\">", LabelName, TextBoxWidth));
            sb.AppendLine(string.Format("                                <DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                                    <DataTemplate>"));
            sb.AppendLine(string.Format("                                        <TextBox"));
            sb.AppendLine(string.Format("                                            HorizontalAlignment=\"Stretch\""));
            if (GetMaxLength() != 0)
            {
                sb.AppendLine(string.Format("                                            MaxLength=\"{0}\"", GetMaxLength()));
            }
            sb.AppendLine(string.Format("                                            Style=\"{{StaticResource {0}}}\"", GetStyle()));
            sb.AppendLine(string.Format("                                            Text=\"{{Binding {0}, UpdateSourceTrigger=LostFocus}}\" />", ColumnName));
            sb.AppendLine(string.Format("                                    </DataTemplate>"));
            sb.AppendLine(string.Format("                                </DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                            </DataGridTemplateColumn>"));
            return sb.ToString();
        }

        private string GetStyle()
        {
            switch (CharacterType)
            {
                case "半角英字":
                case "半角英数字":
                case "半角英数字記号":
                    return "textbox-grid-imedisable";
                case "半角カタカナ":
                    return "textbox-grid-harfkana";
                case "全角カタカナ":
                    return "textbox-grid-fullkana";
            }

            return "textbox-grid-default";
        }

        private string GetXamlCodeNumber()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                            <DataGridTemplateColumn Header=\"{0}\" Width=\"{1}\">", LabelName, TextBoxWidth));
            sb.AppendLine(string.Format("                                <DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                                    <DataTemplate>"));
            sb.AppendLine(string.Format("                                        <CustomContorol:NumberTextBox"));
            sb.AppendLine(string.Format("                                            HorizontalAlignment=\"Stretch\""));
            sb.AppendLine(string.Format("                                            MaxLength=\"{0}\"", GetMaxLength()));
            sb.AppendLine(string.Format("                                            MinValue=\"{0}\"", MinValue));
            sb.AppendLine(string.Format("                                            MaxValue=\"{0}\"", MaxValue));
            sb.AppendLine(string.Format("                                            DecimalPart=\"{0}\"", GetSizeDecimalPart()));
            sb.AppendLine(string.Format("                                            Style=\"{{StaticResource textbox-grid-number}}\""));
            sb.AppendLine(string.Format("                                            Text=\"{{Binding {0}, UpdateSourceTrigger=LostFocus}}\" />", ColumnName));
            sb.AppendLine(string.Format("                                    </DataTemplate>"));
            sb.AppendLine(string.Format("                                </DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                            </DataGridTemplateColumn>"));
            return sb.ToString();
        }

        private string GetXamlCodeComboBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                            <DataGridTemplateColumn Header=\"{0}\" Width=\"144\">", LabelName));
            sb.AppendLine(string.Format("                                <DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                                    <DataTemplate>"));
            sb.AppendLine(string.Format("                                        <ComboBox"));
            sb.AppendLine(string.Format("                                            HorizontalAlignment=\"Stretch\""));
            sb.AppendLine(string.Format("                                            DisplayMemberPath=\"{0}\"", ComboBoxMemberPath));
            sb.AppendLine(string.Format("                                            ItemsSource=\"{{Binding DataContext.{0}.Value, ElementName=View{1}}}\"", ComboBoxItemsSource, XamlName));
            sb.AppendLine(string.Format("                                            SelectedValue=\"{{Binding {0}, UpdateSourceTrigger=LostFocus}}\"", ColumnName));
            sb.AppendLine(string.Format("                                            SelectedValuePath=\"{0}\"", ComboBoxSelectedValuePath));
            sb.AppendLine(string.Format("                                            Style=\"{{StaticResource combobox-grid}}\" />"));
            sb.AppendLine(string.Format("                                    </DataTemplate>"));
            sb.AppendLine(string.Format("                                </DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                            </DataGridTemplateColumn>"));
            return sb.ToString();
        }

        private string GetXamlCodeDatePicker()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                            <DataGridTemplateColumn Header=\"{0}\" Width=\"100\">", LabelName));
            sb.AppendLine(string.Format("                                <DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                                    <DataTemplate>"));
            sb.AppendLine(string.Format("                                        <DatePicker"));
            sb.AppendLine(string.Format("                                            HorizontalAlignment=\"Stretch\""));
            sb.AppendLine(string.Format("                                            Style=\"{{StaticResource datepicker-grid}}\""));
            sb.AppendLine(string.Format("                                            Text=\"{{Binding {0}, UpdateSourceTrigger=LostFocus, Mode=TwoWay}}\" />", ColumnName));
            sb.AppendLine(string.Format("                                    </DataTemplate>"));
            sb.AppendLine(string.Format("                                </DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                            </DataGridTemplateColumn>"));
            return sb.ToString();
        }

        private string GetXamlCodeCheckBox()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("                            <DataGridTemplateColumn Header=\"{0}\" Width=\"25\">", LabelName));
            sb.AppendLine(string.Format("                                <DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                                    <DataTemplate>"));
            sb.AppendLine(string.Format("                                        <CheckBox"));
            sb.AppendLine(string.Format("                                            HorizontalAlignment=\"Stretch\""));
            sb.AppendLine(string.Format("                                            Style=\"{{StaticResource checkbox-grid}}\""));
            sb.AppendLine(string.Format("                                            IsChecked=\"{{Binding {0}, UpdateSourceTrigger=LostFocus}}\" />", ColumnName));
            sb.AppendLine(string.Format("                                    </DataTemplate>"));
            sb.AppendLine(string.Format("                                </DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(string.Format("                            </DataGridTemplateColumn>"));
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
    }
}
