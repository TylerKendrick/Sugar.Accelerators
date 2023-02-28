using System;

namespace MyNamespace
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class GenerateTestDoubleAttribute : Attribute
    {
        public Type GenerateTestDoubleFor { get; }

        public GenerateTestDoubleAttribute(Type generateTestDoubleFor)
        {
            GenerateTestDoubleFor = generateTestDoubleFor;
        }
    }
}
