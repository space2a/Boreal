using System;
using System.Collections.Generic;

using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace boreal.engine
{
    public class Collider : Component
    {
        private IShapeF _collisionShape = null;
        public Rigidbody2D rigidbody2D { get; internal set; }

        internal IShapeF collisionShape
        {
            get { return _collisionShape; }
            set { _collisionShape = value; colliderActor.Bounds = value; }
        }

        protected Point2 position
        {
            get { return new Point2(collisionShape.Position.X, collisionShape.Position.Y); }
            set { collisionShape.Position = new MonoGame.Extended.Point2(value.X, value.Y); }
        }

        internal ColliderActor colliderActor = new ColliderActor();

        public delegate void OnCollision(CollidingInput collidingInput);
        public event OnCollision OnCollisionEnter;
        public event OnCollision OnCollisionStay;
        public event OnCollision OnCollisionExit;

        public List<CollidingInfo> collidingWith = new List<CollidingInfo>();

        public bool isTrigger = false;

        protected override void Start()
        {
            if (rigidbody2D == null) rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            colliderActor.collider = this;

            colliderActor.OnCollisionEvent += ColliderActor_OnCollisionEvent;

            if (collisionShape == null) RequestDefaultCollisionShape();
            Launcher.core._collisionComponent.Insert(colliderActor);
        }

        private void ColliderActor_OnCollisionEvent(object sender, EventArgs e)
        {
            var cinfo = (e as CollisionEventArgs);
            var otherCollider = (cinfo.Other as ColliderActor).collider;

            CollidingInput collidingInput = new CollidingInput()
            {
                inputGameObject = otherCollider.gameObject,
                inputCollider = otherCollider,
                thisCollider = this,
                force = new engine.Vector2(cinfo.PenetrationVector)
            };

            rigidbody2D?.OnAnyCollision(collidingInput);
            int index = collidingWith.FindIndex(x => x.collidingWith == otherCollider);
            if (index != -1) //in the list...
            {
                OnCollisionStay?.Invoke(collidingInput);
                gameObject.OnCollisionStay(collidingInput);
                collidingWith[index].DateTime = DateTime.Now;
            }
            else //not in the list...
            {
                collidingWith.Add(new CollidingInfo() { collidingWith = otherCollider, DateTime = DateTime.Now });
                OnCollisionEnter?.Invoke(collidingInput);
                gameObject.OnCollisionEnter(collidingInput);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            
        }

        protected void CheckCollisions()
        {
            var now = DateTime.Now;
            for (int i = 0; i < collidingWith.Count; i++)
            {
                if ((now - collidingWith[i].DateTime).TotalMilliseconds > 5)
                {
                    CollidingInput collidingInput = new CollidingInput()
                    {
                        inputGameObject = collidingWith[i].collidingWith.gameObject,
                        inputCollider = collidingWith[i].collidingWith,
                        thisCollider = this,
                        force = Vector2.Zero
                    };

                    OnCollisionExit?.Invoke(collidingInput);
                    gameObject.OnCollisionExit(collidingInput);
                    collidingWith.RemoveAt(i);
                }
            }
        }

        public virtual void RequestDefaultCollisionShape() 
        {
        }

        internal override void Draw(Drawer spritesBatch)
        {
            spritesBatch.shapes.DrawRectangleOutline(new Microsoft.Xna.Framework.Rectangle(
                (int)(colliderActor.Bounds.Position.Y), (int)(colliderActor.Bounds.Position.Y), 10, 10)
                , Color.Red, 5);
        }
    }

    public class ColliderActor : ICollisionActor
    {
        public IShapeF Bounds { get; set; }
        public event EventHandler OnCollisionEvent;
        public Collider collider;

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            OnCollisionEvent?.Invoke(this, collisionInfo);
        }
    }

    public class BoxCollider : Collider
    {
        private engine.Size2 _collisionSize;
        public engine.Size2 collisionSize
        {
            get { return _collisionSize; }
            set
            {
                _collisionSize = value;
                collisionShape = new RectangleF(new MonoGame.Extended.Point2(transform.position.X, transform.position.Y),
                    new MonoGame.Extended.Size2(value.width, value.height));
            }
        }

        public BoxCollider() { }
        public BoxCollider(Rigidbody2D rigidbody2D)
        {
            this.rigidbody2D = rigidbody2D;
        }

        protected override void Update(GameTime gameTime)
        {
            colliderActor.Bounds.Position = new MonoGame.Extended.Point2((transform.position.X - (_collisionSize.width / 2)),
                (int)(transform.position.Y - (_collisionSize.height / 2)));

            //colliderActor.Bounds.Position = new MonoGame.Extended.Point2((transform.position.X),
            //(int)(transform.position.Y));

            CheckCollisions();
        }

        public override void RequestDefaultCollisionShape()
        {
            collisionSize = new Size2(transform.scale);
            collisionShape = new RectangleF(new MonoGame.Extended.Point2(transform.position.X, transform.position.Y), new MonoGame.Extended.Size2(collisionSize.width, collisionSize.height));
        }

        internal override void Draw(Drawer spritesBatch)
        {
            spritesBatch.CreateDrawAction(() => spritesBatch.sprites.DrawRectangle((RectangleF)colliderActor.Bounds, Microsoft.Xna.Framework.Color.Red, 0.1f), -100);
        }

        internal override void DestroyComponent()
        {
            colliderActor.Bounds.Position = new MonoGame.Extended.Point2(-100000, 0);
        }
    }

    public class CircleCollider : Collider
    {
        private float _collisionSize = 1;
        public float collisionSize
        {
            get { return _collisionSize; }
            set
            {
                _collisionSize = value;
                if(transform != null)
                    collisionShape = new CircleF(new MonoGame.Extended.Point2(transform.position.X, transform.position.Y), collisionSize);
            }
        }
        public CircleCollider() { }
        public CircleCollider(Rigidbody2D rigidbody2D)
        {
            this.rigidbody2D = rigidbody2D;
        }

        protected override void Update(GameTime gameTime)
        {
            colliderActor.Bounds.Position =
                new MonoGame.Extended.Point2(transform.position.X, transform.position.Y);

            CheckCollisions();
        }

        public override void RequestDefaultCollisionShape()
        {
            collisionSize = _collisionSize;
            collisionShape = new CircleF(
                new MonoGame.Extended.Point2(transform.position.X, transform.position.Y)
                , 100);
        }

        internal override void Draw(Drawer spritesBatch)
        {
            spritesBatch.shapes.DrawCircleOutline((CircleF)collisionShape, Color.Green, 3);
        }

        internal override void DestroyComponent()
        {
            colliderActor.Bounds.Position = new MonoGame.Extended.Point2(-100000, 0);
        }

    }

    public class CollidingInfo
    {
        public Collider collidingWith;
        public DateTime DateTime;
    }

    public class CollidingInput
    {
        public GameObject inputGameObject;
        public Collider inputCollider;
        public Collider thisCollider;
        public Vector2 force;
    }


}
