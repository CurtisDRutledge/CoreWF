// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities;
using Internals;
using Runtime;

#if DYNAMICUPDATE
using System.Activities.DynamicUpdate;
#endif

public abstract class NativeActivity : Activity
#if DYNAMICUPDATE
        , IInstanceUpdatable 
#endif
{
    protected NativeActivity()
        : base()
    {
    }

    [IgnoreDataMember]
    public sealed override Version ImplementationVersion
    {
        get => null;
        set
        {
            if (value != null)
            {
                throw FxTrace.Exception.AsError(new NotSupportedException());
            }
        }
    }

    [IgnoreDataMember]
    [Fx.Tag.KnownXamlExternal]
    public sealed override Func<Activity> Implementation
    {
        get => null;
        set
        {
            if (value != null)
            {
                throw FxTrace.Exception.AsError(new NotSupportedException());
            }
        }
    }

    protected virtual bool CanInduceIdle => false;

    public override bool InternalCanInduceIdle => CanInduceIdle;

    protected abstract void Execute(NativeActivityContext context);

    protected virtual void Abort(NativeActivityAbortContext context) { }

    protected virtual void Cancel(NativeActivityContext context)
    {
        if (!context.IsCancellationRequested)
        {
            throw FxTrace.Exception.AsError(new InvalidOperationException(SR.DefaultCancelationRequiresCancelHasBeenRequested));
        }
        context.Cancel();
    }

    sealed public override void OnInternalCacheMetadata(bool createEmptyBindings)
    {
        NativeActivityMetadata metadata = new(this, GetParentEnvironment(), createEmptyBindings);
        CacheMetadata(metadata);
        metadata.Dispose();
    }

    protected sealed override void CacheMetadata(ActivityMetadata metadata)
    {
        throw FxTrace.Exception.AsError(new InvalidOperationException(SR.WrongCacheMetadataForNativeActivity));
    }

    protected virtual void CacheMetadata(NativeActivityMetadata metadata)
    {
        ReflectedInformation information = new(this);

        // We bypass the metadata structure to avoid the checks for null entries
        SetArgumentsCollection(information.GetArguments(), metadata.CreateEmptyBindings);
        SetChildrenCollection(information.GetChildren());
        SetDelegatesCollection(information.GetDelegates());
        SetVariablesCollection(information.GetVariables());
    }

#if DYNAMICUPDATE
    public sealed override void OnInternalCreateDynamicUpdateMap(DynamicUpdateMapBuilder.Finalizer finalizer,
        DynamicUpdateMapBuilder.IDefinitionMatcher matcher, Activity originalActivity)
    {
        NativeActivityUpdateMapMetadata metadata = new NativeActivityUpdateMapMetadata(finalizer, matcher, this);
        try
        {
            OnCreateDynamicUpdateMap(metadata, originalActivity);
        }
        finally
        {
            metadata.Dispose();
        }
    }

    protected sealed override void OnCreateDynamicUpdateMap(UpdateMapMetadata metadata, Activity originalActivity)
    {
        throw FxTrace.Exception.AsError(new InvalidOperationException(SR.WrongOnCreateDynamicUpdateMapForNativeActivity));
    }

    [SuppressMessage(FxCop.Category.Design, FxCop.Rule.ConsiderPassingBaseTypesAsParameters,
        Justification = "Runtime passes in derived class to make more functionality availble to overriders")]
    protected virtual void OnCreateDynamicUpdateMap(NativeActivityUpdateMapMetadata metadata, Activity originalActivity)
    {
        // default UpdateMapMetadata.AllowUpdateInsideThisActivity is TRUE 
        if (!metadata.IsUpdateExplicitlyAllowedOrDisallowed && !DoPublicChildrenMatch(metadata, this, originalActivity))
        {
            metadata.DisallowUpdateInsideThisActivity(SR.PublicChildrenChangeBlockDU);
        }
    }

    public static bool DoPublicChildrenMatch(UpdateMapMetadata metadata, Activity updatedActivity, Activity originalActivity)
    {
        return ActivityComparer.ListEquals(updatedActivity.Children, originalActivity.Children, metadata.AreMatch) &&
            ActivityComparer.ListEquals(updatedActivity.Delegates, originalActivity.Delegates, metadata.AreMatch) &&
            ActivityComparer.ListEquals(updatedActivity.ImportedChildren, originalActivity.ImportedChildren, metadata.AreMatch) &&
            ActivityComparer.ListEquals(updatedActivity.ImportedDelegates, originalActivity.ImportedDelegates, metadata.AreMatch);
    }

    void IInstanceUpdatable.InternalUpdateInstance(NativeActivityUpdateContext updateContext)
    {
        UpdateInstance(updateContext);
    }

    protected virtual void UpdateInstance(NativeActivityUpdateContext updateContext)
    {
        // note that this may be called multiple times on this same activity but with different instances
        // Override this only if you need to update runtime state as part of a dynamic update.            
    } 
#endif

    public override void InternalExecute(ActivityInstance instance, ActivityExecutor executor, BookmarkManager bookmarkManager)
    {
        NativeActivityContext context = executor.NativeActivityContextPool.Acquire();
        try
        {
            context.Initialize(instance, executor, bookmarkManager);
            Execute(context);
        }
        finally
        {
            context.Dispose();
            executor.NativeActivityContextPool.Release(context);
        }
    }

    public override void InternalAbort(ActivityInstance instance, ActivityExecutor executor, Exception terminationReason)
    {
        NativeActivityAbortContext context = new(instance, executor, terminationReason);
        try
        {
            Abort(context);
        }
        finally
        {
            context.Dispose();
        }
    }

    public override void InternalCancel(ActivityInstance instance, ActivityExecutor executor, BookmarkManager bookmarkManager)
    {
        NativeActivityContext context = executor.NativeActivityContextPool.Acquire();
        try
        {
            context.Initialize(instance, executor, bookmarkManager);
            Cancel(context);
        }
        finally
        {
            context.Dispose();
            executor.NativeActivityContextPool.Release(context);
        }
    }
}

