using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Consts
{
    public const int MaxPathLengthWindows = 260;

    // Paths
    internal const string MenuName = "UAI Creator/";
    internal const string PathMainFolder = "Assets/UAS/bin/";

    internal const string ProjectSettingsPath =
        PathMainFolder + "Settings/ProjectSettings." + FileExtension_ProjectSettings;

    internal static string CurrentProjectPath => ProjectSettingsService.Instance.GetCurrentProjectDirectory();
    internal static string Folder_TickerSettings_Complete => Folder_TickerSettings + FileName_TickerSettings + "/";
    internal static string Folder_PlayAbleAi_Complete => Folder_PLayAbleAi + FileName_PLayAbleAi + "/";

    internal static string FilePath_TickerSettingsWithExtention =>
        Folder_TickerSettings + FileName_TickerSettings + "." + FileExtension_TickerSettings;

    internal static string FilePath_PlayAbleAiWithExtention =>
        Folder_PLayAbleAi + FileName_PLayAbleAi + "." + FileExtension_PlayAble;

    internal static string Folder_TickerSettings => CurrentProjectPath + "Settings/";
    internal static string Folder_PLayAbleAi => CurrentProjectPath + FileName_PLayAbleAi + "/";
    internal const string FileName_TickerSettings = "TickerSettings";

    internal static string FolderPath_TickerModes_Complete => Folder_TickerSettings_Complete + FolderName_TickerModes;
    internal static string FolderPath_PlayAbleAi_Complete => Folder_PlayAbleAi_Complete + FolderName_PlayAbleAis;

    internal static string FileUasProjectTemp => CurrentProjectPath + "Temp/";

    // internal static string FileUasPlayAblePathWithNameAndExtension => FileUasPlayAblePath + FileName_PLayAbleAi + "." + FileExtension_PlayAble;
    // internal static string FileUasPlayAblePath => CurrentProjectPath + FileName_PLayAbleAi+"/";
    internal const string FileName_PLayAbleAi = "PlAi";


    // Folder Names
    internal const string FileName_Templates = "Te";
    internal const string FolderName_Templates = FileName_Templates + "/";
    internal const string FolderName_PlayAbleAis = FileName_PLayAbleAi + "/";
    internal const string FolderName_Parameters = "Pa/";
    internal const string FolderName_ResponseFunctions = "RF/";
    internal const string FolderName_Segments = "Se/";
    internal const string FolderName_ResponseCurves = "RC/";
    internal const string FolderName_AgentActions = "Act/";
    internal const string FolderName_Considerations = "Co/";
    internal const string FolderName_Decisions = "Dec/";
    internal const string FolderName_Buckets = "Buc/";
    internal const string FolderName_BucketSelectors = "BuS/";
    internal const string FolderName_DecisionSelectors = "DeS/";
    internal const string FolderName_Ais = "Ais/";
    internal const string FolderName_TickerModes = "TM/";
    internal const string FolderName_Weight = "W/";
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

    public const string UCS_RandomXHighest_Description =
        "Returns random element among X highest. If X < 1, all valid AiObjects are considered. Can be completely random or relative to score.";


    public const string Name_USAverageScore = "Average";

    public const string Description_USAverageScore =
        "Returns the average score of all considerations. Returns 0 if one consideration is 0";

    public const string Name_USCompensationScorer = "Compensation";

    public const string Description_USCompensationScorer =
        "Returns all considerations scores multiplied and compensates for multiplying by <1.";

    public const string Name_HighestBucketDSE = "Highest Bucket DSE";
    public const string Description_HighestBucketDSE = "Selects the highest scored object";

    public const string Name_AllBucketsDSE = "All Buckets DSE";
    public const string Description_AllBucketsDSE = "Evaluates all buckets. Decision.utility i multiplied by bucket weight";
    // Editor Windows
    public const string Window_TemplateManager_Name = "Template Manager";
    public const string Window_RuntimeInspector_Name = "Runtime Inspector";
    public const string Window_AiTickerManager_Name = "Settings";
    public const string Window_Logger_Name = "Logger";
    public const string Window_SelectProject_Name = "Select Project";


    // Ticker Modes
    public const string Description_TickerModeUnrestricted = "No restrictions to execution time";

    public const string Description_TickerModeDesiredFrameRate =
        "Dynamically alters ticks/frame to stay above target framerate. Increasing samplerate trades Reaction time for framerate precision.";

    public const string Description_TickerModeTimeBudget =
        "Executes as many ticks as possible in a given timeframe. The Time budget may be exceeded by the execution time of one agent";


    public const int LengthOfLongestExtension = 8;
    // File extensions
    public const string FileExtension_TemplateService = "uasproj";
    public const string FileExtension_PlayAble = "uasplay";
    public const string FileExtension_UAI = "uasuai";
    public const string FileExtension_Bucket = "uasbuc";
    public const string FileExtension_Decision = "uasdec";
    public const string FileExtension_Consideration = "uascons";
    public const string FileExtension_AgentAction = "uasact";
    public const string FileExtension_ResponseCurve = "uasrcrv";
    public const string FileExtension_ResponseFunction = "uasrfunc";
    public const string FileExtension_Parameter = "uasparam";
    public const string FileExtension_TickerSettings = "uasttset";
    public const string FileExtension_TickerModes = "uastmod";
    public const string FileExtension_UtilityContainerSelector = "uasucs";
    public const string FileExtension_RestoreAbleCollection = "uascol";
    public const string FileExtension_AiTicker = "uastick";
    public const string FileExtension_ProjectSettings = "uaspset";

    public const string FileExtension_JSON = ".json";

    public static readonly List<string> FileExtensions = new List<string>()
    {
        FileExtension_AgentAction,
        FileExtension_AiTicker,
        FileExtension_Bucket,
        FileExtension_Consideration,
        FileExtension_Decision,
        FileExtension_Parameter,
        FileExtension_PlayAble,
        FileExtension_ProjectSettings,
        FileExtension_ResponseCurve,
        FileExtension_ResponseFunction,
        FileExtension_TemplateService,
        FileExtension_TickerModes,
        FileExtension_RestoreAbleCollection,
        FileExtension_UAI,
        FileExtension_UtilityContainerSelector,
    };

    
    public static readonly string[] FileExtensionsFilters =
    {
        "UAS Objects", "uas*",
        "Project", FileExtension_TemplateService,
        "Ais", FileExtension_UAI,
        "Buckets", FileExtension_Bucket,
        "Decisions", FileExtension_Decision,
        "Considerations", FileExtension_Consideration,
        "Response Curves", FileExtension_ResponseCurve,
        "Ticker Settings", FileExtension_TickerSettings,
        "All Files", "*",
    };

    // UI Element Text
    public const string Text_Button_SortByPerformance = "Performance Sort";
    public const string Text_Button_SortByWeight = "Sort By Weight";
    public const string Text_Button_Folded = "Expand";
    public const string Text_Button_Expanded = "Fold";
    public const string Text_Button_Up = "+";
    public const string Text_Button_Down = "-";

    internal const string LineBreakBaseTypes = "----Base Types----";
    internal const string LineBreakTemplates = "----Templates----";
    internal const string LineBreakDemos = "----Demos----";


    public const string FileName_TickerModeDesiredFrameRate = "DsFr";
    public const string FileName_TickerModeUnrestrictedFrameRate = "Unre";
    public const string FileName_TickerModeTimeBudgetFrameRate = "TiBu";

    internal const string Sequence_CalculateUtility_UAI = "CalculateUtility_UAI";
    internal const string Sequence_CalculateUtility_User = "CalculateUtility_User";
    internal const string Sequence_Persistence_Load = "Load Project";
    internal const string Sequence_Persistence_Save = "Save Project";
}