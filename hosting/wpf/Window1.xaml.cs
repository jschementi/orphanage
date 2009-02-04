using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Scripting.Hosting;


namespace wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            var runtime = ScriptRuntime.CreateFromConfiguration();
            var engine = runtime.GetEngine("IronRuby");
            var scope = engine.CreateScope();
            scope.SetVariable("window", this);
            engine.Execute("window.Message.Text = 'Hello from Ruby!'", scope);
        }
    }
}
