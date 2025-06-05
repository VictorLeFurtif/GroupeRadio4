using System.Collections.Generic;
using UnityEngine;

namespace MANAGER
{
    public class SpotLightFightManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject prefabSportLight;

        [Header("Position")] [SerializeField] private float offSettY;

        public List<GameObject> lightsInstantiate = new List<GameObject>();
        
        //The way I m gonna make it, init in FightManager and will be good
        //is it a pool or Destroy Instantiate ??? because of the time no pool will be easier

        public void InitLight()
        {
            if (FightManager.instance?.currentOrder == null) return;
            
            lightsInstantiate.Clear();
            
            foreach (var character in FightManager.instance.currentOrder)
            {
                GameObject lightCharacter = Instantiate(prefabSportLight,
                    new Vector2(character.entity.transform.position.x,
                        character.entity.transform.position.y + offSettY), Quaternion.identity);
                lightCharacter.transform.Rotate(0,0,-180);
                lightsInstantiate.Add(lightCharacter);
            }
        }

        public void CleanLight()
        {
            foreach (GameObject elementLight in lightsInstantiate)
            {
                Destroy(elementLight);
            }
            lightsInstantiate.Clear();
        }
    }
}
