// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace System.Activities;
using Runtime;
using Validation;

//[SuppressMessage(FxCop.Category.Naming, FxCop.Rule.IdentifiersShouldNotHaveIncorrectSuffix,
//    Justification = "Part of the sanctioned, public WF OM")]
[ContentProperty("Handler")]
public abstract class ActivityDelegate
{
    public static string ArgumentName = "Argument";
    public static string Argument1Name = "Argument1";
    public static string Argument2Name = "Argument2";
    public static string Argument3Name = "Argument3";
    public static string Argument4Name = "Argument4";
    public static string Argument5Name = "Argument5";
    public static string Argument6Name = "Argument6";
    public static string Argument7Name = "Argument7";
    public static string Argument8Name = "Argument8";
    public static string Argument9Name = "Argument9";
    public static string Argument10Name = "Argument10";
    public static string Argument11Name = "Argument11";
    public static string Argument12Name = "Argument12";
    public static string Argument13Name = "Argument13";
    public static string Argument14Name = "Argument14";
    public static string Argument15Name = "Argument15";
    public static string Argument16Name = "Argument16";
    public static string ResultArgumentName = "Result";
    private Activity _owner;
    private bool _isDisplayNameSet;
    private string _displayName;
    private IList<RuntimeDelegateArgument> _delegateParameters;
    private int _cacheId;
    private ActivityCollectionType _parentCollectionType;

    protected ActivityDelegate() { }

    public string DisplayName
    {
        get
        {
            if (string.IsNullOrEmpty(_displayName))
            {
                _displayName = GetType().Name;
            }

            return _displayName;
        }
        set
        {
            _isDisplayNameSet = true;
            _displayName = value;
        }
    }

    [DefaultValue(null)]
    public Activity Handler { get; set; }

    public LocationReferenceEnvironment Environment { get; set; }

    public Activity Owner => _owner;

    public ActivityCollectionType ParentCollectionType => _parentCollectionType;

    public IList<RuntimeDelegateArgument> RuntimeDelegateArguments
    {
        get
        {
            if (_delegateParameters != null)
            {
                return _delegateParameters;
            }

            return new ReadOnlyCollection<RuntimeDelegateArgument>(InternalGetRuntimeDelegateArguments());
        }
    }

    public virtual DelegateOutArgument GetResultArgument() => null;

