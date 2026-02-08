// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities;

[Serializable]
public class ValidationException : SystemException
{
    public ValidationException()
    {
    }

    public ValidationException(string message)
        : base(message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
    protected ValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
