using System;

namespace Agate.Starcade.Runtime.Data
{
    [Serializable]
    public class MailboxDataItem
    {
        public string MailboxId;
        public MailDataHeader Header;
        public MailDataContent Content;
        public RewardBase[] Rewards;
        public MailDataStatus StatusData;
    }
}