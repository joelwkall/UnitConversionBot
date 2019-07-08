using Imgur.Model;
using System.Collections.Generic;

namespace Imgur
{
    public abstract class BaseImgurConnection
    {
        public abstract IEnumerable<Album> GetAlbums(string section, string sort, int page);

        public abstract void UpVote(string id);

        public abstract string PostComment(string id, string parentId, string comment);
    }
}
