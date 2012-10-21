using FilmFinder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace FilmFinder
{
    class UserPosition
    {
        Geolocator geolocator = null;
        Geoposition position = null;
        Venues venues = null;

        /// <summary>
        /// The user's position, along with nearby cinema information.
        /// </summary>
        public UserPosition(Geolocator geolocator)
        {
            this.geolocator = geolocator;
            GetPositionVoidAsync();
        }

        /// <summary>
        /// Asynchronously find the position in the background.
        /// </summary>
        private async void GetPositionVoidAsync()
        {
            var position = await this.geolocator.GetGeopositionAsync();
            if (this.position == null) this.position = position;
        }

        /// <summary>
        /// Return the position, fetching it if it isn't already known.
        /// </summary>
        private async Task<Geoposition> GetPositionAsync()
        {
            if (this.position == null)
            {
                this.position = await this.geolocator.GetGeopositionAsync();
            }
            return this.position;
        }

        public async void getVenuesAsync()
        {
            var pos = await GetPositionAsync();
            string url = "https://maps.googleapis.com/maps/api/place/search/json?" +
                "types=movie_theater&radius=16000&sensor=true&key=AIzaSyC2xB1ebU-zMok06yqdFbLY_TjSF2LztmM" +
                "&location=" + pos.Coordinate.Latitude.ToString() + "," + pos.Coordinate.Longitude.ToString();
            Debug.WriteLine(url);
            WebRequest req = WebRequest.Create(url);
            WebResponse res = await req.GetResponseAsync();
            Stream inputStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(inputStream);
            string json = reader.ReadToEnd();
            this.venues = JsonConvert.DeserializeObject<Venues>(json);
            //XmlReaderSettings xrs = new XmlReaderSettings() { Async = true, CloseInput = true };
            //XmlReader reader = XmlReader.Create(inputStream, xrs);
            //while (await reader.ReadAsync())
            //{
            //    if (reader.NodeType == XmlNodeType.Element && reader.Name == "status")
            //    {
            //        string status = await reader.ReadInnerXmlAsync();
            //        if (status != "OK")
            //        {
            //            Debug.WriteLine("Request returned no results");
            //            // TODO: Do something useful if no cinemas found.
            //        }
            //    }
            //    if (reader.NodeType == XmlNodeType.Element && reader.Name == "result")
            //    {
            //        Venue venue = new Venue();
            //        venue.fromXmlReader(reader.ReadSubtree());
            //        this.venues.Add(venue);
            //    }
            //}
            //// do something useful if list is empty.
            foreach (Result venue in this.venues.results)
            {
                Debug.WriteLine(venue.name);
            }
            //            // take element nodes with name "Address" 
            //            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Address")
            //            {
            //                // create a XmlReader from the Address element 
            //                using (XmlReader subReader = reader.ReadSubtree())
            //                {
            //                    bool isInUS = false;
            //                    while (await subReader.ReadAsync())
            //                    {
            //                        // check if the CountryRegion element contains "United States" 
            //                        if (subReader.Name == "CountryRegion")
            //                        {
            //                            string value = await subReader.ReadInnerXmlAsync();
            //                            if (value.Contains("United States"))
            //                            {
            //                                isInUS = true;
            //                            }
            //                        }

            //                        // write the FormattedAddress node of the reader, if the address is within US 
            //                        if (isInUS && subReader.NodeType == XmlNodeType.Element && subReader.Name == "FormattedAddress")
            //                        {
            //                            await writer.WriteNodeAsync(subReader, false);
            //                        }
            //                    }
            //                }
            //            }
            //    }
            //} 
            //XDocument xmldoc = XDocument.Load(str);
            //string strres = reader.ReadToEnd();
            //Debug.WriteLine(strres);
            //ItemsPage.currentLocation.Text = strres;
        }
        
    }
}
