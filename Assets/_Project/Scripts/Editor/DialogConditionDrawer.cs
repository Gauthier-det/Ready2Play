using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogCondition))]
public class DialogConditionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeProp = property.FindPropertyRelative("type");
        SerializedProperty valueProp = property.FindPropertyRelative("value");
        SerializedProperty valuesProp = property.FindPropertyRelative("values");
        DialogConditionType type = (DialogConditionType)typeProp.enumValueIndex;

        EditorGUI.BeginProperty(position, label, property);

        Rect typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(typeRect, typeProp, label);

        float nextY = typeRect.yMax + EditorGUIUtility.standardVerticalSpacing;

        switch (type)
        {
            case DialogConditionType.LastEquals:
                Rect valueRect = new Rect(position.x, nextY, position.width, EditorGUI.GetPropertyHeight(valueProp));
                EditorGUI.PropertyField(valueRect, valueProp);
                break;
            case DialogConditionType.Contains:
                Rect valuesRect = new Rect(position.x, nextY, position.width, EditorGUI.GetPropertyHeight(valuesProp, true));
                EditorGUI.PropertyField(valuesRect, valuesProp, true);
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeProp = property.FindPropertyRelative("type");
        DialogConditionType type = (DialogConditionType)typeProp.enumValueIndex;

        float height = EditorGUIUtility.singleLineHeight;

        switch (type)
        {
            case DialogConditionType.LastEquals:
                SerializedProperty valueProp = property.FindPropertyRelative("value");
                height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(valueProp);
                break;
            case DialogConditionType.Contains:
                SerializedProperty valuesProp = property.FindPropertyRelative("values");
                height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(valuesProp, true);
                break;
        }

        return height;
    }
}
