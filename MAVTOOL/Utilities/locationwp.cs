using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MAVTOOL.Utilities
{
    /// <summary>
    /// Struct as used in Ardupilot
    /// </summary>
    public struct Locationwp
    {
        public Locationwp Set(double lat, double lng, double alt, ushort id)
        {
            this.lat = lat;
            this.lng = lng;
            this.alt = (float)alt;
            this.id = id;

            return this;
        }

        public static implicit operator MAVLink.mavlink_mission_item_t(Locationwp input)
        {
            return (MAVLink.mavlink_mission_item_t)Convert(input, false);
        }

        public static implicit operator MAVLink.mavlink_mission_item_int_t(Locationwp input)
        {
            return (MAVLink.mavlink_mission_item_int_t)Convert(input, true);
        }

        public static implicit operator Locationwp(MAVLink.mavlink_mission_item_t input)
        {
            Locationwp temp = new Locationwp()
            {
                id = input.command,
                p1 = input.param1,
                p2 = input.param2,
                p3 = input.param3,
                p4 = input.param4,
                lat = input.x,
                lng = input.y,
                alt = input.z,
                _seq = input.seq,
                _frame = input.frame
            };

            return temp;
        }

        public static implicit operator Locationwp(MAVLink.mavlink_mission_item_int_t input)
        {
            Locationwp temp = new Locationwp()
            {
                id = input.command,
                p1 = input.param1,
                p2 = input.param2,
                p3 = input.param3,
                p4 = input.param4,
                lat = input.x / 1.0e7,
                lng = input.y / 1.0e7,
                alt = input.z,
                _seq = input.seq,
                _frame = input.frame
            };

            return temp;
        }

        public static implicit operator Locationwp(MissionFile.Item input)
        {
            Locationwp temp = new Locationwp()
            {
                id = (ushort)input.command,
                p1 = (float)input.@params[0],
                p2 = (float)input.@params[1],
                p3 = (float)input.@params[2],
                p4 = (float)input.@params[3],
                lat = input.coordinate[0],
                lng = input.coordinate[1],
                alt = (float)input.coordinate[2],
                _seq = (ushort)input.doJumpId,
                _frame = (byte)input.frame
            };

            return temp;
        }

        public static implicit operator MissionFile.Item(Locationwp input)
        {
            MissionFile.Item temp = new MissionFile.Item()
            {
                command = input.id,
                @params = new List<double>(new double[] { input.p1, input.p2, input.p3, input.p4 }),
                coordinate = new List<double>(new double[] { input.lat, input.lng, input.alt }),
                doJumpId = input._seq,
                frame = input._frame
            };

            return temp;
        }

        static object Convert(Locationwp cmd, bool isint = false)
        {
            if (isint)
            {
                var temp = new MAVLink.mavlink_mission_item_int_t()
                {
                    command = cmd.id,
                    param1 = cmd.p1,
                    param2 = cmd.p2,
                    param3 = cmd.p3,
                    param4 = cmd.p4,
                    x = (int)(cmd.lat * 1.0e7),
                    y = (int)(cmd.lng * 1.0e7),
                    z = (float)cmd.alt,
                    seq = cmd._seq,
                    frame = cmd._frame
                };

                return temp;
            }
            else
            {
                var temp = new MAVLink.mavlink_mission_item_t()
                {
                    command = cmd.id,
                    param1 = cmd.p1,
                    param2 = cmd.p2,
                    param3 = cmd.p3,
                    param4 = cmd.p4,
                    x = (float)cmd.lat,
                    y = (float)cmd.lng,
                    z = (float)cmd.alt,
                    seq = cmd._seq,
                    frame = cmd._frame
                };

                return temp;
            }
        }

        private ushort _seq;
        private byte _frame;
        public object Tag;

        public ushort id;				// command id
        public byte options;
        public float p1;				// param 1
        public float p2;				// param 2
        public float p3;				// param 3
        public float p4;				// param 4
        public double lat;				// Lattitude * 10**7
        public double lng;				// Longitude * 10**7
        public float alt;				// Altitude in centimeters (meters * 100)
    };


    public class MissionFile
    {
        public static RootObject ReadFile(string filename)
        {
            var file = File.ReadAllText(filename);

            var output = JsonConvert.DeserializeObject<RootObject>(file);

            return output;
        }

        public static void WriteFile(string filename, RootObject format)
        {
            var fileout = JsonConvert.SerializeObject(format, Formatting.Indented);

            File.WriteAllText(filename, fileout);
        }

        public static List<Locationwp> ConvertToLocationwps(RootObject format)
        {
            List<Locationwp> cmds = new List<Locationwp>();

            cmds.Add(ConvertFromMissionItem(format.mission.plannedHomePosition));

            foreach (var missionItem in format.mission.items)
            {
                if (missionItem.type != "SimpleItem")
                {
                    if (missionItem.type == "ComplexItem")
                    {

                    }
                    continue;
                }
                cmds.Add(ConvertFromMissionItem(missionItem));
            }

            return cmds;
        }

        public static Locationwp ConvertFromMissionItem(List<double> missionItem)
        {
            return new Locationwp() { alt = (float)missionItem[2], lat = missionItem[0], lng = missionItem[1] };
        }

        public static Locationwp ConvertFromMissionItem(Item missionItem)
        {
            return missionItem;
        }

        public static Item ConvertFromLocationwp(Locationwp locationwp)
        {
            return locationwp;
        }

        //http://json2csharp.com/#
        public class GeoFence
        {
            public List<double> breachReturn { get; set; }
            public List<List<double>> polygon { get; set; }
            public int version { get; set; }
        }

        public class Camera
        {
            public int focalLength { get; set; }
            public int groundResolution { get; set; }
            public int imageFrontalOverlap { get; set; }
            public int imageSideOverlap { get; set; }
            public string name { get; set; }
            public bool orientationLandscape { get; set; }
            public int resolutionHeight { get; set; }
            public int resolutionWidth { get; set; }
            public double sensorHeight { get; set; }
            public double sensorWidth { get; set; }
        }

        public class Grid
        {
            public double altitude { get; set; }
            public int angle { get; set; }
            public bool relativeAltitude { get; set; }
            public double spacing { get; set; }
            public int turnAroundDistance { get; set; }
        }

        public class Item
        {
            public bool autoContinue { get; set; }
            public int command { get; set; }
            public List<double> coordinate { get; set; }
            public int doJumpId { get; set; }
            public int frame { get; set; }
            public List<double> @params { get; set; }
            public string type { get; set; }
            public Camera camera { get; set; }
            public int? cameraTriggerDistance { get; set; }
            public string complexItemType { get; set; }
            public bool? fixedValueIsAltitude { get; set; }
            public Grid grid { get; set; }
            public bool? hoverAndCapture { get; set; }
            public bool? manualGrid { get; set; }
            public List<List<double?>> polygon { get; set; }
            public bool? refly90Degrees { get; set; }
            public int? version { get; set; }
        }

        public class Mission
        {
            public int cruiseSpeed { get; set; }
            public int firmwareType { get; set; }
            public int hoverSpeed { get; set; }
            public List<Item> items { get; set; }
            public List<double> plannedHomePosition { get; set; }
            public int vehicleType { get; set; }
            public int version { get; set; }
        }

        public class RallyPoints
        {
            public List<List<double>> points { get; set; }
            public int version { get; set; }
        }

        public class RootObject
        {
            public string fileType { get; set; }
            public GeoFence geoFence { get; set; }
            public string groundStation { get; set; }
            public Mission mission { get; set; }
            public RallyPoints rallyPoints { get; set; }
            public int version { get; set; }
        }

        public static RootObject ConvertFromLocationwps(List<Locationwp> list, byte frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT)
        {
            RootObject temp = new RootObject()
            {
                groundStation = "MissionPlanner",
                version = 1
            };

            if (list.Count > 0)
                temp.mission.plannedHomePosition = ConvertFromLocationwp(list[0]).coordinate.ToList();

            if (list.Count > 1)
            {
                int a = -1;
                foreach (var item in list)
                {
                    // skip home
                    if (a == -1)
                    {
                        a++;
                        continue;
                    }

                    var temploc = ConvertFromLocationwp(item);

                    // set frame type
                    temploc.frame = frame;

                    temp.mission.items.Add(temploc);

                    if (item.Tag != null)
                    {
                        //if (!temp.mission.complexItems.Contains(item.Tag))
                        //temp.complexItems.Add(item.Tag);
                    }

                    a++;
                }
            }

            return temp;
        }
    }
}
