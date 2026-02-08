// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities;

[Serializable]
public class WorkflowApplicationAbortedException : WorkflowApplicationException
{
    public WorkflowApplicationAbortedException()
    {
    }

    public WorkflowApplicationAbortedException(string message)
        : base(message)
    {
    }

    public WorkflowApplicationAbortedException(string message, Guid instanceId)
        : base(message, instanceId)
    {
    }

    public WorkflowApplicationAbortedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public WorkflowApplicationAbortedException(string message, Guid instanceId, Exception innerException)
        : base(message, instanceId, innerException)
    {
    }

    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
    protected WorkflowApplicationAbortedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
