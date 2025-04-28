using UnityEngine;

namespace DATA.Script.Tutorial
{
    [CreateAssetMenu(fileName = "CombatTutorialText", menuName = "Tutorial/Combat Tutorial Text")]
    public class FightTutorialTextData : ScriptableObject
    {
        [field: TextArea(2, 4),SerializeField] public string explainBattery { get; private set; }
        [field: TextArea(2, 4),SerializeField] public string explainEnemyOscillation{ get; private set; }
        [field: TextArea(2, 4),SerializeField] public string explainAMFM{ get; private set; }
        [field: TextArea(2, 4),SerializeField] public string explainRadioSlider{ get; private set; }
        [field: TextArea(2, 4),SerializeField] public string explainLockFM{ get; private set; }
        [field: TextArea(2, 4),SerializeField] public string explainPlayerOscillation{ get; private set; }
        [field: TextArea(2, 4),SerializeField] public string explainPlayButton{ get; private set; }
        [field: TextArea(2, 4),SerializeField] public string finished{ get; private set; }
    }
}
