using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemLightning", menuName = "Scriptable Objects/ItemLightning")]
public class ItemLightning : Item
{
    private GameObject _player;

    public override void Activation(PlayerItemManager player) // Item qui réduit la taille du joueur adverse
    {
        string targetPlayerName = player.gameObject.name == "P1" ? "P2" : "P1";
        GameObject targetPlayer = GameObject.Find(targetPlayerName);

        if (targetPlayer != null)
        {
            // Réduit la taille du joueur
            targetPlayer.transform.localScale *= 0.5f;

            // Divise la vitesse du joueur par deux
            CarControler carController = targetPlayer.GetComponent<CarControler>();
            if (carController != null)
            {
                carController.SetSpeedModifier(0.5f); // Divise la vitesse par 2
            }

            // Lance une coroutine pour restaurer la vitesse après 3 secondes
            targetPlayer.GetComponent<MonoBehaviour>().StartCoroutine(RevertSpeed(targetPlayer, carController));
        }
    }

    private IEnumerator RevertSpeed(GameObject targetPlayer, CarControler carController)
    {
        // Attend 3 secondes avant de remettre la vitesse à la normale
        yield return new WaitForSeconds(3f);

        // Restaure la vitesse à la normale
        if (carController != null)
        {
            carController.SetSpeedModifier(1f); // Restaure la vitesse normale
        }
    }
}
