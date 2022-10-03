using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;

namespace Mocks
{
    public class Stub_Consideration_UT : Consideration
    {
        public float ReturnValue;

        public Stub_Consideration_UT()
        {
        }

        public Stub_Consideration_UT(float returnValue, List<ParamBase> parameters)
        {
            ReturnValue = returnValue;
            throw new NotImplementedException("This doesn't work as intended");
            foreach (var parameter in parameters)
            {
                AddParameter(parameter.Name,parameter);
            }

            CurrentResponseCurve = new Mock_ResponseCurve("Mock");
        }
        protected override float CalculateBaseScore(IAiContext context)
        {
            return ReturnValue;
        }

    }
}