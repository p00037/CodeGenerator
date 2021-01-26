using LinqToExcel;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeGenerator.Common;
using CodeGenerator.Models.Entity;
using CodeGenerator.Views;
using System.IO;
using CodeGenerator.Models.Class;

namespace CodeGenerator.ViewModel
{
    public class AutoCodeGeneratorFodyViewModel
    {
        private WpfFody window;

        private ConfigEntity configEntity;

        private List<DetailConfigEntity> detailConfigEntitys;

        private BaseInfoEntityList baseInfoEntityList = new BaseInfoEntityList();

        private List<DetailInfoEntityList> detailInfoEntityLists = new List<DetailInfoEntityList>();

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

        public ReactiveProperty<string> ViewModelCodeNoInheritance { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> AppDbContextCode { get; set; } = new ReactiveProperty<string>();

        public ReactiveCommand BtnViewFileDaialogCommand { get; } = new ReactiveCommand();

        public ReactiveCommand BtnViewProgramFolderDaialogCommand { get; } = new ReactiveCommand();

        public ReactiveCommand BtnExecCommand { get; } = new ReactiveCommand();

        public AutoCodeGeneratorFodyViewModel(WpfFody window)
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
            SetConfigEntitys();
            SetEntityCode();
            SetXamlCode();
            SetDaoCode();
            SetViewModelCode();
            SetXamlcsCode();
            SetAppDbContextCode();

            if (ProgramFolderPath.Value.NullToValue("") != "")
            {
                ExportProgramCodeFile(configEntity.BaseNamespace + ".Infrastructure", configEntity.EntityNamespace, EntityCode.Value);
                ExportProgramCodeFile(configEntity.BaseNamespace + ".Infrastructure", configEntity.EntityNamespace, DetailEntityCode.Value);
                ExportProgramCodeFile(configEntity.BaseNamespace + ".Infrastructure", configEntity.SearchEntityNamespace, SearchEntityCode.Value);
                ExportProgramCodeFile(configEntity.BaseNamespace, configEntity.XamlNamespace, XamlCode.Value);
                ExportProgramCodeFile(configEntity.BaseNamespace, configEntity.XamlNamespace, XamlcsCode.Value);
                ExportProgramCodeFile(configEntity.BaseNamespace + ".Infrastructure", configEntity.DaoNamespace, DaoCode.Value);
                ExportProgramCodeFile(configEntity.BaseNamespace + ".Infrastructure", configEntity.DaoNamespace, DetailDaoCode.Value);
                ExportProgramCodeFile(configEntity.BaseNamespace, configEntity.ViewModelNamespace, ViewModelCode.Value);
            }

            System.Windows.MessageBox.Show("完了");
        }

        private void SetConfigEntitys()
        {
            baseInfoEntityList = new BaseInfoEntityList();
            detailInfoEntityLists = new List<DetailInfoEntityList>();

            var excel = new ExcelQueryFactory(FilePath.Value);
            this.configEntity = excel.Worksheet<ConfigEntity>("設定情報").FirstOrDefault();
            this.baseInfoEntityList.AddRange(excel.Worksheet<BaseInfoEntity>("基本テーブル情報").ToList());
            this.baseInfoEntityList.SetConfig(configEntity);
            this.detailConfigEntitys = excel.Worksheet<DetailConfigEntity>("明細設定情報").ToList();
            for (int i = 1; i <= configEntity.DetailCount; i++)
            {
                var list = new DetailInfoEntityList();
                list.AddRange(excel.Worksheet<DetailInfoEntity>("明細テーブル情報" + i.ToString()).ToList());
                list.SetConfig(configEntity, detailConfigEntitys[i - 1]);

                detailInfoEntityLists.Add(list);
            }

            SetTextBoxWidthAndXamlName();
        }

        private void SetTextBoxWidthAndXamlName()
        {
            foreach (var entity in baseInfoEntityList.Where(e => e.GetMaxLength() > 0).ToList())
            {
                var str = (entity.IsHalfSize() ? 'ｱ' : 'ア');
                window.TextBox1.Text = new string(str, entity.GetMaxLength());
                window.TextBox1.UpdateLayout();
                entity.TextBoxWidth = (int)window.TextBox1.ActualWidth + 2;
            }

            foreach (var item in detailInfoEntityLists.Select((v, i) => new { v, i }))
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

                item.v.ForEach(e => e.XamlName = configEntity.XamlName);
            }
        }

        private void SetEntityCode()
        {
            var entityCode = new EntityCode(this.configEntity, this.baseInfoEntityList);
            EntityCode.Value = entityCode.CreateEntityCode();
            SearchEntityCode.Value = entityCode.CreateSearchEntityCode();

            for (int i = 0; i < detailInfoEntityLists.Count() -1; i++)
            {
                var detailEntityCode = new DetailEntityCode(this.configEntity, this.detailConfigEntitys[i], this.detailInfoEntityLists[i]);
                DetailEntityCode.Value += detailEntityCode.CreateEntityCode();
            }
        }

        private void SetXamlCode()
        {
            var xamlCodeFody = new XamlCodeFody(baseInfoEntityList, detailInfoEntityLists);
            XamlCode.Value = xamlCodeFody.CreateXamlCode();
        }

        private void SetDaoCode()
        {
            DaoCode.Value = baseInfoEntityList.GetDaoCode();
            DetailDaoCode.Value = detailInfoEntityLists.Select(e => e.GetDaoCode()).ConcatWith(Environment.NewLine);
        }

        private void SetViewModelCode()
        {
            var viewModelCodeFody = new ViewModelCodeFody(baseInfoEntityList, detailInfoEntityLists);
            ViewModelCode.Value = viewModelCodeFody.CreateViewModelCode();
            ViewModelCodeNoInheritance.Value = viewModelCodeFody.CreateViewModelCodeNoInheritance();
        }

        private void SetAppDbContextCode()
        {
            AppDbContextCode.Value = baseInfoEntityList.GetAppDbContextCode();
            AppDbContextCode.Value += Environment.NewLine + Environment.NewLine + Environment.NewLine;
            AppDbContextCode.Value += detailInfoEntityLists.Select(e => e.GetAppDbContextCode()).ConcatWith(Environment.NewLine);
        }

        private void SetXamlcsCode()
        {
            XamlcsCode.Value = baseInfoEntityList.GetXamlcsCode();
        }

        private void ExportProgramCodeFile(string projectNameSpace, string namespaceString, string outputText)
        {
            if (outputText == null) return;

            var array = outputText.Split('【');
            foreach (var text in array)
            {
                if (text.Length == 0) continue;

                var array2 = text.Split('】');
                var fileName = array2[0];
                var code = array2[1];
                var folderPath = $"{ProgramFolderPath.Value}\\{projectNameSpace}\\{namespaceString.Replace(projectNameSpace, "").Replace(".", "\\")}";
                var filePath = folderPath + "\\" + fileName;

                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo di = new DirectoryInfo(folderPath);
                    di.Create();
                }

                if (File.Exists(filePath)) File.Delete(filePath);

                File.WriteAllText(filePath, code);
            }
        }
    }
}
