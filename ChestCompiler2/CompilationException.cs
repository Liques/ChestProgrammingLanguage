using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestCompiler
{
    public enum CompiliationErrorType 
    {
        Unknown = 1,
        WrongCodeBlockOrder = 2,
    }
    public class CompilationException : Exception
    {
        public CompiliationErrorType ErrorType { get; set; }

        public CompilationException(CompiliationErrorType type, Exception innerException)
            : base(Enum.GetName(typeof(CompiliationErrorType), type), innerException)
        {
        }
    }
}
