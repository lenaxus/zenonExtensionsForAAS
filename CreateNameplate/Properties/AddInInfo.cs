using Mono.Addins;

// Declares that this assembly is an add-in
[assembly: Addin("CreateNameplate", "1.0")]

// Declares that this add-in depends on the scada v1.0 add-in root
[assembly: AddinDependency("::scada", "1.0")]

[assembly: AddinName("CreateNameplate")]
[assembly: AddinDescription("This Engineering Studio Service Extension creates in the process screen of the project a nameplate out of the informations of the AAS.")]