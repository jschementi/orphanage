using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Scripting.Hosting;

namespace aspnet
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var runtime = ScriptRuntime.CreateFromConfiguration();
            var engine = runtime.GetEngine("IronRuby");
            var scope = engine.CreateScope();
            scope.SetVariable("page", this);
            engine.Execute("page.Message.Text = 'Hello from Ruby!'", scope);
        }
    }
}
