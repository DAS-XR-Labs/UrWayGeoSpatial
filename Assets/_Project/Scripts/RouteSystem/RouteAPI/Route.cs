using System;
using System.Collections.Generic;

namespace DAS.Urway.Routes
{
    [Serializable]
    public class RouteResponse
    {
        public List<Route> routes;
    }

    [Serializable]
    public class Distance
    {
        public string text;
    }

    [Serializable]
    public class Duration
    {
        public string text;
    }

    [Serializable]
    public class EndLocation
    {
        public LatLng latLng;
    }

    [Serializable]
    public class Leg
    {
        public int distanceMeters;
        public string duration;
        public string staticDuration;
        public Polyline polyline;
        public StartLocation startLocation;
        public EndLocation endLocation;
        public List<Step> steps;
        public LocalizedValues localizedValues;
    }

    [Serializable]
    public class LocalizedValues
    {
        public Distance distance;
        public StaticDuration staticDuration;
        public Duration duration;
    }

    [Serializable]
    public class NavigationInstruction
    {
        public string maneuver;
        public string instructions;
    }

    [Serializable]
    public class Polyline
    {
        public string encodedPolyline;
    }
    
    [Serializable]
    public class Route
    {
        public List<Leg> legs;
    }

    [Serializable]
    public class StartLocation
    {
        public LatLng latLng;
    }

    [Serializable]
    public class StaticDuration
    {
        public string text;
    }

    [Serializable]
    public class Step
    {
        public int distanceMeters;
        public string staticDuration;
        public Polyline polyline;
        public StartLocation startLocation;
        public EndLocation endLocation;
        public NavigationInstruction navigationInstruction;
        public LocalizedValues localizedValues;
        public string travelMode;
    }
}