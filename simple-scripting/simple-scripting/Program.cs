using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;

namespace simple_scripting {
    class Program {
        static IDictionary<string, Action> _commands = new Dictionary<string, Action>();
        static Hosting _host;

        static void Main(string[] args) {
            _host = new PythonHost();

            #region Commands
            _commands.Add("quit", () => Environment.Exit(0));
            _commands.Add("exit", () => Environment.Exit(0));
            _commands.Add("%python", () => _host = new PythonHost());
            _commands.Add("%ruby", () => _host = new RubyHost());
            #endregion

            object result = null;
            while (true) {
                Console.Write(_host.Name.Substring(1) + "> ");
                string cmd = Console.ReadLine();
                #region Invoking Commands
                if (_commands.ContainsKey(cmd)) {
                    _commands[cmd].Invoke();
                } else
                {
                #endregion
                    try {
                        result = _host.Execute(cmd);
                        Console.WriteLine("=> " + _host.Inspect(result));
                    } catch (Exception e) {
                        Console.WriteLine(_host.HandleException(e));
                    }
                #region Closing
                }
                #endregion
            }
        }
    }
    #region Hosting
    abstract class Hosting {
        public ScriptEngine Engine { get; protected set; }

        protected ScriptScope _scope;
        public ScriptScope Scope {
            get {
                if (_scope == null)
                    _scope = Engine.CreateScope();
                return _scope;
            }
        }

        public abstract string Inspect(object result);

        public object Execute(string command) {
            return Execute<object>(command);
        }

        public T Execute<T>(string command) {
            return Engine.Execute<T>(command, Scope);
        }

        public string HandleException(Exception e) {
            return Engine.GetService<ExceptionOperations>().FormatException(e);
        }

        public string Name { get { return Engine.Setup.FileExtensions[0]; } }
    }
    #endregion
    #region Python
    class PythonHost : Hosting {
        public PythonHost() {
            Engine = IronPython.Hosting.Python.CreateEngine();
        }

        public override string Inspect(object result) {
            Scope.SetVariable("_", result);
            return (string)Engine.Execute("repr(_)", Scope);
        }
    }
    #endregion
    #region Ruby
    class RubyHost : Hosting {
        public RubyHost() {
            Engine = IronRuby.Ruby.CreateEngine();
        }

        public override string Inspect(object result) {
            return ((IronRuby.Builtins.MutableString) Engine.Operations.InvokeMember(result, "inspect")).ToString();
        }
    }
    #endregion
}
