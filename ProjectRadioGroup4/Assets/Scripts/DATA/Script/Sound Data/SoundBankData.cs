using UnityEngine;

namespace DATA.Script.Sound_Data
{
    [CreateAssetMenu(menuName = "ScriptableObject/BankDataSound", fileName = "dataSound")]
    public class SoundBankData : ScriptableObject
    {
        //Here will be all the sound we need

        public SoundBankDataInstance Instance()
        {
            return new SoundBankDataInstance(this);
        }
    }

    public class SoundBankDataInstance
    {

        public SoundBankDataInstance(SoundBankData data)
        {
            
        }
    }
}