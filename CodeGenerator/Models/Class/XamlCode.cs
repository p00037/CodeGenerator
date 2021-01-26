using CodeGenerator.Common;
using CodeGenerator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Models.Class
{
    public class XamlCodeFody
    {
        private BaseInfoEntityList baseInfoEntityList = new BaseInfoEntityList();

        private List<DetailInfoEntityList> detailInfoEntityLists = new List<DetailInfoEntityList>();

        public XamlCodeFody(BaseInfoEntityList baseInfoEntityList, List<DetailInfoEntityList> detailInfoEntityLists)
        {
            this.baseInfoEntityList = baseInfoEntityList;
            this.detailInfoEntityLists = detailInfoEntityLists;
        }

        public string CreateXamlCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"【{baseInfoEntityList.Config.XamlName}.xaml】");
            sb.AppendLine($" <CustomContorol:WindowBase");
            sb.AppendLine($"     x:Class=\"{baseInfoEntityList.Config.XamlNamespace}.{baseInfoEntityList.Config.XamlName}\"");
            sb.AppendLine($"     xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            sb.AppendLine($"     xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            sb.AppendLine($"     xmlns:Behavior=\"clr-namespace:WpfControlLibrary.Behavior;assembly=WpfControlLibrary\"");
            sb.AppendLine($"     xmlns:CustomContorol = \"clr-namespace:WpfControlLibrary.CustomContorol;assembly=WpfControlLibrary\"");
            sb.AppendLine($"     xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"");
            sb.AppendLine($"     xmlns:i=\"http://schemas.microsoft.com/xaml/behaviors\"");
            sb.AppendLine($"     xmlns:interactivity=\"clr-namespace:Reactive.Bindings.Interactivity;assembly=ReactiveProperty.WPF\"");
            sb.AppendLine($"     xmlns:local=\"clr-namespace:{baseInfoEntityList.Config.XamlNamespace}\"");
            sb.AppendLine($"     xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"");
            sb.AppendLine($"     xmlns:UserControls=\"clr-namespace:{baseInfoEntityList.Config.BaseNamespace}.UserControls\"");
            sb.AppendLine($"     x:Name=\"View{baseInfoEntityList.Config.XamlName}\"");
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
            sb.AppendLine($"             <UserControls:CustomMenu ThisWindow=\"{{ Binding ElementName = View{baseInfoEntityList.Config.XamlName}}}\" />");
            sb.AppendLine($" ");
            sb.AppendLine($"             <DockPanel>");
            sb.AppendLine($"                 <StackPanel Style=\"{{StaticResource stackpanel-header-left}}\">");
            sb.AppendLine($"                     <Label Content=\"{baseInfoEntityList.Config.Title}\" Style=\"{{StaticResource label-title}}\" />");
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
            sb.AppendLine(baseInfoEntityList.GetXamlCodeSearch());
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
            sb.AppendLine(baseInfoEntityList.GetXamlCodeSearchGrid());
            sb.AppendLine($"                    </DataGrid>");
            sb.AppendLine($"                </StackPanel>");
            sb.AppendLine($"             </StackPanel>");
            sb.AppendLine($" ");
            sb.AppendLine($"             <StackPanel Margin=\"10,0,0,0\">");
            sb.AppendLine(baseInfoEntityList.GetXamlCode());
            sb.AppendLine(detailInfoEntityLists.Select(e => e.GetXamlCode()).ConcatWith(""));
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

            return sb.ToString().Replace(".Value", "");
        }
    }
}
