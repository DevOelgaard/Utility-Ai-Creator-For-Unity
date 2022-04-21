using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

internal class ScoreLogComponent: VisualElement
{
    private readonly Label scoreLabel;
    private readonly string identifier;
    public ScoreLogComponent(string identifier, string value)
    {
        scoreLabel = new Label();
        Add(scoreLabel);
        this.identifier = identifier;
        UpdateScore(identifier, value);
    }

    private void UpdateScore(string identifier, string value)
    {
        scoreLabel.text = identifier + ": " + value;
    }

    internal void UpdateScore(float score)
    {
        UpdateScore(this.identifier, score.ToString("0.00"));
    }

}
