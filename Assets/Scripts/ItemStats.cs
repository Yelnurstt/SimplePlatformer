using UnityEngine;

// Values of pickups and hazards in the level
[CreateAssetMenu(fileName = "Item_Stats", menuName = "Game/Item Stats")]
public class ItemStats : ScriptableObject
{
    public int heartHeal = 1;     // HP from one heart
    public int spikeDamage = 1;   // damage from spikes (tag Hazard)
}
