using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Code.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(EnumerableDropdownAttribute))]
    public class EnumerableDropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumerableDropdownAttribute dropdownAttribute = attribute as EnumerableDropdownAttribute;
            ScriptableObject scriptableObject = property.serializedObject.targetObject as ScriptableObject;
            
            IEnumerable<string> options = dropdownAttribute.GetOptions(scriptableObject);
            
            string[] optionsArray = options.ToArray();
            
            int selectedIndex = Mathf.Max(0, Array.IndexOf(optionsArray, property.stringValue));
            
            int newSelectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, optionsArray);
            
            if (newSelectedIndex >= 0 && newSelectedIndex < optionsArray.Length)
            {
                property.stringValue = optionsArray[newSelectedIndex];
            }
        }
    }
}