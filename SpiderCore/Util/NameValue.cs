
using System;

namespace SpiderCore.Util
{
    public class NameValue
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public NameValue()
        {
            Name = String.Empty;
            Value = String.Empty;
        }

        public NameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Name, Value);
        }
    }
}
