// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities;

public static class TypeConverters
{
    public const string ActivityWithResultConverter = "System.Activities.XamlIntegration.ActivityWithResultConverter, Quorum.CoreWF";
    public const string InArgumentConverter = "System.Activities.XamlIntegration.InArgumentConverter, Quorum.CoreWF";
    public const string OutArgumentConverter = "System.Activities.XamlIntegration.OutArgumentConverter, Quorum.CoreWF";
    public const string InOutArgumentConverter = "System.Activities.XamlIntegration.InOutArgumentConverter, Quorum.CoreWF";
    public const string ImplementationVersionConverter = "System.Activities.XamlIntegration.ImplementationVersionConverter, Quorum.CoreWF";
    public const string AssemblyReferenceConverter = "System.Activities.XamlIntegration.AssemblyReferenceConverter, Quorum.CoreWF";
    public const string WorkflowIdentityConverter = "System.Activities.XamlIntegration.WorkflowIdentityConverter, Quorum.CoreWF";
}

public static class OtherXaml
{
    public const string FuncDeferringLoader = "System.Activities.XamlIntegration.FuncDeferringLoader, Quorum.CoreWF";
    public const string Activity = "System.Activities.Activity, System.Activities";
    public const string ArgumentValueSerializer = "System.Activities.XamlIntegration.ArgumentValueSerializer, Quorum.CoreWF";
}
