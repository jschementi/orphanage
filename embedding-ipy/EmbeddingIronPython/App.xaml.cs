using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Hosting = Microsoft.Scripting.Hosting;
using Silverlight = Microsoft.Scripting.Silverlight;

namespace EmbeddingIronPython {
    public partial class App : Application {

        public App() {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        Hosting.ScriptEngine _python;
        Hosting.ScriptScope _scope;
        Silverlight.Repl _repl;

        private void Application_Startup(object sender, StartupEventArgs e) {
            this.RootVisual = new MainPage();

            var runtime = Silverlight.DynamicEngine.CreateRuntime();
            _python = runtime.GetEngine("python");

            _scope = _python.CreateScope();
            _repl = Silverlight.Repl.Show(_python, _scope);
            _scope.SetVariable("app", this);

            try {
                test("Execute strings", "4", "2 + 2");
                test("Import .NET namespace", "hi", @"import System
System.String('hi')");
                _python.Execute(@"import foo
foo.test(app)
foo.test_import(app)", _scope);
            } catch(Exception ex) {
                _repl.OutputBuffer.WriteLine("[FAIL]");
                _repl.OutputBuffer.Write(_python.GetService<Hosting.ExceptionOperations>().FormatException(ex));
            }
        }

        public bool test(string name, string expected, string code) {
            string result = _python.Execute(code, _scope).ToString() == expected ? "PASS" : "FAIL";
            _repl.OutputBuffer.WriteLine(name + " " + "[" + result + "]");
            return result == "PASS";
        }

        private void Application_Exit(object sender, EventArgs e) {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e) {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached) {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e) {
            try {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            } catch (Exception) {
            }
        }
    }
}
