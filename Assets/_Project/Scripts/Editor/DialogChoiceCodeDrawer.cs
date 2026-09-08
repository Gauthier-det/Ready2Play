using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogChoiceCodeAttribute))]
public class DialogChoiceCodeDrawer : PropertyDrawer
{
    private const string NoneOption = "(none)";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DialogContainer container = property.serializedObject.targetObject as DialogContainer;
        if (container == null)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        List<string> options = container.nodes
            .SelectMany(node => node.choices)
            .Select(choice => choice.choiceCode)
            .Where(code => !string.IsNullOrEmpty(code))
            .Distinct()
            .ToList();
        options.Insert(0, NoneOption);

        int currentIndex = options.IndexOf(property.stringValue);
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, options.ToArray());
        property.stringValue = selectedIndex == 0 ? string.Empty : options[selectedIndex];
        EditorGUI.EndProperty();
    }
}
