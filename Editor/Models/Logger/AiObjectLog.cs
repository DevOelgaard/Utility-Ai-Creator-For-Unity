using System;

internal abstract class AiObjectLog : ILogModel
{
    public string Name = "";
    public string UiName = "";
    public string Description = "";
    public string Type = "";
    public int LastSelectedTick;
    public int LastEvaluatedTick;
    public int CurrentTick;

    internal static AiObjectLog SetBasics(AiObjectLog log, AiObjectModel model, int tick)
    {
        log.Name = model.Name;
        log.UiName = model.GetUiName();
        log.Description = model.Description;
        log.Type = model.GetType().ToString();
        log.LastSelectedTick = model.MetaData.LastTickSelected;
        log.LastEvaluatedTick = model.MetaData.LastTickEvaluated;
        log.CurrentTick = tick;
        return log;
    }

    internal static AiObjectLog SetBasics(AiObjectLog log, IAgent agent, int tick)
    {
        try
        {
            log.Name = agent.Model.Name;
            log.Description = "";
            log.UiName = log.Name;
            log.Type = agent.GetType().ToString();
            if(agent.Model.LastTickMetaData == null)
            {
                log.LastSelectedTick = -1;
            }
            else
            {
                log.LastSelectedTick = agent.Model.LastTickMetaData.TickCount;
            }
            log.CurrentTick = tick;
            return log;
        }
        catch (Exception ex)
        {
            var msg = "Model: " + agent.Model.Name;
            msg += " TickMetaData: " + agent.Model.LastTickMetaData;
            msg += " TickCount: " + agent.Model.LastTickMetaData.TickCount;

            throw new Exception(msg, ex);
        }

    }
}
