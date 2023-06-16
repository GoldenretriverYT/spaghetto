using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using spaghetto.BuiltinTypes;
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
            res.LastValue = GoofyAhhEvaluator.Evaluate(GlobalScope, res.AST);
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

            res.Result.LastValue = GoofyAhhEvaluator.Evaluate(GlobalScope, res.Result.AST);
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
                case NodeType.Repeat:
                    return EvaluateRepeatNode(scope, node as RepeatNode);
                default:
                    throw new Exception("hmmm");
            }
        }

        private static SValue EvaluateRepeatNode(Scope scope, RepeatNode self) {
            var timesRaw = Evaluate(scope, self.timesExpr);
            if (timesRaw is not SInt timesSInt) throw new Exception("Repeat x times expression must evaluate to SInt");
            var times = timesSInt.Value;

            if (self.keepScope) {
                if (self.block is not BlockNode blockNode) throw new Exception("Kept-scope repeat expressions must have a full body.");

                for (int i = 0; i < times; i++) {
                    foreach (var n in blockNode.Nodes) {
                        Evaluate(scope, n);
                    }
                }
            } else {
                for (int i = 0; i < times; i++) {
                    Evaluate(scope, self.block);
                }
            }

            return SValue.Null;
        }

        private static SValue EvaluateWhileNode(Scope scope, WhileNode self) {
            Scope whileScope = new(scope, self.StartPosition);
            SValue lastVal = SValue.Null;

            while (true) {
                if (!Evaluate(whileScope, self.condNode).IsTruthy()) break;
                var whileBlockRes = Evaluate(whileScope, self.block);
                if (!whileBlockRes.IsNull()) lastVal = whileBlockRes;

                if (whileScope.State == ScopeState.ShouldBreak) break;
                if (whileScope.State != ScopeState.None) whileScope.SetState(ScopeState.None);
            }

            return lastVal;
        }

        private static SValue EvaluateUnaryExpressionNode(Scope scope, UnaryExpressionNode self) {
            switch (self.token.Type) {
                case SyntaxType.Bang: return Evaluate(scope, self.rhs).Not();
                case SyntaxType.Minus: return Evaluate(scope, self.rhs).ArithNot();
                case SyntaxType.Plus: return Evaluate(scope, self.rhs);
                default: throw new InvalidOperationException();
            }
        }

        private static SValue EvaluateTokenNode(Scope scope, TokenNode self) {
            throw new NotImplementedException();
        }

        private static SValue EvlauateStringLiteralNode(Scope scope, StringLiteralNode self) {
            return new SString((string)self.syntaxToken.Value);
        }

        private static SValue EvaluateReturnNode(Scope scope, ReturnNode self) {
            if (self.ReturnValueNode != null) {
                var v = Evaluate(scope, self.ReturnValueNode);
                scope.SetReturnValue(v);
            }

            scope.SetState(ScopeState.ShouldReturn);

            return SValue.Null;
        }

        private static SValue EvaluateNativeImportNode(Scope scope, NativeImportNode self) {
            if (self.ident.Text == "all") {
                var rootScope = scope.GetRoot();

                foreach (var kvp in rootScope.Table.ToList()) {
                    if (kvp.Key.StartsWith("nlimporter$$")) {
                        if (kvp.Value is not SNativeLibraryImporter importerFromAllLoop) throw new Exception("Found unexpexted type in root tables nlimporters!");
                        importerFromAllLoop.Import(scope);
                    }
                }

                return SValue.Null;
            }

            var val = scope.Get("nlimporter$$" + self.ident.Text);

            if (val == null || val is not SNativeLibraryImporter importer) {
                throw new Exception("Native library " + self.ident.Text + " not found!");
            }

            importer.Import(scope);
            return SValue.Null;
        }

        private static SValue EvaluateListNode(Scope scope, ListNode self) {
            SList sList = new();

            foreach (var n in self.list) {
                sList.Value.Add(Evaluate(scope, n));
            }

            return sList;
        }

        private static SValue EvaluateIntLiteralNode(Scope scope, IntLiteralNode self) {
            var sint = new SInt((int)self.intToken.Value);
            return sint;
        }

        private static SValue EvaluateInstantiateNode(Scope scope, InstantiateNode self) {
            var @class = scope.Get(self.ident.Text);
            if (@class == null || @class is not SClass sclass) throw new Exception("Class " + self.ident.Text + " not found!");


            var instance = new SClassInstance(sclass);

            List<SValue> args = new() { instance };
            foreach (var n in self.argumentNodes) args.Add(Evaluate(scope, n));

            instance.CallConstructor(scope, args);

            return instance;
        }

        private static SValue EvaluateInitVariableNode(Scope scope, InitVariableNode self) {
            if (scope.Get(self.ident.Value.ToString()) != null) {
                throw new InvalidOperationException("Can not initiliaze the same variable twice!");
            }

            if (self.expr != null) {
                var val = Evaluate(scope, self.expr);
                val.TypeIsFixed = self.isFixedType;
                val.IsConstant = self.isConst;

                scope.Set(self.ident.Value.ToString(), val);
                return val;
            } else {
                if (self.isFixedType) throw new InvalidOperationException("Tried to initiliaze a fixed type variable with no value; this is not permitted. Use var% instead.");
                var nul = new SNull();
                nul.TypeIsFixed = self.isFixedType;
                nul.IsConstant = self.isConst;

                scope.Set(self.ident.Value.ToString(), nul);
                return nul;
            }
        }

        private static SValue EvaluateImportNode(Scope scope, ImportNode self) {
            if (!File.Exists(self.path.Text)) throw new Exception($"Failed to import '{self.path.Text}': File not found");
            var text = File.ReadAllText(self.path.Text);

            Interpreter ip = new();
            Scope rootScope = scope.GetRoot();

            // copy available namespaces provided by runtime
            foreach (var kvp in rootScope.Table) {
                if (kvp.Key.StartsWith("nlimporter$$")) {
                    ip.GlobalScope.Table[kvp.Key] = kvp.Value;
                }
            }

            InterpreterResult res = new();

            try {
                ip.Interpret(text, ref res);

                // copy export table

                foreach (var kvp in ip.GlobalScope.ExportTable) {
                    if (scope.Get(kvp.Key) != null) throw new Exception($"Failed to import '{self.path.Text}': Import conflict; file exports '{kvp.Key}' but that identifier is already present in the current scope.");

                    scope.Set(kvp.Key, kvp.Value);
                }
            } catch (Exception ex) {
                throw new Exception($"Failed to import '{self.path.Text}': {ex.Message}");
            }

            return res.LastValue;
        }

        private static SValue EvaluateIfNode(Scope scope, IfNode self) {
            foreach ((SyntaxNode cond, SyntaxNode block) in self.Conditions) {
                var condRes = Evaluate(scope, cond);

                if (condRes.IsTruthy()) {
                    return Evaluate(new Scope(scope, self.StartPosition), block);
                }
            }

            return SValue.Null;
        }

        private static SValue EvaluateIdentifierNode(Scope scope, IdentifierNode self) {
            return scope.Get(self.Token.Text) ?? (self.NonNull ? throw new Exception("Non-null identifier " + self.Token.Text + " resolved to null!") : SValue.Null);
        }

        private static SValue EvaluateFunctionDefinitionNode(Scope scope, FunctionDefinitionNode self) {
            var f = new SFunction(scope, self.nameToken?.Text ?? "<anonymous>", self.args.Select((v) => v.Text).ToList(), self.block);
            if (self.nameToken != null) scope.Set(self.nameToken.Value.Text, f);
            return f;
        }

        private static SValue EvaluateForNode(Scope scope, ForNode self) {
            Scope forScope = new(scope, self.StartPosition);
            SValue lastVal = SValue.Null;
            Evaluate(forScope, self.initialExpressionNode);

            while (true) {
                if (!Evaluate(forScope, self.condNode).IsTruthy()) break;
                var forBlockRes = Evaluate(forScope, self.block);
                if (!forBlockRes.IsNull()) lastVal = forBlockRes;

                if (forScope.State == ScopeState.ShouldBreak) break;
                if (forScope.State != ScopeState.None) forScope.SetState(ScopeState.None);

                Evaluate(forScope, self.stepNode);
            }

            return lastVal;
        }

        private static SValue EvaluateFloatLiteralNode(Scope scope, FloatLiteralNode self) {
            return new SFloat((float)self.syntaxToken.Value);
        }

        private static SValue EvaluateExportNode(Scope scope, ExportNode self) {
            var val = scope.Get(self.ident.Text);
            if (val == null) throw new Exception("Can not export value of non-existent identifier");

            scope.GetRoot().ExportTable.Add(self.ident.Text, val);
            return val;
        }

        private static SValue EvaluateDotNode(Scope scope, DotNode self) {
            var currentValue = Evaluate(scope, self.CallNode);

            foreach (var node in self.NextNodes) {
                if (node is IdentifierNode rvn) {
                    var ident = rvn.Token;
                    currentValue = currentValue.Dot(new SString(ident.Text));
                } else if (node is AssignVariableNode avn) {
                    var ident = avn.Ident;
                    return currentValue.DotAssignment(new SString(ident.Text), Evaluate(scope, avn.Expr));
                } else if (node is CallNode cn) {
                    if (cn.ToCallNode is IdentifierNode cnIdentNode) {
                        var ident = cnIdentNode.Token;
                        var lhs = currentValue.Dot(new SString(ident.Text));

                        var args = cn.EvaluateArgs(scope);
                        if (lhs is SBaseFunction func && func.IsClassInstanceMethod) {
                            var idxOfSelf = func.ExpectedArgs.IndexOf("self");
                            if (idxOfSelf != -1) args.Insert(idxOfSelf, currentValue);
                        }

                        currentValue = lhs.Call(scope, args);
                    } else {
                        throw new Exception("Tried to call a non identifier in dot node stack.");
                    }
                } else {
                    throw new Exception("Unexpected node in dot node stack!");
                }
            }

            return currentValue;
        }

        private static SValue EvaluateDictNode(Scope scope, DictNode self) {
            var dict = new SDictionary();

            foreach (var ent in self.dict) {
                dict.Value.Add((new SString(ent.tok.Text), Evaluate(scope, ent.expr)));
            }

            return dict;
        }

        private static SValue EvaluateContinueNode(Scope scope, ContinueNode self) {
            scope.SetState(ScopeState.ShouldContinue);
            return SValue.Null;
        }

        private static SValue EvaluateClassPropertyDefinitionNode(Scope scope, ClassPropDefinitionNode self) {
            throw new NotImplementedException("This should not be called!");
        }

        private static SValue EvaluateClassFunctionDefinitionNode(Scope scope, ClassFunctionDefinitionNode self) {
            var targetName = self.name.Text;

            if (targetName is "ctor" or "toString") {
                /*if(args.Where(v => v.Text == "self").Count() != 1) {
                    throw new Exception($"Special class method '{targetName}' must contain the argument 'self' exactly once.");
                }*/

                targetName = "$$" + targetName;
            }

            var f = new SFunction(scope, targetName, self.args.Select((v) => v.Text).ToList(), self.body);
            f.IsClassInstanceMethod = !self.isStatic;
            return f;
        }

        private static SValue EvaluateClassDefinitionNode(Scope scope, ClassDefinitionNode self) {
            var @class = new SClass();
            @class.Name = self.className.Text;
            @class.FixedProps = self.fixedProps;

            foreach (var bodyNode in self.body) {
                if (bodyNode is ClassFunctionDefinitionNode cfdn) {
                    var funcRaw = Evaluate(scope, cfdn);
                    if (funcRaw is not SFunction func) throw new Exception("Expected ClassFunctionDefinitionNode to return SFunction");

                    if (func.IsClassInstanceMethod) {
                        if (func.ExpectedArgs.IndexOf("self") == -1) func.ExpectedArgs.Insert(0, "self");

                        @class.InstanceBaseTable.Add((func.FunctionName, func));
                    } else {
                        @class.StaticTable.Add((func.FunctionName, func));
                    }
                } else if (bodyNode is ClassPropDefinitionNode cpdn) {
                    var val = Evaluate(scope, cpdn.Expression);

                    if (!cpdn.IsStatic) {
                        @class.InstanceBaseTable.Add((cpdn.Name.Text, val));
                    } else {
                        @class.StaticTable.Add((cpdn.Name.Text, val));
                    }
                } else {
                    throw new Exception("Unexpected node in class definition");
                }
            }

            scope.Set(self.className.Text, @class);
            return @class;
        }

        private static SValue EvaluateCastNode(Scope scope, CastNode self) {
            return Evaluate(scope, self.node).CastToBuiltin(self.type);
        }

        private static SValue EvaluateCallNode(Scope scope, CallNode self) {
            var toCall = Evaluate(scope, self.ToCallNode) ?? SValue.Null;
            var args = self.EvaluateArgs(scope);
            return toCall.Call(scope, args) ?? SValue.Null;
        }

        internal static SValue EvaluateAssignVariableNode(Scope scope, AssignVariableNode self) {
            if (scope.Get(self.Ident.Value.ToString()) == null) {
                throw new InvalidOperationException("Can not assign to a non-existant identifier");
            }

            var val = Evaluate(scope, self.Expr);
            var key = self.Ident.Value.ToString();

            if (!scope.Update(key, val, out Exception ex)) throw ex;
            return val;
        }

        internal static SValue EvaluateBinaryExpressionNode(Scope scope, BinaryExpressionNode self) {
            var leftRes = Evaluate(scope, self.left);
            var rightRes = Evaluate(scope, self.right);

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
                var res = Evaluate(blockScope, node);

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
