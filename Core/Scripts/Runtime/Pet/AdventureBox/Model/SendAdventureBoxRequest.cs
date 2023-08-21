namespace Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model
{
	public class SendAdventureBoxRequest
    {
        public string FriendCode { get; set; }
        public string BoxId { get; set; }

        public SendAdventureBoxRequest(string friendCode, string boxId)
        {
            FriendCode = friendCode;
            BoxId = boxId;
        }
    }

    public class SendAdventureBoxResponse
    {
        public string FriendCode { get; set; }
        public string BoxId { get; set; }
        public SendAdventureBoxResponse(string friendCode, string boxId)
        {
            FriendCode = friendCode;
            BoxId = boxId;
        }
    }
}