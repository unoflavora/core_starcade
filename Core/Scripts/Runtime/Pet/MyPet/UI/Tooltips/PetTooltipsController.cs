using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade.Core.Runtime.Pet.MyPet.UI.Tooltips
{
    public class PetTooltipsController : MonoBehaviour
    {
        [SerializeField] private PetTooltip _tooltipL;

        [SerializeField] private PetTooltip _tooltipR;

        [SerializeField] List<string> _tooltipData;

        private int _currentTooltip = -1;
        
        public void Next()
        {
            IncrementIndex();

            ShowTooltip();
        }
        
        public void ResetTooltip()
        {
            _currentTooltip = -1;
            
            ShowTooltip();
        }

        private void ShowTooltip()
        {
            if (_currentTooltip == -1)
            {
                _tooltipL.Hide();
                _tooltipR.Hide();
                return;
            }
            
            if (_currentTooltip % 2 == 0)
            {
                _tooltipL.Show(_tooltipData[_currentTooltip]);
                _tooltipR.Hide();
            }
            else
            {
                _tooltipR.Show(_tooltipData[_currentTooltip]);
                _tooltipL.Hide();
            }
        }

        private void IncrementIndex()
        {
            _currentTooltip++;

            if (_currentTooltip >= _tooltipData.Count)
            {
                _currentTooltip = -1;
            }
        }
    }
}
