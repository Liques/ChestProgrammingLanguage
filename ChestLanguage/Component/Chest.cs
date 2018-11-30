using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestLanguage
{
    public class Chest : ILanguageComponent
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

        public IList<uAttach> uAttachments { get; set; }

        private bool isNull = true;

        public bool IsNull
        {
            get { return isNull; }
            private set { isNull = value; }
        }


        internal static readonly Type[] AllowedTypes = {
            typeof(object),
            typeof(System.DBNull),
            typeof(bool),
            typeof(char),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(object), //TypeCode is discontinuous so we need a placeholder.
            typeof(string)
        };

        private Type GetValueType(string value)
        {
            Int32 objInt32;
            Int64 objInt64;
            Double objDouble;
            Boolean objBoolean;
            if (value.IndexOf("\"") >= 0)
            {
                this.Value = value;
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
