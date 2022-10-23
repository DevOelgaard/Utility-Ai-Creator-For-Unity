using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks
{
    public class Mock_Decision : Decision
    {
        public float ReturnValue { get; set; }


        public Mock_Decision(float returnValue = 0f)
        {
            ReturnValue = returnValue;
            Utility = returnValue;
            // var a = new Mock_AgentAction();
            // a.Name = returnValue.ToString();
            // AgentActions.Add(a);

            MetaData = new AiObjectMetaData();
        }

        protected override float CalculateUtility(IAiContext context)
        {
            return ReturnValue;
        }

        public void ForceUpdateInfo()
        {
            UpdateInfo();
        }
    }
}