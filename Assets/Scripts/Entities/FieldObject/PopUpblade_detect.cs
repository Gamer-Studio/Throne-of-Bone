using System;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class PopUpBlade_detect : MonoBehaviour
    {
        [SerializeField] private PopUpBlade popUpBlade;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && popUpBlade.GetState() == PopUpBlade.PopUpBladeState.Detect)
            {
               popUpBlade.ActivateBlade();
               popUpBlade.audioPlayer.Play("Knife_Trap");
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && popUpBlade.GetState() == PopUpBlade.PopUpBladeState.Detect)
            {
                popUpBlade.DeActivateBlade();
            }
        }
    }
}
