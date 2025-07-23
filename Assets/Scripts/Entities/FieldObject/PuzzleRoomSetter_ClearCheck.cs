using System;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class PuzzleRoomSetter_ClearCheck : MonoBehaviour
    {
        private PuzzleRoomSetter puzzleRoomSetter;
        private SpriteRenderer spriteRenderer;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if ( puzzleRoomSetter == null) puzzleRoomSetter = GetComponentInParent<PuzzleRoomSetter>();
            spriteRenderer.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !puzzleRoomSetter.IsCleared)
                puzzleRoomSetter.ClearRoom();
        }
    }
}