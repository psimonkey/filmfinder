using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace FilmFinder.Data
{
    /// <summary>
    /// Base class for <see cref="CinemaDataFilm"/> and <see cref="CinemaDataCinema"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class CinemaDataCommon : FilmFinder.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public CinemaDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(CinemaDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class CinemaDataFilm : CinemaDataCommon
    {
        public CinemaDataFilm(String uniqueId, String title, String subtitle, String imagePath, String description, String content, CinemaDataCinema group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private CinemaDataCinema _group;
        public CinemaDataCinema Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class CinemaDataCinema : FilmFinder.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");
        private GoogleLocationSearchResult.Result _cinema = null;
        private GoogleLocationDetailResult.Result _cinemadetail = null;

        public CinemaDataCinema(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        public CinemaDataCinema(GoogleLocationSearchResult.Result cinema)
        {
            this._cinema = cinema;
            SetImage(cinema.icon);
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        public CinemaDataCinema(GoogleLocationSearchResult.Result cinema, GoogleLocationDetailResult.Result cinemadetail)
        {
            this._cinema = cinema;
            this._cinemadetail = cinemadetail;
            SetImage(cinemadetail.icon);
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get {
                if (this._cinemadetail != null) return this._cinemadetail.id;
                if (this._cinema != null) return this._cinema.id;
                return this._uniqueId;
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get {
                if (this._cinemadetail != null) return this._cinemadetail.name;
                if (this._cinema != null) return this._cinema.name;
                return this._title;
            }
        }

        private Uri _website = null;
        public Uri Website
        {
            get
            {
                if (this._cinemadetail != null) this._website = new Uri(this._cinemadetail.website);
                return this._website;
            }
        }

        public string FormattedAddress
        {
            get {
                if (this._cinemadetail != null) return this._cinemadetail.formatted_address + "\n" + this._cinemadetail.formatted_phone_number;
                return this._cinema.vicinity;
            }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get {
                if (this._cinemadetail != null) return this._cinemadetail.name;
                if (this._cinema != null) return this._cinema.name;
                return this._subtitle;
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get {
                if (this._cinemadetail != null) return this._cinemadetail.name;
                if (this._cinema != null) return this._cinema.name;
                return this._description;
            }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(_baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<CinemaDataFilm> _items = new ObservableCollection<CinemaDataFilm>();
        public ObservableCollection<CinemaDataFilm> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<CinemaDataFilm> _topItem = new ObservableCollection<CinemaDataFilm>();
        public ObservableCollection<CinemaDataFilm> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of venues and films.
    /// </summary>
    public sealed class CinemaDataSource
    {
        public static CinemaDataSource _cinemaDataSource = new CinemaDataSource();
        public String _locality = "Searching...";

        private String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

        private static Geoposition _position = null;
        public async Task<Geoposition> GetPositionAsync()
        {
            await FetchPositionAsync();
            return _position;
        }
        private async Task FetchPositionAsync()
        {
            if (_position == null)
            {
                Geolocator geolocator = new Geolocator();
                geolocator.DesiredAccuracy = PositionAccuracy.Default;
                _position = await geolocator.GetGeopositionAsync();
            }
        }

        public async Task<String> GoogleRequest(String url)
        {
            Debug.WriteLine("URL: " + url);
            WebRequest req = WebRequest.Create(url);
            WebResponse res = await req.GetResponseAsync();
            Stream inputStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(inputStream);
            return reader.ReadToEnd();
        }

        public async Task<GoogleLocationDetailResult> FetchCinemaDetailsAsync(GoogleLocationSearchResult.Result cinema)
        {
            return JsonConvert.DeserializeObject<GoogleLocationDetailResult>(
                await GoogleRequest("https://maps.googleapis.com/maps/api/place/details/json?" +
                    "key=AIzaSyC2xB1ebU-zMok06yqdFbLY_TjSF2LztmM&sensor=true" +
                    "&reference=" + cinema.reference
                    )
                );
        }

        public async Task FetchLocalityAsync()
        {
            var pos = await GetPositionAsync();
            var l = JsonConvert.DeserializeObject<GoogleLocationLocalityResult>(
                await GoogleRequest("https://maps.googleapis.com/maps/api/place/search/json?" +
                    "key=AIzaSyC2xB1ebU-zMok06yqdFbLY_TjSF2LztmM&sensor=true" +
                    "&types=locality&radius=6000&" +
                    "&location=" + pos.Coordinate.Latitude.ToString() + "," + pos.Coordinate.Longitude.ToString()
                    )
                );
            _locality = l.results[0].name;
            Debug.WriteLine(_locality);
        }

        public async void FetchCinemasAsync()
        {
            var pos = await GetPositionAsync();
            var cinemas = JsonConvert.DeserializeObject<GoogleLocationSearchResult>(
                await GoogleRequest("https://maps.googleapis.com/maps/api/place/search/json?" +
                    "key=AIzaSyC2xB1ebU-zMok06yqdFbLY_TjSF2LztmM&sensor=true" +
                    "&types=movie_theater&radius=16000&" +
                    "&location=" + pos.Coordinate.Latitude.ToString() + "," + pos.Coordinate.Longitude.ToString()
                    )
                );
            foreach (GoogleLocationSearchResult.Result cinema in cinemas.results)
            {
                //Debug.WriteLine(cinema.name);
                var details = await FetchCinemaDetailsAsync(cinema);
                //Debug.WriteLine(details);
                var cdc = new CinemaDataCinema(cinema, details.result);
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-1",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-2",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-3",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-4",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-4",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-4",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-4",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-4",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                cdc.Items.Add(new CinemaDataFilm(cinema.name + "Item-4",
                    "Item Title: 1",
                    "Item Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    cdc));
                this.AllCinemas.Add(cdc);
            }
        }

        private ObservableCollection<CinemaDataCinema> _allCinemas = new ObservableCollection<CinemaDataCinema>();
        public ObservableCollection<CinemaDataCinema> AllCinemas
        {
            get { return this._allCinemas; }
        }

        public static IEnumerable<CinemaDataCinema> GetCinemas(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of cinemas");
            return _cinemaDataSource.AllCinemas;
        }

        public static CinemaDataCinema GetCinema(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _cinemaDataSource.AllCinemas.Where((cinema) => cinema.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static CinemaDataFilm GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _cinemaDataSource.AllCinemas.SelectMany(cinema => cinema.Items).Where((film) => film.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public CinemaDataSource()
        {
            FetchLocalityAsync();
            FetchCinemasAsync();

            //var group1 = new CinemaDataCinema("Group-1",
            //        "Group Title: 1",
            //        "Group Subtitle: 1",
            //        "Assets/DarkGray.png",
            //        "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            //group1.Items.Add(new CinemaDataFilm("Group-1-Item-1",
            //        "Item Title: 1",
            //        "Item Subtitle: 1",
            //        "Assets/LightGray.png",
            //        "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
            //        ITEM_CONTENT,
            //        group1));
            //group1.Items.Add(new CinemaDataFilm("Group-1-Item-2",
            //        "Item Title: 2",
            //        "Item Subtitle: 2",
            //        "Assets/DarkGray.png",
            //        "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
            //        ITEM_CONTENT,
            //        group1));
            //this.AllCinemas.Add(group1);

        }
    }

    public class GoogleLocationSearchResult
    {
        public List<object> html_attributions { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }
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
    }

    public class GoogleLocationDetailResult
    {
        public List<object> html_attributions { get; set; }
        public Result result { get; set; }
        public string status { get; set; }
        public class Result
        {
            public List<AddressComponent> address_components { get; set; }
            public string formatted_address { get; set; }
            public string formatted_phone_number { get; set; }
            public Geometry geometry { get; set; }
            public string icon { get; set; }
            public string id { get; set; }
            public string international_phone_number { get; set; }
            public string name { get; set; }
            public string reference { get; set; }
            public List<string> types { get; set; }
            public string url { get; set; }
            public int utc_offset { get; set; }
            public string vicinity { get; set; }
            public string website { get; set; }
        }
    }

    public class GoogleLocationLocalityResult
    {
        public List<object> html_attributions { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }
        public class Result
        {
            public Geometry geometry { get; set; }
            public string icon { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string reference { get; set; }
            public List<string> types { get; set; }
            public string vicinity { get; set; }
        }
        public class Geometry
        {
            public Location location { get; set; }
            public Viewport viewport { get; set; }
        }
    }

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

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

}
