using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using spaghetto.Parsing;
using spaghetto.Parsing.Nodes;

namespace spaghetto
{
    public class Interpreter {
        public Scope GlobalScope { get; private set; }

        public Interpreter() {
            GlobalScope = new(0);

            // Fixed default imports; should be kept as minimal as possible
            GlobalScope.Table["true"] = new SInt(1) { IsConstant = true };
            GlobalScope.Table["false"] = new SInt(0) { IsConstant = true };
            GlobalScope.Table["null"] = new SNull() { IsConstant = true };
        }

        public void Interpret(string text, ref InterpreterResult res) {
            Lexer lexer = new(text);
            res.LexedTokens = lexer.Lex();

            Parser p = new(res.LexedTokens, text);
            res.AST = p.Parse();

            try {
                res.LastValue = res.AST.Evaluate(GlobalScope);
            } catch {
                throw;
            }
        }

        public void Interpret(string text, ref TimingInterpreterResult res) {
            Stopwatch sw = new();

            sw.Start();
            Lexer lexer = new(text);
            res.Result.LexedTokens = lexer.Lex();
            res.LexTime = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            Parser p = new(res.Result.LexedTokens, text);
            res.Result.AST = p.Parse();
            res.ParseTime = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            res.Result.LastValue = res.Result.AST.Evaluate(GlobalScope);
            res.EvalTime = sw.Elapsed.TotalMilliseconds;
            sw.Stop();
        }
    }

    public class GoofyAhhEvaluator {
        public static SValue Evaluate(Scope scope, SyntaxNode node) {
            switch (node.Type) {
                case NodeType.AssignVariable:
                    return EvaluateAssignVariableNode(scope, node as AssignVariableNode);
                case NodeType.BinaryExpression:
                    return EvaluateBinaryExpressionNode(scope, node as BinaryExpressionNode);
                case NodeType.Block:
                    return EvaluateBlockNode(scope, node as BlockNode);
                case NodeType.Break:
                    return EvaluateBreakNode(scope, node as BreakNode);
                case NodeType.Call:
                    return EvaluateCallNode(scope, node as CallNode);
                case NodeType.Cast:
                    return EvaluateCastNode(scope, node as CastNode);
                case NodeType.ClassDefinition:
                    return EvaluateClassDefinitionNode(scope, node as ClassDefinitionNode);
                case NodeType.ClassFunctionDefinition:
                    return EvaluateClassFunctionDefinitionNode(scope, node as ClassFunctionDefinitionNode);
                case NodeType.ClassPropertyDefinition:
                    return EvaluateClassPropertyDefinitionNode(scope, node as ClassPropDefinitionNode);
                case NodeType.Continue:
                    return EvaluateContinueNode(scope, node as ContinueNode);
                case NodeType.Dict:
                    return EvaluateDictNode(scope, node as DictNode);
                case NodeType.Dot:
                    return EvaluateDotNode(scope, node as DotNode);
                case NodeType.Export:
                    return EvaluateExportNode(scope, node as ExportNode);
                case NodeType.FloatLiteral:
                    return EvaluateFloatLiteralNode(scope, node as FloatLiteralNode);
                case NodeType.For:
                    return EvaluateForNode(scope, node as ForNode);
                case NodeType.FunctionDefinition:
                    return EvaluateFunctionDefinitionNode(scope, node as FunctionDefinitionNode);
                case NodeType.Identifier:
                    return EvaluateIdentifierNode(scope, node as IdentifierNode);
                case NodeType.If:
                    return EvaluateIfNode(scope, node as IfNode);
                case NodeType.Import:
                    return EvaluateImportNode(scope, node as ImportNode);
                case NodeType.InitVariable:
                    return EvaluateInitVariableNode(scope, node as InitVariableNode);
                case NodeType.Instantiate:
                    return EvaluateInstantiateNode(scope, node as InstantiateNode);
                case NodeType.IntLiteral:
                    return EvaluateIntLiteralNode(scope, node as IntLiteralNode);
                case NodeType.List:
                    return EvaluateListNode(scope, node as ListNode);
                case NodeType.NativeImport:
                    return EvaluateNativeImportNode(scope, node as NativeImportNode);
                case NodeType.Return:
                    return EvaluateReturnNode(scope, node as ReturnNode);
                case NodeType.StringLiteral:
                    return EvlauateStringLiteralNode(scope, node as StringLiteralNode);
                case NodeType.Token:
                    return EvaluateTokenNode(scope, node as TokenNode);
                case NodeType.UnaryExpression:
                    return EvaluateUnaryExpressionNode(scope, node as UnaryExpressionNode);
                case NodeType.While:
                    return EvaluateWhileNode(scope, node as WhileNode);
                default:
                    throw new Exception("hmmm");
            }
        }

        private static SValue EvaluateWhileNode(Scope scope, WhileNode self) {
            Scope whileScope = new(scope, self.StartPosition);
            SValue lastVal = SValue.Null;

            while (true) {
                if (!self.condNode.Evaluate(whileScope).IsTruthy()) break;
                var whileBlockRes = self.block.Evaluate(whileScope);
                if (!whileBlockRes.IsNull()) lastVal = whileBlockRes;

                if (whileScope.State == ScopeState.ShouldBreak) break;
                if (whileScope.State != ScopeState.None) whileScope.SetState(ScopeState.None);
            }

            return lastVal;
        }

