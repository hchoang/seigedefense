using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SiegeDefense {
    public class LevelDescription {
        public List<Vector3> SpawnPoints { get; set; } = new List<Vector3>();
        public float MapCellSize { get; set; }
        public float MapDeltaHeight { get; set; }
        public Vector3 PlayerStartPoint { get; set; }
        public Vector3 HeadquarterPosition { get; set; }
        public string TerrainName { get; set; }

        public static LevelDescription LoadFromXML(string path) {
            XmlSerializer s = new XmlSerializer(typeof(LevelDescription));
            using (StreamReader reader = new StreamReader(path)) {
                return (LevelDescription)s.Deserialize(reader);
            }
        }

        public void SaveToXML(string path) {
            XmlSerializer s = new XmlSerializer(typeof(LevelDescription));
            using (StreamWriter writer = new StreamWriter(path)) {
                s.Serialize(writer, this);
            }
        }
    }
}