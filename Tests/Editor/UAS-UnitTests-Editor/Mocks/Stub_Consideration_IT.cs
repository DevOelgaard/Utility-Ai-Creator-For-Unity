using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks
{
    public class Stub_Consideration_IT : Consideration
    {
        public float ReturnValue;

        public Stub_Consideration_IT(float returnValue, List<ParamFloat> parameters, float min = 0f, float max = 1f)
        {
            ReturnValue = returnValue;
            throw new NotImplementedException("This doesn't work as inteded");
            foreach (var parameter in parameters)
            {
                // AddParameter(parameter);
            }
            CurrentResponseCurve = new Mock_ResponseCurve_IT("Mock");
            MinFloat.Value = min;
            MaxFloat.Value = max;
        }

        protected override float CalculateBaseScore(IAiContext context)
        {
            var result = ReturnValue;
            foreach(var param in Parameters)
            {
                throw new NotImplementedException("This doesn't work as inteded");

                // result += Convert.ToSingle(param.Value);
            }
            return result;
        }

    }
}