

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;

public class ScorerService
{
    //public ReactiveList<UtilityContainerSelector> ContainerSelectors;
    public ReactiveList<IUtilityScorer> UtilityScorers;
    
    private ScorerService()
    {
        //LoadContainerSelectors();
        LoadUtilityScorers();
    }
    
    private void LoadUtilityScorers()
    {
        UtilityScorers = new ReactiveList<IUtilityScorer>();
        var elements = AssetService.GetInstancesOfType<IUtilityScorer>();
        elements = elements
            .Where(e => 
                UtilityScorers
                .Values
                .FirstOrDefault(element => element.GetType() == e.GetType()) == null) // Avoids Duplicates
            .ToList();

        UtilityScorers.Add(elements);
    }

    private static ScorerService instance;
    public static ScorerService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ScorerService();
            }
            return instance;
        }
    }
}
