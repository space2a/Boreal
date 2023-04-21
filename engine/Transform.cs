namespace boreal.engine
{
    public class Transform
    {

        private Vector2 _position = new Vector2(0, 0); //object position
        public Vector2 position
        {
            get
            {
                if (gameObject.parent == null)
                    return _position;
                else
                    return gameObject.parent.transform.position + _position;
            }
            set { _position = value; }
        }

        public Vector2 _scale = new Vector2(1, 1);

        public Vector2 scale
        {
            get
            {
                if (gameObject.parent == null)
                    return _scale;
                else
                    return gameObject.parent.transform.scale * _scale;
            }
            set { _scale = value; }
        }

        private float _rotation = 0; //object rotation
        public float rotation
        {
            get
            {
                if (gameObject.parent == null)
                    return _rotation;
                else
                    return gameObject.parent.transform._rotation + _rotation;
            }
            set { _rotation = value; }
        }

        public GameObject gameObject;

        public void MoveToward(Transform transform, float speed)
        {
            MoveToward(transform.position, speed);
        }

        public void MoveToward(engine.Vector2 position, float speed)
        {
            engine.Vector2 direction = position - this.position;
            direction.Normalize();
            if (!direction.isNaN())
            {
                this.position += direction * speed;
                //this.rotation = (float)Math.Atan2(direction.Y, direction.X);
            }

        }

    }
}