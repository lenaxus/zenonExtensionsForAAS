using Mono.Addins;

// Declares that this assembly is an add-in
[assembly: Addin("TrackCleaning", "1.0")]

// Declares that this add-in depends on the scada v1.0 add-in root
[assembly: AddinDependency("::scada", "1.0")]

[assembly: AddinName("TrackCleaning")]
[assembly: AddinDescription("This Project Service Extension tracks the cleaning and stores it to the AAS.")]