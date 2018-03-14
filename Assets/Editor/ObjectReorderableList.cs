using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace AI
{
    public class ObjectReorderableList<T> : ReorderableList where T : ScriptableObject
    {
        private Object target;
        private Object highlightedElement;
        private Editor highlightedElementEditor;
        private UnityAction repaint;

        public Object HighlightedElement
        {
            get { return highlightedElement; }
        }

        public Editor HighlightedElementEditor
        {
            get { return highlightedElementEditor; }
        }

        public ObjectReorderableList(UnityAction repaint, SerializedObject serializedObject, SerializedProperty property) : base(serializedObject, property)
        {
            this.repaint = repaint;
            target = serializedObject.targetObject;
            drawHeaderCallback += DrawHeader;
            drawElementCallback += DrawElement;
            onAddCallback += OnAddElement;
            onRemoveCallback += OnRemoveElement;
            onSelectCallback += OnChange;
            onChangedCallback += OnChange;
        }

        private void OnChange(ReorderableList list)
        {
            if(list.index >= 0 && list.index < list.serializedProperty.arraySize)
            {
                highlightedElement = list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue;

                if(null != highlightedElement && (null == highlightedElementEditor) || highlightedElementEditor.target != highlightedElement)
                {
                    if(null != highlightedElementEditor)
                    {
                        Object.DestroyImmediate(highlightedElementEditor, true);
                        highlightedElementEditor = null;
                    }
                }

                highlightedElementEditor = Editor.CreateEditor(highlightedElement);
            }
            else if(null != highlightedElementEditor)
            {
                Object.DestroyImmediate(highlightedElementEditor, true);
                highlightedElementEditor = null;
            }
        }

        private void OnRemoveElement(ReorderableList list)
        {
            int index = list.index;
            Object element = serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;

            if (element != null)
            {
                Undo.RecordObjects(new Object[] { element, target }, "Destroy Object");
                Undo.RecordObjects(new Object[] { target }, "Destroy Object");
                serializedProperty.DeleteArrayElementAtIndex(index);
                Undo.RecordObject(target, "Destroy Object");
                Undo.RecordObjects(new Object[] { target }, "Destroy Object");
                serializedProperty.DeleteArrayElementAtIndex(index);
                Undo.RecordObjects(new Object[] { element }, "Destroy Object");
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                Undo.DestroyObjectImmediate(element);
            }
            else
            {
                Undo.RecordObject(target, "Destroy Object");
                serializedProperty.DeleteArrayElementAtIndex(index);
            }

            serializedProperty.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);

            OnChange(this);

            if (repaint != null)
            {
                repaint();
            }
        }

        private void OnAddElement(ReorderableList list)
        {
            var menu = new GenericMenu();
            IEnumerable < MonoScript > monoScripts = Resources.FindObjectsOfTypeAll<MonoScript>().Where(s => s.GetClass() != null && !s.GetClass().IsAbstract && s.GetClass().IsSubclassOf(typeof(T))).OrderBy(s => s.name);

            for (int i = 0; i < monoScripts.Count(); i++)
            {
                MonoScript monoScript = monoScripts.ElementAt(i);
                menu.AddItem(new GUIContent(monoScript.name), false,
                    () =>
                    {
                        var newObject = ScriptableObject.CreateInstance(monoScript.GetClass());

                        if(null != newObject)
                        {
                            newObject.hideFlags = HideFlags.HideInHierarchy;

                            AssetDatabase.AddObjectToAsset(newObject, target);
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));

                            Undo.RegisterCreatedObjectUndo(newObject, "Add");
                            Undo.RecordObjects(new Object[] { target, newObject }, "Add");
                            newObject.name = newObject.GetType().Name;

                            Undo.RecordObject(target, "Add");
                            serializedProperty.arraySize += 1;

                            Undo.RecordObject(target, "Add");
                            serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1).objectReferenceValue = newObject;
                            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

                            serializedProperty.serializedObject.ApplyModifiedProperties();

                            list.index = serializedProperty.arraySize - 1;
                            OnChange(this);

                            if(null != repaint)
                            {
                                repaint();
                            }
                        }
                    }
                );
            }

            menu.ShowAsContext();
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty elementProperty = serializedProperty.GetArrayElementAtIndex(index);

            if (null != elementProperty.objectReferenceValue)
            {
                var objectReference = elementProperty.objectReferenceValue;

                if (null != objectReference)
                {
                    EditorGUI.LabelField(rect, objectReference.name);
                    return;
                }
            }

            EditorGUI.LabelField(rect, "Missing (Delete me)");
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, serializedProperty.displayName);
        }
    }
}

