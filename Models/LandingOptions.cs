namespace Landing.API.Models
{
    public class LandingOptions
    {
        /// <summary>
        /// Uses for interacting Admin panel with API
        /// </summary>
        public string AdminPanelAccessToken { get; set; }
        /// <summary>
        /// BaseAdress of API service
        /// </summary>
        public string ApiBaseAddress { get; set; }
    }
}
