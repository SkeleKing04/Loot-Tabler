using System.Collections.Generic;
using LootTabler.Entry;
using UnityEditor;
using UnityEngine;

namespace LootTabler.Table{
    [CustomEditor(typeof(CardTable))]
    public class CardTableEditor : TableEditorBase<CardEntry>
    {
        Keyframe[] m_weightCache = new Keyframe[0];
        [Range(0, 1)]
        float m_time = 0;
        float m_cachedTime = 0;
        protected override void _setupEditor()
        {
            if(m_tbl.GetEntries.Length > 0){
                foreach(var entry in m_tbl.GetEntries){
                    entry.GetWeight();
                }
            }
            base._setupEditor();
        }
        protected override void _drawTable()
        {
            m_list.elementHeight = EditorGUIUtility.singleLineHeight * 2;
            base._drawTable();
        }
        protected override void _drawSingleElement(SerializedProperty element, Rect rect, int index)
        {
            Debug.Log($"Drew the element at index {index} on the table {target.name}.");
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Item"), GUIContent.none);
            m_tbl.GetEntries[index].WeightCurve = EditorGUI.CurveField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width - 100, EditorGUIUtility.singleLineHeight), m_tbl.GetEntries[index].WeightCurve);
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 50, rect.y, 50, EditorGUIUtility.singleLineHeight), m_tbl.GetEntries[index].DropChance.ToString());            
            EditorGUI.EndDisabledGroup();
        }

        protected override void _checkForWeightUpdates(){
            //Get all the keys
            List<Keyframe> keys = new();
            foreach(var ent in m_tbl.GetEntries){
                keys.AddRange(ent.WeightCurve.keys);
            }
            for(int i = 0; i < keys.Count; i++){
                if(m_weightCache[i].value != keys[i].value || m_weightCache[i].time != keys[i].time){
                    m_weightCache[i] = keys[i];
                    _reloadTable();
                }
            }
        }
    /// <summary>
    /// Updates the weight cache of this table
    /// </summary>
    protected override void _updateWeightCache(){
        //Get all the keys
        List<Keyframe> keys = new();
        foreach(var ent in m_tbl.GetEntries){
            keys.AddRange(ent.WeightCurve.keys);
        }
        //resize the cache if not matching
        if(m_weightCache.Length != keys.Count)
            m_weightCache = new Keyframe[keys.Count];
        //save each entries value and time to the cache
        for(int i = 0; i < keys.Count; i++){
            m_weightCache[i] = keys[i];
        }
    }
        protected override void _initalGUIUpdate()
        {
            base._initalGUIUpdate();
            m_time = EditorGUILayout.Slider(m_time, 0, 1);
        }
        protected override void _resolveGUIUpdate()
        {
            if(m_cachedTime != m_time){
            foreach(var entry in m_tbl.GetEntries){
                entry.SetTime(m_time);
                _reloadTable();
            }
                m_cachedTime = m_time;
            }
            base._resolveGUIUpdate();
        }
    }
}