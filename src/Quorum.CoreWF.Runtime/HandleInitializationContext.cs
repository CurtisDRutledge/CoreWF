// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities;
using Internals;
using Runtime;

[Fx.Tag.XamlVisible(false)]
public sealed class HandleInitializationContext
{
    private readonly ActivityExecutor _executor;
    private readonly ActivityInstance _scope;
    private bool _isDisposed;

    public HandleInitializationContext(ActivityExecutor executor, ActivityInstance scope)
    {
        _executor = executor;
        _scope = scope;
    }

    public ActivityInstance OwningActivityInstance => _scope;

    public ActivityExecutor Executor => _executor;

    public THandle CreateAndInitializeHandle<THandle>() where THandle : Handle
    {
        ThrowIfDisposed();
        THandle value = Activator.CreateInstance<THandle>();

        value.Initialize(this);

        // If we have a scope, we need to add this new handle to the LocationEnvironment.
        if (_scope != null)
        {
            _scope.Environment.AddHandle(value);
        }
        // otherwise add it to the Executor.
        else
        {
            _executor.AddHandle(value);
        }

        return value;
    }

    public T GetExtension<T>() where T : class => _executor.GetExtension<T>();

    public void UninitializeHandle(Handle handle)
    {
        ThrowIfDisposed();
        handle.Uninitialize(this);
    }

    public object CreateAndInitializeHandle(Type handleType)
    {
        Fx.Assert(ActivityUtilities.IsHandle(handleType), "This should only be called with Handle subtypes.");

        object value = Activator.CreateInstance(handleType);

        ((Handle)value).Initialize(this);

        // If we have a scope, we need to add this new handle to the LocationEnvironment.
        if (_scope != null)
        {
            _scope.Environment.AddHandle((Handle)value);
        }
        // otherwise add it to the Executor.
        else
        {
            _executor.AddHandle((Handle)value);
        }

        return value;
    }

    public BookmarkScope CreateAndRegisterBookmarkScope() => _executor.BookmarkScopeManager.CreateAndRegisterScope(Guid.Empty);

    public void UnregisterBookmarkScope(BookmarkScope bookmarkScope)
    {
        Fx.Assert(bookmarkScope != null, "The sub instance should not equal null.");

        _executor.BookmarkScopeManager.UnregisterScope(bookmarkScope);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw FxTrace.Exception.AsError(new ObjectDisposedException(SR.HandleInitializationContextDisposed));
        }
    }

    public void Dispose() => _isDisposed = true;
}
