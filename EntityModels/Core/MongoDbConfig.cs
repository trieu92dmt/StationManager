namespace EntityModels.Core
{
    public class MongoDbConfig
    {
        public const string ConnectionString = "mongodb+srv://isd:isd082022@cluster0.wm2mbjz.mongodb.net/?retryWrites=true&w=majority"  /*"mongodb://localhost:27017"*/;
        public const string DatabaseName = "iMES_API";
        public const string CollectionName = "ResourceMonitorModel";
    }
}
