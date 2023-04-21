using System;
using System.Collections.Generic;

namespace boreal.engine
{
    public class GameObject
    {
        public string name = "";
        public string tag = "";
            
        public bool isDisabled = false;
        public bool isRendering = true;

        public bool isInCanvas { get; internal set; }

        public readonly Transform transform = new Transform();

        private List<Component> components = new List<Component>();

        public List<GameObject> childrens { get; private set; }

        public GameObject parent { get; private set; }

        public Scene currentScene
        {
            get
            {
                return SceneManager.currentScene;
            }
        }

        public GameObject()
        {
            InitializeGameObject();
        }

        public GameObject(string name)
        {
            InitializeGameObject();
            this.name = name;
        }

        public GameObject(Component component)
        {
            InitializeGameObject();
            AddComponent(component);
        }

        public GameObject(List<Component> components)
        {
            InitializeGameObject();
            for (int i = 0; i < components.Count; i++)
                AddComponent(components[i]);
        }

        private void InitializeGameObject()
        {
            transform.gameObject = this;
            childrens = new List<GameObject>();
            GetNameByAttribute();
        }

        private void GetNameByAttribute()
        {
            GameObjectName goName =
            (GameObjectName)Attribute.GetCustomAttribute(this.GetType(), typeof(GameObjectName));
            if(goName != null) { this.name = goName.Name; }
        }

        public GameObject AddChildren(GameObject gameObject, bool callStart = true)
        {
            childrens.Add(gameObject);
            gameObject.parent = this;

            if(callStart)
                gameObject.Start();
            return gameObject;
        }

        public virtual void Start()
        {

        }

        internal void PreUpdate(GameTime gameTime)
        {
            if (isDisabled) return;

            for (int i = 0; i < components.Count; i++)
                components[i].PreUpdate(gameTime);

            for (int i = 0; i < childrens.Count; i++)
                childrens[i].PreUpdate(gameTime);

            Update(gameTime);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        internal void PreDestroy()
        {
            foreach (var c in components)
                c.Destroy();

            foreach (var child in childrens) { child.PreDestroy(); }

            if (parent == null)
                SceneManager.currentScene.gameObjects?.Remove(this);
            else
                parent.childrens.Remove(this);

            if (isDisabled) return;
            OnDestroy();
        }

        public virtual void OnDestroy()
        {

        }

        public void Destroy()
        {
            PreDestroy();
        }


        internal void Draw(Sprites spritesBatch)
        {
            if (!isRendering) return;

            for (int i = 0; i < childrens.Count; i++)
                childrens[i].Draw(spritesBatch);

            for (int i = 0; i < components.Count; i++)
                components[i].Draw(spritesBatch);
        }

        internal void DrawUI(Sprites spritesBatch)
        {
            if (!isRendering) return;

            for (int i = 0; i < components.Count; i++)
                components[i].PreDrawUI(spritesBatch);


            for (int i = 0; i < childrens.Count; i++)
                childrens[i].DrawUI(spritesBatch);


            for (int i = 0; i < components.Count; i++)
                components[i].EndDrawUI(spritesBatch);

        }

        public virtual void OnCollisionStay(CollidingInput collidingInput) { }
        public virtual void OnCollisionEnter(CollidingInput collidingInput) { }
        public virtual void OnCollisionExit(CollidingInput collidingInput) { }

        public void AddComponent(Component component)
        {
            if (GetComponent<Component>() != null && Attribute.IsDefined(component.GetType(), typeof(UniqueComponent)))
            {
                throw new Exception("Unable to add a second component of type " + component.GetType() + ", this component is marked at unique.");
            }

            component.gameObject = this;
            component.PreStart();
            components.Add(component);
        }

        public void AddComponents(Component first, Component second)
        {
            AddComponent(first);
            AddComponent(second);
        }

        public void AddComponents(Component first, Component second, Component third)
        {
            AddComponent(first);
            AddComponent(second);
            AddComponent(third);
        }

        public void AddComponents(Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                AddComponent(components[i]);
            }
        }

        public void AddComponent(Component component, out Component outComponent)
        {
            AddComponent(component);
            outComponent = component;
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)components.Find(x => x.GetType() == typeof(T));
        }

        public T[] GetComponents<T>() where T : Component
        {
            return (T[])components.FindAll(x => x.GetType() == typeof(T)).ToArray();
        }

        public Component[] GetComponents() 
        {
            return components.ToArray();
        }

        public bool RemoveComponent<Component>(bool destroy = false)
        {
            int index = components.FindIndex(x => x.GetType() == typeof(Component));
            if (index != -1)
            {
                if (destroy) { components[index].Destroy(); }
                components.RemoveAt(index);
                return true;
            }
            else return false;
        }

        public bool RemoveComponent(Component component, bool destroy = false)
        {
            int index = components.IndexOf(component);
            if (index != -1)
            {
                if(destroy) { components[index].Destroy(); }
                components.RemoveAt(index);
                return true;
            }
            else return false;
        }

        public void RemoveAllComponent(bool destroy = false)
        {
            if (destroy)
                for (int i = 0; i < components.Count; i++)
                    components[i].Destroy();

            components.Clear();
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class PersistentGameObject : System.Attribute { }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class GameObjectName : System.Attribute
    {
        public string Name;
        public GameObjectName(string name) { Name = name; }
    }
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class GameObjectChildren : System.Attribute
    {
        public GameObject gameObject;
        public GameObjectChildren(GameObject gameObject) { this.gameObject = gameObject; }
    }
}
