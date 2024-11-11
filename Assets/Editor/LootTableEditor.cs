using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(LootTable))]
public class LootTableEditor : Editor
{
    private ReorderableList m_list;
    private LootTable m_tbl;
    private float[] m_range = new float[] {0, 1};
    private float[] m_dirtyRange = new float[] {0, 1};
    private float[] m_weightCache = new float[0];
    public void OnEnable(){
        //get the table object can cal all the chances
        m_tbl = (LootTable)target;
        m_list = new(serializedObject, serializedObject.FindProperty("m_entries"), true, true, true, true);
        m_list.onAddCallback += list => { m_tbl.AddNewEntry(); _updateWeightCache(); _reloadTable(); };
        m_list.onRemoveCallback += list => { m_tbl.RemoveLastEntry(); _updateWeightCache(); _reloadTable(); };
        if(m_tbl.GetEntries.Count() > 0){
            m_dirtyRange = new float[] {0, m_tbl.TableHigh()}; 
            m_range = new float[] {m_dirtyRange[0], m_dirtyRange[1]};
            m_tbl.CalChances(m_range[1], m_range[0]);
            _updateWeightCache();
        } 
        _drawTable();
    }
    public override void OnInspectorGUI(){
        //update the object that's selected
        serializedObject.Update();

        //draw the list (fr this time)
        m_list.DoLayoutList();

        //EDITOR & TEST STUFF
        bool doTest = false;
        if(m_tbl.GetEntries.Count() > 1) doTest = GUILayout.Button("Test Table");

        m_dirtyRange[0] = EditorGUILayout.FloatField(m_dirtyRange[0]);
        m_dirtyRange[1] = EditorGUILayout.FloatField(m_dirtyRange[1]);
        
        if(m_dirtyRange[0] <= m_dirtyRange[1] && m_dirtyRange[1] > 0 && (m_dirtyRange[0] != m_range[0] || m_dirtyRange[1] != m_range[1])){
            Debug.Log("Range updated");
            m_range = new float[] {m_dirtyRange[0], m_dirtyRange[1]};
            _reloadTable();
        }
        for(int i = 0; i < m_tbl.GetEntries.Length; i++){
            if(m_weightCache[i] != m_tbl.GetEntries[i].Weight){
                m_weightCache[i] = m_tbl.GetEntries[i].Weight;
                _reloadTable();
            }
        }

        if(doTest){
                Object newObject = m_tbl.GetRandomItem(m_range[1], m_range[0]);
                Debug.Log((newObject != null) ? $"Returned item {newObject.name}" : "Failed to get item (I did you set the range too low?)");
        }
        serializedObject.ApplyModifiedProperties();
    }
    private void _reloadTable(){
        m_tbl.CalChances(m_range[1], m_range[0]);
        _drawTable();
    }
    private void _drawTable(){
        //draw the list
        //override the list drawing funcs
        m_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Item"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width - 90, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Weight"), GUIContent.none);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 50, rect.y, 50, EditorGUIUtility.singleLineHeight), m_tbl.GetEntries[index].DropChance.ToString());            
            EditorGUI.EndDisabledGroup();            
        };
        //can the lable
        m_list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, m_tbl.name);
        };
    }
    private void _updateWeightCache(){
        var ent = m_tbl.GetEntries;
            m_weightCache = new float[ent.Length];
            for(int i = 0; i < ent.Length; i++){
                m_weightCache[i] = ent[i].Weight;
            }
    }
}
