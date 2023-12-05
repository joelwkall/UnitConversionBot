using Imgur.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace Imgur
{
    public class LiveImgurConnection: BaseImgurConnection
    {
        private string _imgurClientId;
        private string _imgurClientSecret;
        private string _accessToken;
        private string _refreshToken;

        private static readonly HttpClient _client = new HttpClient();

        public LiveImgurConnection(string imgurClientId, string imgurClientSecret, string refreshToken)
        {
            _imgurClientId = imgurClientId;
            _imgurClientSecret = imgurClientSecret;
            _refreshToken = refreshToken;
        }

        private string AccessToken
        {
            get
            {
                if (_accessToken != null)
                    return _accessToken;

                var values = new Dictionary<string, string>
                {
                    { "refresh_token", _refreshToken},
                    { "client_id", _imgurClientId },
                    { "client_secret", _imgurClientSecret },
                    {"grant_type", "refresh_token" }
                };
                
                var json = GetJObject("https://api.imgur.com/oauth2/token", null, new FormUrlEncodedContent(values));

                if (json["data"] != null && json["data"]["error"] != null)
                    throw new Exception("Error when refreshing token: " + json["data"]["error"].ToObject<string>());

                return json["access_token"].ToObject<string>();
            }
        }

        private JObject GetJObject(string url, string accessToken, FormUrlEncodedContent content = null)
        {
            var request = new HttpRequestMessage(content == null ? HttpMethod.Get : HttpMethod.Post, url);

            if(accessToken != null)
                request.Headers.Add("Authorization", "Bearer " + accessToken);

            if (content != null)
                request.Content = content;

            var response = _client.SendAsync(request);

            var responseString = response.Result.Content.ReadAsStringAsync().Result;

            try
            {
                var json = JObject.Parse(responseString);
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("Error when parsing: " + (responseString.Length > 200 ? (responseString.Substring(0, 200)+"...") : responseString), ex);
            }
        }

        public override IEnumerable<Album> GetAlbums(string section, string sort, int page)
        {
            var window = "day";
            var showViral = true;
            var showMature = false;
            var albumPreviews = true;

            //TODO: usersub rising often times out server side (503 first byte). Could timeout client side much faster to improve speed
            var url = $"https://api.imgur.com/3/gallery/{section}/{sort}/{window}/{page}?showViral={showViral}&mature={showMature}&album_previews={albumPreviews}";

            var json = GetJObject(url, AccessToken);

            return json["data"].ToObject<Album[]>();
        }

        public override void UpVote(string id)
        {
            var json = GetJObject($"https://api.imgur.com/3/gallery/{id}/vote/up", AccessToken, new FormUrlEncodedContent(new Dictionary<string,string>()));
        }

        public override string PostComment(string id, string parentId, string comment)
        {
            var values = new Dictionary<string, string>
            {
                { "image_id", id},
                { "comment", comment }
            };

            if (parentId != null)
                values["parent_id"] = parentId;

            var json = GetJObject("https://api.imgur.com/3/comment", AccessToken, new FormUrlEncodedContent(values));

            if (json["success"].ToObject<bool>() == false)
            {
                if (json["data"] != null && json["data"]["error"] != null)
                {
                    // error can be string or object (rate limiting)
                    var errorObj = json["data"]["error"];

                    if (errorObj.Type == JTokenType.String)
                        throw new Exception(errorObj.ToObject<string>());

                    //rate limited, try again in 1 minute
                    else if (errorObj.Type == JTokenType.Object && errorObj["code"].ToObject<int>() == 2008)
                    {
                        var seconds = errorObj["exception"]["wait"].ToObject<int>();

                        //TODO: log
                        //Log($"Rate limited, waiting {seconds} seconds...");

                        Thread.Sleep(seconds * 1000);
                        return PostComment(id, parentId, comment);
                    }
                }

                //TODO: log
                //Log("Unknown error. Response: " + json.ToString());
                return null;
            }

            return json["data"]["id"].ToObject<string>();
        }
    }
}
