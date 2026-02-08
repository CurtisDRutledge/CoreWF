// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace System.Activities.XamlIntegration;

[Serializable]
public class TextExpressionCompilerError
{
    public TextExpressionCompilerError() { }

    public bool IsWarning { get; set; }

    public int SourceLineNumber { get; set; }

    public string Message { get; set; }

    public string Number { get; set; }

    // To be used with reflection in Studio Web
    // marked as so it's not referenced in wrong places
    public Diagnostic Diagnostic { get; set; }

    public override string ToString()
    {
        return Message;
    }
}
