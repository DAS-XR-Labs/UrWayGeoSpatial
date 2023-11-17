
using System.Threading;
using Newtonsoft.Json;

namespace DAS.Urway.Routes
{
    
    //NOTE: WALK, BICYCLE, and TWO_WHEELER routes are in beta and might sometimes be missing clear sidewalks, pedestrian paths, or bicycling paths.
    public enum RouteTravelMode
    {
        TRAVEL_MODE_UNSPECIFIED = 0, //No travel mode specified. Defaults to DRIVE.
        DRIVE = 1, //Travel by passenger car.
        BICYCLE =2, //Travel by bicycle.
        WALK =3, //Travel by walking.
        TWO_WHEELER = 4, //Two-wheeled, motorized vehicle. For example, motorcycle. Note that this differs from the BICYCLE travel mode which covers human-powered mode.
        TRANSIT = 5 //Travel by public transit routes, where available.
    }

    //Specifies the preferred type of polyline to be returned.
    public enum PolylineQuality
    {
        POLYLINE_QUALITY_UNSPECIFIED =0, //No polyline type preference specified. Defaults to ENCODED_POLYLINE. 
        HIGH_QUALITY = 1, //Specifies a polyline encoded using the polyline encoding algorithm.
        OVERVIEW =2 //Specifies a polyline using the GeoJSON LineString format
    }

    [System.Serializable]
    public class RouteRequest
    {
        public Origin origin;
        public Destination destination;
        public string travelMode;
        public string polylineQuality;
        public bool computeAlternativeRoutes;
        public string languageCode;
        public string units;
        
        public RouteRequest(Location origin, Location destination, PolylineQuality polylineQuality = PolylineQuality.HIGH_QUALITY, RouteTravelMode mode = RouteTravelMode.WALK, bool alternativeRoutes = false, string languageCode = "en-US", string units = "IMPERIAL")
        {
            Origin startPoint = new Origin(origin);
            this.origin = startPoint;
            Destination endPoint = new Destination(destination);
            this.destination = endPoint;
            this.travelMode = mode.ToString();
            this.polylineQuality = polylineQuality.ToString();
            this.computeAlternativeRoutes = alternativeRoutes;
            this.languageCode = languageCode;
            this.units = units;
        }
    }
    
    [System.Serializable]
    public class Origin
    {
        public Location location;

        public Origin(Location location)
        {
            this.location = location;
        }
    }
    
    [System.Serializable]
    public class Destination
    {
        public Location location;
        
        public Destination(Location location)
        {
            this.location = location;
        }
    }

    [System.Serializable]
    public class Location
    {
        public LatLng latLng;

        public Location(double latitude, double longitude)
        {
            latLng = new LatLng();
            latLng.latitude = latitude;
            latLng.longitude = longitude;
        }
    }
    
    [System.Serializable]
    public class LatLng
    {
        public double latitude;
        public double longitude;
    }
}