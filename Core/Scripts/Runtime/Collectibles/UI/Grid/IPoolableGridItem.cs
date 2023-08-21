using System;

namespace Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Grid
{
    /// <summary>
    /// Interface for object that is instantiated by "CollectibleSet"
    /// This should be implemented by any gameobject that is being put into UI by "Collectible Set" Layout
    /// </summary>
    /// <typeparam name="T">Data contained within the object;</typeparam>
    public interface IPoolableGridItem<T>
    {
        public void SetupData(T gridItem, int index, GridUIOptions uiOptions = null); // Set the data inside the slot
        public Action<T> OnItemClicked { get; set; } // On slot clicked, this method should be called
    }
    
    /// <summary>
    ///  Options for Grid UI, feel free to add something.
    /// </summary>
    public class GridUIOptions
    {
        public bool ShowItemCount = true;   // Show or hide item count UI
        
        public bool ShowQuestionMark = false;    // Show or hide question mark UI

        public bool ShowNewLabel = false;  // Show or hide new label UI
        
        public bool ShowItemName = true;   // Show or hide item name UI
        
        public GridUIOptions(){}
    }
    
}