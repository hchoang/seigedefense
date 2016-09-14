namespace SiegeDefense {
    public class _3DGameObject : GameObject {
        public virtual Renderer renderer { get; protected set; }
        public virtual Collider collider { get; protected set; }
        public virtual GamePhysics physics { get; protected set; }

        private Camera _mainCamera;
        protected Camera mainCamera {
            get {
                if (_mainCamera == null) {
                    _mainCamera = FindObjects<Camera>()[0];
                }
                return _mainCamera;
            }
        }

        private Map _map;
        protected Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        private Skybox _sky;
        protected Skybox sky {
            get {
                if (_sky == null) {
                    _sky = FindObjects<Skybox>()[0];
                }
                return _sky;
            }
        }
    }
}
