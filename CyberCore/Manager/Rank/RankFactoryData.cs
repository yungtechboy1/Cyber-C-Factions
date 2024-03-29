﻿using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Crmf;

namespace CyberCore.Manager.Rank
{
    public class RankFactoryData
    {
        public Dictionary<int, Rank2> ranks = new Dictionary<int, Rank2>();

        public RankFactoryData(RankFactory r = null)
        {
            if(r!= null)ranks = r.ranks;
        }
    }
}