using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class AmpouleManager : MonoBehaviour
    {
        public Image ampoule1;
        public Image ampoule2;
        public Image ampoule3;
    
        public Sprite spriteEteint;
        public Sprite spriteAllume;
    
        public static int ampouleAllumee = -1;

        void Start()
        {
            TurnAllAmpouleOff();
            //en lien avec le Start dans le radioController pour que se soit initialiser à lampe 0 
            ChangeAmpoule(0);
        }

        private void ChangeAmpoule(int numeroAmpoule)
        {
            if (ampouleAllumee == numeroAmpoule) return;
        
            TurnAllAmpouleOff();
        
            ampouleAllumee = numeroAmpoule;
            GetAmpouleImage(numeroAmpoule).sprite = spriteAllume;
        }

        private void TurnAllAmpouleOff()
        {
            ampoule1.sprite = spriteEteint;
            ampoule2.sprite = spriteEteint;
            ampoule3.sprite = spriteEteint;
            ampouleAllumee = -1;
        }

        private Image GetAmpouleImage(int index)
        {
            switch(index)
            {
                case 0: return ampoule1;
                case 1: return ampoule2;
                case 2: return ampoule3;
                default: return null;
            }
        }

        public void AllumerAmpoule1() => ChangeAmpoule(0);
        public void AllumerAmpoule2() => ChangeAmpoule(1);
        public void AllumerAmpoule3() => ChangeAmpoule(2);
    }
}