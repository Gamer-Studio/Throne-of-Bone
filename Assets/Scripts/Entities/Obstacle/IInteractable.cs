namespace ToB.Entities.Obstacle
{
    public interface IInteractable
    {
        /// <summary>
        /// 상호작용 시 호출
        /// </summary>
        public void Interact();

        /// <summary>
        /// 현재 상호작용 가능한 상태인지 여부
        /// </summary>
        public bool IsInteractable { get; set; }
        
    }
}