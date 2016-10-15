using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public abstract class AI : GameObjectComponent {
        private GameObject _player;
        public GameObject Player {
            get {
                if (_player == null) {
                    _player = FindObjectsByTag("Player")[0];
                }
                return _player;
            }
            set {
                _player = value;
            }
        }

        private Map _map;
        protected Map Map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
            set {
                _map = value;
            }
        }

        public Vector3 steeringForce { get; set; } = Vector3.Zero;

        public StateMachine stateMachine { get; set; }
        public Dictionary<string, State> stateMap { get; set; } = new Dictionary<string, State>();
        public Dictionary<string, Func<bool>> conditionMap { get; set; } = new Dictionary<string, Func<bool>>();
        public string currentState { get; set; }

        // casting objects
        public _3DGameObject AIObject { get { return (_3DGameObject)baseObject; } }
        public OnlandVehicle AILandVehicle { get { return (OnlandVehicle)baseObject; } }
        public Tank AITank { get { return (Tank)baseObject; } }

        public override void Update(GameTime gameTime) {

            foreach (KeyValuePair<string, string> transition in stateMachine.transitionMap[currentState]) {
                bool conditionMeet = conditionMap[transition.Key]();
                if (conditionMeet) {
                    stateMap[currentState].OnExit();
                    currentState = transition.Value;
                    stateMap[currentState].OnEnter();
                    break;
                }
            }

            foreach (KeyValuePair<string, string> subState in stateMachine.subStateMap[currentState]) {
                bool conditionMeet = conditionMap[subState.Key]();
                if (conditionMeet) {
                    stateMap[subState.Value].Update(gameTime);
                } else {
                    stateMap[subState.Value].PassiveUpdate(gameTime);
                }
            }

            foreach (string stateName in stateMap.Keys) {
                if (stateName.Equals(currentState)) {
                    stateMap[stateName].Update(gameTime);
                } else {
                    stateMap[stateName].PassiveUpdate(gameTime);
                }
            }

            base.Update(gameTime);
        }

        private BasicEffect _basiscEffect;
        public BasicEffect basicEffect {
            get {
                if (_basiscEffect == null) {
                    _basiscEffect = Game.Services.GetService<BasicEffect>();
                }
                return _basiscEffect;
            }
        }

        private Camera _camera;
        public Camera camera {
            get {
                if (_camera == null) {
                    _camera = FindObjects<Camera>()[0];
                }
                return _camera;
            }
        }

        public override void Draw(GameTime gameTime) {
            if (!GameObject.isSteeringForceDisplay) {
                return;
            }

            Vector3 yOffset = new Vector3(0, 20, 0);
            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0].Position = baseObject.transformation.Position + yOffset;
            vertices[0].Color = Color.Red;
            vertices[1].Position = baseObject.transformation.Position + Vector3.Normalize(steeringForce) * 50 + yOffset;
            vertices[1].Color = Color.Red;

            int[] indices = new int[2] { 0, 1 };
            
            basicEffect.EnableDefaultLighting();
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.FogEnabled = false;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 2, indices, 0, 1);
            }

            base.Draw(gameTime);
        }
    }
}
