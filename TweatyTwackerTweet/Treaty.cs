namespace TweatyTwackerTweet
{
    using System;
    using System.Linq;

    class Treaty
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LeadOrganisation { get; set; }
        public string Series { get; set; }
        public string WorkPackageId { get; set; }
        public DateTimeOffset LaidDate { get; set; }
        public string Link { get; set; }
        public bool IsTweeted { get; set; }

        public string ShortTitle
        {
            get
            {
                if (Name.Length > 200)
                {
                    return Name.Substring(0, 198) + "..";
                }
                else
                {
                    return Name;
                }
            }
        }
        public string Url
        {
            get
            {
                return "https://treaties.parliament.uk/treaty/" + Id.Split('/').Last() + "/";
            }
        }

        public string TweetText
        {
            get
            {
                string tweet_text = "";
                tweet_text += LeadOrganisation;
                tweet_text += " treaty ";
                tweet_text += Series;
                tweet_text += " has been laid by the FCDO ";
                tweet_text += Url;
                return tweet_text;
            }
        }

        public string Description
        {
            get
            {
                string description = "";
                description += Name;
                description += " was laid on ";
                description += LaidDate.ToString("dd-MM-yyyy");
                description += " by the FCDO, as ";
                description += Series;
                description += ". Lead by the ";
                description += LeadOrganisation;
                description += ".";
                return description;
            }
        }
    }
}
