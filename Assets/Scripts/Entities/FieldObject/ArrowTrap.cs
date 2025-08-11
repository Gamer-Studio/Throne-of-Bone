using System.Collections;
using ToB.Entities.Projectiles;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Entities.FieldObject
{
    public class ArrowTrap : FieldObjectProgress
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        
        [SerializeField] private AssetReference arrowPrefab;
        [SerializeField] private GameObject TrapSprite;
        public bool IsDetected;
        private bool IsShootingCooldown;
        [SerializeField] Direction _Direction;
        private Vector2 arrowDirection;
        [SerializeField] private float ShootDelayTime;
        [SerializeField] private float ShootCoolTime;
        [SerializeField] private float ArrowSpeed;
        [SerializeField] private float ArrowDamage;
        [SerializeField] private float ArrowKnockBack;
        private Coroutine shootingCo;
        private ObjectAudioPlayer audioPlayer;
        public override void OnLoad()
        {
            DetermineDirection();
            if (audioPlayer == null) audioPlayer = GetComponent<ObjectAudioPlayer>();
        }
        
        private void Update()
        {
            // IsDetected는 감지 콜라이더가 관리
            if (IsDetected && !IsShootingCooldown)
            {
                IsShootingCooldown = true;
                
                StartCoroutine(Shoot(arrowDirection));
            }
        }

        private IEnumerator Shoot(Vector2 _direction)
        {
            //딜레이 시간 동안 대기한 뒤 발사
            yield return new WaitForSeconds(ShootDelayTime);
            audioPlayer.Play("Arrow_Trap");
            var eff = arrowPrefab.Pooling().GetComponent<Arrow>();
            eff.transform.position = transform.position;
            eff.Direction = arrowDirection;
            eff.damage = ArrowDamage;
            eff.knockBackForce = ArrowKnockBack;
            eff.speed = ArrowSpeed;
            eff.Team = Team.Enemy;
            
            eff.ClearEffect();
            
            //쿨타임 동안 기다렸다가 bool 전환
            yield return new WaitForSeconds(ShootCoolTime);
            IsShootingCooldown = false;
        }

        private void DetermineDirection()
        {
            switch (_Direction)
            {
                case Direction.Up:
                    arrowDirection = Vector2.up;
                    TrapSprite.transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case Direction.Down:
                    arrowDirection = Vector2.down;
                    TrapSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Left:
                    arrowDirection = Vector2.left;
                    TrapSprite.transform.rotation = Quaternion.Euler(0, 0, -90);
                    break;
                case Direction.Right:
                    arrowDirection = Vector2.right;
                    TrapSprite.transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
            }
        }




    }
}