//using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Loot System/Loot Table")]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    public class TableEntry{
        public Object Item;
        [Min(0)]
        public float Weight;
        public float EffectiveWeight(float min, float max){
            return DirtyEffectiveWeight = Weight >= min && Weight <= max ? Weight : 0;
        }
        public float DirtyEffectiveWeight {get; private set;}

        public float Chance(float tableWeight, float prevChance = 0){
            DropChance = DirtyEffectiveWeight / tableWeight;
            DirtyChance = DropChance + prevChance;
            return DirtyChance;
        }
        public float DirtyChance {get; private set;}
        public float DropChance {get; private set;}
    }

    [SerializeField] private TableEntry[] m_entries = new TableEntry[0];
    public TableEntry[] GetEntries => m_entries;
    public float TableWeight() {
        DirtyTableWeight = 0; 
        foreach(var entry in m_entries){
            DirtyTableWeight += entry.DirtyEffectiveWeight;
        }
        return DirtyTableWeight;
    }
    public float[] DirtyLowHigh {get; private set; } = new float[] {0, 1};
    public float DirtyTableWeight {get; private set;}
    public float TableLow() {
        if(DirtyLowHigh.Length == 0)
            DirtyLowHigh = new float[2];
        DirtyLowHigh[0] = m_entries[0].Weight;
        foreach(var entry in m_entries){
            if(entry.Weight < DirtyLowHigh[0]){
                DirtyLowHigh[0] = entry.Weight;
            }
        }
        return DirtyLowHigh[0];
    }
    public float TableHigh() {
        if(DirtyLowHigh.Length == 0)
            DirtyLowHigh = new float[2];
        DirtyLowHigh[1] = m_entries[0].Weight;
        foreach(var entry in m_entries){
            if(entry.Weight > DirtyLowHigh[1]){
                DirtyLowHigh[1] = entry.Weight;
            }
        }
        return DirtyLowHigh[1];
    }
    public float CalSingleChance(float max, int index, float min = 0){
        m_entries[index].EffectiveWeight(min, max);
        return m_entries[index].Chance(TableWeight(), index > 0 ? m_entries[index - 1].DirtyChance : 0);
    }
    public void CalChances(float max, float min = 0){
        foreach(var entry in m_entries){
            entry.EffectiveWeight(min, max);
        }
        TableWeight();
        for(int i = 0; i < m_entries.Length; i++){
            m_entries[i].Chance(DirtyTableWeight, i > 0 ? m_entries[i - 1].DirtyChance : 0);
        }
    }
    public Object GetRandomItem(){
        return GetRandomItem(TableHigh());
    }
    public Object GetRandomItem(float max, float min = 0)//, bool guaranteeItem = false)
    {
        //min max validation
        if(min > TableHigh()) min = TableHigh();
        if(max < TableLow()) max = TableLow();
        if(min > max) min = max;

        CalChances(max);

        //Scale the given values
        float norMin = min;
        float norMax = max;
        if(max > 1){
            norMin /= max;
            norMax /= max;
        }

        //Generate a random number
        float rnd = Random.Range(norMin, norMax);

        //Go though until an entry is found that is greater then the rnd
        foreach(var entry in m_entries)
        {
            if(entry.DirtyChance >= rnd)
                return entry.Item;
        }
        return null;
    }
}
