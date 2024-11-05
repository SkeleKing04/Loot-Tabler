using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(LootTable))]
public class LootTableEditor : Editor
{
    ReorderableList list;
    float[] range = new float[] {0, 1};
    bool guarentee = false;
    void OnEnable(){
        list = new(serializedObject, serializedObject.FindProperty("m_entries"), true, true, true, true);

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 40, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Item"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Weight"), GUIContent.none);
        };

        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Table");
        };
    }
    public override void OnInspectorGUI()
    {
        LootTable tbl = (LootTable)target;

        serializedObject.Update();
        if(GUILayout.Button("Get Chances")){
            string pnt = "";
            //sum the weights
            foreach(var entry in tbl.GetEntries){
                pnt += $"{entry.Item.name} chance is {entry.FindChance(tbl.CalTableWeight) * 100}%\n";
            }
            Debug.Log(pnt);
        }
        list.DoLayoutList();

        bool doTest = false;
        GUILayout.BeginHorizontal();
        doTest = GUILayout.Button("Test Table");
        range[0] = EditorGUILayout.FloatField(range[0]);
        range[1] = EditorGUILayout.FloatField(range[1]);
        if(range[0] < 0) range[0] = 0;
        else if(range[0] > range[1]) range[1] = range[0];
        GUILayout.EndHorizontal();
        guarentee = GUILayout.Toggle(guarentee, new GUIContent("Should Guarentee"));
        if(doTest){
            float dud = tbl.CalTableWeight;
            for(int i = 0; i < 100; i++){
            Object newObject = tbl.GetRandomItem(range[0], range[1], guarentee);
            Debug.Log((newObject != null) ? $"Returned item {newObject.name}" : "Failed to get item (I did you set the range too low?)");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
