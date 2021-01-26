using LinqToExcel.Attributes;

namespace CodeGenerator.Models.Entity
{
    public class DetailConfigEntity
    {
        [ExcelColumn("テーブル名")]
        public string TableName { get; set; }

        [ExcelColumn("Grid名")]
        public string GridName { get; set; }

        [ExcelColumn("Entity空間名")]
        public string EntityNamespace { get; set; }

        [ExcelColumn("Dao空間名")]
        public string DaoNamespace { get; set; }

    }
}
