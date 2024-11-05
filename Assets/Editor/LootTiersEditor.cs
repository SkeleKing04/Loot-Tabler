
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(LootTiers))]
public class LootTiersEditor : Editor
{
    LootTiers m_target;
    string[] m_newKeys = new string[2];
    int m_index = 0;
    public override VisualElement CreateInspectorGUI()
    {
        m_target = (LootTiers)target;
        if(!m_target.Tiers.ContainsKey("Test"))
            m_target.Tiers.Add("Test", 4);
        if(!m_target.Tiers.ContainsKey("Test2"))
            m_target.Tiers.Add("Test2", 14);
        return base.CreateInspectorGUI();
    }
    public override void OnInspectorGUI()
    {
        if(target)
        {
            //EditorGUILayout.BeginHorizontal();
            //m_index = EditorGUILayout.Popup(m_index, m_target.Tiers.Keys.ToArray());
            //string currentKey = m_target.Tiers.Keys.ToArray()[m_index];
            //m_target.Tiers[currentKey] = EditorGUILayout.FloatField(m_target.Tiers[currentKey]);
            //EditorGUILayout.EndHorizontal();

            Dictionary<string, float> pairs = new();
            foreach(var key in m_target.Tiers.Keys){
                pairs.Add(key, m_target.Tiers[key]);
                EditorGUILayout.BeginHorizontal();
                string keyCheck = EditorGUILayout.TextField(key);
                pairs[key] = EditorGUILayout.FloatField(pairs[key]);
                if(keyCheck != key){
                    pairs.Add(keyCheck, pairs[key]);
                    pairs.Remove(key);
                }
                EditorGUILayout.EndHorizontal();
            }
//
            foreach(var key in pairs.Keys){
                if(m_target.Tiers.ContainsKey(key))
                    m_target.Tiers[key] = pairs[key];
                else
                    m_target.Tiers.Add(key, pairs[key]);
            }
            List<string> keysToRemove = new();
            foreach(var key in m_target.Tiers.Keys){
                if(!pairs.ContainsKey(key)){
                    keysToRemove.Add(key);
                }
            }
            foreach(var key in keysToRemove){
                m_target.Tiers.Remove(key);
            }

            //GUILayout.Re
//
            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.BeginVertical();
            //m_newKeys[0] = EditorGUILayout.TextField(m_newKeys[0]);
            //if(GUILayout.Button("New Entry") && !m_target.Tiers.ContainsKey(m_newKeys[0])){
            //        m_target.Tiers.Add(m_newKeys[0], 0);
            //}
            //EditorGUILayout.EndVertical();
            //EditorGUILayout.BeginVertical();
            //m_newKeys[1] = EditorGUILayout.TextField(m_newKeys[1]);
            //if(GUILayout.Button("Remove Entry") && m_target.Tiers.ContainsKey(m_newKeys[1])){
            //        m_target.Tiers.Remove(m_newKeys[1]);
            //}
            //EditorGUILayout.EndVertical();
            //EditorGUILayout.EndHorizontal();
        }
        //base.OnInspectorGUI();
        
    }
    //int m_lastKnownCount = 0;
    //TextField[] tfs = new TextField[0];
    //[MenuItem("Tools/Loot Table Tiers")]
    //static void ShowWindow(){
    //    LootTiersEditor wnd = GetWindow<LootTiersEditor>();
    //    wnd.titleContent = new GUIContent("Loot Tiers");
    //    LootTiers.m_lootTiers = new string[] {"tier 1", "tier 2", "3"};
    //}
    //public void CreateGUI(){
    //    m_lastKnownCount = LootTiers.m_lootTiers.Length;
    //    DisplayGUI();
    //}
    //public void Update(){
    //    if(m_lastKnownCount != LootTiers.m_lootTiers.Length){
    //        DisplayGUI();
    //        m_lastKnownCount = LootTiers.m_lootTiers.Length;
    //    }
    //    for(int i = 0; i < m_lastKnownCount; i++){
    //        if(tfs[i].value != LootTiers.m_lootTiers[i]){
    //            LootTiers.m_lootTiers[i] = tfs[i].value;
    //        }
    //    }
    //}
    //private void DisplayGUI(){
    //    m_lastKnownCount = LootTiers.m_lootTiers.Length;
    //    VisualElement m_root = rootVisualElement;
    //    m_root.Clear();
    //    
    //    tfs = new TextField[m_lastKnownCount];
    //    for(int i = 0; i < m_lastKnownCount; i++){
    //        tfs[i] = new();
    //        tfs[i].value = LootTiers.m_lootTiers[i];
    //        m_root.Add(tfs[i]);
    //    }
    //
    //    Button button = new Button();
    //    button.name = "button";
    //    button.text = "Add new tier";
    //    button.clicked += AddNewTier;
    //    m_root.Add(button);
    //}
    //void AddNewTier(){
    //    int currentCount = LootTiers.m_lootTiers.Length;
    //    string[] newTiers = new string[currentCount + 1];
    //    for(int i = 0; i < currentCount; i++){
    //        newTiers[i] = LootTiers.m_lootTiers[i];
    //    }
    //    LootTiers.m_lootTiers = newTiers;
    //}
}
