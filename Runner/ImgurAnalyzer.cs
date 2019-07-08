using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Conversion;
using Conversion.Converters;
using Conversion.Scanners;
using Imgur;
using Imgur.Model;

namespace Core
{
    public class ImgurAnalyzer
    {
        private static readonly string LogFileName = "Log-" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".txt";
        private static Random _random = new Random();

        private TextAnalyzer _analyzer;

        private BaseImgurConnection _imgurConnection;

        public ImgurAnalyzer(TextAnalyzer analyzer, BaseImgurConnection imgurConnection)
        {
            _analyzer = analyzer;
            _imgurConnection = imgurConnection;
        }

        public static void Log(string str)
        {
            using (var writer = new StreamWriter(LogFileName, true))
            {
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ": " + str);
            }
            Console.WriteLine(str);
        }

        public void Run()
        {
            var seenAlbums = GetSeenAlbumIds().ToList();
            
            foreach(var section in new[] { "hot", "top", "user"})
            {
                foreach (var sort in new[] { "viral", "top", "time", "rising" })
                {
                    //rising is only available in user
                    if (sort == "rising" && section != "user")
                        continue;

                    var pages = section == "user" ? 30 : 5;
                    
                    for(var page = 0; page < pages; page++)
                    {
                        IEnumerable<Album> albums = null;
                        try
                        {
                            Log($"Fetching {section} albums sorted by {sort}, page {page}...");
                            albums = _imgurConnection.GetAlbums(section, sort, page);
                            Log($"Fetched {albums.Count()} albums.");
                        }
                        catch (Exception ex)
                        {
                            Log("Exception when fetching: " + ex.ToString());
                            continue;
                        }

                        int processed = 0;
                        foreach (var album in albums)
                        {
                            try
                            {
                                //TODO: ignore albums that are too old, only store SEEN for relevant dates

                                //TODO: might want to check comments for seen albums, in case there are new comments?
                                if (seenAlbums.Contains(album.Id))
                                {
                                    continue;
                                }
                                
                                var texts = new List<string>();

                                texts.Add(album.Title);
                                texts.Add(album.Description);

                                if (album.Images != null)
                                {
                                    foreach (var image in album.Images)
                                    {
                                        texts.Add(image.Title);
                                        texts.Add(image.Description);
                                    }
                                }

                                //no duplicates
                                texts = texts.Distinct().ToList();

                                var results = texts.SelectMany(t => _analyzer.FindConversions(t)).ToList();

                                if (results.Count() > 0)
                                {
                                    Log("Posting comment: " + string.Join(", ", results));

                                    PostComments(album.Id, null, results);

                                    Log("Comment posted to album " + album.Id);
                                }

                                if (album.Comment_Count > 0)
                                {
                                    //TODO: convert units in comments
                                }
                            }
                            catch (Exception ex)
                            {
                                Log($"Exception when processing album {album.Id}: " + ex.ToString());
                                continue;
                            }

                            seenAlbums.Add(album.Id);
                            AddSeenAlbumId(album.Id);
                            processed++;
                        }

                        Log($"Finished processing {processed} albums.");
                    }
                }
            }

            Log("Out of albums to process.");
        }

        private void PostComments(string id, string parentId, IEnumerable<string> comments)
        {
            if (comments.Count() == 1)
                _imgurConnection.PostComment(id, parentId, comments.First());
            else
            {
                var firstCommentId = _imgurConnection.PostComment(id, parentId, comments.First());

                foreach (var comment in comments.Skip(1))
                {
                    _imgurConnection.PostComment(id, firstCommentId, comment);
                }
            }

            //upvote everything voted on
            _imgurConnection.UpVote(id);
        }

        public static IEnumerable<string> GetSeenAlbumIds()
        {
            var filePath = "seenAlbumIds.txt";
            if (!File.Exists(filePath))
                yield break;

            using (var reader = new StreamReader(filePath))
            {
                while (true)
                {
                    var line = reader.ReadLine();

                    if (line == null)
                        break;

                    yield return line;
                }
            }
        }

        public void AddSeenAlbumId(string id)
        {
            using (var writer = new StreamWriter("seenAlbumIds.txt", true))
            {
                writer.WriteLine(id);
            }
        }
    }
}
