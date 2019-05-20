using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestCompiler
{
    public class Chest
    {
        internal Chest()
        {
        }

        public Chest(string value)
        {
            var values = value.Split(' ');
            this.Name = values.First();
            this.Type = values.Length >= 1 ? GetValueType(value.Substring(this.Name.Length).Trim()) : typeof(object);

        }

        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }

        private Type GetValueType(string value)
        {
            Int32 objInt32;
            Int64 objInt64;
            Double objDouble;
            Boolean objBoolean;
            if (value.IndexOf("\"") >= 0)
            {
                this.Value = value.Trim('"');
                return typeof(string);
            }
            else if (Int32.TryParse(value, out objInt32))
            {
                this.Value = objInt32;
                return typeof(int);
            }
            else if (Int64.TryParse(value, out objInt64))
            {
                this.Value = objInt64;
                return typeof(long);
            } 
            else if (Double.TryParse(value, out objDouble))
            {
                this.Value = objDouble;
                return typeof(double);
            }
            else if (Boolean.TryParse(value, out objBoolean))
            {
                this.Value = objBoolean;
                return typeof(bool);
            }

            this.Value = value;
            return typeof(object);
        }
    }
}
