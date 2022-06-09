using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaghetto {
    internal abstract class Node {
        public Position posStart;
        public Position posEnd;

        public abstract RuntimeResult Visit(Context context);
    }

    internal class NumberNode : Node {
        public Token token;

        public NumberNode(Token token) {
            this.token = token;
            this.posStart = token.posStart;
            this.posEnd = token.posEnd;
        }

        public override string ToString() {
            return token.ToString();
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().Success(new Number((double)token.value).SetContext(context).SetPosition(this.posStart, this.posEnd));
        }
    }

    internal class StringNode : Node {
        public Token token;

        public StringNode(Token token) {
            this.token = token;
            this.posStart = token.posStart;
            this.posEnd = token.posEnd;
        }

        public override string ToString() {
            return token.ToString();
        }

        public override RuntimeResult Visit(Context context) {
            return new RuntimeResult().Success(new StringValue((string)token.value).SetContext(context).SetPosition(this.posStart, this.posEnd));
        }
    }

    internal class ListNode : Node {
        public List<Node> elementNodes = new();

        public ListNode(List<Node> elementNodes, Position posStart, Position posEnd) {
            this.elementNodes = elementNodes;
            this.posStart = posStart;
            this.posEnd = posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            List<Value> elements = new();

            foreach(Node elementNode in elementNodes) {
                elements.Add(res.Register(elementNode.Visit(context)));
                if (res.error) return res;
            }

            return res.Success(new ListValue(elements).SetContext(context).SetPosition(posStart, posEnd));
        }
    }

    internal class BinaryOperationNode : Node {
        public Node leftNode, rightNode;
        public Token op;

        public BinaryOperationNode(Node leftNode, Token op, Node rightNode) {
            this.leftNode = leftNode;
            this.op = op;
            this.rightNode = rightNode;

            this.posStart = leftNode.posStart;
            this.posEnd = rightNode.posEnd;
        }

        public override string ToString() {
            return $"({leftNode}, {op}, {rightNode})";
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new RuntimeResult();

            Value result = null;
            SpaghettoException error = null;

            Value left = res.Register(leftNode.Visit(context));
            if (res.error != null) return res;
            Value right = res.Register(rightNode.Visit(context));
            if (res.error != null) return res;

            if (op.type == TokenType.Plus) {
                (result, error) = left.AddedTo(right);
            } else if (op.type == TokenType.Minus) {
                (result, error) = left.SubtractedBy(right);
            } else if (op.type == TokenType.Mul) {
                (result, error) = left.MultipliedBy(right);
            } else if (op.type == TokenType.Div) {
                (result, error) = left.DividedBy(right);
            } else if (op.type == TokenType.Pow) {
                (result, error) = left.PoweredBy(right);
            } else if (op.type == TokenType.Mod) {
                (result, error) = left.Modulo(right);
            } else if (op.type == TokenType.EqualsEquals) {
                (result, error) = left.IsEqualTo(right);
            } else if (op.type == TokenType.NotEquals) {
                (result, error) = left.IsNotEqualTo(right);
            } else if (op.type == TokenType.GreaterThan) {
                (result, error) = left.IsGreaterThan(right);
            } else if (op.type == TokenType.GreaterThanOrEquals) {
                (result, error) = left.IsGreaterThanOrEquals(right);
            } else if (op.type == TokenType.LessThan) {
                (result, error) = left.IsLessThan(right);
            } else if (op.type == TokenType.LessThanOrEquals) {
                (result, error) = left.IsLessThanOrEquals(right);
            } else if (op.type == TokenType.LessThanOrEquals) {
                (result, error) = left.IsLessThanOrEquals(right);
            } else if (op.type == TokenType.Index) {
                (result, error) = left.IndexedBy(right);
            } else if (op.Matches(TokenType.Keyword, "and")) {
                (result, error) = left.AndBy(right);
            } else if (op.Matches(TokenType.Keyword, "or")) {
                (result, error) = left.OrBy(right);
            } else {
                throw new Exception("Internal error occurred. Unknown operator type " + op.type);
            }

            if (error) return res.Failure(error);

            return res.Success(result.SetPosition(this.posStart, this.posEnd));
        }
    }

    internal class UnaryOperatorNode : Node {
        public Node node;
        public Token opToken;

        public UnaryOperatorNode(Token opToken, Node node) {
            this.node = node;
            this.opToken = opToken;

            this.posStart = opToken.posStart;
            this.posEnd = node.posEnd;
        }

        public override string ToString() {
            return $"({opToken}, {node})";
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new RuntimeResult();

            Value number = res.Register(node.Visit(context));
            if (res.error != null) return res;

            SpaghettoException error = null;

            if (opToken.type == TokenType.Minus)
                (number, error) = number.MultipliedBy(new Number(-1));
            else if (opToken.Matches(TokenType.Keyword, "not"))
                (number, error) = number.Not();

            if (error) return res.Failure(error);
            return res.Success(number.SetPosition(this.posStart, this.posEnd));
        }
    }

    internal class VariableAccessNode : Node {
        public Token varNameToken;

        public VariableAccessNode(Token varNameToken) {
            this.varNameToken = varNameToken;
            this.posStart = varNameToken.posStart;
            this.posEnd = varNameToken.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();

            string varName = (string)varNameToken.value;
            Value value = context.symbolTable.Get(varName);

            if(value == null) {
                return res.Failure(new RuntimeError(posStart, posEnd, $"{varName} is not defined", context));
            }

            value = value.Copy().SetPosition(posStart, posEnd).SetContext(context);
            return res.Success(value);
        }
    }

    internal class VariableAssignNode : Node {
        public Token varNameToken;
        public Node valueNode;
        public string varName;

        public VariableAssignNode(Token varNameToken, Node valueNode) {
            this.varNameToken = varNameToken;
            this.valueNode = valueNode;
            this.varName = (string)varNameToken.value;

            this.posStart = varNameToken.posStart;
            this.posEnd = valueNode.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            
            Value value = res.Register(valueNode.Visit(context));
            if (res.error) return res;

            context.symbolTable.Set(varName, value);
            return res.Success(value);
        }
    }

    internal class IfNode : Node {
        public List<(Node cond, Node expr)> cases;

        public IfNode(List<(Node cond, Node expr)> cases) {
            this.cases = cases;
            this.posStart = cases.First().cond.posStart;
            this.posEnd = cases.Last().expr.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();

            foreach((Node cond, Node expr) in cases) {
                Value conditionValue = res.Register(cond.Visit(context));
                if (res.error) return res;

                if(conditionValue.IsTrue()) {
                    Value expressionValue = res.Register(expr.Visit(context));
                    if (res.error) return res;
                    return res.Success(expressionValue);
                } 
            }

            return res.Success(null);
        }
    }

    internal class ForNode : Node {
        public Token varNameToken;
        public Node varStartExpression, condition, continuationExpression, func;
        public bool isStepMode;

        public ForNode(Token varNameToken, Node varStartExpression, Node condition, Node continuationExpression, bool isStep, Node func) {
            this.varNameToken = varNameToken;
            this.varStartExpression = varStartExpression;
            this.condition = condition;
            this.continuationExpression = continuationExpression;
            this.isStepMode = isStep;
            this.func = func;

            this.posStart = varNameToken.posStart;
            this.posEnd = func.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            System.Diagnostics.Stopwatch sw = new();

            sw.Start();

            RuntimeResult res = new();
            string varName = (string)varNameToken.value;
            Value value = res.Register(varStartExpression.Visit(context));
            Value lastValue = null;

            if (res.error != null) return res;
            context.symbolTable.Set(varName, value);

            double currentIteratorValue = (value as Number).value;

            bool isStepStatic = (continuationExpression is NumberNode ? true : false);
            System.Diagnostics.Debug.WriteLine("IsStepStatic: " + isStepStatic);

            double stepStaticValue = (isStepStatic ? (continuationExpression.Visit(context).value as Number).value : 0);

            while(res.Register(condition.Visit(context)).IsTrue()) {
                lastValue = res.Register(func.Visit(context));

                if (isStepMode) {
                    double stepResValue = 0;

                    if (!isStepStatic) {
                        RuntimeResult stepRes = continuationExpression.Visit(context);
                        if (stepRes.error) return res.Failure(stepRes.error);
                        stepResValue = (stepRes.value as Number).value;
                    }

                    currentIteratorValue += (isStepStatic ? stepStaticValue : stepResValue);
                    context.symbolTable.Set(varName, new Number(currentIteratorValue));
                } else {
                    res.Register(continuationExpression.Visit(context));
                }
            }

            Console.WriteLine("\nFinished for loop with var final value being " + context.symbolTable.Get(varName) + " and took " + sw.ElapsedMilliseconds + "ms");

            sw.Stop();

            return res.Success(lastValue);
        }
    }

    internal class WhileNode : Node {
        public Node condition, func;

        public WhileNode(Node condition,  Node func) {
            this.condition = condition;
            this.func = func;

            this.posStart = condition.posStart;
            this.posEnd = func.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            Value lastValue = null;
            
            while (res.Register(condition.Visit(context)).IsTrue()) {
                if (res.error) return res;
                lastValue = res.Register(func.Visit(context));
                if (res.error) return res;
            }

            return res.Success(lastValue);
        }
    }

    internal class FunctionDefinitionNode : Node {
        public Token varNameToken = null;
        public List<Token> argNameTokens;
        public Node bodyNode;

        public FunctionDefinitionNode(Token varNameToken, List<Token> argNameTokens, Node bodyNode) {
            this.varNameToken = varNameToken;
            this.argNameTokens = argNameTokens;
            this.bodyNode = bodyNode;

            if (varNameToken != null) {
                posStart = varNameToken.posStart;
            } else if(argNameTokens.Count > 0) {
                posStart = argNameTokens[0].posStart;
            } else {
                posStart = bodyNode.posStart;
            }

            posEnd = bodyNode.posEnd;
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();

            string funcName = (varNameToken == null ? null : (string)varNameToken.value);
            List<string> argNames = (from tok in argNameTokens select (string)tok.value).ToList();
            Value funcValue = new Function(funcName, bodyNode, argNames).SetContext(context).SetPosition(posStart, posEnd);

            if(varNameToken != null) {
                context.symbolTable.Set(funcName, funcValue);
            }

            return res.Success(funcValue);
        }
    }

    internal class CallNode : Node {
        public Node nodeToCall;
        public List<Node> argNodes;

        public CallNode(Node nodeToCall, List<Node> argNodes) {
            this.nodeToCall = nodeToCall;
            this.argNodes = argNodes;

            posStart = nodeToCall.posStart;

            if(argNodes.Count > 0) {
                posEnd = argNodes.Last().posEnd;
            }else {
                posEnd = nodeToCall.posEnd;
            }
        }

        public override RuntimeResult Visit(Context context) {
            RuntimeResult res = new();
            List<Value> args = new();

            Value valueToCall = res.Register(nodeToCall.Visit(context));
            if (res.error) return res;
            valueToCall = valueToCall.Copy().SetPosition(posStart, posEnd);

            foreach(Node argNode in argNodes) {
                args.Add(res.Register(argNode.Visit(context)));
                if (res.error) return res;
            }

            Value retValue = res.Register(valueToCall.Execute(args));
            if (res.error) return res;
            if(retValue != null) retValue = retValue.Copy().SetPosition(posStart, posEnd).SetContext(context);

            return res.Success(retValue);
        }
    }
}
