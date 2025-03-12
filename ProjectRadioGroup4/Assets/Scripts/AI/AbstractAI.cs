using DATA.Script.Entity_Data.AI;
using DATA.ScriptData.Entity_Data;
using MANAGER;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(Rigidbody2D),(typeof(BoxCollider2D)))]
    public abstract class AbstractAI : MonoBehaviour
    {
        [SerializeField]
        private AbstractEntityData _abstractEntityData;
        public AbstractEntityDataInstance _abstractEntityDataInstance;
        protected AiFightState _aiFightState;
        protected enum AiFightState
        {
            InFight,
            OutFight
        }
        
        private void Update()
        {
            AiShift();
        }

        protected abstract void AiShift();

        private void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            _abstractEntityDataInstance = _abstractEntityData.Instance();
            _aiFightState = AiFightState.OutFight;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            // no need to implement virtual/override because in each type of ai the trigger will launch the FightManager
            if (!other.CompareTag("Player")) return;
            Debug.Log("Player in Fight Zone");
            _aiFightState = AiFightState.InFight;
            FightManager.instance.fightState = FightManager.FightState.InFight;
            FightManager.instance.InitialiseList();
        }
    }
}
