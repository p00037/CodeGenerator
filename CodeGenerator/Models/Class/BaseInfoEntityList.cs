using CodeGenerator.Common;
using CodeGenerator.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator.ViewModel
{
    public class BaseInfoEntityList : InfoBaseList<BaseInfoEntity>
    {
        public ConfigEntity Config { get; set; }
        public void SetConfig(ConfigEntity Config)
        {
            this.Config = Config;
        }

        public string GetEntityCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("【{0}Entity.cs】", Config.TableName));
            sb.AppendLine(string.Format("using System;"));
            sb.AppendLine(string.Format("using System.Collections.Generic;"));
            sb.AppendLine(string.Format("using System.Linq;"));
            sb.AppendLine(string.Format("using System.ComponentModel.DataAnnotations;"));
            sb.AppendLine(string.Format("using System.ComponentModel.DataAnnotations.Schema;"));
            sb.AppendLine(string.Format("using {0}.Framework;", Config.BaseNamespace));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("namespace {0}", Config.EntityNamespace));
            sb.AppendLine(string.Format("{{"));
            sb.AppendLine(string.Format("    public class {0}Entity : DBEntityBase", Config.TableName));
            sb.AppendLine(string.Format("    {{"));
            sb.AppendLine(this.Select(e => e.GetEntitiyCode())
                              .ToList()
                              .ConcatWith(Environment.NewLine + Environment.NewLine));
            sb.AppendLine(string.Format("    }}"));
            sb.AppendLine(string.Format("}}"));
            return sb.ToString();
        }

        public string GetSearchEntityCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("【{0}SearchEntity.cs】", Config.XamlName));
            sb.AppendLine(string.Format("using System;"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("namespace {0}.Models.SearchEntity", Config.BaseNamespace));
            sb.AppendLine(string.Format("{{"));
            sb.AppendLine(string.Format("    public class {0}SearchEntity", Config.XamlName));
            sb.AppendLine(string.Format("    {{"));
            sb.AppendLine(this.Where(e => e.ComparisonMethod != null)
                              .Select(e => e.GetSearchEntitiyCode())
                              .ToList()
                              .ConcatWith(Environment.NewLine));
            sb.AppendLine(string.Format("    }}"));
            sb.AppendLine(string.Format("}}"));
            return sb.ToString();
        }

        public string GetXamlCode()
        {
            return this
                .Where(e => e.NothingCreateControlFlg == false)
                .OrderBy(e => e.Order)
                .Select(e => e.GetXamlCode())
                .ToList()
                .ConcatWith(Environment.NewLine);
        }

        public string GetXamlCodeSearch()
        {
            return this
                .Where(e => e.ComparisonMethod != null)
                .Select(e => e.GetXamlCodeSearch())
                .ToList()
                .ConcatWith(Environment.NewLine);
        }

        public string GetXamlCodeSearchGrid()
        {
            var list = new List<string>();
            list.Add(string.Format("                        <DataGrid.Columns>"));
            list.Add(this.Where(e => e.DisplayGridForSearch == true)
                         .Select(e => e.GetXamlCodeSearchGrid())
                         .ToList()
                         .ConcatWith(Environment.NewLine));
            list.Add(string.Format("                        </DataGrid.Columns>"));
            return list.ConcatWith(Environment.NewLine);
        }

        public string GetXamlcsCode()
        {
            var list = new List<string>();
            list.Add(string.Format("【{0}.xaml.cs】", Config.XamlName));
            list.Add(string.Format("using System.Windows;"));
            list.Add(string.Format("using WpfControlLibrary.CustomContorol;"));
            list.Add(string.Format("using {0};",Config.ViewModelNamespace));
            list.Add(string.Format(""));
            list.Add(string.Format("namespace {0}",Config.XamlNamespace));
            list.Add(string.Format("{{"));
            list.Add(string.Format("    public partial class {0} : WindowBase", Config.XamlName));
            list.Add(string.Format("    {{"));
            list.Add(string.Format("        {0}ViewModel viewModel = new {0}ViewModel();",Config.XamlName));
            list.Add(string.Format("        public {0}()", Config.XamlName));
            list.Add(string.Format("        {{"));
            list.Add(string.Format("            InitializeComponent();"));
            list.Add(string.Format(""));
            list.Add(string.Format("            DataContext = this.viewModel;"));
            list.Add(string.Format("        }}"));
            list.Add(string.Format("    }}"));
            list.Add(string.Format("}}"));
            return list.ConcatWith(Environment.NewLine);
        }

        public string GetDaoCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("【Dao{0}.cs】", Config.TableName));
            sb.AppendLine(string.Format("using ExtensionsLibrary;"));
            sb.AppendLine(string.Format("using System.Collections.Generic;"));
            sb.AppendLine(string.Format("using System.Linq;"));
            sb.AppendLine(string.Format("using {0}.Common;", Config.BaseNamespace));
            sb.AppendLine(string.Format("using {0}.Framework;", Config.BaseNamespace));
            sb.AppendLine(string.Format("using {0};", Config.EntityNamespace));
            sb.AppendLine(string.Format("using {0};", Config.SearchEntityNamespace));
            sb.AppendLine(string.Format("using static {0}.Common.ComEnum;", Config.BaseNamespace));
            sb.AppendLine(string.Format("using static {0}.Common.DB;", Config.BaseNamespace));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("namespace {0}", Config.DaoNamespace));
            sb.AppendLine(string.Format("{{"));
            sb.AppendLine(string.Format("    public class Dao{0} : DaoBase ", Config.TableName));
            sb.AppendLine(string.Format("    {{"));
            sb.AppendLine(string.Format("        public List<{0}Entity> Get{0}List()", Config.TableName));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            return Get{0}List(new {1}SearchEntity());", Config.TableName, Config.XamlName));
            sb.AppendLine(string.Format("        }}"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public List<{0}Entity> Get{0}List({1}SearchEntity searchData)", Config.TableName, Config.XamlName));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            return this.context.{0}Entitys", Config.TableName));
            sb.AppendLine(this.Where(e => e.ComparisonMethod != null)
                              .Select(e => e.GetSearchWherePartCode())
                              .ToList()
                              .ConcatWith(Environment.NewLine));
            sb.AppendLine(string.Format("                .ToList();"));
            sb.AppendLine(string.Format("        }}"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public {0}Entity Get{0}({1})", Config.TableName, GetArgumentString()));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            return this.context.{0}Entitys.Find({1});", Config.TableName, GetColumnName("")));
            sb.AppendLine(string.Format("        }}"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public void Save(EnmEditMode editMode, {0}Entity data)", Config.TableName));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            var contexForSave = CreateAppDbContextForSave();"));
            sb.AppendLine(string.Format("            contexForSave.Entry(data).State = ConvertEnmEditModeToEntityState(editMode);"));
            sb.AppendLine(string.Format("        }}"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public List<string> GetErrorMessage(EnmEditMode editMode, {0}Entity data)", Config.TableName));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            if (editMode == EnmEditMode.Delete) return new List<string>();"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("            var errorMessages = GetErrorMessageEntityValidation(data);"));
            sb.AppendLine(string.Format("            if (editMode == EnmEditMode.Insert)"));
            sb.AppendLine(string.Format("            {{"));
            sb.AppendLine(string.Format("                if (this.Get{0}({1}) != null)", Config.TableName, GetColumnName("data.")));
            sb.AppendLine(string.Format("                {{"));
            sb.AppendLine(string.Format("                    errorMessages.Add(\"{0}は既に登録されています。\");", GetLabelName()));
            sb.AppendLine(string.Format("                }}"));
            sb.AppendLine(string.Format("            }}"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("            return errorMessages;"));
            sb.AppendLine(string.Format("        }}"));
            sb.AppendLine(string.Format("    }}"));
            sb.AppendLine(string.Format("}}"));
            return sb.ToString();
        }

        public string GetViewModelCodePropertyAndCommand()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("        public ReactiveProperty<List<{0}Entity>> SearchResultEntitys {{ get; set; }} = new ReactiveProperty<List<{0}Entity>>(new List<{0}Entity>());", Config.TableName));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public ReactiveProperty<{0}Entity> EditData {{ get; set; }} = new ReactiveProperty<{0}Entity>(new {0}Entity());", Config.TableName));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public ReactiveProperty<{1}SearchEntity> SearchOptionEntity {{ get; set; }} = new ReactiveProperty<{1}SearchEntity>(new {1}SearchEntity());", Config.TableName, Config.XamlName));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(GetComboProperty());
            return sb.ToString().TrimEnd('\r', '\n'); ;
        }

        public string GetAppDbContextCode()
        {
            var list = new List<string>();
            list.Add(string.Format("public DbSet<{0}Entity> {0}Entitys {{ get; set; }}", Config.TableName));
            list.Add(string.Format(""));
            list.Add(string.Format("modelBuilder.Entity<{0}Entity>().ToTable(\"{0}\").HasKey(c => new {{ {1} }});", Config.TableName, GetColumnName("c.")));
            return list.ConcatWith(Environment.NewLine);
        }

        private string GetComboProperty()
        {
            return this
                .Where(e => e.ComboBoxFlg == true)
                .Select(e => e.GetComboProperty())
                .ConcatWith(Environment.NewLine + Environment.NewLine);
        }

        public string GetViewModelCodeLoad()
        {
            return this
                .Where(e => e.ComboBoxFlg == true)
                .Select(e => e.GetViewModelCodeLoad())
                .ConcatWith(Environment.NewLine + Environment.NewLine);
        }

        public string GetViewModelCodeSetSearchResultEntitys()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("            var dao = new Dao{0}();", Config.TableName));
            sb.AppendLine(string.Format("            this.SearchResultEntitys.Value = dao.Get{0}List(SearchOptionEntity.Value);", Config.TableName));
            return sb.ToString().TrimEnd('\r', '\n');
        }

        public string GetViewModelCodeSaveErrorMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("            var dao{0} = new Dao{0}();", Config.TableName));
            sb.AppendLine(string.Format("            errorMessage.AddRange(dao{0}.GetErrorMessage(EditMode, this.EditData.Value));", Config.TableName));
            return sb.ToString().TrimEnd('\r', '\n');
        }

        public string GetViewModelCodeSave()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("            dao{0}.Save(EditMode, this.EditData.Value);", Config.TableName));
            return sb.ToString().TrimEnd('\r', '\n');
        }

        public string GetViewModelCodeDelete()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("            var dao{0} = new Dao{0}();", Config.TableName));
            sb.AppendLine(string.Format("            dao{0}.Save(ComEnum.EnmEditMode.Delete, this.EditData.Value);", Config.TableName ));
            return sb.ToString().TrimEnd('\r', '\n');
        }

        public string GetViewModelCodeSetEditDataToInsert()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("            EditData.Value = new {0}Entity();", Config.TableName));
            return sb.ToString().TrimEnd('\r', '\n');
        }

        public string GetViewModelCodeSetEditDataToUpdate()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("            var dao{0} = new Dao{0}();", Config.TableName));
            sb.AppendLine(string.Format("            EditData.Value = dao{0}.Get{0}({1});", Config.TableName, GetColumnName("selectEntity.")));
            return sb.ToString().TrimEnd('\r', '\n');
        }

        public string GetViewModelGetJsonEditData()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("            jsonEditdata += JsonConvert.SerializeObject(this.EditData.Value);"));
            return sb.ToString().TrimEnd('\r', '\n');
        }
    }
}
