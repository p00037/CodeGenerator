using CodeGenerator.Common;
using CodeGenerator.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator.ViewModel
{
    public class DetailInfoEntityList : InfoBaseList<DetailInfoEntity>
    {
        private ConfigEntity config;

        public DetailConfigEntity DetailConfig { get; set; }

        public void SetConfig(ConfigEntity config,DetailConfigEntity DetailConfig)
        {
            this.config = config;
            this.DetailConfig = DetailConfig;
        }

        public string GetEntityCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("【{0}Entity.cs】", DetailConfig.TableName));
            sb.AppendLine(string.Format("using System;"));
            sb.AppendLine(string.Format("using System.Collections.Generic;"));
            sb.AppendLine(string.Format("using System.Linq;"));
            sb.AppendLine(string.Format("using System.ComponentModel.DataAnnotations;"));
            sb.AppendLine(string.Format("using System.ComponentModel.DataAnnotations.Schema;"));
            sb.AppendLine(string.Format("using {0}.Framework;", config.BaseNamespace));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("namespace {0}", DetailConfig.EntityNamespace));
            sb.AppendLine(string.Format("{{"));
            sb.AppendLine(string.Format("    public class {0}Entity : DBEntityBase", DetailConfig.TableName));
            sb.AppendLine(string.Format("    {{"));
            sb.AppendLine(this.Select(e => e.GetEntitiyCode())
                              .ToList()
                              .ConcatWith(Environment.NewLine + Environment.NewLine));
            sb.AppendLine(string.Format("    }}"));
            sb.AppendLine(string.Format("}}"));
            return sb.ToString();
        }

        public string GetXamlCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine(String.Format( Environment.NewLine));
            sb.AppendLine(String.Format("                <StackPanel Style=\"{{StaticResource stackpanel-orientation-vertical}}\">"));
            sb.AppendLine(String.Format("                    <DataGrid"));
            sb.AppendLine(String.Format("                        Width=\"800\""));
            sb.AppendLine(String.Format("                        Height=\"200\""));
            sb.AppendLine(String.Format("                        Behavior:DataGridBehaviors.IsInput=\"True\""));
            sb.AppendLine(String.Format("                        ItemsSource=\"{{Binding Edit{0}Data.Value}}\"", DetailConfig.GridName));
            sb.AppendLine(String.Format("                        Style=\"{{StaticResource grid-input}}\">"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("                        <DataGrid.Columns>"));
            sb.AppendLine(String.Format("                            <DataGridTemplateColumn Width=\"40\">"));
            sb.AppendLine(String.Format("                                <DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(String.Format("                                    <DataTemplate>"));
            sb.AppendLine(String.Format("                                        <Button"));
            sb.AppendLine(String.Format("                                            Width=\"40\""));
            sb.AppendLine(String.Format("                                            Command=\"{{Binding DataContext.BtnDelete{0}RowCommand, ElementName = View{1}}}\"",DetailConfig.GridName, config.XamlName));
            sb.AppendLine(String.Format("                                            CommandParameter=\"{{Binding}}\""));
            sb.AppendLine(String.Format("                                            Content=\"削除\""));
            sb.AppendLine(String.Format("                                            Style=\"{{ StaticResource button-grid}}\" />"));
            sb.AppendLine(String.Format("                                    </DataTemplate>"));
            sb.AppendLine(String.Format("                                </DataGridTemplateColumn.CellTemplate>"));
            sb.AppendLine(String.Format("                            </DataGridTemplateColumn>"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(this.Where(e => e.NothingCreateControlFlg == false)
                                    .OrderBy(e => e.Order)
                                    .Select(e => e.GetXamlCode())
                                    .ToList()
                                    .ConcatWith(Environment.NewLine));
            sb.AppendLine(String.Format("                        </DataGrid.Columns>"));
            sb.AppendLine(String.Format("                    </DataGrid>"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("                    <Button"));
            sb.AppendLine(String.Format("                        Width=\"80\""));
            sb.AppendLine(String.Format("                        Margin=\"5\""));
            sb.AppendLine(String.Format("                        Command=\"{{Binding BtnAdd{0}RowCommand}}\"",DetailConfig.GridName));
            sb.AppendLine(String.Format("                        Content=\"行追加\""));
            sb.AppendLine(String.Format("                        Style=\"{{StaticResource button-default}}\" />"));
            sb.AppendLine(String.Format("                </StackPanel>"));
            return sb.ToString();
        }

        public string GetDaoCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine(String.Format("【Dao{0}.cs】", DetailConfig.TableName));
            sb.AppendLine(string.Format("using System.Collections.Generic;"));
            sb.AppendLine(string.Format("using System.Linq;"));
            sb.AppendLine(string.Format("using {0}.Common;", config.BaseNamespace));
            sb.AppendLine(string.Format("using {0}.Framework;", config.BaseNamespace));
            sb.AppendLine(string.Format("using {0};", DetailConfig.EntityNamespace));
            sb.AppendLine(string.Format("using static {0}.Common.ComEnum;", config.BaseNamespace));
            sb.AppendLine(string.Format("using static {0}.Common.DB;", config.BaseNamespace));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("namespace {0}", DetailConfig.DaoNamespace));
            sb.AppendLine(string.Format("{{"));
            sb.AppendLine(string.Format("    public class Dao{0} : DaoBase ", DetailConfig.TableName));
            sb.AppendLine(string.Format("    {{"));
            sb.AppendLine(String.Format("        public List<{0}Entity> Get{0}List({1})", DetailConfig.TableName, GetArgumentStringWhereDBUpdateKey()));
            sb.AppendLine(String.Format("        {{"));
            sb.AppendLine(String.Format("            return this.context.{0}Entitys", DetailConfig.TableName));
            sb.AppendLine(String.Format("                .Where(e => {0})", GetSameComparisonStringWhereDBUpdateKey("e.","")));
            sb.AppendLine(String.Format("                .ToList();"));
            sb.AppendLine(String.Format("        }}"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("        public void Save(EnmEditMode editMode, {0}Entity data)", DetailConfig.TableName));
            sb.AppendLine(String.Format("        {{"));
            sb.AppendLine(String.Format("            var contexForSave = CreateAppDbContextForSave();"));
            sb.AppendLine(String.Format("            contexForSave.Entry(data).State = ConvertEnmEditModeToEntityState(editMode);"));
            sb.AppendLine(String.Format("        }}"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("        public void Save({0}, List<{1}Entity> list)", GetArgumentStringWhereDBUpdateKey(), DetailConfig.TableName));
            sb.AppendLine(String.Format("        {{"));
            sb.AppendLine(String.Format("            var saveBeforeList = Get{0}List({1});", DetailConfig.TableName, GetColumnNameDelimiterCommaWhereDBUpdateKey()));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("            foreach (var data in list)"));
            sb.AppendLine(String.Format("            {{"));
            sb.AppendLine(String.Format("                var saveBeforeData = saveBeforeList.Where(e => {0}).FirstOrDefault();", GetSameComparisonString("e.", "data.")));
            sb.AppendLine(String.Format("                if (saveBeforeData == null)"));
            sb.AppendLine(String.Format("                {{"));
            sb.AppendLine(String.Format("                    Save(EnmEditMode.Insert, data);"));
            sb.AppendLine(String.Format("                }}"));
            sb.AppendLine(String.Format("                else"));
            sb.AppendLine(String.Format("                {{"));
            sb.AppendLine(String.Format("                    Save(EnmEditMode.Update, data);"));
            sb.AppendLine(String.Format("                }}"));
            sb.AppendLine(String.Format("            }}"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("            foreach (var data in saveBeforeList)"));
            sb.AppendLine(String.Format("            {{"));
            sb.AppendLine(String.Format("                if (list.Any(e => {0}) == false)", GetSameComparisonString("data.","e.")));
            sb.AppendLine(String.Format("                {{"));
            sb.AppendLine(String.Format("                    Save(EnmEditMode.Delete, data);"));
            sb.AppendLine(String.Format("                }}"));
            sb.AppendLine(String.Format("            }}"));
            sb.AppendLine(String.Format("        }}"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("        public List<string> GetErrorMessage(List<{0}Entity> list)", DetailConfig.TableName));
            sb.AppendLine(String.Format("        {{"));
            sb.AppendLine(String.Format("            var errorMessages = new List<string>();"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("            foreach (var item in list.Select((v, i) => new {{ v, i }}))"));
            sb.AppendLine(String.Format("            {{"));
            sb.AppendLine(String.Format("                var tmpErrorMessages = GetErrorMessage(EnmEditMode.Insert, item.v);"));
            sb.AppendLine(String.Format("                if (tmpErrorMessages.Count() > 0)"));
            sb.AppendLine(String.Format("                {{"));
            sb.AppendLine(String.Format("                    errorMessages.Add((item.i+1).ToString() + \"行目:\");"));
            sb.AppendLine(String.Format("                    errorMessages.AddRange(tmpErrorMessages);"));
            sb.AppendLine(String.Format("                }}"));
            sb.AppendLine(String.Format("            }}"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("            return errorMessages;"));
            sb.AppendLine(String.Format("        }}"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("        public List<string> GetErrorMessage(EnmEditMode editMode, {0}Entity data)", DetailConfig.TableName));
            sb.AppendLine(String.Format("        {{"));
            sb.AppendLine(String.Format("            if (editMode == EnmEditMode.Delete) return new List<string>();"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("            var errorMessages = GetErrorMessageEntityValidation(data);"));
            sb.AppendLine(String.Format(""));
            sb.AppendLine(String.Format("            return errorMessages;"));
            sb.AppendLine(String.Format("        }}"));
            sb.AppendLine(string.Format("    }}"));
            sb.AppendLine(string.Format("}}"));
            return sb.ToString();
        }

        public string GetViewModelCodeConstructor()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("            BtnAdd{0}RowCommand.Subscribe(_ => BtnAdd{0}RowAction());", DetailConfig.GridName));
            sb.AppendLine(string.Format("            BtnDelete{0}RowCommand.Subscribe(x => BtnDelete{0}RowAction(x));", DetailConfig.GridName));
            return sb.ToString().TrimEnd('\r', '\n');
        }

        public string GetViewModelCodePropertyAndCommand()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public ReactiveProperty<List<{0}Entity>> Edit{1}Data {{ get; set; }} = new ReactiveProperty<List<{0}Entity>>(new List<{0}Entity>());", DetailConfig.TableName,DetailConfig.GridName));
            sb.AppendLine(GetComboProperty());
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public ReactiveCommand BtnAdd{0}RowCommand {{ get; }} = new ReactiveCommand();", DetailConfig.GridName));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        public ReactiveCommand BtnDelete{0}RowCommand {{ get; }} = new ReactiveCommand();", DetailConfig.GridName));
            return sb.ToString().TrimEnd('\r', '\n');
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

        public string GetViewModelCodeSavePreprocessing()
        {
            var list = new List<string>();
            list.Add(string.Format("            foreach (var item in Edit{0}Data.Value.Select((v, i) => new {{ v, i }}))", DetailConfig.GridName));
            list.Add(string.Format("            {{"));
            list.Add(GetValueSetWhereDBUpdateKey("                item.v.", "EditData.Value."));
            list.Add(GetValueSetSerialNumberWhereDBUpdateKey());
            list.Add(string.Format("            }}"));
            return list.ConcatWith(Environment.NewLine);
        }

        public string GetViewModelCodeSaveErrorMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(string.Format("            var dao{0} = new Dao{0}();", DetailConfig.TableName));
            sb.AppendLine(string.Format("            errorMessage.AddRange(dao{0}.GetErrorMessage(Edit{1}Data.Value.ToList()));", DetailConfig.TableName, DetailConfig.GridName));
            return sb.ToString().TrimEnd('\r', '\n'); ;
        }

        public string GetViewModelCodeSave()
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(string.Format("            dao{0}.Save({1}, Edit{2}Data.Value);", DetailConfig.TableName, GetColumnNameWhereDBUpdateKey("EditData.Value."), DetailConfig.GridName));
            return sb.ToString().TrimEnd('\r', '\n'); ;
        }

        public string GetViewModelCodeDelete()
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(string.Format("            var dao{0} = new Dao{0}();", DetailConfig.TableName));
            sb.AppendLine(string.Format("            dao{0}.Save({1}, new List<{0}Entity>());", DetailConfig.TableName, GetColumnNameWhereDBUpdateKey("EditData.Value.")));
            return sb.ToString().TrimEnd('\r', '\n'); ;
        }

        public string GetViewModelCodeSetEditDataToInsert()
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(string.Format("            Edit{1}Data.Value = new List<{0}Entity>();", DetailConfig.TableName, DetailConfig.GridName));
            return sb.ToString().TrimEnd('\r', '\n'); ;
        }

        public string GetViewModelCodeSetEditDataToUpdate()
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(string.Format("            var dao{0} = new Dao{0}();", DetailConfig.TableName));
            sb.AppendLine(string.Format("            Edit{2}Data.Value = dao{0}.Get{0}List({1});", DetailConfig.TableName, GetColumnNameWhereDBUpdateKey("selectEntity."), DetailConfig.GridName));
            return sb.ToString().TrimEnd('\r', '\n'); ;
        }

        public string GetViewModelCodeGetJsonEditData()
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(string.Format("            jsonEditdata += Environment.NewLine;"));
            sb.AppendLine(string.Format("            jsonEditdata += JsonConvert.SerializeObject(this.Edit{0}Data.Value);", DetailConfig.GridName));
            return sb.ToString().TrimEnd('\r', '\n'); ;
        }

        public string GetViewModelCodeCommandAction()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("        private void BtnAdd{0}RowAction()", DetailConfig.GridName));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            TryCatch.ShowMassage(() =>"));
            sb.AppendLine(string.Format("            {{"));
            sb.AppendLine(string.Format("                Add{0}Row();", DetailConfig.GridName));
            sb.AppendLine(string.Format("            }});"));
            sb.AppendLine(string.Format("        }}"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        private void Add{0}Row()", DetailConfig.GridName));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            Edit{0}Data.Value.Add(new {1}Entity());", DetailConfig.GridName, DetailConfig.TableName));
            sb.AppendLine(string.Format("            Edit{0}Data.Value = Edit{0}Data.Value.DeepCopy();", DetailConfig.GridName));
            sb.AppendLine(string.Format("        }}"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        private void BtnDelete{0}RowAction(object entity)", DetailConfig.GridName));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            TryCatch.ShowMassage(() =>"));
            sb.AppendLine(string.Format("            {{"));
            sb.AppendLine(string.Format("                Delete{0}Row(entity);", DetailConfig.GridName));
            sb.AppendLine(string.Format("            }});"));
            sb.AppendLine(string.Format("        }}"));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("        private void Delete{0}Row(object entity)", DetailConfig.GridName));
            sb.AppendLine(string.Format("        {{"));
            sb.AppendLine(string.Format("            Edit{0}Data.Value.Remove(({1}Entity)entity);", DetailConfig.GridName, DetailConfig.TableName));
            sb.AppendLine(string.Format("            Edit{0}Data.Value = Edit{0}Data.Value.DeepCopy();", DetailConfig.GridName));
            sb.AppendLine(string.Format("        }}"));
            return sb.ToString().TrimEnd('\r', '\n'); ;
        }

        public string GetAppDbContextCode()
        {
            var list = new List<string>();
            list.Add(string.Format("public DbSet<{0}Entity> {0}Entitys {{ get; set; }}", DetailConfig.TableName));
            list.Add(string.Format(""));
            list.Add(string.Format("modelBuilder.Entity<{0}Entity>().ToTable(\"{0}\").HasKey(c => new {{{1}}});", DetailConfig.TableName, GetColumnName("c.")));
            return list.ConcatWith(Environment.NewLine);
        }

        private string GetArgumentStringWhereDBUpdateKey()
        {
            return this
                .Where(e => e.DBUpdateKey == true)
                .Select(e => e.GetEntityTypeName() + " " + e.ColumnName)
                .ConcatWith(",");
        }

        private string GetColumnNameDelimiterCommaWhereDBUpdateKey()
        {
            return this
                .Where(e => e.DBUpdateKey == true)
                .Select(e => e.ColumnName)
                .ConcatWith(",");
        }

        /// <summary>
        /// e.ColumnName == ColumnName && e.ColumnName == ColumnName...
        /// </summary>
        /// <returns></returns>
        private string GetSameComparisonStringWhereDBUpdateKey(string LeftSideHeaderString, string RightSideHeaderString)
        {
            return this
                .Where(e => e.DBUpdateKey == true)
                .Select(e => LeftSideHeaderString + e.ColumnName + " == " + RightSideHeaderString + e.ColumnName)
                .ConcatWith(" && ");
        }
        
        private string GetColumnNameWhereDBUpdateKey(string hederString)
        {
            return this
                .Where(e => e.DBUpdateKey == true)
                .Select(e => hederString + e.ColumnName)
                .ConcatWith(",");
        }

        public string GetValueSetWhereDBUpdateKey(string LeftSideHeaderString, string RightSideHeaderString)
        {
            return this
                .Where(e => e.DBUpdateKey == true)
                .Select(e => LeftSideHeaderString + e.ColumnName + " = " + RightSideHeaderString + e.ColumnName + ";")
                .ConcatWith(Environment.NewLine);
        }

        public string GetValueSetSerialNumberWhereDBUpdateKey()
        {
            return this
                .Where(e => e.SerialNumber == true)
                .Select(e => "                item.v." + e.ColumnName + " = item.i;")
                .ConcatWith(Environment.NewLine);
        }
    }
}
