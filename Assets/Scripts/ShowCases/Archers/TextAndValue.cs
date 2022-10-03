using TMPro;
using UnityEngine;

public class TextAndValue: MonoBehaviour
{
        [SerializeField] private TextMeshPro text;
        [SerializeField] private TextMeshPro value;

        public void SetText(string newText)
        {
                text.text = newText;
        }

        public void SetValue(string newValue)
        {
                value.text = newValue;
        }
}