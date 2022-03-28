using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Consts
{
    // Paths
    internal const string MenuName = "Utility Ai System/";
    internal const string Path_MainFolder = "Assets/UAS/bin/";
    internal const string File_TickerSettings = Path_MainFolder + "Settings/TickerSettings" + "." + FileExtension_TickerSettings;
    //internal const string File_PlayAi = Path_MainFolder + "Ai/Playable" + "." + FileExtension_UasTemplateCollection;
    internal const string File_UASTemplateService_AutoSave = Path_MainFolder + "Saves/AutoSave" + "." + FileExtension_UasTemplateCollection;
    internal const string File_UASTemplateService_BackUp = Path_MainFolder + "Saves/backup" + "." + FileExtension_UasTemplateCollection;
    internal const string File_UASTemplateServicel_Playable = Path_MainFolder + "Saves/AutoSave" + "." + FileExtension_UasTemplateCollection;

    internal const string File_MainSavePath = Path_MainFolder + "Persistence/";

    // Labels
    internal const string Label_MainWindowModel = "MainWindowModel";
    internal const string Label_AgentActionModel = "AgentActions";
    internal const string Label_ConsiderationModel = "Considerations";
    internal const string Label_DecisionModel = "Decisions";
    internal const string Label_BucketModel = "Buckets";
    internal const string Label_UAIModel = "AIs";
    internal const string Label_ResponseCurve = "Response Curve";
    internal const string Label_DebuggerText = "Debugging";



    // Scorers
    public const string Default_UtilityScorer = Name_USAverageScore;
    public const string Default_BucketSelector = UCS_HighestScore_Name;
    public const string Default_DecisionSelector = UCS_HighestScore_Name;

    public const string UCS_HighestScore_Name = "Highest Score";
    public const string UCS_HighestScore_Description = "Selects the highest scored object";

    public const string UCS_RandomXHighest_Name = "Random X Highest";
    public const string UCS_RandomXHighest_Description = "Returns random element among X highest. If X < 1, all valid AiObjects are considered. Can be completely random or relative to score.";


    public const string Name_USAverageScore = "Average";
    public const string Description_USAverageScore = "Returns the average score of all considerations. Returns 0 if one consideration is 0";

    public const string Name_USCompensationScorer = "Compensation";
    public const string Description_USCompensationScorer = "Returns all considerations scores multiplied and compensates for multiplying by <1.";

    public const string Name_DefaultDSE = "Default DSE";
    public const string Description_DefaultDSE = "Selects the highest scored object";

    // Editor Windows
    public const string Window_TemplateManager_Name = "Template Manager";
    public const string Window_AiInspector_Name = "Runtime Inspector";
    public const string Window_AiTickerManager_Name = "Settings";
    public const string Window_Logger_Name = "Logger";


    // Ticker Modes
    public const string Description_TickerModeUnrestricted = "No restrictions to execution time";
    public const string Description_TickerModeDesiredFrameRate = "Dynamically alters ticks/frame to stay above target framerate. Increasing samplerate trades Reaction time for framerate precision.";
    public const string Description_TickerModeTimeBudget = "Executes as many ticks as possible in a given timeframe. The Time budget may be exceeded by the execution time of one agent";


    // File extensions
    public const string FileExtension_AgentAction = "action";
    public const string FileExtension_Consideration = "consideration";
    public const string FileExtension_Decision = "decision";
    public const string FileExtension_Bucket = "bucket";
    public const string FileExtension_UAI = "uai";
    public const string FileExtension_UasTemplateCollection = "uaiproj";
    public const string FileExtension_RestoreAbleCollection = "collection";

    public const string FileExtension_JSON = ".json";

    public const string FileExtension_TickerSettings = "setting";

    public static readonly string[] FileExtensions =
    {
        "Ai Objects", "*" + FileExtension_JSON,
        "Ticker Settings", FileExtension_TickerSettings,
        "All Files", "*",
    };

    // UI Element Text
    public const string Text_Button_SortByPerformance = "Sort By Performance";
    public const string Text_Button_SortByWeight = "Sort By Weight";
    public const string Text_Button_Folded = "Expand";
    public const string Text_Button_Expanded = "Fold";
    public const string Text_Button_Up = "+";
    public const string Text_Button_Down = "-";



}