    protected virtual void OnGetRuntimeDelegateArguments(IList<RuntimeDelegateArgument> runtimeDelegateArguments)
    {
        foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(this))
        {
            if (ActivityUtilities.TryGetDelegateArgumentDirectionAndType(propertyDescriptor.PropertyType, out ArgumentDirection direction, out Type innerType))
            {
                runtimeDelegateArguments.Add(new RuntimeDelegateArgument(propertyDescriptor.Name, innerType, direction, (DelegateArgument)propertyDescriptor.GetValue(this)));
            }
        }
    }

    public virtual IList<RuntimeDelegateArgument> InternalGetRuntimeDelegateArguments()
    {
        IList<RuntimeDelegateArgument> result = new List<RuntimeDelegateArgument>();
        OnGetRuntimeDelegateArguments(result);
        return result;
    }

    public void InternalCacheMetadata()
    {
        _delegateParameters = new ReadOnlyCollection<RuntimeDelegateArgument>(InternalGetRuntimeDelegateArguments());
    }

    public bool CanBeScheduledBy(Activity parent)
    {
        // fast path if we're the sole (or first) child
        if (ReferenceEquals(parent, _owner))
        {
            return _parentCollectionType != ActivityCollectionType.Imports;
        }
        else
        {
            return parent.Delegates.Contains(this) || parent.ImplementationDelegates.Contains(this);
        }
    }

    public bool InitializeRelationship(Activity parent, ActivityCollectionType collectionType, ref IList<ValidationError> validationErrors)
    {
        if (_cacheId == parent.CacheId)
        {
            Fx.Assert(_owner != null, "We must have set the owner when we set the cache ID");

            // This means that we've already encountered a parent in the tree

            // Validate that it is visible.

            // In order to see the activity the new parent must be
            // in the implementation IdSpace of an activity which has
            // a public reference to it.
            Activity referenceTarget = parent.MemberOf.Owner;

            if (referenceTarget == null)
            {
                Activity handler = Handler;

                if (handler == null)
                {
                    ActivityUtilities.Add(ref validationErrors, new ValidationError(SR.ActivityDelegateCannotBeReferencedWithoutTargetNoHandler(parent.DisplayName, _owner.DisplayName), false, parent));
                }
                else
                {
                    ActivityUtilities.Add(ref validationErrors, new ValidationError(SR.ActivityDelegateCannotBeReferencedWithoutTarget(handler.DisplayName, parent.DisplayName, _owner.DisplayName), false, parent));
                }

                return false;
            }
            else if (!referenceTarget.Delegates.Contains(this) && !referenceTarget.ImportedDelegates.Contains(this))
            {
                Activity handler = Handler;

                if (handler == null)
                {
                    ActivityUtilities.Add(ref validationErrors, new ValidationError(SR.ActivityDelegateCannotBeReferencedNoHandler(parent.DisplayName, referenceTarget.DisplayName, _owner.DisplayName), false, parent));
                }
                else
                {
                    ActivityUtilities.Add(ref validationErrors, new ValidationError(SR.ActivityDelegateCannotBeReferenced(handler.DisplayName, parent.DisplayName, referenceTarget.DisplayName, _owner.DisplayName), false, parent));
                }

                return false;
            }

            // This is a valid reference so we want to allow
            // normal processing to proceed.
            return true;
        }

        _owner = parent;
        _cacheId = parent.CacheId;
        _parentCollectionType = collectionType;
        InternalCacheMetadata();

        // We need to setup the delegate environment so that it is
        // available when we process the Handler.
        LocationReferenceEnvironment delegateEnvironment;
        if (collectionType == ActivityCollectionType.Implementation)
        {
            delegateEnvironment = parent.ImplementationEnvironment;
        }
        else
        {
            delegateEnvironment = parent.PublicEnvironment;
        }

        if (RuntimeDelegateArguments.Count > 0)
        {
            ActivityLocationReferenceEnvironment newEnvironment = new(delegateEnvironment);
            delegateEnvironment = newEnvironment;

            for (int argumentIndex = 0; argumentIndex < RuntimeDelegateArguments.Count; argumentIndex++)
            {
                RuntimeDelegateArgument runtimeDelegateArgument = RuntimeDelegateArguments[argumentIndex];
                DelegateArgument delegateArgument = runtimeDelegateArgument.BoundArgument;

                if (delegateArgument != null)
                {
                    if (delegateArgument.Direction != runtimeDelegateArgument.Direction)
                    {
                        ActivityUtilities.Add(ref validationErrors, new ValidationError(SR.RuntimeDelegateArgumentDirectionIncorrect, parent));
                    }

                    if (delegateArgument.Type != runtimeDelegateArgument.Type)
                    {
                        ActivityUtilities.Add(ref validationErrors, new ValidationError(SR.RuntimeDelegateArgumentTypeIncorrect, parent));
                    }

                    // NOTE: We don't initialize this relationship here because
                    // at runtime we'll actually just place these variables in the
                    // environment of the Handler.  We'll initialize and set an
                    // ID when we process the Handler.
                    newEnvironment.Declare(delegateArgument, _owner, ref validationErrors);
                }
            }
        }

        Environment = delegateEnvironment;

        if (Handler != null)
        {
            return Handler.InitializeRelationship(this, collectionType, ref validationErrors);
        }

        return true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeDisplayName() => _isDisplayNameSet;

    public override string ToString() => DisplayName;
}
