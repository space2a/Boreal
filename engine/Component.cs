using boreal.engine.graphics;

namespace boreal.engine
{
    public abstract class Component
    {
        public GameObject gameObject { get; internal set; }
        public Transform transform
        {
            get
            {
                if (gameObject != null)
                    return gameObject.transform;
                else return null;
            }
        }

        internal virtual void PreStart()
        {
            Start();
        }

        protected virtual void Start()
        {

        }

        internal void PreUpdate(GameTime gameTime)
        {
            Update(gameTime);
        }

        protected virtual void Update(GameTime gameTime)
        {

        }

        internal virtual void PreDraw(Drawer spritesBatch)
        {
            Draw(spritesBatch);
            Draw(spritesBatch.essentialDrawer);
        }

        internal virtual void Draw(Drawer spritesBatch)
        {

        }

        protected virtual void Draw(EssentialDrawer essentialDrawer)
        {

        }

        internal virtual void PreDrawUI(Drawer spritesBatch)
        {
            DrawUI(spritesBatch);
            DrawUI(spritesBatch.essentialDrawer);
        }

        internal virtual void DrawUI(Drawer spritesBatch)
        {

        }

        protected virtual void DrawUI(EssentialDrawer essentialDrawer)
        {

        }

        internal virtual void EndDrawUI(Drawer spritesBatch)
        {

        }

        internal virtual void DestroyComponent()
        {

        }

        public void Destroy()
        {
            DestroyComponent();
        }

    }


    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class UniqueComponent : System.Attribute { }
}
