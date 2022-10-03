using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal enum InfoTypes
{
    Info,
    Warning,
}

internal struct InfoModel
{
    internal string Info { get; private set; }
    internal InfoTypes InfoType { get; private set; }

    public InfoModel(string info = "", InfoTypes infoType = InfoTypes.Info)
    {
        Info = info;
        InfoType = infoType;
    }
}
