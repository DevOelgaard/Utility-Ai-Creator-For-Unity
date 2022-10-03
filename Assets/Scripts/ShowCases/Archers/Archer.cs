using System;
using TMPro;
using UniRx;
using UnityEditor.Search;
using UnityEngine;
using Random = UnityEngine.Random;

public class Archer: AgentMono
{
        
        private CompositeDisposable disposable = new CompositeDisposable();
        private GameHandler gameHandler => GameObject.Find("GameHandler").GetComponent<GameHandler>();

        private TurnHandler turnHandler => GameObject.Find("TurnHandler").GetComponent<TurnHandler>();
        public ScoreBoard scoreBoard;

        private ArcherAttributes archerAttributes =>
                GameObject.Find("ArcherAttributes").GetComponent<ArcherAttributes>();

        public float MoveSpeed => archerAttributes.Movement;

        /// <summary>
        /// Between 0-1;
        /// </summary>
        public float Precision => archerAttributes.Precision;
        public int ReloadAmount => archerAttributes.ReloadSpeed;


        public int AmmoCapacity => archerAttributes.AmmoCapacity;

        public float Desperation;
        public int CurrentAmmo
        {
                get => currentAmmo;
                private set
                {
                        currentAmmo = value;
                        valuesChanged.OnNext(true);
                }
        }
        [SerializeField] private int currentAmmo = 10;

        public IObservable<bool> ValuesChanged => valuesChanged;
        private readonly Subject<bool> valuesChanged = new Subject<bool>();
        
        public Field field;
        public Target leftTarget;
        public Target rightTarget;
        public SpriteRenderer spriteRenderer;
        public TextMeshPro text;
        public TargetHandler targetHandler;
        public bool HasFinished => scoreBoard.hitsLeft <= 0;

        void OnEnable()
        {
                turnHandler.TurnChanged.Subscribe(turn =>
                {
                        SetText("");
                        if (scoreBoard.hitsLeft <= 0)
                        {
                                valuesChanged.OnNext(true);
                                return;
                        }

                        var tickMeta = new TickMetaData
                        {
                                TickCount = turn
                        };
                        Tick(tickMeta);
                        scoreBoard.SetName(Uai.Name);
                        Desperation = gameHandler.GetDesperation(scoreBoard.hitsLeft);
                        Debug.Log("Desperation: " + Desperation);
                        valuesChanged.OnNext(true);
                }).AddTo(disposable);
                
        }

        public void Reset()
        {
                var position = transform.localPosition;
                position.x = 0;
                transform.localPosition = position;
                CurrentAmmo = AmmoCapacity;
                valuesChanged.OnNext(true);
        }

        protected override void Start()
        {
                base.Start();
                Reset();
        }
        
        public bool CanMoveToTarget()
        {
                var newDirection = GetDirection(false);
                return Math.Abs(newDirection - transform.localPosition.x) > 0.01f;
        }

        public bool CanMoveToReload()
        {
                var newDirection = GetDirection(true);
                return Math.Abs(newDirection - transform.localPosition.x) > 0.01f;  
        }
        
        public void Move(bool toReload)
        {
                if (!CanMoveToTarget() && !CanMoveToReload())
                {
                        Debug.LogWarning(name + " Can't move already at destination");
                        return;
                }
                var pos = transform.localPosition;
                var directionX = GetDirection(toReload);
                var newPos = new Vector3(directionX, pos.y, pos.z);
                transform.localPosition = newPos;
                SetText("Moving!");
                Debug.Log(name + ": Moving! DirectionX: " + directionX +  " MoveSpeed: " + MoveSpeed);
        }

        public float GetDirection(bool toReload)
        {
                var movement = 0f;
                var destination = 0f;

                if (toReload)
                {
                        destination = 0f;
                }
                else // To Target
                {
                        destination = targetHandler.ActiveTarget.transform.localPosition.x;
                }
                if (destination> transform.localPosition.x)
                {
                        movement = transform.localPosition.x + MoveSpeed;
                        if (movement > destination)
                        {
                                movement = destination;
                        }

                }
                else
                {
                        movement = transform.localPosition.x - MoveSpeed;
                        if (movement < destination)
                        {
                                movement = destination;
                        }
                } 


                return Math.Clamp(movement, field.LeftBorder, field.RightBorder);
        }

        public bool CanShoot()
        {
                return CurrentAmmo > 0;
        }

        public void ShootTarget()
        {
                var target = targetHandler.ActiveTarget;

                if (!CanShoot())
                {
                        Debug.LogWarning(name + " Can't shoot");
                        return;
                }

                CurrentAmmo--;
                
                var chanceToHit = GetCurrentChanceToHit();
                var scoreToBeat = gameHandler.GetNextRandomNumber(this);
                Debug.Log(name + " Shooting! " + " ChanceToHit: " + chanceToHit + " Score to beat: " + scoreToBeat);

                if (chanceToHit > scoreToBeat)
                {
                        target.Hit();
                }
                else
                {
                        target.Miss();
                }
                SetText("Shooting");
        }

        public float GetCurrentChanceToHit()
        {
                var target = targetHandler.ActiveTarget;
                var distance = target.GetDistance(transform.localPosition.x);
                
                return CalculateChanceToHit(distance);
        }

        public float CalculateChanceToHit(float distance)
        {
                // 1-normalized, because a smaller distance is better
                var distanceNormalized = 1-(distance / field.length);
                if (distance > field.length)
                        distanceNormalized = 0;
                // var distChance = Mathf.Pow(distanceNormalized, 2);
                var normalizedResult = (distanceNormalized * archerAttributes.distanceFactor + Precision) / (archerAttributes.distanceFactor+1);
                // var result = Mathf.Pow(normalizedResult, 2);
                // Debug.Log(name + " DistanceNorm: " + distanceNormalized + ", normalizedResult: " + normalizedResult + ", result: " + result);
                return normalizedResult;
        }

        public bool CanReload()
        {
                return CurrentAmmo < AmmoCapacity;
        }

        public void Reload()
        {
                if (transform.localPosition.x != 0)
                {
                        Debug.LogWarning("Can't reload not at station");
                        return;
                }

                if (!CanReload())
                {
                        Debug.LogWarning("Can't reload. Pockets are full");
                        return;
                }

                CurrentAmmo += ReloadAmount;
                CurrentAmmo = Math.Clamp(CurrentAmmo, 0, AmmoCapacity);
                SetText("Reloading");
                Debug.Log(name + " Reloading: Ammo taken: " + ReloadAmount + " Current Ammo: " + CurrentAmmo);
        }

        private void SetText(string newText)
        {
                text.text = newText;
        }
        
        private void OnDisable()
        {
                disposable.Clear();
        }

        public void Idle(string text)
        {
                SetText(text);
        }
}