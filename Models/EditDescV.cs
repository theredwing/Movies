namespace Movies.Models
{
    public class EditDescV
    {
        public EditDescV() { }
        public int intID { get; set; }
        public string strDesc { get; set; }
        public string strTitle { get; set; }
        public string strSrchName { get; set; }
        public string strSrchPos { get; set; }
        public string strSrchCat { get; set; }
        public bool TriggerOnLoad { get; set; }
    }
}