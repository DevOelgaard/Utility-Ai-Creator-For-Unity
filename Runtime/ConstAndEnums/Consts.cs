using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Consts
{
    // Paths
    internal const string MenuName = "Utility Ai System/";
    internal const string PathMainFolder = "Assets/UAS/bin/";
    internal const string FileTickerSettings = PathMainFolder + "Settings/TickerSettings" + "." + FileExtension_TickerSettings;
    internal const string ProjectSettingsPath = PathMainFolder + "Settings/ProjectSettings." + FileExtension_ProjectSettings;


    //internal const string File_PlayAi = Path_MainFolder + "Ai/Playable" + "." + FileExtension_UasTemplateCollection;
    internal const string File_UASTemplateService_AutoSave = PathMainFolder + "Saves/AutoSave" + "." + FileExtension_UasProject;
    internal const string FileUasProjectBackUp = PathMainFolder + "BackUp/";
    internal const string FileUasProjectPlayAbles = PathMainFolder + "PlayAbles/";
    internal const string File_UASTemplateServicel_Playable = PathMainFolder + "Saves/Playable" + "." + FileExtension_UasProject;

    // Folder Names
    internal const string FolderName_Parameters = "Parameters/";
    internal const string FolderName_ResponseFunctions = "ResponseFunctions/";
    internal const string FolderName_Segments = "Segments/";
    internal const string FolderName_ResponseCurves = "ResponseCurves/";
    internal const string FolderName_AgentActions = "AgentActions/";
    internal const string FolderName_Considerations = "Considerations/";
    internal const string FolderName_Decisions = "Decisions/";
    internal const string FolderName_Buckets = "Buckets/";
    internal const string FolderName_BucketSelectors = "BucketSelector/";
    internal const string FolderName_DecisionSelectors = "DecisionSelectors/";
    internal const string FolderName_Ais = "Ais/";
    internal const string FolderName_TickerModes = "TickerModes/";
    internal const string FolderName_Weight = "Weight/";
    internal const string FolderName_MinParameter = "Min/";
    internal const string FolderName_MaxParameter = "Max/";


    internal const string File_MainSavePath = PathMainFolder + "Persistence/";

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
    public const string Window_SelectProject_Name = "Select Project";


    // Ticker Modes
    public const string Description_TickerModeUnrestricted = "No restrictions to execution time";
    public const string Description_TickerModeDesiredFrameRate = "Dynamically alters ticks/frame to stay above target framerate. Increasing samplerate trades Reaction time for framerate precision.";
    public const string Description_TickerModeTimeBudget = "Executes as many ticks as possible in a given timeframe. The Time budget may be exceeded by the execution time of one agent";


    // File extensions
    public const string FileExtension_UasProject = "uasproj";
    public const string FileExtension_UAI = "uasuai";
    public const string FileExtension_Bucket = "uasbuc";
    public const string FileExtension_Decision = "uasdec";
    public const string FileExtension_Consideration = "uascons";
    public const string FileExtension_AgentAction = "uasact";
    public const string FileExtension_ResponseCurve = "uasrcrv";
    public const string FileExtension_ResponseFunction= "uasrfunc";
    public const string FileExtension_Parameter = "uasparam";
    public const string FileExtension_TickerSettings = "uasttset";
    public const string FileExtension_TickerModes = "uastmod";
    public const string FileExtension_UtilityContainerSelector = "uasucs";
    public const string FileExtension_RestoreAbleCollection = "uascol";
    public const string FileExtension_AiTicker = "uastick";
    public const string FileExtension_ProjectSettings = "uaspset";

    public const string FileExtension_JSON = ".json";


    public static readonly string[] FileExtensionsFilters =
    {
        "UAS Objects", "uas*",
        "Project", FileExtension_UasProject,
        "Ais", FileExtension_UAI,
        "Buckets", FileExtension_Bucket,
        "Decisions", FileExtension_Decision,
        "Considerations", FileExtension_Consideration,
        "Response Curves", FileExtension_ResponseCurve,
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