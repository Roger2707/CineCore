namespace P3.SearchService.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public string MovieCollection { get; set; } = "MovieSearch";
        public string ScheduleCollection { get; set; } = "ScheduleSearch";
        public string CinemaCollection { get; set; } = "CinemaSearch";
    }
}
