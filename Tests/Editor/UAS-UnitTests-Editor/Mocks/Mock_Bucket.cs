using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks
{
    public class Mock_Bucket : Bucket
    {
        public float ReturnValue { get; private set; }

        public Mock_Bucket(float returnValue = 0f)
        {
            ReturnValue = returnValue;
            Utility = returnValue;
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