public abstract class NativeActivity<TResult> : Activity<TResult>
#if DYNAMICUPDATE
    , IInstanceUpdatable 
#endif
{

    protected NativeActivity()
        : base()
    {
    }

    public sealed override Version ImplementationVersion
    {
        get => null;
        set
        {
            if (value != null)
            {
                throw FxTrace.Exception.AsError(new NotSupportedException());
            }
        }
    }

    [IgnoreDataMember]
    [Fx.Tag.KnownXamlExternal]
    public sealed override Func<Activity> Implementation
    {
        get => null;
        set
        {
            if (value != null)
            {
                throw FxTrace.Exception.AsError(new NotSupportedException());
            }
        }
    }

    protected virtual bool CanInduceIdle => false;

    public override bool InternalCanInduceIdle => CanInduceIdle;

    protected abstract void Execute(NativeActivityContext context);

    protected virtual void Abort(NativeActivityAbortContext context) { }

    protected virtual void Cancel(NativeActivityContext context)
    {
        if (!context.IsCancellationRequested)
        {
            throw FxTrace.Exception.AsError(new InvalidOperationException(SR.DefaultCancelationRequiresCancelHasBeenRequested));
        }
        context.Cancel();
    }

    sealed public override void OnInternalCacheMetadataExceptResult(bool createEmptyBindings)
    {
        NativeActivityMetadata metadata = new(this, GetParentEnvironment(), createEmptyBindings);
        CacheMetadata(metadata);
        metadata.Dispose();
    }

    protected sealed override void CacheMetadata(ActivityMetadata metadata)
    {
        throw FxTrace.Exception.AsError(new InvalidOperationException(SR.WrongCacheMetadataForNativeActivity));
    }

    protected virtual void CacheMetadata(NativeActivityMetadata metadata)
    {
        ReflectedInformation information = new(this);

        // We bypass the metadata structure to avoid the checks for null entries
        SetArgumentsCollection(information.GetArguments(), metadata.CreateEmptyBindings);
        SetChildrenCollection(information.GetChildren());
        SetDelegatesCollection(information.GetDelegates());
        SetVariablesCollection(information.GetVariables());
    }

#if DYNAMICUPDATE
    public sealed override void OnInternalCreateDynamicUpdateMap(DynamicUpdateMapBuilder.Finalizer finalizer,
        DynamicUpdateMapBuilder.IDefinitionMatcher matcher, Activity originalActivity)
    {
        NativeActivityUpdateMapMetadata metadata = new NativeActivityUpdateMapMetadata(finalizer, matcher, this);
        try
        {
            OnCreateDynamicUpdateMap(metadata, originalActivity);
        }
        finally
        {
            metadata.Dispose();
        }
    }

    protected sealed override void OnCreateDynamicUpdateMap(UpdateMapMetadata metadata, Activity originalActivity)
    {
        throw FxTrace.Exception.AsError(new InvalidOperationException(SR.WrongOnCreateDynamicUpdateMapForNativeActivity));
    }

    [SuppressMessage(FxCop.Category.Design, FxCop.Rule.ConsiderPassingBaseTypesAsParameters,
        Justification = "Runtime passes in derived class to make more functionality availble to overriders")]
    protected virtual void OnCreateDynamicUpdateMap(NativeActivityUpdateMapMetadata metadata, Activity originalActivity)
    {
        // default UpdateMapMetadata.AllowUpdateInsideThisActivity is TRUE 
        if (!metadata.IsUpdateExplicitlyAllowedOrDisallowed && !NativeActivity.DoPublicChildrenMatch(metadata, this, originalActivity))
        {
            metadata.DisallowUpdateInsideThisActivity(SR.PublicChildrenChangeBlockDU);
        }
    }

    void IInstanceUpdatable.InternalUpdateInstance(NativeActivityUpdateContext updateContext)
    {
        UpdateInstance(updateContext);
    }

    protected virtual void UpdateInstance(NativeActivityUpdateContext updateContext)
    {
        // note that this may be called multiple times on this same activity but with different instances
        // Override this only if you need to update runtime state as part of a dynamic update.            
    } 
#endif

    public override void InternalExecute(ActivityInstance instance, ActivityExecutor executor, BookmarkManager bookmarkManager)
    {
        NativeActivityContext context = executor.NativeActivityContextPool.Acquire();
        try
        {
            context.Initialize(instance, executor, bookmarkManager);
            Execute(context);
        }
        finally
        {
            context.Dispose();
            executor.NativeActivityContextPool.Release(context);
        }
    }

    public override void InternalAbort(ActivityInstance instance, ActivityExecutor executor, Exception terminationReason)
    {
        NativeActivityAbortContext context = new(instance, executor, terminationReason);
        try
        {
            Abort(context);
        }
        finally
        {
            context.Dispose();
        }
    }

    public override void InternalCancel(ActivityInstance instance, ActivityExecutor executor, BookmarkManager bookmarkManager)
    {
        NativeActivityContext context = executor.NativeActivityContextPool.Acquire();
        try
        {
            context.Initialize(instance, executor, bookmarkManager);
            Cancel(context);
        }
        finally
        {
            context.Dispose();
            executor.NativeActivityContextPool.Release(context);
        }
    }
}
