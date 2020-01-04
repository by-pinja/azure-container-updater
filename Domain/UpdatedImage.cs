namespace azure_container_updater.Controllers
{
    public class ImageUpdateResult
    {
        public string Image { get; set; }
        public string Tag { get; set; }
        public string ResourceId { get; set; }
        public long TimeStamp { get; set; }
        public string Message { get; set; }
    }
}
