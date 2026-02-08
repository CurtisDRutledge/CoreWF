// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities;

public abstract class ActivityWithResult : Activity
{
    public ActivityWithResult() : base() { }

    public Type ResultType => InternalResultType;

    [IgnoreDataMember] // this member is repeated by all subclasses, which we control
    public OutArgument Result
    {
        get => ResultCore;
        set => ResultCore = value;
    }

    public abstract Type InternalResultType { get; }

    public abstract OutArgument ResultCore { get; set; }

    public RuntimeArgument ResultRuntimeArgument { get; set; }

    public abstract object InternalExecuteInResolutionContextUntyped(CodeActivityContext resolutionContext);

    public override bool IsActivityWithResult => true;
}
