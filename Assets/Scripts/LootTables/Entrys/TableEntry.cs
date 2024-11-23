using LootTabler.Table;
using UnityEngine;

namespace LootTabler.Entry{

[System.Serializable]
//This is the generic type of table entry
public class TableEntry{
    public Object Item;
    [Min(0)]
    public float Weight;
    /// <summary>
    /// Finds the effective weight of this item
    /// </summary>
    /// <param name="min">The upper limit</param>
    /// <param name="max">The lower limit</param>
    /// <returns>The effective weight</returns>
    public virtual float EffectiveWeight(float min, float max){
        //Compare the set weight to the min and max values
        //if outside those values, set the effective weight to 0
        DirtyEffectiveWeight = Weight >= min && Weight <= max ? Weight : 0;
        Debug.Log($"Loot table item ({Item})'s effective weight is {DirtyEffectiveWeight}; given a weight of {Weight} and minMax of ({min},{max})");
        return DirtyEffectiveWeight;
    }
    public float DirtyEffectiveWeight {get; protected set;}
    /// <summary>
    /// Find the chance of this item
    /// </summary>
    /// <param name="tableWeight">The weight of all items on the table this item is from.</param>
    /// <param name="prevChance">The chance of the last item in the table</param>
    /// <returns>The chance of this item pulled</returns>
    public virtual float Chance(float tableWeight, float prevChance = 0){
        //Find the drop chance - this is the chance compared to the rest of the table.
        DropChance = DirtyEffectiveWeight / tableWeight;
        //Add the previous chance onto this item's - this is done to find the item when searching the table.
        DirtyChance = DropChance + prevChance;
        Debug.Log($"The drop chance of {Item} is {DropChance}.");
        return DirtyChance;
    }
    public float DirtyChance {get; protected set;}
    public float DropChance {get; protected set;}
}
}
