using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace FilmFinder
{
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class OpeningHours
    {
        public bool open_now { get; set; }
    }

    public class Result
    {
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public double rating { get; set; }
        public string reference { get; set; }
        public List<string> types { get; set; }
        public string vicinity { get; set; }
        public OpeningHours opening_hours { get; set; }
    }

    public class Venues
    {
        public List<object> html_attributions { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
    //class Venues
    //{
    //    public List<Venue> venues { get; set; }
    //}

    //class Venue
    //{

    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public double rating { get; set; }
    //    public string reference { get; set; }
    //    public string vicinity { get; set; }
    //    public OpeningHours opening_hours { get; set; }

    //    public class OpeningHours
    //    {
    //        public bool open_now { get; set; }
    //    }

    //    public String ToString()
    //    {
    //        return this.name;
    //    }

    //    public async void fromXmlReader(XmlReader reader)
    //    {
    //        while (await reader.ReadAsync())
    //        {
    //            if (reader.NodeType == XmlNodeType.Element && reader.Name == "name")
    //            {
    //                this.name = await reader.ReadInnerXmlAsync();
    //            }
    //        }
    //    }
    //}
}
