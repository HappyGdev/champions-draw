//using UnityEngine;
//using UnityEditor;


//[CustomPropertyDrawer(typeof(PropertyColor))]
//public class PropertyColorDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        var atrr = attribute as PropertyColor; 

//        if(atrr.color != null)
//        {
//            Color originalColor=GUI.color;
//            GUI.color=atrr.color;
//            EditorGUI.PropertyField(position, property, label,true);
//            GUI.color=originalColor;
//        }
//    }
//}
