using System;


using Microsoft.Xna.Framework;

namespace boreal.engine
{
    [UniqueComponent]
    public class Camera : Component
    {
        public readonly static float MinZ = 1f;
        public readonly static float MaxZ = 2048f;

        public Canvas canvas;

        public boreal.engine.Vector3 rotation = new boreal.engine.Vector3(0f, 0f, 0f);
        public Vector3 position { get { return _position; } set { _position = value; CreateBoundingRectangle(); } }

        private Vector3 _position;
        internal float baseZ;

        private float aspectRatio;
        private float fieldOfView;

        internal Matrix view, proj;

        private float _zoom = 1;
        public float zoom { get { return _zoom; } set { _zoom = value; position.Z = baseZ / _zoom; CreateBoundingRectangle(); } }
        public float minZoom = 0.55f;
        public float maxZoom = 20;

        public Rectangle boundingRectangle { get; private set; }
        internal Microsoft.Xna.Framework.Rectangle XNABoundingRectangle { get; private set; }

        public Camera()
        {
            _position = new Vector3(0, 0, 0);

            aspectRatio = (float)Launcher.core.windowProfile.renderedResolution.width / Launcher.core.windowProfile.renderedResolution.height;
            fieldOfView = MathHelper.PiOver2;

            baseZ = GetZFromHeight(Launcher.core.windowProfile.renderedResolution.height);
            position.Z = baseZ;

            UpdateMatrices();
        }
        
        internal void UpdateMatrices()
        {
            view = Matrix.CreateLookAt(new Microsoft.Xna.Framework.Vector3(_position.X, _position.Y, _position.Z), new Microsoft.Xna.Framework.Vector3(_position.X, _position.Y, 0) + rotation.xnaV3, Microsoft.Xna.Framework.Vector3.Up);
            proj = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, MinZ, MaxZ);
        }

        private float GetZFromHeight(float height)
        {
            return 0.5f * height / MathF.Tan(0.5f * fieldOfView);
        }

        public float GetHeightFromZ()
        {
            return position.Z * MathF.Tan(0.5f * fieldOfView) * 2f;
        }

        private void MoveZ(float amount)
        {
            position.Z += amount;
            //clamp z between minz and maxz
        }

        public void ResetZ() { position.Z = baseZ; }

        public void Move(Vector3 amout)
        {
            position += amout;
        }

        public void MoveTo(Vector2 position)
        {
            this.position = new Vector3(position.X, position.Y, 0);
        }

        public void MoveTo(Vector3 position)
        {
            this.position = position;
        }

        public void ZoomIn()
        {
            zoom = MathU.Clamp(zoom += 0.025f, minZoom, maxZoom);
        }

        public void ZoomOut()
        {
            zoom = MathU.Clamp(zoom -= 0.025f, minZoom, maxZoom);
        }

        public void SetZoom(float zoom)
        {
            this.zoom = zoom;
        }

        public void GetExtents(out float width, out float height)
        {
            height = GetHeightFromZ();
            width = height * aspectRatio;
        }

        public void GetExtents(out float left, out float right, out float bottom, out float top)
        {
            GetExtents(out float width, out float height);
            left = _position.X - width * 0.5f;
            right = left + width;
            bottom = _position.Y - height * 0.5f;
            top = bottom + height;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            GetExtents(out float left, out float right, out float bottom, out float top);
            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }

        public void CreateBoundingRectangle()
        {
            GetExtents(out float width, out float height);

            boundingRectangle = new engine.Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), (int)width, (int)height);
            XNABoundingRectangle = boundingRectangle.rect;
        }

        public void FollowTarget(Vector2 target, float smoothTime, float ellapsedTime)
        {
            float interpolation = smoothTime * ellapsedTime;

            position.X = MathHelper.Lerp(position.X, target.X, interpolation);
            position.Y = MathHelper.Lerp(position.Y, target.Y, interpolation);
        }

        internal void DrawCanvas(Sprites spriteBatch)
        {
            if (canvas == null) return;
            //canvas?.CallDraw(spriteBatch);
            canvas?.CallDrawUI(spriteBatch);
        }

    }
}
