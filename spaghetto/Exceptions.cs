using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal class SpaghettoException : Exception {
        public Position posStart;
        public Position posEnd;
        public string errorName;
        public object details;

        public SpaghettoException(Position posStart, Position posEnd, string errorName, object details) {
            this.posStart = posStart;
            this.posEnd = posEnd;
            this.errorName = errorName;
            this.details = details;
        }

        public static implicit operator bool(SpaghettoException ex) => (ex != null);

        public override string Message => GetMessage();

        public virtual string GetMessage() {
            string result = $"{errorName}: {details}";
            result += @$"
  at {posStart.fileName}:{posStart.ln + 1}

{ErrorPointer.GenerateErrorPointer(posStart.fileText, posStart, posEnd)}";
            return result;
        }
    }

    internal class IllegalCharError : SpaghettoException {
        public IllegalCharError(Position posStart, Position posEnd, object details) : base(posStart, posEnd, "Illegal Character", details) { }
    }

    internal class ExpectedCharError : SpaghettoException {
        public ExpectedCharError(Position posStart, Position posEnd, object details) : base(posStart, posEnd, "Expected Character", details) { }
    }

    internal class IllegalSyntaxError : SpaghettoException {
        public IllegalSyntaxError(Position posStart, Position posEnd, object details, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) : base(posStart, posEnd, "Illegal Syntax", details) {
            System.Diagnostics.Debug.WriteLine("IllegalSyntaxError instatiated at " + memberName + " in " + fileName + ":" + lineNumber);
        }
    }

    internal class InvalidNumericalValueError : SpaghettoException {
        public InvalidNumericalValueError(Position posStart, Position posEnd, object details) : base(posStart, posEnd, "Invalid numerical value", details) { }
    }

    internal class RuntimeError : SpaghettoException {
        public Context ctx;

        public RuntimeError(Position posStart, Position posEnd, object details, Context ctx) : base(posStart, posEnd, "RuntimeError", details) {
            this.ctx = ctx;
        }

        public override string GetMessage() {
            string result = @$"{GenerateTraceback()}
{errorName}: {details}

{ErrorPointer.GenerateErrorPointer(posStart.fileText, posStart, posEnd)}";
            
            return result;
        }

        public string GenerateTraceback() {
            string result = "";
            //Position pos = posStart;
            Context ctx = this.ctx;

            while(ctx != null) {
                result = $"  at {posStart.fileName}:{posStart.ln + 1} in {ctx.displayName}\n" + result;
                //pos = ctx.parentEntryPosition;
                ctx = ctx.parentContext;
            }

            return "Traceback:\n" + result;
        }
    }

    internal class TypeError : SpaghettoException {
        public TypeError(Position posStart, Position posEnd, object details) : base(posStart, posEnd, "TypeError", details) { }
    }
}
