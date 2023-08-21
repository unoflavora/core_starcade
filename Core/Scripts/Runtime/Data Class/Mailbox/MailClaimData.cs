using System;

namespace Agate.Starcade.Runtime.Data
{
    [Serializable]
    public class MailClaimData
    {
        public string MailboxId;
        public RewardBase[] GrantedItems;
        public MailDataStatus StatusData;
        public string[] LootboxGachaResult;
    }
}