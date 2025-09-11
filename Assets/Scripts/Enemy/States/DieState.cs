namespace Enemy
{
    public class DieState : IEnemyState
    {
        private EnemyController _controller;
        
        public DieState(EnemyController controller)
        {
            _controller = controller;
        }
        public void Enter()
        {
            _controller.IsChangeState = false;
        }

        public void Update()
        {
        }

        public void Exit()
        {
            
        }
    }
}