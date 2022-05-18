using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks
{
    public class Mock_ConsiderationSimple : Consideration
    {
        public float ReturnValue = 0f;

        public Mock_ConsiderationSimple()
        {
        }

        public void SetIsModifier(bool isModifier)
        {
            IsModifier = isModifier;
        }

        public void SetIsScorer(bool isScorer)
        {
            IsScorer = isScorer;
        }

        protected override float CalculateBaseScore(IAiContext context)
        {
            return ReturnValue;
        }

        public override float CalculateScore(IAiContext context)
        {
            return ReturnValue;
        }

        protected override List<Parameter> GetParameters()
        {
            return new List<Parameter>();
        }
    }
}