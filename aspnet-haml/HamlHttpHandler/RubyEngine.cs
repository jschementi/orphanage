using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Scripting.Hosting;
using IronRuby.Builtins;
using IronRuby.Runtime;
using System.Configuration;

namespace IronRuby.Haml {
    public static class RubyEngine {
        public static ScriptEngine Engine;
        private static ScriptScope scope;

        public static void Init() {
            Engine = Ruby.CreateEngine();
            scope = Engine.CreateScope();
        }

        public static RubyContext Context {
            get {
                return Ruby.GetExecutionContext(Engine);
            }
        }

        public static object Require(string file) {
            return Require(file, null);
        }

        public static object Require(string file, string gemVersion) {
            var command = new MutableString();
            if(gemVersion != null) {
                command.Append(string.Format("gem '{0}', '{1}';", file, gemVersion));
            }
            command.Append(string.Format("require '{0}'", file));
            return Execute(command);
        }

        public static object ExecuteFile(string fileName) {
            return Engine.CreateScriptSourceFromString(FindFile(fileName)).Execute(scope);
        }

        public static object Execute(string code) {
            return Execute(code, scope);
        }

        public static object Execute(string code, ScriptScope aScope) {
            return Engine.CreateScriptSourceFromString(code).Execute(aScope);
        }

        public static T ExecuteMethod<T>(object instance, string methodName)
        {
            return ExecuteMethod<T>(instance, methodName, new object[] { });
        }

        public static T ExecuteMethod<T>(object instance, string methodName, params object[] args)
        {
            return (T)Engine.Operations.InvokeMember(instance, methodName, args);
        }

        public static object AddLoadPath(string path) {
            return Engine.Execute(string.Format("$LOAD_PATH.unshift '{0}'", path));
        }

        public static void SetConstant(string name, object value) {
            Engine.Runtime.Globals.SetVariable(name, value);
        }

        public static string FindFile(string file) {
            foreach (var path in Engine.GetSearchPaths()) {
                var fullPath = TryGetFullPath(path, file);
                if (File.Exists(fullPath)) {
                    return fullPath;
                }
            }
            return null;
        }

        public static void SetGemPath()
        {
            string gempath = ConfigurationManager.AppSettings["GEM_PATH"];
            if (gempath != null)
            {
                gempath = Path.GetFullPath(gempath);

                if (Directory.Exists(gempath))
                {
                    Environment.SetEnvironmentVariable("GEM_PATH", gempath);
                }
            }
        }

        private static string TryGetFullPath(string/*!*/ dir, string/*!*/ file) {
            try {
                return Path.GetFullPath(Path.Combine(dir, file));
            } catch {
                return null;
            }
        }
    
    }
}
