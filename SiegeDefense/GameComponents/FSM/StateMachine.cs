using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SiegeDefense{
    public class StateMachine {
        public Dictionary<string, Dictionary<string, string>> transitionMap { get; set; } = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, Dictionary<string, string>> subStateMap { get; set; } = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, string> configurationMap { get; set; } = new Dictionary<string, string>();
        public string initState { get; set; }

        public static StateMachine ReadFromXML(string path) {
            XmlReader reader = XmlReader.Create(path);

            StateMachine sm = new StateMachine();

            while (reader.Read()) {

                if (reader.NodeType == XmlNodeType.Element) {
                    if (reader.Name.Equals("fsm")) {
                        sm.initState = reader.GetAttribute("startState");

                    } else if (reader.Name.Equals("state")) {

                        Dictionary<string, string> transitions = new Dictionary<string, string>();
                        Dictionary<string, string> subStates = new Dictionary<string, string>();
                        string fromState = reader.GetAttribute("fromState");
                        sm.transitionMap.Add(fromState, transitions);
                        sm.subStateMap.Add(fromState, subStates);

                        XmlReader stateReader = reader.ReadSubtree();

                        while (stateReader.Read()) {
                            if (stateReader.NodeType == XmlNodeType.Element) {

                                if (stateReader.Name.Equals("transition")) {
                                    transitions.Add(reader.GetAttribute("condition"), reader.GetAttribute("toState"));
                                } else if (stateReader.Name.Equals("subState")) {
                                    subStates.Add(reader.GetAttribute("condition"), reader.GetAttribute("name"));
                                }

                            }
                        }

                    } else if (reader.Name.Equals("configurationMap")) {

                        sm.configurationMap = new Dictionary<string, string>();

                        if (reader.ReadToDescendant("configuration")) {
                            do {
                                sm.configurationMap.Add(reader.GetAttribute("name"), reader.GetAttribute("value"));
                            } while (reader.ReadToNextSibling("configuration"));
                        }

                    }
                }
            }

            return sm;
        }
    }
}
