using System;

namespace Graph
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodeTitleAttribute : Attribute
    {
        public string Title;

        public NodeTitleAttribute(string title)
        {
            Title = title;
        }
    }
}