using System.Linq;
using LootTabler.Entry;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace LootTabler.Table{

public class TableEditorBase<T> : Editor
{
    //The selected loot table
    protected LootTable<T> m_tbl;
    //The list for displaying the table
    protected ReorderableList m_list;
    //The debug limits set
    protected float[] m_range = new float[] {0, 1};
    //Used for when the range is changed
    protected float[] m_dirtyRange = new float[] {0, 1};
    //All the weights of the items in the selected table
    protected float[] m_weightCache = new float[0];
    private bool m_doTest = false;
    public void OnEnable(){
        //get the table object can cal all the chances
        m_tbl = (LootTable<T>)target;
        //prep the list for display
        m_list = new(serializedObject, serializedObject.FindProperty("m_entries"), true, true, true, true);
        //update the callback functions of the table to add the entries properly
        m_list.onAddCallback += list => { m_tbl.AddNewEntry(); _updateWeightCache(); _reloadTable(); };
        m_list.onRemoveCallback += list => { m_tbl.RemoveLastEntry(); _updateWeightCache(); _reloadTable(); };
        _setupEditor();
    }
    protected virtual void _setupEditor(){
        //if the table already has data
        if(m_tbl.GetEntries.Count() > 0){
            //set the debugging ranges
            m_dirtyRange = new float[] {0, m_tbl.TableHigh()}; 
            m_range = new float[] {m_dirtyRange[0], m_dirtyRange[1]};
            //get the chance of all the items, and update the weight cache
            m_tbl.CalChances(m_range[1], m_range[0]);
            _updateWeightCache();
        }
        //draw the table
        _drawTable();
    }
    public override void OnInspectorGUI(){
        //update the object that's selected
        serializedObject.Update();
        _initalGUIUpdate();
        _resolveGUIUpdate();
        
    }
    protected virtual void _initalGUIUpdate(){
        //draw the list
        m_list.DoLayoutList();

        //EDITOR & TEST STUFF
        m_doTest = false;
        //Display the option to test the table if there are more then one entries
        if(m_tbl.GetEntries.Count() > 1) m_doTest = GUILayout.Button("Test Table");

        //Save the ranges set
        m_dirtyRange[0] = EditorGUILayout.FloatField(m_dirtyRange[0]);
        m_dirtyRange[1] = EditorGUILayout.FloatField(m_dirtyRange[1]);
    }
    protected virtual void _resolveGUIUpdate(){

        //If the ranges have been updated
        //Reload thee table
        if(m_dirtyRange[0] <= m_dirtyRange[1] && m_dirtyRange[1] > 0 && (m_dirtyRange[0] != m_range[0] || m_dirtyRange[1] != m_range[1])){
            Debug.Log("Range updated");
            m_range = new float[] {m_dirtyRange[0], m_dirtyRange[1]};
            _reloadTable();
        }
        _checkForWeightUpdates();

        //If the user wants to run a test, pull a random item and report it to the log.
        if(m_doTest){
                Object newObject = m_tbl.GetRandomItem(m_range[1], m_range[0]);
                Debug.Log((newObject != null) ? $"Returned item {newObject.name}" : "Failed to get item (I did you set the range too low?)");
        }
        serializedObject.ApplyModifiedProperties();
    }
    /// <summary>
    /// Rerolls and draws the table
    /// </summary>
    protected void _reloadTable(){
        m_tbl.CalChances(m_range[1], m_range[0]);
        _drawTable();
    }
    /// <summary>
    /// The basic draw function of the table
    /// </summary>
    protected virtual void _drawTable()
    {
        //override the list drawing funcs
        //set the lable
        m_list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, m_tbl.name);
        };
        m_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            //Get the element in the table at the index
            var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
            _drawSingleElement(element, rect, index);         
        };
    }
    /// <summary>
    /// Draw a single element from the table
    /// </summary>
    /// <param name="element">The element to draw</param>
    /// <param name="rect"></param>
    /// <param name="index">The index of the item to draw</param>
    protected virtual void _drawSingleElement(SerializedProperty element, Rect rect, int index){
        //Space the entry
            rect.y += 2;
            //Draw the item field
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Item"), GUIContent.none);
            //Draw the weight field
            EditorGUI.PropertyField(new Rect(rect.x + rect.width - 90, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Weight"), GUIContent.none);

            //Draw an uneditable field showing the drop chance.
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 50, rect.y, 50, EditorGUIUtility.singleLineHeight), (m_tbl.GetEntries[index] as TableEntry).DropChance.ToString());            
            EditorGUI.EndDisabledGroup();      
    }
    /// <summary>
    /// Check if any of the weights have been updated and if so, update the cache.
    /// </summary>
    protected virtual void _checkForWeightUpdates(){
        for(int i = 0; i < m_tbl.GetEntries.Length; i++){
            TableEntry entry = m_tbl.GetEntries[i] as TableEntry;
            if(m_weightCache[i] != entry.Weight){
                m_weightCache[i] = entry.Weight;
                _reloadTable();
            }
        }
    }
    /// <summary>
    /// Updates the weight cache of this table
    /// </summary>
    protected virtual void _updateWeightCache(){
        //get the table entries
        T[] ent = m_tbl.GetEntries;
        //resize the cache if not matching
        if(m_weightCache.Length != ent.Length)
            m_weightCache = new float[ent.Length];
        //save each entries weight to the cache
        for(int i = 0; i < ent.Length; i++){
            m_weightCache[i] = (ent[i] as TableEntry).Weight;
        }
    }
}
}