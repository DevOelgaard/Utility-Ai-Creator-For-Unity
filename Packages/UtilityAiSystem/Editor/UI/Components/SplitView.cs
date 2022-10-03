using UnityEngine.UIElements;

public class SplitView: TwoPaneSplitView
{
    public SplitView()
    {

    }
    public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }

}
