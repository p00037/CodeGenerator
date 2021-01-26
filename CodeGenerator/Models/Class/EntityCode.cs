using CodeGenerator.Common;
using CodeGenerator.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator.Models.Class
{
    public class EntityCode
    {
        private List<BaseInfoEntity> baseInfoEntitys;
        private ConfigEntity config;

        public EntityCode(ConfigEntity config, List<BaseInfoEntity> baseInfoEntitys)
        {
            this.config = config;
            this.baseInfoEntitys = baseInfoEntitys;
        }

        public string CreateEntityCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"【{config.TableName}Entity.cs】");
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"using System.Linq;");
            sb.AppendLine($"using System.ComponentModel.DataAnnotations;");
            sb.AppendLine($"using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine($"using {config.BaseNamespace}.Framework;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {config.EntityNamespace}");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {config.TableName}Entity : DBEntityBase");
            sb.AppendLine($"    {{");
            sb.AppendLine(this.baseInfoEntitys.Select(e => e.GetEntitiyCode())
                              .ToList()
                              .ConcatWith(Environment.NewLine + Environment.NewLine));
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            return sb.ToString();
        }

        public string CreateSearchEntityCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"【{config.XamlName}SearchEntity.cs】");
            sb.AppendLine($"using System;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {config.BaseNamespace}.Models.SearchEntity");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {config.XamlName}SearchEntity");
            sb.AppendLine($"    {{");
            sb.AppendLine(this.baseInfoEntitys.Where(e => e.ComparisonMethod != null)
                              .Select(e => e.GetSearchEntitiyCode())
                              .ToList()
                              .ConcatWith(Environment.NewLine));
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            return sb.ToString();
        }
    }
}