        private static SValue EvaluateUnaryExpressionNode(Scope scope, UnaryExpressionNode self) {
            switch (self.token.Type) {
                case SyntaxType.Bang: return self.rhs.Evaluate(scope).Not();
                case SyntaxType.Minus: return self.rhs.Evaluate(scope).ArithNot();
                case SyntaxType.Plus: return self.rhs.Evaluate(scope);
                default: throw new InvalidOperationException();
            }
        }

        private static SValue EvaluateTokenNode(Scope scope, TokenNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvlauateStringLiteralNode(Scope scope, StringLiteralNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateReturnNode(Scope scope, ReturnNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateNativeImportNode(Scope scope, NativeImportNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateListNode(Scope scope, ListNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateIntLiteralNode(Scope scope, IntLiteralNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateInstantiateNode(Scope scope, InstantiateNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateInitVariableNode(Scope scope, InitVariableNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateImportNode(Scope scope, ImportNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateIfNode(Scope scope, IfNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateIdentifierNode(Scope scope, IdentifierNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateFunctionDefinitionNode(Scope scope, FunctionDefinitionNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateForNode(Scope scope, ForNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateFloatLiteralNode(Scope scope, FloatLiteralNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateExportNode(Scope scope, ExportNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateDotNode(Scope scope, DotNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateDictNode(Scope scope, DictNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateContinueNode(Scope scope, ContinueNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateClassPropertyDefinitionNode(Scope scope, ClassPropDefinitionNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateClassFunctionDefinitionNode(Scope scope, ClassFunctionDefinitionNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateClassDefinitionNode(Scope scope, ClassDefinitionNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateCastNode(Scope scope, CastNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvaluateCallNode(Scope scope, CallNode self) {
            throw new NotImplementedException();
        }

        internal static SValue EvaluateAssignVariableNode(Scope scope, AssignVariableNode self) {
            if (scope.Get(self.Ident.Value.ToString()) == null) {
                throw new InvalidOperationException("Can not assign to a non-existant identifier");
            }

            var val = self.Expr.Evaluate(scope);
            var key = self.Ident.Value.ToString();

            if (!scope.Update(key, val, out Exception ex)) throw ex;
            return val;
        }

        internal static SValue EvaluateBinaryExpressionNode(Scope scope, BinaryExpressionNode self) {
            var leftRes = self.left.Evaluate(scope);
            var rightRes = self.right.Evaluate(scope);

            switch (self.operatorToken.Type) {
                case SyntaxType.Plus:
                    return leftRes.Add(rightRes);
                case SyntaxType.Minus:
                    return leftRes.Sub(rightRes);
                case SyntaxType.Div:
                    return leftRes.Div(rightRes);
                case SyntaxType.Mul:
                    return leftRes.Mul(rightRes);
                case SyntaxType.Mod:
                    return leftRes.Mod(rightRes);
                case SyntaxType.EqualsEquals:
                    return leftRes.Equals(rightRes);
                case SyntaxType.BangEquals:
                    return leftRes.Equals(rightRes).Not();
                case SyntaxType.Idx:
                    return leftRes.Idx(rightRes);
                case SyntaxType.LessThan:
                    return leftRes.LessThan(rightRes);
                case SyntaxType.LessThanEqu:
                    return leftRes.LessThanEqu(rightRes);
                case SyntaxType.GreaterThan:
                    return leftRes.GreaterThan(rightRes);
                case SyntaxType.GreaterThanEqu:
                    return leftRes.GreaterThanEqu(rightRes);
                case SyntaxType.AndAnd:
                    return new SInt((leftRes.IsTruthy() && rightRes.IsTruthy()) ? 1 : 0);
                case SyntaxType.OrOr:
                    return new SInt((leftRes.IsTruthy() || rightRes.IsTruthy()) ? 1 : 0);
                default:
                    throw new NotImplementedException($"Operator {self.operatorToken.Type} does not have an implementation for binary expressions.");
            }
        }

        internal static SValue EvaluateBlockNode(Scope scope, BlockNode self) {
            var lastVal = SValue.Null;
            var blockScope = scope;

            if (self.createNewScope) blockScope = new Scope(scope, self.StartPosition);

            foreach (var node in self.Nodes) {
                var res = node.Evaluate(blockScope);

                if (!res.IsNull()) {
                    lastVal = res;
                }

                if (scope.State == ScopeState.ShouldBreak
                    || scope.State == ScopeState.ShouldContinue) return lastVal;

                if (scope.State == ScopeState.ShouldReturn) {
                    //Debug.WriteLine("Returning from block node at range " + StartPosition + ".." + EndPosition + " with value " + scope.ReturnValue.ToString());
                    var v = scope.ReturnValue;
                    return v;
                }
            }
            return lastVal;
        }

        internal static SValue EvaluateBreakNode(Scope scope, BreakNode self) {
            scope.SetState(ScopeState.ShouldBreak);
            return SValue.Null;
        }
    }

    public struct InterpreterResult {
        public List<SyntaxToken> LexedTokens = null;
        public SyntaxNode AST = null;
        public SValue LastValue = null;

        public InterpreterResult() { }
    }

    public struct TimingInterpreterResult {
        public InterpreterResult Result = new();

        public double LexTime = 0, ParseTime = 0, EvalTime = 0;

        public TimingInterpreterResult() { }
    }
}
