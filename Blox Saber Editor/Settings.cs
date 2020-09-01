using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound_Space_Editor
{
    static class Settinga
    {
        private static List<Property> Properties = new List<Property>();

        public static Property GetProperty(String property)
        {
            foreach (Property prp in Properties)
            {
                if (prp.Name == property.ToLower())
                {
                    return prp;
                }
            }
            return new Property();
        }
        public static void SetProperty(string property, string value)
        {
            foreach (Property prp in Properties)
            {
                if (prp.Name == property.ToLower())
                {
                    prp.Value = value;
                    return;
                }
            }
            Property prop = new Property(property, value);
            Properties.Add(prop);
        }
        public static void ReadProperties(string file)
        {
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    string[] strings = line.Split('=');
                    if (strings.Length == 2)
                    {
                        var name = strings[0];
                        var value = strings[1];
                        Properties.Add(new Property(name, value));
                    }
                }
            }
        }
    }

    class Property
    {
        public string Name;
        public string Value;

        public Property()
        {}
        public Property(string Name, string Value)
        {
            this.Name = Name.ToLower();
            this.Value = Value;
        }
    }
}
