﻿using Sandbox.Game.Entities;
using System.Collections.Generic;
using System.Text;
using VRage.Collections;
using VRage.Groups;

namespace ALE_Biggest_Grids_Broadcast.GridDetection {
    internal class BiggestGridDetectionPcuStrategy : AbstractGridDetectionStrategy {

        public static readonly BiggestGridDetectionPcuStrategy INSTANCE = new BiggestGridDetectionPcuStrategy();

        private BiggestGridDetectionPcuStrategy() {

        }

        public override string GetUnitName() {
            return "PCU";
        }

        public override List<KeyValuePair<long, List<MyCubeGrid>>> FindGrids(GridsBroadcastConfig config, bool connected) {

            List<KeyValuePair<long, List<MyCubeGrid>>> gridsList = new List<KeyValuePair<long, List<MyCubeGrid>>>();

            if (connected) {

                foreach (var group in MyCubeGridGroups.Static.Physical.Groups) {

                    var grids = CheckGroupsPcu(group.Nodes, config);

                    if(grids.Value.Count > 0)
                        gridsList.Add(grids);
                }

            } else {

                foreach (var group in MyCubeGridGroups.Static.Mechanical.Groups) {

                    var grids = CheckGroupsPcu(group.Nodes, config);

                    if (grids.Value.Count > 0)
                        gridsList.Add(grids);
                }
            }

            gridsList.Sort(delegate (KeyValuePair<long, List<MyCubeGrid>> pair1, KeyValuePair<long, List<MyCubeGrid>> pair2) {
                return pair2.Key.CompareTo(pair1.Key);
            });

            return gridsList;
        }

        private KeyValuePair<long, List<MyCubeGrid>> CheckGroupsPcu(HashSetReader<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Node> nodes, GridsBroadcastConfig config) {

            List<MyCubeGrid> gridsList = new List<MyCubeGrid>();
            long pcu = 0;

            foreach (var groupNodes in nodes) {

                MyCubeGrid cubeGrid = groupNodes.NodeData;

                if (cubeGrid.Physics == null)
                    continue;

                if (!IsGridInsideFilter(cubeGrid, config))
                    continue;

                gridsList.Add(cubeGrid);

                pcu += cubeGrid.BlocksPCU;
            }

            return new KeyValuePair<long, List<MyCubeGrid>>(pcu, gridsList);
        }

        private KeyValuePair<long, List<MyCubeGrid>> CheckGroupsPcu(HashSetReader<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Node> nodes, GridsBroadcastConfig config) {

            List<MyCubeGrid> gridsList = new List<MyCubeGrid>();
            long pcu = 0;

            foreach (var groupNodes in nodes) {

                MyCubeGrid cubeGrid = groupNodes.NodeData;

                if (cubeGrid.Physics == null)
                    continue;

                if (!IsGridInsideFilter(cubeGrid, config))
                    continue;

                gridsList.Add(cubeGrid);

                pcu += cubeGrid.BlocksPCU;
            }

            return new KeyValuePair<long, List<MyCubeGrid>>(pcu, gridsList);
        }

        public override void WriteSettings(StringBuilder sb, int top, int playerdistance, int min, bool filterOffline, bool ignoreNpcs, bool connected, GridsBroadcastConfig pluginConfig) {

            sb.AppendLine("[Top " + top + " grids by " + GetUnitName() + "]");
            sb.AppendLine();
            sb.AppendLine("Used Settings");
            sb.AppendLine("---------------------------------------");
            sb.AppendLine("Number of grids: " + top);
            sb.AppendLine("Player distance: " + playerdistance);
            sb.AppendLine("Min " + GetUnitName() + ": " + min);
            sb.AppendLine("Show Offline: " + !filterOffline);
            sb.AppendLine("Show NPCs: " + !ignoreNpcs);
            sb.AppendLine("Include: " + (!connected ? "Phyiscal Connections (Rotors, Pistons)" : "Mechanical Connections (Rotors, Pistons, Connectors)"));
        }
    }
}
