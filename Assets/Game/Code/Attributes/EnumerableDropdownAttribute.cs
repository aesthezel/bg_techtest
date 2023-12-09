using System.Collections.Generic;
using UnityEngine;

namespace Game.Code.Attributes
{
    public class EnumerableDropdownAttribute : PropertyAttribute
    {
        private readonly string targetProperty;

        public EnumerableDropdownAttribute(string propertyName)
        {
            targetProperty = propertyName;
        }

        public IEnumerable<string> GetOptions(ScriptableObject scriptableObject)
        {
            var propertyInfo = scriptableObject.GetType().GetProperty(targetProperty);
            return propertyInfo.GetValue(scriptableObject) is not IEnumerable<string> options ? new[] {"Empty"} : options;
        }
    }
}