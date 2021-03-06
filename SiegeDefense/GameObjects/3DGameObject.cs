﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class _3DGameObject : GameObject {
        public virtual _3DRenderer renderer { get; set; }
        public virtual Collider collider { get; set; }
        public virtual GamePhysics physics { get; set; }
        public virtual WireFrameBoxRenderer boundingBoxRenderer { get; set; }

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
        
        public Partition partition { get; set; }
        public void AddToGameWorld() {
            if (partition == null) {
                partition = Partition.RootPartition;
            }

            partition.AddObject(this);
        }

        public void RemoveFromGameWorld() {
            if (partition != null) {
                partition.RemoveObject(this);
            }
        }

        public virtual List<T> FindObjectsInPartition<T>() where T:_3DGameObject {
            if (this.partition == null) {
                return new List<T>();
            }
            return Partition.GetObjectsForCollisionDetection(this).Where(x => x is T).Cast<T>().ToList();
            //return partition.managedObjects.Where(x => x is T).Cast<T>().ToList();
        }

        public override void Update(GameTime gameTime) {
            if (boundingBoxRenderer != null) {
                boundingBoxRenderer.Visible = GameObject.isBoundingBoxDisplay;
            }

            base.Update(gameTime);
        }
    }
}
