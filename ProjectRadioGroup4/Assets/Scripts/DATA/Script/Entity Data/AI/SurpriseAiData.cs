using UnityEngine;

namespace DATA.Script.Entity_Data.AI
{
    [CreateAssetMenu(menuName = "ScriptableObject/AiSurprise", fileName = "data")]
    public class SurpriseAiData : AbstractEntityData
    {
        [field: Header("Surprise Zone Range"), SerializeField]
        public int SurpriseZoneRange { get; private set; }

        public override AbstractEntityDataInstance Instance(GameObject entity)
        {
            return new SurpriseAiDataInstance(this, entity);
        }
    }

    public class SurpriseAiDataInstance: AbstractEntityDataInstance
    {
        public int surpriseZoneRange;
    
        public SurpriseAiDataInstance(SurpriseAiData data, GameObject entity) : base(data,entity)
        {
            surpriseZoneRange = data.SurpriseZoneRange;
        }
    }
}