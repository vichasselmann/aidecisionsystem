using UnityEditor;

namespace AI
{
    [CustomEditor(typeof(ConsiderationHandler))]
    public class ConsiderationHandlerEditor : Editor
    {
        private SerializedProperty considerationProperty;
        private ObjectReorderableList<Consideration> considerationList;

        private void OnEnable()
        {
            considerationProperty = serializedObject.FindProperty("considerations");
            considerationList = new ObjectReorderableList<Consideration>(Repaint, serializedObject, considerationProperty);
        }

        private void OnDisable()
        {
            if(null != considerationList.HighlightedElementEditor)
            {
                DestroyImmediate(considerationList.HighlightedElementEditor, true);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            {
                serializedObject.Update();
                EditorGUILayout.Space();
                considerationList.DoLayoutList();
                EditorGUILayout.Space();
                serializedObject.ApplyModifiedProperties();
            }

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            if(null != considerationList.HighlightedElementEditor && null != considerationList.HighlightedElement)
            {
                EditorGUI.BeginChangeCheck();
                {
                    considerationList.HighlightedElementEditor.serializedObject.Update();
                    considerationList.HighlightedElementEditor.OnInspectorGUI();
                    considerationList.HighlightedElementEditor.serializedObject.ApplyModifiedProperties();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(considerationList.HighlightedElement);
                }
            }

        }
    }
}
