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
    public class RootObject
    {
        [DataMember]
        public BitrixMessage[] result { get; set; }
        [DataMember]
        public string next { get; set; }

    }
}