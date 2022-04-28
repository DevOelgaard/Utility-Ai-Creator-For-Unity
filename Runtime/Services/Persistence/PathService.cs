public class PathService
{
    public static string FormatFileName(RestoreState restoreState)
    {
        var name = restoreState.FileName;
        if (name.Length > 4)
        {
            name = restoreState.FileName[..4];
        }
        else
        {
            // return restoreState.FileName;
        }

        if (restoreState.Index >= 0)
        {
            name = restoreState.Index + " " + name;
        }

        return name;
    }
}