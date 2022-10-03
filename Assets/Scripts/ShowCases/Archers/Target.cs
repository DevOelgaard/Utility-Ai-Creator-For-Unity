using System;
using TMPro;
using UniRx;
using UnityEngine;

public class Target: MonoBehaviour
{
    private CompositeDisposable disposable = new CompositeDisposable();
    private GameHandler gameHandler => GameObject.Find("GameHandler").GetComponent<GameHandler>();

    public IObservable<bool> TargetHit => targetHit;
    private readonly Subject<bool> targetHit = new Subject<bool>();
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    public IObservable<bool> HpDepleted => hPDepleted;
    private readonly Subject<bool> hPDepleted = new Subject<bool>();
    public TextMeshPro text;
    public TextMeshPro lifeCounter;
    public int HP = 10;
    private TurnHandler turnHandler => GameObject.Find("TurnHandler").GetComponent<TurnHandler>();
    public Field field;

    private int lastHitMissTurn = -1;

    void OnEnable()
    {
        spriteRenderer.color = Color.white;
        turnHandler.TurnChanged.Subscribe(turn =>
        {
            text.text = "";
            if(lastHitMissTurn != turn)
                spriteRenderer.color = Color.white;
        }).AddTo(disposable);
        gameHandler.OnReset.Subscribe(_ => Reset()).AddTo(disposable);
    }

    void Reset()
    {
        spriteRenderer.color = Color.white;
    }
    
    public void SetHp(int newHp)
    {
        HP = newHp;
        lifeCounter.text = HP.ToString();
    }

    public void Hit()
    {
        Debug.Log(name + " Hit!");
        text.text = "Hit";
        targetHit.OnNext(true);
        SetHp(HP-1);
        spriteRenderer.color = Color.green;
        lastHitMissTurn = turnHandler.CurrentTurn;
        if (HP <= 0)
        {
            hPDepleted.OnNext(true);
        }
    }

    public void Miss()
    {
        Debug.Log(name + " Miss!");
        text.text = "Miss";
        spriteRenderer.color = Color.red;
        lastHitMissTurn = turnHandler.CurrentTurn;
        targetHit.OnNext(false);
    }

    public float GetDistance(float currentX)
    {
        var localXOffset = transform.localPosition.x + field.length;
        var currentXOffset = currentX + field.length;
        
        return Mathf.Abs(localXOffset-currentXOffset);
    }

    public float GetDistance(Transform t)
    {
        return GetDistance(t.localPosition.x);
    }

    public float GetDistanceNormalized(float positionX)
    {
        var distance = GetDistance(positionX);
        return distance / field.length;
    }
    
    private void OnDisable()
    {
        disposable.Clear();
    }
}