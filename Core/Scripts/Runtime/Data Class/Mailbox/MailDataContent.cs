using System;

namespace Agate.Starcade.Runtime.Data
{
    [Serializable]
    public class MailDataContent
    {
        public string Text;
        public string BannerId;
        public string BannerLink;
        public string CustomLink;
        public string CustomLinkCaption;
    }
}