using System.Collections.Generic;
using System.Collections.Immutable;

namespace System.Activities.Validation
{
    public sealed class ValidationScope
    {
        private readonly Dictionary<string, ExpressionToValidate> _expressionsToValidate = new();
        private string _language;

        public void AddExpression<T>(ExpressionToValidate expressionToValidate, string language)
        {
            _language ??= language;
            if (_language != language)
            {
                expressionToValidate.Activity.AddTempValidationError(new ValidationError(SR.DynamicActivityMultipleExpressionLanguages(language), expressionToValidate.Activity));
                return;
            }
            _expressionsToValidate.Add(expressionToValidate.Activity.Id, expressionToValidate);
        }

        public string Language => _language;

        public ExpressionToValidate GetExpression(string activityId) => _expressionsToValidate[activityId];

        public ImmutableArray<ExpressionToValidate> GetAllExpressions() => _expressionsToValidate.Values.ToImmutableArray();

        public void Clear() => _expressionsToValidate.Clear();
    }
}