using System;
using UnityEngine;

namespace DATA.Script.Chips_data
{
    [CreateAssetMenu(menuName = "ChipsData", fileName = "Chips")]
    public class ChipsData : ScriptableObject
    {
        [field: Header("Identifiant numérique unique"),Range(1,4),SerializeField]
        public int Index { get; private set; }
        [field: Header("SpriteRenderer de la Chips"),Tooltip("1*1 size les GAs STP"),SerializeField]
        public Sprite VisuelChips { get; private set; }
        [field: Header("Name color chips"),SerializeField]
        public string ColorLinkChips { get; private set; }
        
        public ChipsDataInstance Instance()
        {
            return new ChipsDataInstance(this);
        }
    }
    [Serializable]
    public class ChipsDataInstance
    {
        public int index;
        public Sprite visuelChips;
        public string colorLinkChips;
        public bool isSelected;

        public ChipsDataInstance(ChipsData data)
        {
            index = data.Index;
            visuelChips = data.VisuelChips;
            colorLinkChips = data.ColorLinkChips;
            isSelected = false;
        }
    }
}