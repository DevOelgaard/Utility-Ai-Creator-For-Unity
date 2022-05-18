using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.PlayerLoop;

namespace Mocks
{
    public class Mock_AgentAction : AgentAction
    {
        public Mock_AgentAction()
        {
        }

        protected override List<Parameter> GetParameters()
        {
            return new List<Parameter>();
        }
    }
}