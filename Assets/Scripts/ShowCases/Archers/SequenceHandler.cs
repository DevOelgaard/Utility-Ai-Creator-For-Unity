using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SequenceHandler: MonoBehaviour
{
    public int totalHitsNeeded = 100;
    public int maxPrSequence = 7;
    public List<TargetHandler> targetHandlers;

    private List<int> sequence = new List<int>();

    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        sequence = new List<int>();
        var countLeft = totalHitsNeeded;
        var retryCounter = 100000;
        while (countLeft > 0 && retryCounter > 0)
        {
            retryCounter--;
            var sequenceNumber = Random.Range(1, maxPrSequence);
            sequenceNumber = Math.Clamp(sequenceNumber, 1, countLeft);
            countLeft -= sequenceNumber;
            sequence.Add(sequenceNumber);
        }

        foreach (var targetHandler in targetHandlers)
        {
            targetHandler.Reset();
        }

        PrintSequence();
    }

    private void PrintSequence()
    {
        var text = "Sequence: ";

        var index = 0;
        var total = 0;
        foreach (var value in sequence)
        {
            index++;
            if (IsLeft(index))
            {
                text += "L-";
            }
            else
            {
                text += "R-";
            }
            text+= value;
            if (index < sequence.Count)
            {
                text += ", ";
            }

            total += value;
        }
        Debug.Log(text + " Total: " + total);
    }

    public bool IsLeft(int index)
    {
        return index % 2 == 0;
    }

    public int GetSequenceAtIndex(int index)
    {
        if (index >= sequence.Count) return -1;

        return sequence[index];
    }
}