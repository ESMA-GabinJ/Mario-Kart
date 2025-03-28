using System.Collections;
using TMPro;
using UnityEngine;

public class ItemBox : MonoBehaviour
{

    [SerializeField]
    private MeshRenderer _meshRenderer, _text;
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private float _waitBeforeRespawn = 1;
    private void OnTriggerEnter(Collider other)
    {
        PlayerItemManager playerItemManagerInContact = other.GetComponent<PlayerItemManager>(); //donne un item au joueur lorsqu'il le touche
        if(playerItemManagerInContact != null)
        {
            playerItemManagerInContact.GenerateItem();
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn() //réaparé apres temps de seconde
    {
        _collider.enabled = false;
        _text.enabled = false;
        _meshRenderer.enabled = false;
        yield return new WaitForSeconds(_waitBeforeRespawn);
        _collider.enabled = true;
        _text.enabled = true;
        _meshRenderer.enabled = true;

    }
}
