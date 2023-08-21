using Agate.Starcade.Core.Runtime.Pet.AdventureBox.Model;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Starcade.Core.Runtime.Pet.AdventureBox.Model
{
	public class AdventureBoxInventory
    {
        public List<AdventureBoxData> Inventory { get; set; }

        public void AddAdventureBox(string adventureBoxId, int amount = 1)
        {
            var data = Inventory.FirstOrDefault( x => x.AdventureBoxId == adventureBoxId);
            data.Amount += amount;
        }

        public void RemoveAdventureBox(string adventureBoxId, int amount = 1)
        {
            var data = Inventory.FirstOrDefault(x => x.AdventureBoxId == adventureBoxId);
            data.Amount -= amount;
        }
    }
}