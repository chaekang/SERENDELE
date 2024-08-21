/*using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    [CustomPropertyDrawer(typeof(MinAttribute))]
    sealed class MinDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinAttribute attribute = (MinAttribute)base.attribute;

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                int v = EditorGUI.IntField(position, label, property.intValue);
                property.intValue = (int)Mathf.Max(v, attribute.min);
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                float v = EditorGUI.FloatField(position, label, property.floatValue);
                property.floatValue = Mathf.Max(v, attribute.min);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Min with float or int.");
            }
        }
    }
}*/

/*갑자기 여기서 컴파일 에러(error CS0104: 'MinAttribute' is an ambiguous reference between 'UnityEngine.PostProcessing.MinAttribute' and 'UnityEngine.MinAttribute'
가 생겨서 잠시 주석처리 해뒀습니다. 스크립트 파일에 없고 에셋 파일에 있길래 안 쓴 코드인가 해서... 코드는 아예 건드린거 없어요! -보민 */
