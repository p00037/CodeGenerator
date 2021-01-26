using LinqToExcel;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeGenerator.Common;
using CodeGenerator.Models.Entity;
using CodeGenerator.Views;
using System.IO;

namespace CodeGenerator.ViewModel
{
    public class AutoCodeGeneratorViewModel
    {
        private AutoCodeGenerator window;

        private BaseInfoEntityList autoCodeGeneratorBaseInfoEntitys = new BaseInfoEntityList();

        private ConfigEntity autoCodeGeneratorConfigEntity;

        private List<DetailConfigEntity> autoCodeGeneratorDetailConfigEntityList;

        private List<DetailInfoEntityList> autoCodeGeneratorDetailInfoEntitysList = new List<DetailInfoEntityList>();

        public ReactiveProperty<string> FilePath { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> ProgramFolderPath { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> EntityCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> DetailEntityCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> SearchEntityCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> XamlCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> XamlcsCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> DaoCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> DetailDaoCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> ViewModelCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> AppDbContextCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveCommand BtnViewFileDaialogCommand { get; } = new ReactiveCommand();

        public ReactiveCommand BtnViewProgramFolderDaialogCommand { get; } = new ReactiveCommand();

        public ReactiveCommand BtnExecCommand { get; } = new ReactiveCommand();


        public AutoCodeGeneratorViewModel(AutoCodeGenerator window)
        {
            this.window = window;

            BtnViewFileDaialogCommand.Subscribe(_ => BtnViewFileDaialogCommandAction());
            BtnViewProgramFolderDaialogCommand.Subscribe(_ => BtnViewProgramFolderDaialogCommanddAction());
            BtnExecCommand.Subscribe(_ => BtnExecCommandAction());
        }

        private void BtnViewFileDaialogCommandAction()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "ファイルを開く";
            dialog.Filter = "テキストファイル|*.xlsx";

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                FilePath.Value = dialog.FileName;
            }
        }

