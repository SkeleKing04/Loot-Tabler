//using System;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Loot System/Loot Table")]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    public class TableEntry{
        public Object Item;
        [Min(0)]
        public float Weight;
        public float EffectiveWeight(float min, float max){
            DirtyEffectiveWeight = Weight >= min && Weight <= max ? Weight : 0;
            Debug.Log($"Loot table item ({Item})'s effective weight is {DirtyEffectiveWeight}; given a weight of {Weight} and minMax of ({min},{max})");
            return DirtyEffectiveWeight;
        }
        public float DirtyEffectiveWeight {get; private set;}

        public float Chance(float tableWeight, float prevChance = 0){
            DropChance = DirtyEffectiveWeight / tableWeight;
            DirtyChance = DropChance + prevChance;
            Debug.Log($"The drop chance of {Item} is {DropChance}.");
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
        Debug.Log($"The sum of the table weight is {DirtyTableWeight}.");
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
        Debug.Log($"The lowest value on the table is {DirtyLowHigh[0]}");
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
        Debug.Log($"The highest value on the table is {DirtyLowHigh[1]}");
        return DirtyLowHigh[1];
    }
    public float CalSingleChance(float max, int index, float min = 0){
        m_entries[index].EffectiveWeight(min, max);
        return m_entries[index].Chance(TableWeight(), index > 0 ? m_entries[index - 1].DirtyChance : 0);
    }
    public void CalChances(float max, float min = 0){
        Debug.Log($"Calculating the chances for {this.name} with the minMax of ({min},{max}).");
        foreach(var entry in m_entries){
            entry.EffectiveWeight(min, max);
        }
        Debug.Log($"All effective weights calculated.");
        TableWeight();
        Debug.Log($"Calculating the chances.");
        for(int i = 0; i < m_entries.Length; i++){
            m_entries[i].Chance(DirtyTableWeight, i > 0 ? m_entries[i - 1].DirtyChance : 0);
        }
        Debug.Log($"All chance calculated.");
    }
    public Object GetRandomItem(){
        return GetRandomItem(TableHigh());
    }
    public Object GetRandomItem(float max, float min = 0)//, bool guaranteeItem = false)
    {
        Debug.Log($"Getting a random item");
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
    public void AddNewEntry(){
        TableEntry[] newArray = new TableEntry[m_entries.Length + 1];
        for(int i = 0; i < m_entries.Length; i++){
            newArray[i] = m_entries[i];
        }
        newArray[m_entries.Length] = new TableEntry();
        m_entries = newArray;
    }
    public void RemoveLastEntry(){
        TableEntry[] newArray = new TableEntry[m_entries.Length - 1];
        for(int i = 0; i < newArray.Length; i++){
            newArray[i] = m_entries[i];
        }
        m_entries = newArray;
    }
}
