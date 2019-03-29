using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForceFeedback.Adapters.VisualStudio
{
    public static class SyntaxNodeExtension
    {
        public static bool IsSyntaxBlock(this SyntaxNode node)
        {
            return node.Kind() == SyntaxKind.Block;
        }

        public static bool IsMethod(this SyntaxNode node)
        {
            return node.Kind() == SyntaxKind.MethodDeclaration;
        }

        public static bool IsConstructor(this SyntaxNode node)
        {
            return node.Kind() == SyntaxKind.ConstructorDeclaration;
        }

        public static bool IsSetter(this SyntaxNode node)
        {
            return node.Kind() == SyntaxKind.SetAccessorDeclaration;
        }

        public static bool IsGetter(this SyntaxNode node)
        {
            return node.Kind() == SyntaxKind.GetAccessorDeclaration;
        }
    }
}