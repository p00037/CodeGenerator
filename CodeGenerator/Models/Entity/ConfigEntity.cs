using CodeGenerator.Models.Interface;
using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Models.Entity
{
    public class ConfigEntity : IConfigFodyEntity
    {
        [ExcelColumn("テーブル名")]
        public string TableName { get; set; }

        [ExcelColumn("Xaml名")]
        public string XamlName { get; set; }

        [ExcelColumn("タイトル")]
        public string Title { get; set; }

        [ExcelColumn("明細数")]
        public int? DetailCount { get; set; }

        [ExcelColumn("基本空間名")]
        public string BaseNamespace { get; set; }

        [ExcelColumn("Xaml空間名")]
        public string XamlNamespace { get; set; }

        [ExcelColumn("ViewModel空間名")]
        public string ViewModelNamespace { get; set; }

        [ExcelColumn("Entity空間名")]
        public string EntityNamespace { get; set; }

        [ExcelColumn("SearchEntity空間名")]
        public string SearchEntityNamespace { get; set; }

        [ExcelColumn("Dao空間名")]
        public string DaoNamespace { get; set; }

        [ExcelColumn("ContextFactory名")]
        public string ContextFactoryName { get; set; }

        [ExcelColumn("DBプロジェクト空間名")]
        public string DBNamespace { get; set; }
    }
}
