using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameHandler: MonoBehaviour
{
        private CompositeDisposable disposable = new CompositeDisposable();
        private Archer one => GameObject.Find("One").GetComponent<Archer>();
        private Archer two => GameObject.Find("Two").GetComponent<Archer>();
        private Archer three => GameObject.Find("Three").GetComponent<Archer>();
        public SequenceHandler sequenceHandler;
        public TurnHandler turnHandler;
        public WinHandler winHandler;

        public ArcherAttributes archerAttributes =>
                GameObject.Find("ArcherAttributes").GetComponent<ArcherAttributes>();
        private readonly Dictionary<Archer, int> indexByArcher = new Dictionary<Archer, int>();
        private readonly List<float> randomNumbers = new List<float>();
        public int numberOfRandomNumbers = 1000000;

        public Slider gamesToPlaySlider;
        public TextMeshProUGUI gamesToPlayTextValue;
        public TextMeshProUGUI currentGameUiText;
        private int gamesToPlay = 1;

        public IObservable<bool> OnReset => onReset;
        private Subject<bool> onReset = new Subject<bool>();
        private Dictionary<Archer, bool> archersReadyToReset = new Dictionary<Archer, bool>();
        private int archersFinished = 0;
        private void OnEnable()
        {
                gamesToPlay = (int)gamesToPlaySlider.value;

                Reset();
                gamesToPlaySlider.onValueChanged.AddListener(value =>
                {
                        gamesToPlay = (int)value;
                        gamesToPlayTextValue.text = gamesToPlay.ToString();
                        currentGameUiText.text = "Game # " + (completedGames + 1) + " / " + gamesToPlay;
                });
                gamesToPlayTextValue.text = gamesToPlay.ToString();
                
                // one.ValuesChanged.Subscribe(_ => CheckWinner(one)).AddTo(disposable);
                // two.ValuesChanged.Subscribe(_ => CheckWinner(two)).AddTo(disposable);
                // three.ValuesChanged.Subscribe(_ => CheckWinner(three)).AddTo(disposable);

                SetArcherFinished(one,false);
                SetArcherFinished(two,false);
                SetArcherFinished(three,false);
                
                // one.scoreBoard.NoHitsLeft.Subscribe(_ => CheckReset(one)).AddTo(disposable);
                // two.scoreBoard.NoHitsLeft.Subscribe(_ => CheckReset(two)).AddTo(disposable);
                // three.scoreBoard.NoHitsLeft.Subscribe(_ => CheckReset(three)).AddTo(disposable);
                
                completedGames = 0;
                currentGameUiText.text = "Game # " + (completedGames + 1) + " / " + gamesToPlay;
        }

        public void Reset()
        {
                completedGames++;
                for (int i = 0; i < numberOfRandomNumbers; i++)
                {
                        var x = Random.Range(0, 1f);
                        randomNumbers.Add(x);
                } 
                winTurn = int.MaxValue;
                archersFinished = 0;
                turnHandler.Reset();
                winHandler.Reset();
                sequenceHandler.Reset();

                onReset.OnNext(true);
                SetArcherFinished(one,false);
                SetArcherFinished(two,false);
                SetArcherFinished(three,false);
                
                
                archerAttributes.Reset();
                one.Reset();
                two.Reset();
                three.Reset();
                one.scoreBoard.Reset();
                two.scoreBoard.Reset();
                three.scoreBoard.Reset();
                // winnerFound = false;
                currentGameUiText.text = "Game # " + (completedGames) + " / " + gamesToPlay;
        }

        private int winTurn = int.MaxValue;
        private bool winnerFound => winTurn < turnHandler.CurrentTurn;
        private bool ContinuePlaying => completedGames < gamesToPlay;
        public int completedGames = 0;

        private void SetArcherFinished(Archer archer, bool hasFinished)
        {
                if (!archersReadyToReset.ContainsKey(archer))
                {
                        archersReadyToReset.Add(archer,hasFinished);
                }
                else
                {
                        archersReadyToReset[archer] = hasFinished;
                }
        }
        
        public void CheckReset()
        {
                if (one.HasFinished && two.HasFinished && three.HasFinished)
                {
                        if (ContinuePlaying)
                        {
                                Reset();
                        }
                }
        }
        
        // public void CheckReset(Archer archer)
        // {
        //         if (archer.HasFinished && !archersReadyToReset[archer])
        //         {
        //                 archersFinished++;
        //                 SetArcherFinished(archer,true);
        //         }
        //         winHandler.HandleNoHitsLeft(archer.scoreBoard);
        //         // if (archersReadyToReset[one] && archersReadyToReset[two] && archersReadyToReset[three])
        //         if (archersFinished == 3)
        //         {
        //                 if (ContinuePlaying)
        //                 {
        //                         Reset();
        //                 }
        //         }
        // }

        // private void CheckWinner(Archer archer)
        // {
        //         if (!archer.HasFinished) return;
        //
        //         if (winnerFound)
        //         {
        //                 return;
        //         }
        //         else
        //         {
        //                 winTurn = turnHandler.CurrentTurn;
        //                 archer.scoreBoard.IncrementWins(); 
        //         }
        // }

        public float GetNextRandomNumber(Archer requester)
        {
                if (!indexByArcher.ContainsKey(requester))
                {
                        indexByArcher.Add(requester,0);
                }

                var index = indexByArcher[requester];
                if (index >= numberOfRandomNumbers-1)
                {
                        index = 0;
                }
                indexByArcher[requester]++;
                return randomNumbers[index];
        }

        public float GetDesperation(int hitsLeft)
        {
                var leaderHitsLeft = one.scoreBoard.hitsLeft;
                if (two.scoreBoard.hitsLeft < leaderHitsLeft)
                        leaderHitsLeft = two.scoreBoard.hitsLeft;

                if (three.scoreBoard.hitsLeft < leaderHitsLeft)
                        leaderHitsLeft = three.scoreBoard.hitsLeft;

                return CalculateDesperation(hitsLeft, leaderHitsLeft);
        }


        public float GetDesperation(Archer self, Archer rival)
        {
                var hitsLeftSelf = self.scoreBoard.hitsLeft;
                var hitsLeftRival = rival.scoreBoard.hitsLeft;

                if (hitsLeftSelf <= hitsLeftRival) return 0;

                return CalculateDesperation(hitsLeftSelf, hitsLeftRival);
        }
        
        private float CalculateDesperation(int hitsLeft, int leaderHitsLeft)
        {
                var difference = hitsLeft - leaderHitsLeft;
                if (difference == 0) return 0; //returning because archer is leader
                var percentageDiffFromLeader = (float)difference / sequenceHandler.totalHitsNeeded;
                var leaderPercentageSeverity = 1-(float)leaderHitsLeft / sequenceHandler.totalHitsNeeded;
                // ReSharper disable once PossibleLossOfFraction
                return (percentageDiffFromLeader + leaderPercentageSeverity) / 2;
        }

        private void OnDisable()
        {
                disposable.Clear();
        }
}