        private void BtnViewProgramFolderDaialogCommanddAction()
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "フォルダを開く";

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                ProgramFolderPath.Value = dialog.SelectedPath;
            }
        }

        private void BtnExecCommandAction()
        {
            SetAutoCodeGeneratorConfigEntitys();
            SetEntityCode();
            SetXamlCode();
            SetDaoCode();
            SetViewModelCode();
            SetXamlcsCode();
            SetAppDbContextCode();

            if (ProgramFolderPath.Value.NullToValue("") != "")
            {
                ExportProgramCodeFile(autoCodeGeneratorConfigEntity.EntityNamespace, EntityCode.Value);
                ExportProgramCodeFile(autoCodeGeneratorConfigEntity.EntityNamespace, DetailEntityCode.Value);
                ExportProgramCodeFile(autoCodeGeneratorConfigEntity.SearchEntityNamespace, SearchEntityCode.Value);
                ExportProgramCodeFile(autoCodeGeneratorConfigEntity.XamlNamespace, XamlCode.Value);
                ExportProgramCodeFile(autoCodeGeneratorConfigEntity.XamlNamespace, XamlcsCode.Value);
                ExportProgramCodeFile(autoCodeGeneratorConfigEntity.DaoNamespace, DaoCode.Value);
                ExportProgramCodeFile(autoCodeGeneratorConfigEntity.DaoNamespace, DetailDaoCode.Value);
                ExportProgramCodeFile(autoCodeGeneratorConfigEntity.ViewModelNamespace, ViewModelCode.Value);
            }

            System.Windows.MessageBox.Show("完了");
        }
        
        private void SetAutoCodeGeneratorConfigEntitys()
        {
            autoCodeGeneratorBaseInfoEntitys = new BaseInfoEntityList();
            autoCodeGeneratorDetailInfoEntitysList = new List<DetailInfoEntityList>();

            var excel = new ExcelQueryFactory(FilePath.Value);
            this.autoCodeGeneratorConfigEntity = excel.Worksheet<ConfigEntity>("設定情報").FirstOrDefault();
            this.autoCodeGeneratorBaseInfoEntitys.AddRange(excel.Worksheet<BaseInfoEntity>("基本テーブル情報").ToList());
            this.autoCodeGeneratorBaseInfoEntitys.SetConfig(autoCodeGeneratorConfigEntity);
            this.autoCodeGeneratorDetailConfigEntityList = excel.Worksheet<DetailConfigEntity>("明細設定情報").ToList();
            for (int i = 1; i <= autoCodeGeneratorConfigEntity.DetailCount; i++)
            {
                var list = new DetailInfoEntityList();
                list.AddRange(excel.Worksheet<DetailInfoEntity>("明細テーブル情報" + i.ToString()).ToList());
                list.SetConfig(autoCodeGeneratorConfigEntity,autoCodeGeneratorDetailConfigEntityList[i - 1]);

                autoCodeGeneratorDetailInfoEntitysList.Add(list);
            }

            SetTextBoxWidthAndXamlName();
        }

        private void SetTextBoxWidthAndXamlName()
        {
            var textBox = new TextBox();

            foreach (var entity in autoCodeGeneratorBaseInfoEntitys.Where(e => e.GetMaxLength() > 0).ToList())
            {
                var str = (entity.IsHalfSize() ? 'ｱ' : 'ア');
                window.TextBox1.Text = new string(str, entity.GetMaxLength());
                window.TextBox1.UpdateLayout();
                entity.TextBoxWidth = (int)window.TextBox1.ActualWidth + 2;
            }

            foreach (var item in autoCodeGeneratorDetailInfoEntitysList.Select((v, i) => new { v, i }))
            {
                foreach (var entity in item.v.Where(e => e.GetMaxLength() > 0).ToList())
                {
                    var str = (entity.IsHalfSize() ? 'ｱ' : 'ア');
                    var maxLength = entity.GetMaxLength();
                    if (entity.LabelName.GetByteCount() > maxLength)
                    {
                        str = 'ｱ';
                        maxLength = entity.LabelName.GetByteCount();
                    }
                    window.TextBox1.Text = new string(str, maxLength);
                    window.TextBox1.UpdateLayout();
                    entity.TextBoxWidth = (int)window.TextBox1.ActualWidth + 2;
                }

                item.v.ForEach(e => e.XamlName = autoCodeGeneratorConfigEntity.XamlName);
            }
        }

        private void SetEntityCode()
        {
            EntityCode.Value = autoCodeGeneratorBaseInfoEntitys.GetEntityCode();
            SearchEntityCode.Value = autoCodeGeneratorBaseInfoEntitys.GetSearchEntityCode();
            DetailEntityCode.Value = autoCodeGeneratorDetailInfoEntitysList.Select(e => e.GetEntityCode()).ConcatWith(Environment.NewLine);
        }

        private void SetXamlCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"【{autoCodeGeneratorConfigEntity.XamlName}.xaml】");
            sb.AppendLine($" <CustomContorol:WindowBase");
            sb.AppendLine($"     x:Class=\"{autoCodeGeneratorConfigEntity.XamlNamespace}.{autoCodeGeneratorConfigEntity.XamlName}\"");
            sb.AppendLine($"     xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            sb.AppendLine($"     xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            sb.AppendLine($"     xmlns:Behavior=\"clr-namespace:WpfControlLibrary.Behavior;assembly=WpfControlLibrary\"");
            sb.AppendLine($"     xmlns:CustomContorol=\"clr-namespace:WpfControlLibrary.CustomContorol;assembly=WpfControlLibrary\"");
            sb.AppendLine($"     xmlns:UserControls=\"clr-namespace:{autoCodeGeneratorConfigEntity.BaseNamespace}.UserControls\"");
            sb.AppendLine($"     xmlns:Interactivity=\"clr-namespace:Reactive.Bindings.Interactivity;assembly=ReactiveProperty.NET46\"");
            sb.AppendLine($"     xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"");
            sb.AppendLine($"     xmlns:i=\"http://schemas.microsoft.com/expression/2010/interactivity\"");
            sb.AppendLine($"     xmlns:local=\"clr-namespace:{autoCodeGeneratorConfigEntity.XamlNamespace}\"");
            sb.AppendLine($"     xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"");
            sb.AppendLine($"     x:Name=\"View{autoCodeGeneratorConfigEntity.XamlName}\"");
            sb.AppendLine($"     Title=\"\"");
            sb.AppendLine($"     Width =\"1200\"");
            sb.AppendLine($"     Height=\"680\"");
            sb.AppendLine($"     WindowStartupLocation=\"CenterScreen\"");
            sb.AppendLine($"     mc:Ignorable=\"d\">");
            sb.AppendLine($"     <Window.Resources>");
            sb.AppendLine($"         <ResourceDictionary>");
            sb.AppendLine($"             <ResourceDictionary.MergedDictionaries>");
            sb.AppendLine($"                 <ResourceDictionary Source=\"/Style/StyleDic.xaml\" />");
            sb.AppendLine($"             </ResourceDictionary.MergedDictionaries>");
            sb.AppendLine($"         </ResourceDictionary>");
            sb.AppendLine($"     </Window.Resources>");
            sb.AppendLine($"     <i:Interaction.Triggers>");
            sb.AppendLine($"         <i:EventTrigger EventName=\"Loaded\">");
            sb.AppendLine($"             <i:InvokeCommandAction Command=\"{{Binding LoadedCommand}}\" />");
            sb.AppendLine($"         </i:EventTrigger>");
            sb.AppendLine($"     </i:Interaction.Triggers>");
            sb.AppendLine($"     ");
            sb.AppendLine($"     <Grid Style=\"{{StaticResource grid-defalut}}\">");
            sb.AppendLine($"         <StackPanel>");
            sb.AppendLine($"             <UserControls:CustomMenu ThisWindow=\"{{ Binding ElementName = View{autoCodeGeneratorConfigEntity.XamlName}}}\" />");
            sb.AppendLine($" ");
            sb.AppendLine($"             <DockPanel>");
            sb.AppendLine($"                 <StackPanel Style=\"{{StaticResource stackpanel-header-left}}\">");
            sb.AppendLine($"                     <Label Content=\"{autoCodeGeneratorConfigEntity.Title}\" Style=\"{{StaticResource label-title}}\" />");
            sb.AppendLine($"                 </StackPanel>");
            sb.AppendLine($" ");
            sb.AppendLine($"                 <StackPanel Style=\"{{StaticResource stackpanel-header-right}}\">");
            sb.AppendLine($"                     <StackPanel Style=\"{{StaticResource stackpanel-header-button}}\">");
            sb.AppendLine($"                         <Button Style=\"{{StaticResource btn-new}}\" />");
            sb.AppendLine($"                         <Button Style=\"{{StaticResource btn-delete}}\" />");
            sb.AppendLine($"                         <Button Style=\"{{StaticResource btn-registration}}\" />");
            sb.AppendLine($"                     </StackPanel>");
            sb.AppendLine($"                 </StackPanel>");
            sb.AppendLine($"            </DockPanel>");
            sb.AppendLine($" ");
            sb.AppendLine($"         <StackPanel Style=\"{{StaticResource stackpanel-body}}\">");
            sb.AppendLine($"             <StackPanel>");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetXamlCodeSearch());
            sb.AppendLine($"                <StackPanel Style=\"{{ StaticResource stackpanel-defalut}}\">");
            sb.AppendLine($"                    <Button Style=\"{{StaticResource button-search}}\" />");
            sb.AppendLine($"                </StackPanel>");
            sb.AppendLine($"");
            sb.AppendLine($"                <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">");
            sb.AppendLine($"                    <DataGrid");
            sb.AppendLine($"                        x:Name=\"dataGrid1\"");
            sb.AppendLine($"                        Width=\"280\"");
            sb.AppendLine($"                        Height=\"450\"");
            sb.AppendLine($"                        Style=\"{{StaticResource grid-search}}\">");
            sb.AppendLine($"                        <i:Interaction.Triggers>");
            sb.AppendLine($"                            <i:EventTrigger EventName=\"MouseDoubleClick\">");
            sb.AppendLine($"                                <i:InvokeCommandAction Command=\"{{Binding DataGridMouseDoubleClickComand}}\" />");
            sb.AppendLine($"                            </i:EventTrigger>");
            sb.AppendLine($"                        </i:Interaction.Triggers>");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetXamlCodeSearchGrid());
            sb.AppendLine($"                    </DataGrid>");
            sb.AppendLine($"                </StackPanel>");
            sb.AppendLine($"             </StackPanel>");
            sb.AppendLine($" ");
            sb.AppendLine($"             <StackPanel Margin=\"10,0,0,0\">");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetXamlCode());
            sb.AppendLine(autoCodeGeneratorDetailInfoEntitysList.Select(e => e.GetXamlCode()).ConcatWith(""));
            sb.AppendLine($" ");
            sb.AppendLine($"                 <StackPanel Style=\"{{StaticResource stackpanel-defalut}}\">");
            sb.AppendLine($"                     <Border Style=\"{{StaticResource border-errormessage}}\">");
            sb.AppendLine($"                         <TextBox Style=\"{{StaticResource textbox-errormessage}}\" />");
            sb.AppendLine($"                     </Border>");
            sb.AppendLine($"                 </StackPanel>");
            sb.AppendLine($"             </StackPanel>");
            sb.AppendLine($"         </StackPanel>");
            sb.AppendLine($"         </StackPanel>");
            sb.AppendLine($"     </Grid>");
            sb.AppendLine($" </CustomContorol:WindowBase>");
            sb.AppendLine($" ");

            XamlCode.Value = sb.ToString();
        }

        private void SetDaoCode()
        {
            DaoCode.Value = autoCodeGeneratorBaseInfoEntitys.GetDaoCode();

            DetailDaoCode.Value = autoCodeGeneratorDetailInfoEntitysList.Select(e => e.GetDaoCode()).ConcatWith(Environment.NewLine);
        }

        private void SetViewModelCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"【{autoCodeGeneratorConfigEntity.XamlName}ViewModel.cs】");
            sb.AppendLine($"using ExtensionsLibrary;");
            sb.AppendLine($"using Newtonsoft.Json;");
            sb.AppendLine($"using Reactive.Bindings;");
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"using System.Linq;");
            sb.AppendLine($"using {autoCodeGeneratorConfigEntity.BaseNamespace}.Common;");
            sb.AppendLine($"using {autoCodeGeneratorConfigEntity.DaoNamespace};");
            sb.AppendLine($"using {autoCodeGeneratorConfigEntity.BaseNamespace}.Framework;");
            sb.AppendLine($"using {autoCodeGeneratorConfigEntity.EntityNamespace};");
            sb.AppendLine($"using {autoCodeGeneratorConfigEntity.SearchEntityNamespace};");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {autoCodeGeneratorConfigEntity.ViewModelNamespace}");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {autoCodeGeneratorConfigEntity.XamlName}ViewModel : MasterViewModelBase<{autoCodeGeneratorConfigEntity.TableName}Entity>");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        public " + autoCodeGeneratorConfigEntity.XamlName + "ViewModel()");
            sb.AppendLine($"        {{");
            sb.Append(GetViewModelCodeDetailInfoConstructor());
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetViewModelCodePropertyAndCommand());
            sb.Append(GetViewModelCodeDetailInfoPropertyAndCommand());
            sb.AppendLine($"");
            sb.AppendLine($"        protected override void Load()");
            sb.AppendLine($"        {{");
            sb.Append(GetViewModelCodeDetailInfoLoad());
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        protected override void SetSearchResultEntitys()");
            sb.AppendLine($"        {{");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetViewModelCodeSetSearchResultEntitys());
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        protected override void Save()");
            sb.AppendLine($"        {{");
            sb.AppendLine(autoCodeGeneratorDetailInfoEntitysList.Select(e => e.GetViewModelCodeSavePreprocessing()).ConcatWith(Environment.NewLine));
            sb.AppendLine($"");
            sb.AppendLine($"            var errorMessage = new List<string>();");
            sb.AppendLine($"");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetViewModelCodeSaveErrorMessage());
            sb.Append(GetViewModelCodeDetailInfoSaveErrorMessage());
            sb.AppendLine($"            if (errorMessage.Count() > 0) throw new SaveErrorMessageExcenption(errorMessage.ConcatWith(Environment.NewLine);");
            sb.AppendLine($"");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetViewModelCodeSave());
            sb.Append(GetViewModelCodeDetailInfoSave());
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        protected override void Delete()");
            sb.AppendLine($"        {{");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetViewModelCodeDelete());
            sb.Append(GetViewModelCodeDetailInfoDelete());
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        protected override void SetEditDataToInsert()");
            sb.AppendLine($"        {{");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetViewModelCodeSetEditDataToInsert());
            sb.Append(GetViewModelCodeDetailInfoSetEditDataToInsert());
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        protected override void SetEditDataToUpdate({autoCodeGeneratorConfigEntity.TableName}Entity selectEntity)");
            sb.AppendLine($"        {{");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetViewModelCodeSetEditDataToUpdate());
            sb.Append(GetViewModelCodeDetailInfoSetEditDataToUpdate());
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        protected override string GetJsonEditData()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var jsonEditdata = \"\";");
            sb.AppendLine(autoCodeGeneratorBaseInfoEntitys.GetViewModelGetJsonEditData());
            sb.Append(GetViewModelCodeDetailInfoGetJsonEditData());
            sb.AppendLine($"");
            sb.AppendLine($"            return jsonEditdata;");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.Append(GetViewModelCodeDetailInfoCommandAction());
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            ViewModelCode.Value = sb.ToString();
        }

        private void SetAppDbContextCode()
        {
            AppDbContextCode.Value = autoCodeGeneratorBaseInfoEntitys.GetAppDbContextCode() ;
            AppDbContextCode.Value += Environment.NewLine + Environment.NewLine + Environment.NewLine;
            AppDbContextCode.Value += autoCodeGeneratorDetailInfoEntitysList.Select(e => e.GetAppDbContextCode()).ConcatWith(Environment.NewLine);
        }

        private string GetViewModelCodeDetailInfoConstructor()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeConstructor())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoPropertyAndCommand()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodePropertyAndCommand())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoLoad()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeLoad())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoSaveErrorMessage()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeSaveErrorMessage())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoSave()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeSave())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoDelete()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeDelete())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoSetEditDataToInsert()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeSetEditDataToInsert())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoSetEditDataToUpdate()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeSetEditDataToUpdate())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoGetJsonEditData()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeGetJsonEditData())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private string GetViewModelCodeDetailInfoCommandAction()
        {
            var returnValue = autoCodeGeneratorDetailInfoEntitysList
                .Select(e => e.GetViewModelCodeCommandAction())
                .ConcatWith(Environment.NewLine);
            if (returnValue != "") returnValue += Environment.NewLine;
            return returnValue;
        }

        private void SetXamlcsCode()
        {
            XamlcsCode.Value = autoCodeGeneratorBaseInfoEntitys.GetXamlcsCode();
        }

        private void ExportProgramCodeFile(string namespaceString ,string outputText)
        {
            var array = outputText.Split('【');
            foreach(var text in array)
            {
                if (text.Length == 0) continue;

                var array2 = text.Split('】');

                var filePath = ProgramFolderPath.Value + "\\" + namespaceString.Replace(".", "\\") + "\\" + array2[0];

                if (File.Exists(filePath)) File.Delete(filePath);

                File.WriteAllText(filePath, array2[1]);
            }
        }
    }
}
