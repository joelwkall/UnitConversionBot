namespace Imgur
{
    //TODO: override the fetching so that it doesnt need a user account
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
