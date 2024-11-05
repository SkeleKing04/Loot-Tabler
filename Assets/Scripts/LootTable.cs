using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Loot System/Loot Table")]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    public struct TableEntry{
        public Object Item;
        [Min(0)]
        public float Weight;
        //private float chance;
        void Set(Object newItem, float newWeight){
            Item = newItem;
            Weight = newWeight;
        }
        public float Chance {get; private set;}
        public float FindChance(float tableWeight){
            return Chance = Weight / tableWeight;
        }
    }

    [SerializeField] private TableEntry[] m_entries = new TableEntry[0];
    public TableEntry[] GetEntries => m_entries;
    public float GetTableWeight {get; private set;}
    public float CalTableWeight { get{
        GetTableWeight = 0; 
        foreach(var entry in m_entries){
            GetTableWeight += entry.Weight;
        }
        return GetTableWeight;
    }}
    public float[] GetTableLowHigh {get; private set;} = new float[2];
    public float[] FindTableLowHigh{ get{
        GetTableLowHigh = new float[] {1, 0};
        foreach(var entry in m_entries){
            entry.FindChance(GetTableWeight);
            if(entry.Chance < GetTableLowHigh[0]){
                GetTableLowHigh[0] = entry.Chance;
            }
            if(entry.Chance > GetTableLowHigh[1]){
                GetTableLowHigh[1] = entry.Chance;
            }
        }
        return GetTableLowHigh;
    }}
    public Object GetRandomItem(float min = 0, float max = 1, bool guaranteeItem = false)
    {
        //Scale the given values
        if(max > 1){
            min /= max;
            max /= max;
        }
        //Too low check
        if(max <= FindTableLowHigh[0]){
            Debug.LogError("Maximum value too low");
            return null;
        }

        //Clamp the values (should only effect values when guaranteed)
        min = Mathf.Clamp(min, guaranteeItem ? GetTableLowHigh[0] : 0, guaranteeItem ? GetTableLowHigh[1] : max);
        max = Mathf.Clamp(max, guaranteeItem ? GetTableLowHigh[0] : min, guaranteeItem ? GetTableLowHigh[1] : 1);

        //Generate a random number
        float rnd = Random.Range(min, max);
        //Clamp it between the min and max values
        rnd = Mathf.Clamp(rnd, min, max);
        List<Object> pool = new List<Object>();
        //Collect every item that fits in the given range that is greater then the random number
        foreach(var entry in m_entries)
        {
            entry.FindChance(CalTableWeight);
            if((entry.Chance <= max && entry.Chance >= rnd) || (guaranteeItem && entry.Chance >= GetTableLowHigh[0] && entry.Chance <= rnd))
                pool.Add(entry.Item);
        }

        //Get a random item from the pool
        int rndItem = Random.Range(0, pool.Count);
        return (pool.Count > 0) ? pool[rndItem] : null;
    }
    public Object GetRandomFromHighestBand(float max = 1)
    {
        max = Mathf.Clamp(max, FindTableLowHigh[0], GetTableLowHigh[1]);

        List<TableEntry> pool = new();
        //Go thought the entries
        foreach(var entry in m_entries){
            entry.FindChance(CalTableWeight);
            //if an entry is within the max limit
            if(entry.Chance <= max ){
                //and the rarity is greater
                if(pool.Count != 0 && entry.Weight > pool[0].Weight){
                    //reset the item pool
                    pool.Clear();
                }
                if(pool.Count == 0 || entry.Weight == pool[0].Weight){
                    //add this entry
                    pool.Add(entry);
                }
            }
        }

        int rndItem = Random.Range(0, pool.Count);
        return pool[rndItem].Item;
    }
}
