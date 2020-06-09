#region Help:  Introduction to the script task
/* The Script Task allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services control flow. 
 * 
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script task. */
#endregion


#region Namespaces
using System.Data.Linq.Mapping;
#endregion


namespace BitrixLists
{
    [Table(Name = "aberration.dbo.list_330_new")]
    public class messages
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int id { get; set; }
        [Column]
        public int element_id { get; set; }
        [Column]
        public string date_create { get; set; }
        [Column]
        public string delivery_id { get; set; }
        [Column]
        public string message { get; set; }      
        [Column]
        public string message_url { get; set; }       
        [Column]
        public string author { get; set; }
        [Column]
        public string task_url { get; set; }
    }    
}