using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(LootTable))]
public class LootTableEditor : Editor
{
    ReorderableList list;
    LootTable m_tbl;
    float[] range = new float[] {0, 1};
    float[] dirtyRange = new float[] {0, 1};
    float[] weightCache = new float[0];
    //bool guarentee = false;
    public void OnEnable(){
        //get the table object can cal all the chances
        m_tbl = (LootTable)target;
        if(m_tbl.GetEntries.Count() > 0){
            dirtyRange = new float[] {0, m_tbl.TableHigh()}; 
            range = new float[] {dirtyRange[0], dirtyRange[1]};
            m_tbl.CalChances(range[1], range[0]);
            var ent = m_tbl.GetEntries;
            weightCache = new float[ent.Count()];
            for(int i = 0; i < ent.Count(); i++){
                weightCache[i] = ent[i].Weight;
            }
        } 
        DrawTable();
    }
    public override void OnInspectorGUI(){
        //update the object that's selected
        serializedObject.Update();

        //draw the list (fr this time)
        list.DoLayoutList();

        //EDITOR & TEST STUFF
        bool doTest = false;
        if(m_tbl.GetEntries.Count() > 1) doTest = GUILayout.Button("Test Table");

        dirtyRange[0] = EditorGUILayout.FloatField(dirtyRange[0]);
        dirtyRange[1] = EditorGUILayout.FloatField(dirtyRange[1]);
        
        if(dirtyRange[0] != range[0] || dirtyRange[1] != range[1]){
            range = dirtyRange;
            ReloadTable();
        }
        for(int i = 0; i < weightCache.Length; i++){
            if(weightCache[i] != m_tbl.GetEntries[i].Weight){
                weightCache[i] = m_tbl.GetEntries[i].Weight;
                ReloadTable();
            }
        }

        if(doTest){
            for(int i = 0; i < 100; i++){
                Object newObject = m_tbl.GetRandomItem();
                Debug.Log((newObject != null) ? $"Returned item {newObject.name}" : "Failed to get item (I did you set the range too low?)");
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    private void ReloadTable(){
        m_tbl.CalChances(range[1], range[0]);
        DrawTable();
    }
    private void DrawTable(){
        //draw the list
        list = new(serializedObject, serializedObject.FindProperty("m_entries"), true, true, true, true);
        //override the list drawing funcs
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Item"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width - 90, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Weight"), GUIContent.none);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 50, rect.y, 50, EditorGUIUtility.singleLineHeight), m_tbl.GetEntries[index].DropChance.ToString());            
            EditorGUI.EndDisabledGroup();

            
        };
        //can the lable
        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, m_tbl.name);
        };
    }
}
