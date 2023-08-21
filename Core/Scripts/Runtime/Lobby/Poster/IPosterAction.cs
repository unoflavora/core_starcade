using System;
using System.Threading.Tasks;
using Agate.Starcade.Scripts.Runtime.Data_Class;

namespace Agate.Starcade.Core.Runtime.Lobby
{
    public interface IPosterAction
    {
        public Task OnClickAction(PosterData posterDataData, Action clickAction);
        public void OnClaimAction();
    }
}