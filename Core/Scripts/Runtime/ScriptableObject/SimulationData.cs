using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Starcade Scriptable Object/Simulation Data")]
public class SimulationData : ScriptableObject
{    
    public float ballPrice;
    public float jackpotRequirement;
    
    public float totalReturn;
    public float totalCost;
    public float RTP;
    public int starcadeCount;
    public int jackpotCount;

    
    public List<int> ballDropPoint;
    public List<float> goalEnteredBall;
    public List<float> probability;
    public List<float> totalRewardPerGoal;

}