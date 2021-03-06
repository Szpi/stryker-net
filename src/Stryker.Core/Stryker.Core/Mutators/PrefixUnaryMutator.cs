﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class PrefixUnaryMutator : Mutator<PrefixUnaryExpressionSyntax>, IMutator
    {
        private static readonly Dictionary<SyntaxKind, SyntaxKind> UnaryWithOpposite = new Dictionary<SyntaxKind, SyntaxKind>
        {
            {SyntaxKind.UnaryMinusExpression, SyntaxKind.UnaryPlusExpression},
            {SyntaxKind.UnaryPlusExpression, SyntaxKind.UnaryMinusExpression},
            {SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression},
            {SyntaxKind.PreDecrementExpression, SyntaxKind.PreIncrementExpression},
        };

        private static readonly HashSet<SyntaxKind> UnaryToInitial = new HashSet<SyntaxKind>
        {
            SyntaxKind.BitwiseNotExpression,
            SyntaxKind.LogicalNotExpression
        };

        public override IEnumerable<Mutation> ApplyMutations(PrefixUnaryExpressionSyntax node)
        {
            var unaryKind = node.Kind();
            if (UnaryWithOpposite.TryGetValue(unaryKind, out var oppositeKind))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.PrefixUnaryExpression(oppositeKind, node.Operand),
                    DisplayName = $"{unaryKind} to {oppositeKind} mutation",
                    Type = nameof(PrefixUnaryMutator)
                };
            }
            else if (UnaryToInitial.Contains(unaryKind))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = node.Operand,
                    DisplayName = $"{unaryKind} to un-{unaryKind} mutation",
                    Type = nameof(PrefixUnaryMutator)
                };
            }
        }
    }
}