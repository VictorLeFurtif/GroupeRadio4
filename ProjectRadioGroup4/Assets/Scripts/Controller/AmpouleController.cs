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
            EteindreToutes();
        }

        private void ChangerAmpoule(int numeroAmpoule)
        {
            if (ampouleAllumee == numeroAmpoule) return;
        
            EteindreToutes();
        
            ampouleAllumee = numeroAmpoule;
            GetAmpouleImage(numeroAmpoule).sprite = spriteAllume;
        }

        private void EteindreToutes()
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

        public void AllumerAmpoule1() => ChangerAmpoule(0);
        public void AllumerAmpoule2() => ChangerAmpoule(1);
        public void AllumerAmpoule3() => ChangerAmpoule(2);
    }
}