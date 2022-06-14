using System.Runtime.CompilerServices;

namespace spaghetto {
    public class ParseResult {
        public SpaghettoException error;
        public Node node;
        public int advanceCount = 0;
        public int lastRegisteredAdvanceCount = 0;
        public int toReverseCount = 0;

        public Node Register(ParseResult parseResult) {
            lastRegisteredAdvanceCount = parseResult.advanceCount;
            advanceCount += parseResult.advanceCount;
            if (parseResult.error != null) this.error = parseResult.error;
            return parseResult.node;
        }

        public void RegisterAdvancement() {
            lastRegisteredAdvanceCount = 1;
            advanceCount++;
        }

        public ParseResult Success(Node node) {
            this.node = node;
            return this;
        }

        public Node TryRegister(ParseResult res) {
            if(res.error) {
                this.toReverseCount = res.advanceCount;
                return default;
            }

            return Register(res);
        }

        public ParseResult Failure(SpaghettoException error, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0) {
            System.Diagnostics.Debug.WriteLine("Failure at " + memberName + " in " + fileName + ":" + lineNumber);

            System.Diagnostics.Debug.WriteLine("ParserResult Failure: " + error.Message);
            if(this.error == null || advanceCount == 0)
                this.error = error;
            return this;
        }
    }
}
