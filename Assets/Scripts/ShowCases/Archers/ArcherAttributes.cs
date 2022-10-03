using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArcherAttributes: MonoBehaviour
{
        private CompositeDisposable disposable = new CompositeDisposable();
        private GameHandler gameHandler => GameObject.Find("GameHandler").GetComponent<GameHandler>();
        
        public float distanceFactor = 4;
        public int reloadSpeedMin = 1; 
        public int reloadSpeedMax = 10;
        public int ReloadSpeed { get; private set; }
        
        public float precisionMin = 0.1f;
        public float precisionMax = 1f;
        public float Precision { get; private set; }

        public float movementMin = 0.1f;
        public float movementMax = 2f;
        public float Movement { get; private set; }

        public int ammoCapacityMin = 1;
        public int ammoCapacityMax = 10;
        public int AmmoCapacity { get; private set; }

        private void OnEnable()
        {
                Reset();
                // gameHandler.OnReset.Subscribe(_ => Reset())
                //         .AddTo(disposable);
        }

        public void Reset()
        {
                AmmoCapacity = Random.Range(ammoCapacityMin, ammoCapacityMax);
                ReloadSpeed = Random.Range(reloadSpeedMin, AmmoCapacity);
                Precision = Random.Range(precisionMin, precisionMax);
                Movement = Random.Range(movementMin, movementMax);
        }

        private void OnDisable()
        {
                disposable.Clear();
        }
}