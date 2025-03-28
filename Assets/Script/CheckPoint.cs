using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool isFinishLine;

    private void OnTriggerEnter(Collider other) //quand le joueur le touche = active checkpoint
    {
        var otherLapManager = other.GetComponent<LapManager>();
        if(otherLapManager != null )
        {
            otherLapManager.AddCheckPoint( this );
        }
    }
}
