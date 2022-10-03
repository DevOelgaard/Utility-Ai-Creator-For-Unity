using System;
using UnityEngine;
using UniRx;

public class TargetHandler: MonoBehaviour
{
        private IDisposable resetSub;
        private CompositeDisposable disposable = new CompositeDisposable();
        private GameHandler gameHandler => GameObject.Find("GameHandler").GetComponent<GameHandler>();

        private IDisposable activeTargetSubscription;
        public Target leftTarget;
        public Target rightTarget;
        public Target ActiveTarget { get; private set; }

        public IObservable<Target> ActiveTargetChanged => activeTargetChanged;
        private readonly Subject<Target> activeTargetChanged = new Subject<Target>();

        private static SequenceHandler SequenceHandler => GameObject.Find("SequenceHandler").GetComponent<SequenceHandler>();

        private int currentSequenceIndex = 0;
        void Start()
        {
                Reset();
                // resetSub = gameHandler.OnReset.Subscribe(_ => Reset());
        }

        public void Reset()
        {
                currentSequenceIndex = 0;
                ActivateNextTarget();
        }

        private void ActivateNextTarget()
        {
                ActivateTarget(SequenceHandler.IsLeft(currentSequenceIndex) ? leftTarget : rightTarget);
                currentSequenceIndex++;
        }

        private void ActivateTarget(Target target)
        {
                leftTarget.gameObject.SetActive(false);
                rightTarget.gameObject.SetActive(false);
                
                disposable.Clear();
                
                ActiveTarget = target;
                var hp = SequenceHandler.GetSequenceAtIndex(currentSequenceIndex);
                if (hp < 0)
                {
                        return;
                }
                ActiveTarget.gameObject.SetActive(true);
                ActiveTarget.SetHp(hp);
                activeTargetChanged.OnNext(ActiveTarget);

                ActiveTarget.HpDepleted.Subscribe(depleted =>
                {
                        ActivateNextTarget();
                }).AddTo(disposable);
        }

        private void OnDisable()
        {
                disposable.Clear();
                resetSub?.Dispose();
        }
}