using System;
using UnityEngine;
using UnityEngine.Events;

namespace Agate.Starcade.Scripts.Runtime.Data
{
    [Serializable]
    public class UserExperienceData 
    {
        [HideInInspector]public bool MilestoneInit;
        public ExperienceData Data;
        public NextExperienceRewardData NextMilestone;
        public NextExperienceRewardData NextLevel;

        [HideInInspector]public UnityEvent OnExperienceChanged;
        [HideInInspector]public UnityEvent<int> OnLevelUpChanged;
        [HideInInspector]public UnityEvent OnMilestoneReached;
    }
}
