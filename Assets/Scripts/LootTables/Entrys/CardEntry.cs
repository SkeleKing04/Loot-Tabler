using UnityEngine;
namespace LootTabler.Entry{
    [System.Serializable]
public class CardEntry : TableEntry
{
        public AnimationCurve WeightCurve;
        private float m_time;
        public CardEntry(){
            WeightCurve = new AnimationCurve(new Keyframe[] {new Keyframe(1,1) });
        }
        public float GetWeight(float time = 0){
            return Weight = WeightCurve.Evaluate(time);
        }
        public float SetTime(float value, bool doClamp = true){
            if(doClamp)
                value = Mathf.Clamp(value, 0, 1);

            return m_time = value;
        }
        public override float EffectiveWeight(float min, float max)
        {
            var currentWeight = WeightCurve.Evaluate(m_time);
            DirtyEffectiveWeight = currentWeight >= min && currentWeight <= max ? currentWeight : 0;
            Debug.Log($"Loot table item ({Item})'s effective weight is {DirtyEffectiveWeight}; given a weight of {Weight} and minMax of ({min},{max})");
            return DirtyEffectiveWeight;
        }
    }
}
