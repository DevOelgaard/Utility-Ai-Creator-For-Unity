using System;
using TMPro;
using UniRx;
using UnityEditor.Search;
using UnityEngine;

public class ScoreBoard: MonoBehaviour
{
        private CompositeDisposable disposable = new CompositeDisposable();
        private GameHandler gameHandler => GameObject.Find("GameHandler").GetComponent<GameHandler>();

        public TextMeshPro currentTurns;
        public TextMeshPro totalTurns;
        public TextMeshPro hitsLeftCounter;
        public TextMeshPro nameText;
        public TextMeshPro accuracyText;
        public TextMeshPro winsText;
        public Target leftTarget;
        public Target rightTarget;

        public int hitsLeft = -1;
        public int turnsSpent = 0;

        private TurnHandler turnHandler => GameObject.Find("TurnHandler").GetComponent<TurnHandler>();
        private SequenceHandler sequenceHandler => GameObject.Find("SequenceHandler").GetComponent<SequenceHandler>();

        public bool IsFinished => hitsLeft <= 0;
        private int totalHits = 0;
        private int totalMisses = 0;
        private int wins = 0;
        private int totalShots => totalHits + totalMisses;
        private float accuracy => (float)totalHits / totalShots * 100f;
        private int currentTurnsCounter = 0;

        public IObservable<bool> NoHitsLeft => noHitsLeft;
        private Subject<bool> noHitsLeft = new Subject<bool>();
        private void OnEnable()
        {
                Reset();
                Subscribe();
        }

        public void Reset()
        {
                hitsLeft = sequenceHandler.totalHitsNeeded;
                hitsLeftCounter.text = hitsLeft.ToString();
                currentTurnsCounter = 0;
        }

        public void IncrementWins()
        {
                wins++;
                winsText.text = wins.ToString();
        }

        private void Subscribe()
        {
                // gameHandler.OnReset.Subscribe(_ => Reset()).AddTo(disposable);
                        
                leftTarget.TargetHit.Subscribe(hit =>
                        {
                                if (hit)
                                {
                                        ReduceHitsLeft();
                                        totalHits++;
                                }
                                else
                                {
                                        totalMisses++;
                                }

                                accuracyText.text = accuracy.ToString("0.00") + "%";
                        })
                        .AddTo(disposable);
                
                rightTarget.TargetHit.Subscribe(hit =>
                        {
                                if (hit)
                                {
                                        ReduceHitsLeft(); 
                                }
                        })
                        .AddTo(disposable);

                turnHandler.TurnChanged.Subscribe(turn =>
                        {
                                IncrementTurnsSpent();
                        })
                        .AddTo(disposable);
        }

        private void IncrementTurnsSpent()
        {
                if (hitsLeft <= 0)
                {
                        return;
                }

                turnsSpent++;
                currentTurnsCounter++;
                currentTurns.text = currentTurnsCounter.ToString();
                totalTurns.text = turnsSpent.ToString();
        }

        private void ReduceHitsLeft()
        {
                if (hitsLeft == 0)
                {
                        noHitsLeft.OnNext(true);
                        return;
                }
                hitsLeft--;
                if (hitsLeft <= 0)
                {
                        hitsLeft = 0;
                        // throw new NotImplementedException();
                }
                hitsLeftCounter.text = hitsLeft.ToString();

                if (hitsLeft == 0)
                {
                        noHitsLeft.OnNext(true);
                }
        }

        public void SetName(string newName)
        {
                nameText.text = newName;
        }
        
        private void OnDisable()
        {
                disposable.Clear();
        }
}