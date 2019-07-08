namespace Imgur.Model
{
    public class Item
    {
        public string Id;
        public string Title;
        public string Description;
        public string Link;
        public int? Comment_Count;
    }

    public class Album : Item
    {
        public Image[] Images;

        public class Image : Item
        {
        }
    }
}
