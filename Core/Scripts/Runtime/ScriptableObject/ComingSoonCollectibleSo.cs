using UnityEngine;


namespace Starcade.Core.Runtime.ScriptableObject
{
    /// <summary>
    /// A placeholder for "Coming Soon" Pin and Album, Each SO is representing one Pin Album
    /// <para>
    /// <list type="number">
    ///  <listheader>
    ///     <term> How To Use </term>
    ///  </listheader>
    ///      <item> Create this SO in the asset menu </item>
    ///      <item> Put this SO into Collectibles Scene </item>
    ///      <item> Add or Remove another dummySO as many as you want </item>
    /// </list>
    /// </para>
    /// </summary>
    
    [CreateAssetMenu(menuName = "Starcade/Core/Collectible/Coming Soon Collectible")]
    public class ComingSoonCollectibleSo : UnityEngine.ScriptableObject
    {
        public string SetId;
    }
}
