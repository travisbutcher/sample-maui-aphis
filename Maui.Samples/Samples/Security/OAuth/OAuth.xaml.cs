// Copyright 2022 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using ArcGIS.Helpers;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Data;

namespace ArcGIS.Samples.OAuth
{
    [ArcGIS.Samples.Shared.Attributes.Sample(
        name: "Authenticate with OAuth",
        category: "Security",
        description: "Authenticate with ArcGIS Online (or your own portal) using OAuth2 to access secured resources (such as private web maps or layers).",
        instructions: "When you run the sample, the app will load a web map which contains premium content. You will be challenged for an ArcGIS Online login to view the private layers. Enter a user name and password for an ArcGIS Online named user account (such as your ArcGIS for Developers account). If you authenticate successfully, the traffic layer will display, otherwise the map will contain only the public basemap layer.",
        tags: new[] { "OAuth", "OAuth2", "authentication", "cloud", "credential", "portal", "security" })]
    [ArcGIS.Samples.Shared.Attributes.ClassFile("Helpers/ArcGISLoginPrompt.cs")]
    public partial class OAuth : ContentPage
    {
        // - The URL of the portal to authenticate with
        private const string ServerUrl = "https://maps-stg.mrp.usda.gov/arcgis";

        // - The ID for a web map item hosted on the server (the ID below is for a traffic map of Paris).
        private const string WebMapId = "a6963a6779cb4e6cb02a1a311be5ca41";

        public OAuth()
        {
            InitializeComponent();

            // Call a function to initialize the app and request a web map (with secured layers) to display.
            _ = Initialize();
        }

        private async Task Initialize()
        {
            try
            {
                Esri.ArcGISRuntime.Security.AuthenticationManager.Current.RemoveAllCredentials();

                // Set up the AuthenticationManager to use OAuth for secure ArcGIS Online requests.
                ArcGISLoginPrompt.SetChallengeHandler();

                // Connect to the portal (ArcGIS Online, for example).
                ArcGISPortal arcgisPortal = await ArcGISPortal.CreateAsync(new Uri(ServerUrl), true);

                //Get addtional secure content using the portal
                //PortalItem secureLayer = await PortalItem.CreateAsync(arcgisPortal, "96be54ff479146cf9ab331cbdedfd114");
                PortalItem secureLayer = await PortalItem.CreateAsync(arcgisPortal, "1db75a8b540b4384a6b9947c33b9470f");
   
                //Get the Service Featrure Table from the portal
                ServiceFeatureTable serviceTable = new ServiceFeatureTable(secureLayer, 0);
                FeatureLayer featureLayer = new FeatureLayer(serviceTable);
                await featureLayer.LoadAsync();

                QueryParameters queryParams = new QueryParameters();
                queryParams.WhereClause = "1=1";
                queryParams.ReturnGeometry = true;
                FeatureQueryResult features = await serviceTable.QueryFeaturesAsync(queryParams);

                // Get a web map portal item using its ID.
                // If the item contains layers not shared publicly, the user will be challenged for credentials at this point.
                PortalItem portalItem = await PortalItem.CreateAsync(arcgisPortal, WebMapId);
                MyMapView.Map = new Map(portalItem);
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "OK");
            }
        }
    }
}