using System;
using System.Collections.Generic;
using Agate.Starcade.Arcades.MT02.Scripts.Runtime.Backend.Data;
using Agate.Starcade.Scripts.Runtime.Data;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Backend.Data
{
    public class MonstamatchSpinData
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
        public bool isStarted { get; set; }
        public SpinSession SpinSession { get; set; }
        public PlayerBalance balance { get; set; }
        public Pay pay { get; set; }
        public ExperienceData experienceData { get; set; }
        public NextExperienceRewardData nextLevelUpReward { get; set; }

    }

}