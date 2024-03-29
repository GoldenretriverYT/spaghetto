﻿using spaghetto.BuiltinTypes;
using spaghetto.Helpers;
using System.Text;

namespace spaghetto.Parsing.Nodes
{
    internal class DotNode : SyntaxNode
    {
        public DotNode(SyntaxNode callNode) : base(callNode.StartPosition, -1) // ending pos is overwritten
        {
            CallNode = callNode;
        }

        public SyntaxNode CallNode { get; }
        public List<SyntaxNode> NextNodes { get; internal set; } = new();
        public override int EndPosition => NextNodes.GetEndingPosition(CallNode.EndPosition);

        public override NodeType Type => NodeType.Dot;

        public override SValue Evaluate(Scope scope)
        {
            var currentValue = CallNode.Evaluate(scope);

            foreach (var node in NextNodes)
            {
                if (node is IdentifierNode rvn)
                {
                    var ident = rvn.Token;
                    currentValue = currentValue.Dot(new SString(ident.Text));
                }else if(node is AssignVariableNode avn) {
                    var ident = avn.Ident;
                    return currentValue.DotAssignment(new SString(ident.Text), avn.Expr.Evaluate(scope));
                }
                else if (node is CallNode cn)
                {
                    if (cn.ToCallNode is IdentifierNode cnIdentNode)
                    {
                        var ident = cnIdentNode.Token;
                        var lhs = currentValue.Dot(new SString(ident.Text));

                        var args = cn.EvaluateArgs(scope);
                        if (lhs is SBaseFunction func && func.IsClassInstanceMethod) {
                            var idxOfSelf = func.ExpectedArgs.IndexOf("self");
                            if(idxOfSelf != -1) args.Insert(idxOfSelf, currentValue);
                        }

                        currentValue = lhs.Call(scope, args);
                    }
                    else
                    {
                        throw new Exception("Tried to call a non identifier in dot node stack.");
                    }
                }
                else
                {
                    throw new Exception("Unexpected node in dot node stack!");
                }
            }

            return currentValue;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CallNode;

            foreach (var node in NextNodes) yield return node;
        }

        public override string ToString()
        {
            return "DotNode:";
        }

        public DotNode Clone()
        {
            var dn = new DotNode(CallNode);
            dn.NextNodes = this.NextNodes.ToList(); return dn;
        }

        public override string GenerateSource(int depth) {
            var sb = new StringBuilder();

            sb.Append(CallNode.GenerateSource(depth + 1));
            if (NextNodes.Count > 0) sb.Append(".");

            foreach (var n in NextNodes) {
                sb.Append(n.GenerateSource(depth + 1));
                if (n != NextNodes.Last()) sb.Append(".");
            }

            return sb.ToString();
        }
    }
}
