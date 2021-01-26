using CodeGenerator.Common;
using CodeGenerator.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Models.Class
{
    public class DetailEntityCode
    {
        private List<DetailInfoEntity> detailInfoEntitys;
        private ConfigEntity config;
        private DetailConfigEntity detailConfig;

        public DetailEntityCode(ConfigEntity config, DetailConfigEntity detailConfig, List<DetailInfoEntity> detailInfoEntitys)
        {
            this.config = config;
            this.detailInfoEntitys = detailInfoEntitys;
            this.detailConfig = detailConfig;
        }

        public string CreateEntityCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("【{0}Entity.cs】", detailConfig.TableName));
            sb.AppendLine(string.Format("using System;"));
            sb.AppendLine(string.Format("using System.Collections.Generic;"));
            sb.AppendLine(string.Format("using System.Linq;"));
            sb.AppendLine(string.Format("using System.ComponentModel.DataAnnotations;"));
            sb.AppendLine(string.Format("using System.ComponentModel.DataAnnotations.Schema;"));
            sb.AppendLine(string.Format("using {0}.Framework;", config.BaseNamespace));
            sb.AppendLine(string.Format(""));
            sb.AppendLine(string.Format("namespace {0}", detailConfig.EntityNamespace));
            sb.AppendLine(string.Format("{{"));
            sb.AppendLine(string.Format("    public class {0}Entity : DBEntityBase", detailConfig.TableName));
            sb.AppendLine(string.Format("    {{"));
            sb.AppendLine(this.detailInfoEntitys.Select(e => e.GetEntitiyCode())
                              .ToList()
                              .ConcatWith(Environment.NewLine + Environment.NewLine));
            sb.AppendLine(string.Format("    }}"));
            sb.AppendLine(string.Format("}}"));
            return sb.ToString();
        }
    }
}
