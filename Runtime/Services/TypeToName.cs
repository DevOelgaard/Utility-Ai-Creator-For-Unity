using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal static class TypeToName
{
    internal const string RF_Bool = "Bool";
    internal const string RF_Exponential = "Exponential";
    internal const string RF_FixedValue = "Fixed Value";
    internal const string RF_InverseLogistic = "InverseLogitic";
    internal const string RF_Linear = "Linear";
    internal const string RF_Logarithmic = "Logarithmic";
    internal const string RF_Logistic = "Logistic";
    internal const string RF_NormalDistribution = "Normal Distribution";

    internal static string ResponseFunctionToName(Type t)
    {
        if (t == typeof(BoolFunction))
            return RF_Bool;
        else if (t == typeof(ExponentialFunction))
            return RF_Exponential;
        else if (t == typeof(FixedValueFunction))
            return RF_FixedValue;
        else if (t == typeof(InverseLogisticFunction))
            return RF_InverseLogistic;
        else if (t == typeof(LinearFunction))
            return RF_Linear;
        else if (t == typeof(LogarithmicFunction))
            return RF_Logarithmic;
        else if (t == typeof(LogisticFunction))
            return RF_Logistic;
        else if (t == typeof(NormalDistributionFunction))
            return RF_NormalDistribution;

        return t.ToString();
    }
}
