using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TurnHandler: MonoBehaviour
{
    private CompositeDisposable disposable = new CompositeDisposable();
    private GameHandler gameHandler => GameObject.Find("GameHandler").GetComponent<GameHandler>();

        public IObservable<int> TurnChanged => turnChanged;
        private readonly Subject<int> turnChanged = new Subject<int>();

        public ScoreBoard scoreOne;
        public ScoreBoard scoreTwo;
        public ScoreBoard scoreThree;
        public WinHandler winHandler;
        public Slider speedSlider;
        [FormerlySerializedAs("animate")] public Toggle maxSpeed;
        
        public int CurrentTurn { get; private set; } = 0;
        public bool isPlaying = false;
        public float msBetweenTurns = 150f;
        private float timer = 0;

        private void OnEnable()
        {
            speedSlider.maxValue = 500f;
            speedSlider.minValue = 1f;
            speedSlider.value = msBetweenTurns;
            Reset();
            // gameHandler.OnReset.Subscribe(_ => Reset()).AddTo(disposable);
        }

        public void Reset()
        {
            CurrentTurn = 0;
        }

        public void IncrementTurn()
        {
                CurrentTurn++;
                turnChanged.OnNext(CurrentTurn);
                winHandler.CheckWin();
                gameHandler.CheckReset();
        }

        public void TogglePlay()
        {
            isPlaying = !isPlaying;
        }

        public void SpeedUp()
        {
            msBetweenTurns *= 0.9f;
            if (msBetweenTurns < 1)
            {
                msBetweenTurns = 1;
            }
        }

        public void SlowDown()
        {
            msBetweenTurns *= 1.1f;
            if (msBetweenTurns > 1000)
            {
                msBetweenTurns = 1000;
            }
        }

        private void Update()
        {
            var totalHitsLeft = scoreOne.hitsLeft + scoreTwo.hitsLeft + scoreThree.hitsLeft;
            var gameRunning = totalHitsLeft > 0;
            if (isPlaying && gameRunning)
            {
                if (maxSpeed.isOn)
                {
                    IncrementTurn();
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer > speedSlider.value/1000f)
                    {
                        IncrementTurn();
                        timer = 0;
                    }
                }
            }
        }

        private void OnDisable()
        {
            disposable.Clear();
        }
}