using UnityEngine;

namespace LootTabler.Table{
    using System;
    using Entry;
    public class LootTable<T> : ScriptableObject
    {
        [SerializeField] private T[] m_entries = new T[0];
        public T[] GetEntries => m_entries;
        /// <summary>
        /// Finds the table weight
        /// </summary>
        /// <returns></returns>
        public float TableWeight() {
            //Reset the weight
            DirtyTableWeight = 0; 
            foreach(var entry in m_entries){
                //Add the weight of all individual weights
                DirtyTableWeight += (entry as TableEntry).DirtyEffectiveWeight;
            }
            Debug.Log($"The sum of the table weight is {DirtyTableWeight}.");
            return DirtyTableWeight;
        }
        public float DirtyTableWeight {get; private set;}
        public float[] DirtyLowHigh {get; private set; } = new float[] {0, 1};
        /// <summary>
        /// Find the lowest weight in the table.
        /// </summary>
        /// <returns></returns>
        public float TableLow(float min = 0) {
        //Make sure the array is set
        if(DirtyLowHigh.Length == 0)
            DirtyLowHigh = new float[2];
        //Start with the first weight
        DirtyLowHigh[0] = (m_entries[0] as TableEntry).Weight;
        //Compare each weight, and if less then the saved value, override it.
        foreach(var untypedEntry in m_entries){
            TableEntry entry = untypedEntry as TableEntry;
            //if the old weight is less than the min or the new weight is less than the old weight and the new weight is greater or equal to the min, override
            if((DirtyLowHigh[0] < min || entry.Weight < DirtyLowHigh[0]) && entry.Weight >= min){
                DirtyLowHigh[0] = entry.Weight;
            }
        }
        Debug.Log($"The lowest value on the table is {DirtyLowHigh[0]}");
        return DirtyLowHigh[0];
        }
        /// <summary>
        /// Finds the highest weight in the table.
        /// </summary>
        /// <returns></returns>
        public float TableHigh() {
            //Make sure the array is set
        if(DirtyLowHigh.Length == 0)
            DirtyLowHigh = new float[2];
            //Start with the first weight
        DirtyLowHigh[1] = (m_entries[0] as TableEntry).Weight;
        foreach(var untypedEntry in m_entries){
            TableEntry entry = untypedEntry as TableEntry;
            //if the old weight is greater then the max or the new weight is greater than the old weight and the new weight is less than or equal to the max, override
            if(entry.Weight > DirtyLowHigh[1]){
                DirtyLowHigh[1] = entry.Weight;
            }
        }
        Debug.Log($"The highest value on the table is {DirtyLowHigh[1]}");
        return DirtyLowHigh[1];
        }
        /// <summary>
        /// Get the chance of a single entry in the table
        /// </summary>
        /// <param name="max">The upper bound of the table</param>
        /// <param name="min">The lower bound of the table</param>
        /// <param name="index">The index of the item to find the weight of</param>
        /// <returns>The weight of the item</returns>
        public float CalSingleChance(float max, int index, float min = 0){
            //Read the entry as a basic table entry
            TableEntry entry = m_entries[index] as TableEntry;
            entry.EffectiveWeight(min, max);
            return entry.Chance(TableWeight(), index > 0 ? (m_entries[index - 1] as TableEntry).DirtyChance : 0);
        }
        /// <summary>
        /// Gets the chances of all the entries in the table
        /// </summary>
        /// <param name="max">The upper bound of the table</param>
        /// <param name="min">The lower bound of the table</param>
        public void CalChances(float max, float min = 0){
        Debug.Log($"Calculating the chances for {this.name} with the minMax of ({min},{max}).");
        foreach(var untabledEntry in m_entries){
            //Read the entry as a basic table entry
            (untabledEntry as TableEntry).EffectiveWeight(min, max);
        }
        Debug.Log($"All effective weights calculated.");
        TableWeight();
        Debug.Log($"Calculating the chances.");
        for(int i = 0; i < m_entries.Length; i++){
            //Get the chances
            (m_entries[i] as TableEntry).Chance(DirtyTableWeight, i > 0 ? (m_entries[i - 1] as TableEntry).DirtyChance : 0);
        }
        Debug.Log($"All chance calculated.");
        }
        /// <summary>
        /// Returns a random item from the table
        /// </summary>
        /// <returns></returns>
        public UnityEngine.Object GetRandomItem(){
        return GetRandomItem(TableHigh());
        }
        /// <summary>
        /// Returns a random item from the table
        /// </summary>
        /// <param name="max">The upper bounds of the table</param>
        /// <param name="min">The lower bounds of the table</param>
        /// <returns></returns>
        public UnityEngine.Object GetRandomItem(float max, float min = 0)
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
        float rnd = UnityEngine.Random.Range(norMin, norMax);

        //Go though until an entry is found that is greater then the rnd
        foreach(var untypedEntry in m_entries)
        {
            TableEntry entry = untypedEntry as TableEntry;
            if(entry.DirtyChance >= rnd)
                return entry.Item;
        }
        return null;
    }
    //Adds a new item into the table
        public void AddNewEntry(){
        T[] newArray = new T[m_entries.Length + 1];
        for(int i = 0; i < m_entries.Length; i++){
            newArray[i] = m_entries[i];
        }
        newArray[m_entries.Length] = (T)Activator.CreateInstance(typeof(T), new object[] {});
        m_entries = newArray;
    }
    //Remove the most recent item from the table
        public void RemoveLastEntry(){
        T[] newArray = new T[m_entries.Length - 1];
        for(int i = 0; i < newArray.Length; i++){
            newArray[i] = m_entries[i];
        }
        m_entries = newArray;
    }
    }
}