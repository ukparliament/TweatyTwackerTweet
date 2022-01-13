namespace TweatyTwackerTweet
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            List<Treaty> treaties = GetTreaties();
            foreach (var treaty in treaties)
            {
                Tweet(treaty);
            }
            UpdateTweetStatus(treaties);
        }

        static void UpdateTweetStatus(List<Treaty> treaties)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TweatyTwackerSqlServer"].ConnectionString;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            foreach (var treaty in treaties.Where(x=>x.IsTweeted))
            {
                using (SqlCommand cmd = new SqlCommand("Add to database", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "InsertUpdateTweatyTwackerTreaty";
                    cmd.Parameters.AddWithValue("@TreatyName", treaty.Name != null ? (object)treaty.Name : DBNull.Value);
                    cmd.Parameters.AddWithValue("@LeadOrg", treaty.LeadOrganisation != null ? (object)treaty.LeadOrganisation : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Series", treaty.Series != null ? (object)treaty.Series : DBNull.Value);
                    cmd.Parameters.AddWithValue("@LaidDate", treaty.LaidDate != null ? (object)treaty.LaidDate : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TreatyUri", treaty.Id != null ? (object)treaty.Id : DBNull.Value);
                    cmd.Parameters.AddWithValue("@WorkPackageUri", treaty.WorkPackageId != null ? (object)treaty.WorkPackageId : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TnaUri", treaty.Link != null ? (object)treaty.Link : DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsTweeted", (object)1);
                    cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    string msg = cmd.Parameters["@Message"].Value.ToString();
                    Console.WriteLine($"Title: {treaty.Id}, {msg}");
                }
            }
            connection.Close();
        }

        static List<Treaty> GetTreaties()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TweatyTwackerSqlServer"].ConnectionString;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            List<Treaty> treaties = new List<Treaty>();
            using (SqlCommand cmd = new SqlCommand("Read from database", connection))
            {
                String sql = @"SELECT 
	                                  [TreatyName]
                                      ,[LeadOrg]
                                      ,[Series]
                                      ,[LaidDate]
                                      ,[TreatyUri]
                                      ,[WorkPackageUri]
                                      ,[TnaUri]
                                FROM [dbo].[TweatyTwackerTreaty]
                                WHERE IsTweeted = 0";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var treaty = new Treaty();
                            treaty.Name = reader.GetString(0);
                            treaty.LeadOrganisation = reader.GetString(1);
                            treaty.Series = reader.GetString(2);
                            treaty.LaidDate = reader.GetDateTimeOffset(3);
                            treaty.Id = reader.GetString(4);
                            treaty.WorkPackageId = reader.GetString(5);
                            treaty.Link = reader.GetString(6);
                            treaty.IsTweeted = false;
                            treaties.Add(treaty);
                        }
                    }
                }
            }

            connection.Close();
            return treaties;
        }

        static void Tweet (Treaty treaty)
        {
            string oauth_consumer_key = ConfigurationManager.AppSettings["oauth_consumer_key"];
            string oauth_consumer_secret = ConfigurationManager.AppSettings["oauth_consumer_secret"];
            string oauth_token = ConfigurationManager.AppSettings["oauth_token"];
            string oauth_token_secret = ConfigurationManager.AppSettings["oauth_token_secret"];

            var twitter = new TwitterApi(oauth_consumer_key, oauth_consumer_secret, oauth_token, oauth_token_secret);
            var response = twitter.Tweet(treaty.TweetText);
            treaty.IsTweeted = true;
            Console.WriteLine(response);
        }
    }
}
