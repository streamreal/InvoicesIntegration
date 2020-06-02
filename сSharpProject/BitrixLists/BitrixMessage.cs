#region Help:  Introduction to the script task
/* The Script Task allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services control flow. 
 * 
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script task. */
#endregion


#region Namespaces
using System.Runtime.Serialization;
#endregion

namespace BitrixLists
{
    [DataContract]
    public class BitrixMessage
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string NAME { get; set; }
        [DataMember]
        public string DATE_CREATE { get; set; }
        [DataMember] 
        public string DETAIL_TEXT { get; set; }
        [DataMember]
        public string CREATED_USER_NAME { get; set; }
    }
}