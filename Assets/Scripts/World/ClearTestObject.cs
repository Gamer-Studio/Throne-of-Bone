using System;
using ToB.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToB
{
    public class ClearTestObject : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerCharacter>())
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
