using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DATA.Script.Sound_Data
{
    [CreateAssetMenu(menuName = "ScriptableObject/BankDataSound", fileName = "dataSound")]
    public class SoundBankData : ScriptableObject
    {
        [Serializable]
        public class AvatarSound
        {
            public AudioClip attackLowWave;
            public AudioClip attackStrongWave;

            public AudioClip ScanSlow;
            public AudioClip ScanFast;
            
            public AudioClip walk;

            public AudioClip mouvementVetScan;
            public AudioClip respirationJoueur;
        }
        [Serializable]
        public class EnemySound
        {
            public List<AudioClip> listEnemyAttack;
            public List<AudioClip> listVocalEnemy;

            public AudioClip enemySound;

            public AudioClip bruitCoupNMI;
            public AudioClip bruitRadioMatch;
            public AudioClip grognementAttaque;
            public AudioClip takeDamage;
            public AudioClip respirationNmiCombat;
        }
        [Serializable]
        public class EnviroSound
        {
            public AudioClip whiteNoiseVentilation;
            public AudioClip NeonBuzz;

            public AudioClip mice1;
            public AudioClip mice2;
            public AudioClip mice3;
            public AudioClip mice4;
            public AudioClip mice5;
            public AudioClip mice6;
            public AudioClip mice7;
            public AudioClip mice8;
            public AudioClip mice9;

            public AudioClip waterDroplet1;
            public AudioClip waterDroplet2;
            public AudioClip waterDroplet3;
            public AudioClip waterDroplet4;
            public AudioClip waterDroplet5;
            public AudioClip waterDroplet6;
            public AudioClip waterDroplet7;
            public AudioClip waterDroplet8;
            public AudioClip waterDroplet9;
            public AudioClip waterDroplet10;
            public AudioClip waterDroplet11;
            public AudioClip waterDroplet12;
            public AudioClip waterDroplet13;
            public AudioClip waterDroplet14;

            public AudioClip woodCreakig1;
            public AudioClip woodCreakig2;
            public AudioClip woodCreakig3;
            public AudioClip woodCreakig4;
            public AudioClip woodCreakig5;
            public AudioClip woodCreakig6;
            public AudioClip woodCreakig7;
            public AudioClip woodCreakig8;
            public AudioClip woodCreakig9;
            public AudioClip woodCreakig10;

            public AudioClip signal;

        }
        [Serializable]
        public class UxSound
        {
            public AudioClip click;
            public AudioClip clickAmpoule;
            public AudioClip clikGros;
            public AudioClip clikPetit;
        }
        [Serializable]
        public class Music
        {
            public AudioClip audioMenu;
            public AudioClip DarkerThanDark;
            public AudioClip audioBoss;
        }
        
        [Serializable]
        public class Event
        {
            public AudioClip apparitionUiCombat;
            public AudioClip batterieCasse;
            public AudioClip disparitionUiCombat;
            public AudioClip failMatchRevers;
            public AudioClip recupBatterie;
            public AudioClip scanFort;
            public AudioClip scanMoyen;
            public AudioClip scanFaible;
            public AudioClip spawnNmiScreamer;
            public AudioClip spawnNmi;
            public AudioClip timer;
            public AudioClip validation;
            public AudioClip validationFinal;
            public AudioClip zoneCombat;
            public AudioClip zoneRangeFinder;
            public AudioClip openingDoor;
        }

        public AvatarSound AvatarSoundData = new AvatarSound();
        public EnemySound EnemySoundData = new EnemySound();
        public EnviroSound EnviroSoundData = new EnviroSound();
        public UxSound UxSoundData = new UxSound();
        public Music MusicSoundData = new Music();
        public Event EventSoundData = new Event();

        public SoundBankDataInstance Instance()
        {
            return new SoundBankDataInstance(this);
        }
    }

    public class SoundBankDataInstance
    {
        public SoundBankData.AvatarSound avatarSound;
        public SoundBankData.EnemySound enemySound;
        public SoundBankData.EnviroSound enviroSound;
        public SoundBankData.UxSound uxSound;
        public SoundBankData.Music musicSound;
        public SoundBankData.Event eventSound;
        
        public SoundBankDataInstance(SoundBankData data)
        {
            avatarSound = data.AvatarSoundData;
            enemySound = data.EnemySoundData;
            enviroSound = data.EnviroSoundData;
            uxSound = data.UxSoundData;
            musicSound = data.MusicSoundData;
            eventSound = data.EventSoundData;
        }

        public AudioClip SelectRandomSoundFromList(List<AudioClip>list)
        {
            int rnd = Random.Range(0, list.Count - 1);
            return list[rnd];
        }
    }
}