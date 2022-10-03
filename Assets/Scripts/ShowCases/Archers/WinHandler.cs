using System;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WinHandler: MonoBehaviour
{
        private CompositeDisposable disposable = new CompositeDisposable();
        public ScoreBoard scoreBoardOne;
        public ScoreBoard scoreBoardTwo;
        public ScoreBoard scoreBoardThree;
        public TurnHandler turnHandler;
        public GameHandler gameHandler;

        private int winTurn = int.MinValue;
        private bool winnerFound = false;

        private void OnEnable()
        {
                // scoreBoardOne.NoHitsLeft.Subscribe(_ => HandleNoHitsLeft(scoreBoardOne)).AddTo(disposable);
                // scoreBoardTwo.NoHitsLeft.Subscribe(_ => HandleNoHitsLeft(scoreBoardTwo)).AddTo(disposable);
                // scoreBoardThree.NoHitsLeft.Subscribe(_ => HandleNoHitsLeft(scoreBoardThree)).AddTo(disposable);
                // gameHandler.OnReset.Subscribe(_ => Reset()).AddTo(disposable);
                Reset();
        }

        public void Reset()
        {
                winnerFound = false;
                winTurn = int.MinValue;
        }

        public void CheckWin()
        {
                var currentTurn = turnHandler.CurrentTurn;

                if (winnerFound) return;
                
                CheckScoreBoard(scoreBoardOne);
                CheckScoreBoard(scoreBoardTwo);
                CheckScoreBoard(scoreBoardThree);
        }

        private void CheckScoreBoard(ScoreBoard scoreBoard)
        {
                if (scoreBoard.IsFinished)
                {
                        scoreBoard.IncrementWins();
                        winnerFound = true;
                }
        }

        // public void HandleNoHitsLeft(ScoreBoard scoreBoard)
        // {
        //         var currentTurn = turnHandler.CurrentTurn;
        //
        //         // For multiple winners same turn where first winner has been found
        //         if (winnerFound && winTurn == currentTurn)
        //         {
        //                 scoreBoard.IncrementWins();
        //                 return;
        //         }
        //
        //         if (!winnerFound)
        //         {
        //                 winTurn = currentTurn;
        //                 winnerFound = true;
        //                 scoreBoard.IncrementWins();
        //         }
        // }

        private void OnDisable()
        {
                disposable.Clear();
        }
}