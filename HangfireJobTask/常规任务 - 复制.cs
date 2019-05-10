using Hangfire;
using Hangfire.Server;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HangfireJobTask
{
    public abstract class RazorPage
    {
        public abstract void Execute();


        //public RazorTemplateBase Layout { get; set; }

        //public void Clear();
        //public virtual void Execute();
        //public string RenderBody();
        //public string TransformText();
        //public void Write(object value);
        //public void Write(decimal value);
        //public void Write(bool value);
        //public void Write(float value);
        //public void Write(long value);
        //public void Write(int value);
        //public void Write(double value);
        //public void WriteAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params object[] fragments);
        //public void WriteAttributeTo(TextWriter writer, string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params object[] fragments);
        public void WriteLiteral(string textToAppend) { }
        //public void WriteLiteralTo(TextWriter writer, string text) { }
        //public void WriteTo(TextWriter writer, bool value);
        //public void WriteTo(TextWriter writer, int value);
        //public void WriteTo(TextWriter writer, long value);
        //public void WriteTo(TextWriter writer, float value);
        //public void WriteTo(TextWriter writer, double value);
        //public void WriteTo(TextWriter writer, decimal value);
        //public void WriteTo(TextWriter writer, object value);
    }
}
