using System;

namespace Graph
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodePathAttribute : Attribute
    {
        public string Path;

        public NodePathAttribute(string path)
        {
            Path = path;
        }
    }
}