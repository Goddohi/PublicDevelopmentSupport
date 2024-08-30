using DevSup.Core;
using System.Collections.ObjectModel;
using System.Xml.Serialization;


public class DBUserEntity : BaseEntityObject
{
    public string DB { get; set; }
    public string USER { get; set; }
    public string CONNECT_STRING { get; set; }
    public string DATASOURCE { get; set; }
    public string USERID { get; set; }
    public string USERPW { get; set; }
    public string INTERGRATEDSEC { get; set; }
}