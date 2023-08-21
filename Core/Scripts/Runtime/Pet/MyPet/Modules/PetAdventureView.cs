using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Pet.Core.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.Modules
{
    public class PetAdventureView : MonoBehaviour
    {
        [SerializeField] private PetAdventureRewardListView itemPrefab;
        [SerializeField] private VerticalLayoutGroup _basicReward;
        [SerializeField] private VerticalLayoutGroup _petSkillReward;

        private Stack<PetAdventureRewardListView> _adventureRewardListViews;
    
        public void SetupView(PetInventoryData pet, AdventureConfig adventureConfig)
        {
            if (_adventureRewardListViews == null) _adventureRewardListViews = new Stack<PetAdventureRewardListView>();
        
            List<PetAdventureRewardListView> usedItems = new List<PetAdventureRewardListView>();
        
            var basicSkillList = GetSkillItem(_petSkillReward, 0, usedItems);
            basicSkillList.transform.SetAsFirstSibling();
            basicSkillList.SetRewardFromBasicSkill(pet.BasicSkill);

            for (int i = 0; i < pet.SubSkills.Count; i++)
            {
                var subSkillList = GetSkillItem(_petSkillReward,i + 1, usedItems);
                subSkillList.SetRewardFromSubSkill(pet.SubSkills[i]);
            }

            for (int i = 0; i < adventureConfig.Rewards.Count; i++)
            {
                var basicRewardList = GetSkillItem(_basicReward, i, usedItems);
                basicRewardList.SetRewardFromBasicReward(adventureConfig.Rewards[i]);
            }
        
            foreach (var view in _adventureRewardListViews)
            {
                view.gameObject.SetActive(false);
            }

            foreach (var usedItem in usedItems)
            {
                _adventureRewardListViews.Push(usedItem);

            }
        }

        private PetAdventureRewardListView GetSkillItem(VerticalLayoutGroup parent, int index, List<PetAdventureRewardListView> usedItems)
        {
            PetAdventureRewardListView view;
            if (_adventureRewardListViews.Count > 0)
            {
                view = _adventureRewardListViews.Pop();
                view.transform.SetParent(parent.transform);
            }
            else
            {
                view = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, parent.transform);
            }

            view.transform.SetSiblingIndex(index);
            view.gameObject.SetActive(true);
            usedItems.Add(view);
            return view;
        }
    }
}
