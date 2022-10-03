using System.Collections.Generic;

namespace Mocks
{
    public class Mock_ResponseFunction: ResponseFunction
    {
        protected override float CalculateResponseInternal(float x)
        {
            return x;
        }


        public void InvokeParametersChanged()
        {
            onParametersChanged.OnNext(true);
        }
    }
}