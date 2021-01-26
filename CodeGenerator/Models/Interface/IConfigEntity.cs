using LinqToExcel.Attributes;

namespace CodeGenerator.Models.Interface
{
    public interface IConfigEntity
    {
        [ExcelColumn("テーブル名")]
        string TableName { get; set; }

        [ExcelColumn("Xaml名")]
        string XamlName { get; set; }

        [ExcelColumn("タイトル")]
        string Title { get; set; }

        [ExcelColumn("明細数")]
        int? DetailCount { get; set; }

        [ExcelColumn("基本空間名")]
        string BaseNamespace { get; set; }

        [ExcelColumn("Xaml空間名")]
        string XamlNamespace { get; set; }

        [ExcelColumn("ViewModel空間名")]
        string ViewModelNamespace { get; set; }

        [ExcelColumn("Entity空間名")]
        string EntityNamespace { get; set; }

        [ExcelColumn("SearchEntity空間名")]
        string SearchEntityNamespace { get; set; }

        [ExcelColumn("Dao空間名")]
        string DaoNamespace { get; set; }

        [ExcelColumn("ContextFactory名")]
        string ContextFactoryName { get; set; }
    }
}
