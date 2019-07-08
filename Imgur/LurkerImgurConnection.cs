namespace Imgur
{
    public class LurkerImgurConnection : LiveImgurConnection
    {
        public LurkerImgurConnection(string imgurClientId, string imgurClientSecret, string refreshToken): base(imgurClientId, imgurClientSecret, refreshToken)
        {}

        public override void UpVote(string id)
        {
            //TODO: log this
            return;
        }

        public override string PostComment(string id, string parentId, string comment)
        {
            //TODO: log this
            return null;
        }
    }
}
