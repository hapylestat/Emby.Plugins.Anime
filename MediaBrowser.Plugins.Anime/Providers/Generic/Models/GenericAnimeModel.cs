using System.Collections.Generic;

namespace MediaBrowser.Plugins.Anime.Providers.Generic.Models
{

    public enum RelationType
    {
        Sequel,
        Prequel
    }

    public class LanguageItem
    {
        public LanguageItem(string itemLanguage, string itemValue)
        {
            ItemLanguage = itemLanguage;
            ItemLanguage = itemValue;
        }
        public LanguageItem() { }
        public string ItemLanguage { get; set; }
        public string ItemValue { get; set; }
    }

    public class Name
    {
        public Name(string first, string last, string native)
        {
            First = first;
            Last = last;
            Native = native;
        }
        public Name() { }
        public string First { get; set; }
        public string Last { get; set; }
        public string Full {
            get
            {
                return First + " " + Last;
            }
         }
        public string Native { get; set; }
    }

    public class Image
    {
        public Image(string medium, string large)
        {
            Medium = medium;
            Large = large;
        }
        public Image() { }

        public string Medium { get; set; }
        public string Large { get; set; }
    }
    public  class Date
    {
        public Date(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }
        public Date() { }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }

    public class VoiceActor
    {
        public VoiceActor(string providerId, Name actorName, Image actorImage, string language)
        {
            ProviderId = providerId;
            ActorName = actorName;
            ActorImage = actorImage;
            Language = language;
        }
        public VoiceActor() { }
        public string ProviderId { get; set; }
        public Name ActorName { get; set; }
        public Image ActorImage { get; set; }
        public string Language { get; set; }

    }

    public class Character
    {
        public Character(string name, string role, List<VoiceActor> voiceActors)
        {
            Name = name;
            Role = role;
            VoiceActors = voiceActors;
        }
        public Character() { }
        public string Name { get; set; }
        public string Role { get; set; }
        public List<VoiceActor> VoiceActors { get; set; }
    }

    public class RelationItem
    {
        public RelationItem(string providerId, RelationType relation)
        {
            ProviderId = providerId;
            Relation = relation;
        }
        public RelationItem() { }
        public string ProviderId { get; set; }
        public RelationType Relation { get; set; }
    }

    public class AnimeSerie
    {
        public string ProviderId { get; set; }
        public List<LanguageItem> Description { get; set; }
        public List<Character> Character { get; set; }
        public int AverageScore { get; set; }
        public bool IsAdult { get; set; }
        public Date StartDate { get; set; }
        public Date EndDate { get; set; }
        public List<LanguageItem> Title { get; set; }
        public List<Image> CoverImage { get; set; }
        public string BannerImage { get; set; }
        public int Episodes { get; set; }
        public string Season { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Synonyms { get; set; }
        public List<RelationItem> Relations { get; set; }
    }
}
