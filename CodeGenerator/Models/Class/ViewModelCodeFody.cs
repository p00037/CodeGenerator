using CodeGenerator.Common;
using CodeGenerator.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator.Models.Class
{
    public class ViewModelCodeFody
    {
        private BaseInfoEntityList baseInfoEntityList = new BaseInfoEntityList();

        private List<DetailInfoEntityList> detailInfoEntitysLists = new List<DetailInfoEntityList>();

        public ViewModelCodeFody(BaseInfoEntityList baseInfoEntityList, List<DetailInfoEntityList> detailInfoEntitysLists)
        {
            this.baseInfoEntityList = baseInfoEntityList;
            this.detailInfoEntitysLists = detailInfoEntitysLists;
        }

        public string CreateViewModelCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"【{baseInfoEntityList.Config.XamlName}ViewModel.cs】");
            sb.AppendLine($"using {baseInfoEntityList.Config.BaseNamespace}.Common;");
            sb.AppendLine($"using {baseInfoEntityList.Config.BaseNamespace}.Factory;");
            sb.AppendLine($"using {baseInfoEntityList.Config.BaseNamespace}.Framework;");
            sb.AppendLine($"using {baseInfoEntityList.Config.DBNamespace}.Entity;");
            sb.AppendLine($"using {baseInfoEntityList.Config.DBNamespace}.Factory;");
            sb.AppendLine($"using {baseInfoEntityList.Config.DBNamespace}.OriginalException;");
            sb.AppendLine($"using {baseInfoEntityList.Config.DBNamespace}.SearchEntity;");
            sb.AppendLine($"using Reactive.Bindings;");
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"using System.Collections.ObjectModel;");
            sb.AppendLine($"using System.Linq;");
            sb.AppendLine($"using System.Windows.Controls;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace JIRITSU.ViewModel");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {baseInfoEntityList.Config.XamlName}ViewModel : MasterViewModelBase<{baseInfoEntityList.Config.TableName}Entity>");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        public {baseInfoEntityList.Config.XamlName}ViewModel()");
            sb.AppendLine($"        {{");
            foreach (var item in detailInfoEntitysLists)
            {
                sb.AppendLine($"            BtnAdd{item.DetailConfig.GridName}RowCommand.Subscribe(_ => AddGridDetailRow());");
                sb.AppendLine($"            BtnDelete{item.DetailConfig.GridName}RowCommand.Subscribe(x => BtnDeleteGridDetailRow(x));");
            }
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        public {baseInfoEntityList.Config.XamlName}SearchEntity SearchOptionEntity {{ get; set; }} = new {baseInfoEntityList.Config.XamlName}SearchEntity();");
            sb.AppendLine($"");
            foreach (var item in baseInfoEntityList.Where(m => m.ComboBoxFlg == true))
            {
                sb.AppendLine($"        public ObservableCollection<{item.ComboBoxDataAcquisitionTable}Entity> {item.ComboBoxItemsSource} {{ get; set; }} = new ObservableCollection<{item.ComboBoxDataAcquisitionTable}Entity>();");
                sb.AppendLine($"");
            }
            foreach (var list in detailInfoEntitysLists)
            {
                foreach (var item in list.Where(m => m.ComboBoxFlg == true))
                {
                    sb.AppendLine($"        public ObservableCollection<{item.ComboBoxDataAcquisitionTable}> {item.ComboBoxItemsSource} {{ get; set; }} = new ObservableCollection<{item.ComboBoxDataAcquisitionTable}>();");
                    sb.AppendLine($"");
                }
            }
            foreach (var item in detailInfoEntitysLists)
            {
                sb.AppendLine($"        public ReactiveCommand BtnAdd{item.DetailConfig.GridName}RowCommand {{ get; }} = new ReactiveCommand();");
                sb.AppendLine($"");
                sb.AppendLine($"        public ReactiveCommand BtnDelete{item.DetailConfig.GridName}RowCommand {{ get; }} = new ReactiveCommand();");
                sb.AppendLine($"");
            }
            sb.AppendLine($"        private void Load()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            //Comboのデータセットを書いてください");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void SetSearchResultEntitys()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var service = {baseInfoEntityList.Config.XamlName}ApplicationServiceFactory.Create({baseInfoEntityList.Config.ContextFactoryName}.Create());");
            sb.AppendLine($"            this.SearchResultEntitys = new ObservableCollection<{baseInfoEntityList.Config.TableName}Entity>(service.GetList(SearchOptionEntity));");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void Save()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var service = {baseInfoEntityList.Config.XamlName}ApplicationServiceFactory.Create({baseInfoEntityList.Config.ContextFactoryName}.Create());");
            sb.AppendLine($"            if (EditMode == ComEnum.EnmEditMode.Insert)");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                service.Register(this.EditData);");
            sb.AppendLine($"            }}");
            sb.AppendLine($"            else");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                service.Update(this.EditData);");
            sb.AppendLine($"            }}");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void Delete()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var service = {baseInfoEntityList.Config.XamlName}ApplicationServiceFactory.Create({baseInfoEntityList.Config.ContextFactoryName}.Create());");
            sb.AppendLine($"            service.Delete(this.EditData);");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void SetEditDataWhenNew()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            this.EditData = new {baseInfoEntityList.Config.TableName}Entity();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void SetEditDataWhenUpdate({baseInfoEntityList.Config.TableName}Entity selectEntity)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var service = {baseInfoEntityList.Config.XamlName}ApplicationServiceFactory.Create({baseInfoEntityList.Config.ContextFactoryName}.Create());");
            var keys = baseInfoEntityList.Where(m => m.PrimaryKey == true).Select(m => "selectEntity." + m.ColumnName);
            sb.AppendLine($"            this.EditData = service.Get({keys.ConcatWith(",")});");
            sb.AppendLine($"        }}");
            foreach (var item in detailInfoEntitysLists)
            {
                sb.AppendLine($"        private void Add{item.DetailConfig.GridName}Row()");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            EditData.{item.DetailConfig.TableName}Entitys.Add(new {item.DetailConfig.TableName}Entity());");
                sb.AppendLine($"        }}");
                sb.AppendLine($"");
                sb.AppendLine($"        private void BtnDelete{item.DetailConfig.GridName}Row(object entity)");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            EditData.{item.DetailConfig.TableName}Entitys.Remove(({item.DetailConfig.TableName}Entity)entity);");
                sb.AppendLine($"        }}");
                sb.AppendLine($"");
            }
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            sb.AppendLine($"");

            return sb.ToString();
        }

        public string CreateViewModelCodeNoInheritance()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"【{baseInfoEntityList.Config.XamlName}ViewModel.cs】");
            sb.AppendLine($"using using ExtensionsLibrary;");
            sb.AppendLine($"using {baseInfoEntityList.Config.BaseNamespace}.Common;");
            sb.AppendLine($"using {baseInfoEntityList.Config.BaseNamespace}.Entity;");
            sb.AppendLine($"using {baseInfoEntityList.Config.BaseNamespace}.Factory;");
            sb.AppendLine($"using {baseInfoEntityList.Config.BaseNamespace}.Framework;");
            sb.AppendLine($"using {baseInfoEntityList.Config.DBNamespace}.Entity;");
            sb.AppendLine($"using {baseInfoEntityList.Config.DBNamespace}.Factory;");
            sb.AppendLine($"using {baseInfoEntityList.Config.DBNamespace}.OriginalException;");
            sb.AppendLine($"using {baseInfoEntityList.Config.DBNamespace}.SearchEntity;");
            sb.AppendLine($"using Reactive.Bindings;");
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"using System.Collections.ObjectModel;");
            sb.AppendLine($"using System.Linq;");
            sb.AppendLine($"using System.Windows;");
            sb.AppendLine($"using System.Windows.Controls;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace JIRITSU.ViewModel");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {baseInfoEntityList.Config.XamlName}ViewModel : ViewModelBase");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        public {baseInfoEntityList.Config.XamlName}ViewModel()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            BtnNewCommand.Subscribe(_ => BtnNewAction());");
            sb.AppendLine($"            BtnDeleteCommand.Subscribe(_ => BtnDeleteAction());");
            sb.AppendLine($"            BtnRegistrationCommand.Subscribe(_ => BtnRegistrationAction());");
            sb.AppendLine($"            LoadedCommand.Subscribe(_ => LoadAction());");
            sb.AppendLine($"            BtnSearchCommand.Subscribe(_ => SearchAction());");
            sb.AppendLine($"            DataGridMouseDoubleClickComand.Subscribe(_ => DataGridMouseDoubleClickAction());");
            foreach (var item in detailInfoEntitysLists)
            {
                sb.AppendLine($"            BtnAdd{item.DetailConfig.GridName}RowCommand.Subscribe(_ => AddGridDetailRow());");
                sb.AppendLine($"            BtnDelete{item.DetailConfig.GridName}RowCommand.Subscribe(x => BtnDeleteGridDetailRow(x));");
            }
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        public {baseInfoEntityList.Config.TableName}Entity EditData {{ get; set; }} = new {baseInfoEntityList.Config.TableName}Entity();");
            sb.AppendLine($"");
            sb.AppendLine($"        public {baseInfoEntityList.Config.XamlName}SearchEntity SearchOptionEntity {{ get; set; }} = new {baseInfoEntityList.Config.XamlName}SearchEntity();");
            sb.AppendLine($"");
            foreach (var item in baseInfoEntityList.Where(m => m.ComboBoxFlg == true))
            {
                sb.AppendLine($"        public ObservableCollection<{item.ComboBoxDataAcquisitionTable}> {item.ComboBoxItemsSource} {{ get; set; }} = new ObservableCollection<{item.ComboBoxDataAcquisitionTable}>();");
                sb.AppendLine($"");
            }
            foreach (var list in detailInfoEntitysLists)
            {
                foreach (var item in list.Where(m => m.ComboBoxFlg == true))
                {
                    sb.AppendLine($"        public ObservableCollection<{item.ComboBoxDataAcquisitionTable}> {item.ComboBoxItemsSource} {{ get; set; }} = new ObservableCollection<{item.ComboBoxDataAcquisitionTable}>();");
                    sb.AppendLine($"");
                }
            }
            sb.AppendLine($"        public ErrorEntity Error {{ get; set; }} = new ErrorEntity();");
            sb.AppendLine($"");
            sb.AppendLine($"        public EnabledEntity Enable {{ get; set; }} = new EnabledEntity(ComEnum.EnmEditMode.Insert);");
            sb.AppendLine($"");
            sb.AppendLine($"        public {baseInfoEntityList.Config.TableName}Entity SelectedItem {{ get; set; }} = new {baseInfoEntityList.Config.TableName}Entity();");
            sb.AppendLine($"");
            sb.AppendLine($"        public ReactiveCommand BtnSearchCommand {{ get; }} = new ReactiveCommand();");
            sb.AppendLine($"");
            sb.AppendLine($"        public ReactiveCommand DataGridMouseDoubleClickComand {{ get; }} = new ReactiveCommand();");
            sb.AppendLine($"");
            sb.AppendLine($"        public ReactiveCommand LoadedCommand {{ get; }} = new ReactiveCommand();");
            sb.AppendLine($"");
            sb.AppendLine($"        public ReactiveCommand BtnRegistrationCommand {{ get; }} = new ReactiveCommand();");
            sb.AppendLine($"");
            sb.AppendLine($"        public ReactiveCommand BtnNewCommand {{ get; }} = new ReactiveCommand();");
            sb.AppendLine($"");
            sb.AppendLine($"        public ReactiveCommand BtnDeleteCommand {{ get; }} = new ReactiveCommand();");
            sb.AppendLine($"");
            foreach (var item in detailInfoEntitysLists)
            {
                sb.AppendLine($"        public ReactiveCommand BtnAdd{item.DetailConfig.GridName}RowCommand {{ get; }} = new ReactiveCommand();");
                sb.AppendLine($"");
                sb.AppendLine($"        public ReactiveCommand BtnDelete{item.DetailConfig.GridName}RowCommand {{ get; }} = new ReactiveCommand();");
                sb.AppendLine($"");
            }
            sb.AppendLine($"        private string JsonBeforeEditData {{ get; set; }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private ComEnum.EnmEditMode EditMode {{ get; set; }} = ComEnum.EnmEditMode.Insert;");
            sb.AppendLine($"");
            sb.AppendLine($"        private void LoadAction()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            SetSearchResultEntitys();");
            sb.AppendLine($"            Load();");
            sb.AppendLine($"            ChangeEditModeToInsert();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void SearchAction()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            SetSearchResultEntitys();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void DataGridMouseDoubleClickAction()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            ChangeEditData();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        public void BtnRegistrationAction()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            try");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                this.Error = new ErrorEntity();");
            sb.AppendLine($"                Save();");
            sb.AppendLine($"                SetSearchResultEntitys();");
            sb.AppendLine($"                ChangeEditModeToInsert();");
            sb.AppendLine($"                MessageBox.Show(ComMessage.RESISTERED);");
            sb.AppendLine($"            }}");
            sb.AppendLine($"            catch (SaveErrorMessageExcenption ex)");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                Error = new ErrorEntity(ex.Message);");
            sb.AppendLine($"            }}");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void BtnNewAction()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            ChangeEditModeToInsert();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void BtnDeleteAction()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            Delete();");
            sb.AppendLine($"            SetSearchResultEntitys();");
            sb.AppendLine($"            ChangeEditModeToInsert();");
            sb.AppendLine($"            MessageBox.Show(ComMessage.DELETED);");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void ChangeEditModeToInsert()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            this.Error = new ErrorEntity();");
            sb.AppendLine($"            this.EditMode = ComEnum.EnmEditMode.Insert;");
            sb.AppendLine($"            this.Enable = new EnabledEntity(ComEnum.EnmEditMode.Insert);");
            sb.AppendLine($"            SetEditDataWhenNew();");
            sb.AppendLine($"            SetBeforeEditData();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void ChangeEditModeToUpdate({baseInfoEntityList.Config.TableName}Entity selectEntity)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            this.Error = new ErrorEntity();");
            sb.AppendLine($"            this.EditMode = ComEnum.EnmEditMode.Update;");
            sb.AppendLine($"            this.Enable = new EnabledEntity(ComEnum.EnmEditMode.Update);");
            sb.AppendLine($"            SetEditDataWhenUpdate(selectEntity);");
            sb.AppendLine($"            SetBeforeEditData();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void SetBeforeEditData()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            this.JsonBeforeEditData = this.EditData.Json();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void ChangeEditData()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            if (SelectedItem == null) return;");
            sb.AppendLine($"");
            sb.AppendLine($"            if (ViewModelBase.ConfirmChangEditData(JsonBeforeEditData,this.EditData) == false) return;");
            sb.AppendLine($"");
            sb.AppendLine($"            ChangeEditModeToUpdate(SelectedItem);");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void Load()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            //Comboのデータセットを書いてください");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void SetSearchResultEntitys()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var service = {baseInfoEntityList.Config.XamlName}ApplicationServiceFactory.Create({baseInfoEntityList.Config.ContextFactoryName}.Create());");
            sb.AppendLine($"            this.SearchResultEntitys = new ObservableCollection<{baseInfoEntityList.Config.TableName}Entity>(service.GetList(SearchOptionEntity));");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void Save()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var service = {baseInfoEntityList.Config.XamlName}ApplicationServiceFactory.Create({baseInfoEntityList.Config.ContextFactoryName}.Create());");
            sb.AppendLine($"            if (EditMode == ComEnum.EnmEditMode.Insert)");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                service.Register(this.EditData);");
            sb.AppendLine($"            }}");
            sb.AppendLine($"            else");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                service.Update(this.EditData);");
            sb.AppendLine($"            }}");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void Delete()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var service = {baseInfoEntityList.Config.XamlName}ApplicationServiceFactory.Create({baseInfoEntityList.Config.ContextFactoryName}.Create());");
            sb.AppendLine($"            service.Delete(this.EditData);");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void SetEditDataWhenNew()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            this.EditData = new {baseInfoEntityList.Config.TableName}Entity();");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        private void SetEditDataWhenUpdate({baseInfoEntityList.Config.TableName}Entity selectEntity)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var service = {baseInfoEntityList.Config.XamlName}ApplicationServiceFactory.Create({baseInfoEntityList.Config.ContextFactoryName}.Create());");
            var keys = baseInfoEntityList.Where(m => m.PrimaryKey == true).Select(m => "selectEntity." + m.ColumnName);
            sb.AppendLine($"            this.EditData = service.Get({keys.ConcatWith(",")});");
            sb.AppendLine($"        }}");
            foreach (var item in detailInfoEntitysLists)
            {
                sb.AppendLine($"        private void Add{item.DetailConfig.GridName}Row()");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            this.EditData.{item.DetailConfig.TableName}Entitys.Add(new {item.DetailConfig.TableName}Entity());");
                sb.AppendLine($"        }}");
                sb.AppendLine($"");
                sb.AppendLine($"        private void BtnDelete{item.DetailConfig.GridName}Row(object entity)");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            this.EditData.{item.DetailConfig.TableName}Entitys.Remove(({item.DetailConfig.TableName}Entity)entity);");
                sb.AppendLine($"        }}");
                sb.AppendLine($"");
            }
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            sb.AppendLine($"");

            return sb.ToString();
        }
    }
}
