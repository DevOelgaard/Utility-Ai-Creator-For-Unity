using System;
using System.Globalization;
using UniRx;
using UnityEngine;

public class ArcherInfo: MonoBehaviour
{
        private readonly CompositeDisposable disposable = new CompositeDisposable();
        public Archer archer;
        public TextAndValue ammo;
        public TextAndValue reload;
        public TextAndValue precision;
        public TextAndValue movement;
        public TextAndValue desperation;

        private void OnEnable()
        {
                UpdateValues();
                archer.ValuesChanged.Subscribe(_ =>
                {
                        UpdateValues();
                }).AddTo(disposable);
        }

        private void UpdateValues()
        {
                ammo.SetValue(archer.CurrentAmmo + " / " + archer.AmmoCapacity);
                reload.SetValue(archer.ReloadAmount.ToString());
                precision.SetValue(archer.Precision.ToString("0.00"));
                movement.SetValue(archer.MoveSpeed.ToString("0.00"));
                desperation.SetValue(archer.Desperation.ToString("0.00"));
        }

        void OnDisable()
        {
                disposable.Clear();
        }
}