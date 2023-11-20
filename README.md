
# AR-Tourism

The AR Tourism app uses Google VPS and augmented reality technology to help tourists find their way around the city. The app provides information on restaurants, bars, outdoor parks, hiking trails, hot springs, and other nearby activities. It places markers with descriptions and ratings around the user's location, making it easy to find small local businesses. The app also helps users find deals and discounts. Overall, the Ouray Colorado app is a useful tool for tourists looking to explore the city and make the most of their visit.



## Requirements

**Unity Version:** 2021.3.25f1

**ARCore Extensions:** v1.31.0

**Mobile Notifications:** v2.0.2
## Documentation
`POI` - an anchor specified by geo coordinates, contains information about the location
The scriptable object `LocationData` with mock data is located in the `Resources->SO` folder. 

`CategoryData SO` - a scriptable object that contains category data. The `GeospatialController` must have a reference to the current object to associate the POI with the corresponding category.

#### Important: 
`GeospatialController` and `LocationData` should hava ref on the same `LocationData` scriptable object.

#### Adding a New POI Dataset:
To incorporate a new Point of Interest (POI) dataset, follow these steps:
1. Open the directory: `Resources -> SO -> LocationData`.
2. Within the `LocationData` folder, generate a new dataset using the context menu. Right-click and choose `Create -> Scriptable Object -> LocationData`.
3. Once the new dataset is created, establish a reference to it within both the `GeospatialController` and `LocationData` components. This step ensures seamless integration and proper functionality.

#### Main components:

- `GeospatialController.cs` - manages all POI anchors and everything related to them.
- `LocationData.cs` - provides access to location information
- `RouteMe.cs` - handles routing and navigating the user to destinations POI
- `POINotificationController.cs` - manages local notifications. Registers notifications for nearby deals based on location triggers.
The iOS system limits the number of location-based triggers that it schedules at the same time up to 64.
- `DealsManager.cs` - receives a list of deals from the server and manages them. Contains a local list of mock deals for dev testing.

## Instructions
Please follow the written instructions on how to connect API keys and create custom Location data
https://drive.google.com/file/d/1OqholyejLRWwGXLBJv63WuANPdislJ16/view?usp=sharing
