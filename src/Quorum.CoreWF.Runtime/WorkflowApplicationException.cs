// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities;

[Serializable]
public class WorkflowApplicationException : Exception
{
    private const string InstanceIdName = "instanceId";
    private readonly Guid _instanceId;

    public WorkflowApplicationException()
        : base(SR.DefaultWorkflowApplicationExceptionMessage) { }

    public WorkflowApplicationException(string message)
        : base(message) { }

    public WorkflowApplicationException(string message, Guid instanceId)
        : base(message)
    {
        _instanceId = instanceId;
    }

    public WorkflowApplicationException(string message, Exception innerException)
        : base(message, innerException) { }

    public WorkflowApplicationException(string message, Guid instanceId, Exception innerException)
        : base(message, innerException)
    {
        _instanceId = instanceId;
    }

    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
    protected WorkflowApplicationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        _instanceId = (Guid)info.GetValue(InstanceIdName, typeof(Guid));
    }

    public Guid InstanceId => _instanceId;

    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(InstanceIdName, _instanceId);
    }
}
