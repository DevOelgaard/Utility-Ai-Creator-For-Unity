using System.Collections.Generic;
using MoreLinq;
using NUnit.Framework;

namespace Mocks
{
    public class Mock_UtilityContainerSelector: UtilityContainerSelector
    {
        public Bucket BestBucket;
        public Decision BestDecision;
        public override Bucket GetBestUtilityContainer(List<Bucket> containers, IAiContext context)
        {
            return BestBucket ?? containers.MaxBy(c => c.Utility);
        }

        public override Decision GetBestUtilityContainer(List<Decision> containers, IAiContext context)
        {
            return BestDecision ?? containers.MaxBy(c => c.Utility);
        }

        public override string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        public override string GetName()
        {
            throw new System.NotImplementedException();
        }

    }
}