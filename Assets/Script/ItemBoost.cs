using UnityEngine;

[CreateAssetMenu(fileName = "ItemBoost", menuName = "Scriptable Objects/ItemBoost")]
public class ItemBoost : Item
{
    public override void Activation(PlayerItemManager player) //Item qui permet de Boost
    {
        player.carControler.Boost();
    }
}
