using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using IronRuby.Builtins;

namespace IronRuby.Haml
{
    public class HamlHandlerFactory : IHttpHandlerFactory
    {
        private static HamlHandler _Handler;

        public IHttpHandler GetHandler(HttpContext/*!*/ context, string/*!*/ requestType, string/*!*/ url, string/*!*/ pathTranslated)
        {
            if (_Handler == null)
            {
                try
                {
                    _Handler = new HamlHandler();
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = 200;
                    return null;
                }
            }

            return _Handler;
        }

        public void ReleaseHandler(IHttpHandler/*!*/ handler)
        {

        }
    }

    internal sealed class HamlHandler : IHttpHandler
    {
        public HamlHandler()
        {
            RubyEngine.Init();
            RubyEngine.Require("rubygems");
            RubyEngine.Require("haml");
            RubyEngine.Require("sass");
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string path = context.Request.PhysicalPath;
            string directory = Path.GetDirectoryName(path);
            var extension = Path.GetExtension(path);

            if (extension == string.Empty) {
                if (File.Exists(directory + Path.DirectorySeparatorChar + "index.haml"))
                {
                    path = directory + Path.DirectorySeparatorChar + "index.html";
                    extension = ".html";
                }
                else
                {
                    return;
                }
            }

            if (File.Exists(path))
            {
                context.Response.Write(File.ReadAllText(path));
                return;
            }

            string type = null;
            if (extension == ".html")
            {
                extension = ".haml";
                type = "Haml";
            }
            else if (extension == ".css")
            {
                extension = ".sass";
                type = "Sass";
            }

            string realPath = directory + Path.DirectorySeparatorChar.ToString() + 
                Path.GetFileNameWithoutExtension(path) + extension;
            if (!File.Exists(realPath))
            {
                context.Response.StatusCode = 404;
                return;
            }

            string template = File.ReadAllText(realPath);
            context.Response.Write(RenderTemplate(template, type));
        }

        private string RenderTemplate(string template, string engine) 
        {    
            var constant = RubyEngine.Execute(engine + "::Engine");
            var engineObject = RubyEngine.ExecuteMethod<RubyObject>(constant, "new", MutableString.Create(template));
            return RubyEngine.ExecuteMethod<MutableString>(engineObject, "render").ToString();
        }
    }
}
