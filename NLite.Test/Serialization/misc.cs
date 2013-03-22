using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Test.Serialization
{
    public class PersonForXml
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public int Age { get; set; }
        public decimal Percent;
        public long BigNumber;
        public bool IsCool { get; set; }
        public List<Friend> Friends { get; set; }
        public Friend BestFriend { get; set; }

        protected string Ignore { get; set; }
        public string IgnoreProxy { get { return Ignore; } }

        protected string ReadOnly { get { return null; } }
        public string ReadOnlyProxy { get { return ReadOnly; } }

        public FoeList Foes { get; set; }

        public Guid UniqueId { get; set; }
        public Guid EmptyGuid { get; set; }

        public Uri Url { get; set; }
        public Uri UrlPath { get; set; }

        public Order Order { get; set; }

        public Disposition Disposition { get; set; }

    }

    public class PersonForJson
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public int Age { get; set; }
        public decimal Percent { get; set; }
        public long BigNumber { get; set; }
        public bool IsCool { get; set; }
        public List<Friend> Friends { get; set; }
        public Friend BestFriend { get; set; }
        public Guid Guid { get; set; }
        public Guid EmptyGuid { get; set; }
        public Uri Url { get; set; }
        public Uri UrlPath { get; set; }

        protected string Ignore { get; set; }
        public string IgnoreProxy { get { return Ignore; } }

        protected string ReadOnly { get { return null; } }
        public string ReadOnlyProxy { get { return ReadOnly; } }

        public Dictionary<string, Foe> Foes { get; set; }

        public Order Order { get; set; }

        public Disposition Disposition { get; set; }
    }

    public enum Order
    {
        First,
        Second,
        Third
    }

    public enum Disposition
    {
        Friendly,
        SoSo,
        SteerVeryClear
    }

    public class Friend
    {
        public string Name { get; set; }
        public int Since { get; set; }
    }

    public class Foe
    {
        public string Nickname { get; set; }
    }

    public class FoeList : List<Foe>
    {
        public string Team { get; set; }
    }

    public class Birthdate
    {
        public DateTime Value { get; set; }
    }

    public class OrderedProperties
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class DatabaseCollection : List<Database>
    {
    }

    public class Database
    {
        public string Name { get; set; }
        public string InitialCatalog { get; set; }
        public string DataSource { get; set; }
    }

    public class Generic<T>
    {
        public T Data { get; set; }
    }

    public class GenericWithList<T>
    {
        public List<T> Items { get; set; }
    }

    public class DateTimeTestStructure
    {
        public DateTime DateTime { get; set; }
        public DateTime? NullableDateTimeWithNull { get; set; }
        public DateTime? NullableDateTimeWithValue { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public DateTimeOffset? NullableDateTimeOffsetWithNull { get; set; }
        public DateTimeOffset? NullableDateTimeOffsetWithValue { get; set; }
    }

    public class TimeSpanTestStructure
    {
        public TimeSpan Tick { get; set; }
        public TimeSpan Millisecond { get; set; }
        public TimeSpan Second { get; set; }
        public TimeSpan Minute { get; set; }
        public TimeSpan Hour { get; set; }
        public TimeSpan? NullableWithoutValue { get; set; }
        public TimeSpan? NullableWithValue { get; set; }
    }

    public class JsonEnumsTestStructure
    {
        public Disposition Upper { get; set; }
        public Disposition Lower { get; set; }
        public Disposition CamelCased { get; set; }
        public Disposition Underscores { get; set; }
        public Disposition LowerUnderscores { get; set; }
        public Disposition Dashes { get; set; }
        public Disposition LowerDashes { get; set; }
    }

    public class VenueSearch
    {
        public string total_items { get; set; }
        public string page_size { get; set; }
        public string page_count { get; set; }
        public string page_number { get; set; }
        public string page_items { get; set; }
        public string first_item { get; set; }
        public string last_item { get; set; }
        public string search_time { get; set; }
        public List<Venue> venues { get; set; }
    }

    public class PerformerSearch
    {
        public string total_items { get; set; }
        public string page_size { get; set; }
        public string page_count { get; set; }
        public string page_number { get; set; }
        public string page_items { get; set; }
        public string first_item { get; set; }
        public string last_item { get; set; }
        public string search_time { get; set; }
        public List<Performer> performers { get; set; }
    }

    public class Performer
    {
        public string id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string short_bio { get; set; }
        public DateTime? created { get; set; }
        public string creator { get; set; }
        public string demand_count { get; set; }
        public string demand_member_count { get; set; }
        public string event_count { get; set; }
        public ServiceImage image { get; set; }
    }

    public class Venue
    {
        public string id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string venue_name { get; set; }
        public string description { get; set; }
        public string venue_type { get; set; }
        public string address { get; set; }
        public string city_name { get; set; }
        public string region_name { get; set; }
        public string postal_code { get; set; }
        public string country_name { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string event_count { get; set; }
    }

    public class ServiceImage
    {
        public ServiceImage1 thumb { get; set; }
        public ServiceImage1 small { get; set; }
        public ServiceImage1 medium { get; set; }
    }

    public class ServiceImage1
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Event
    {
        public string id { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string start_time { get; set; }
        public string venue_name { get; set; }
        public string venue_id { get; set; }
        public List<Performer> performers { get; set; }
    }

    public class Lastfm
    {
        public class Event : LastfmBase
        {
            public string id { get; set; }
            public string title { get; set; }
            public EventArtistList artists { get; set; }
            public Venue venue { get; set; }
            public DateTime startDate { get; set; }
            public string description { get; set; }
            public int attendance { get; set; }
            public int reviews { get; set; }
            public string tag { get; set; }
            public string url { get; set; }
            public string headliner { get; set; }
            public int cancelled { get; set; }
        }

        public class EventArtistList : List<artist>
        {
        }

        public class artist
        {
            public string Value { get; set; }
        }

        public abstract class LastfmBase
        {
            public string status { get; set; }
            public Error error { get; set; }
        }

        public class Error
        {
            public string Value { get; set; }
            public int code { get; set; }
        }
    }

    public class xml_api_reply
    {
        public string version { get; set; }
        public Weather weather { get; set; }
    }

    public class Weather : List<forecast_conditions>
    {
        public string module_id { get; set; }
        public string tab_id { get; set; }
        public string mobile_row { get; set; }
        public string mobile_zipped { get; set; }
        public string row { get; set; }
        public string section { get; set; }
        public Forecast_information forecast_information { get; set; }
        public Current_conditions current_conditions { get; set; }
        //public T forecast_conditions { get; set; }
    }

    public class DataElement
    {
        public string data { get; set; }
    }

    public class BooleanTest
    {
        public bool Value { get; set; }
    }

    public class Forecast_information
    {
        public DataElement city { get; set; }
        public DataElement postal_code { get; set; }
        public DataElement forecast_date { get; set; }
        public DataElement unit_system { get; set; }
    }

    public class Current_conditions
    {
        public DataElement condition { get; set; }
        public DataElement temp_c { get; set; }
        public DataElement humidity { get; set; }
        public DataElement icon { get; set; }
        public DataElement wind_condition { get; set; }
    }

    public class forecast_conditions
    {
        public DataElement day_of_week { get; set; }
        public DataElement condition { get; set; }
        public DataElement low { get; set; }
        public DataElement high { get; set; }
        public DataElement icon { get; set; }
    }

    public class NullableValues
    {
        public int? Id { get; set; }
        public DateTime? StartDate { get; set; }
        public Guid? UniqueId { get; set; }
    }

    public class TwilioCallList : List<Call>
    {
        public int Page { get; set; }
        public int NumPages { get; set; }
    }

    public class Call
    {
        public string Sid { get; set; }
    }

    public class SimpleTypesListSample
    {
        public List<string> Names { get; set; }
        public List<int> Numbers { get; set; }
    }

    public class InlineListSample
    {
        public int Count { get; set; }
        public List<image> images { get; set; }
        public List<Image> Images { get; set; }
    }

    public class NestedListSample
    {
        public List<image> images { get; set; }
        public List<Image> Images { get; set; }
    }

    public class EmptyListSample
    {
        public List<image> images { get; set; }
        public List<Image> Images { get; set; }
    }

    public class Image
    {
        public string Src { get; set; }
        public string Value { get; set; }
    }

    public class image
    {
        public string Src { get; set; }
        public string Value { get; set; }
    }
}
