using UnityEngine;

[CreateAssetMenu(fileName = "ItemLightning", menuName = "Scriptable Objects/ItemLightning")]
public class ItemLightning : Item
{
    private GameObject _player;

    public override void Activation(PlayerItemManager player) //Item qui réduit la taille
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _player.transform.localScale = _player.transform.localScale * 0.5f;
    }
}
