using System;
using S22.Imap;
using System.Net.Mail;
using System.IO;

namespace сSharpProject
{
    class MailGetAttachments
    {
        public static void Download()
        {
            using (ImapClient Client = new ImapClient("10.10.0.50", 143, "declaration@eltransplus.ru", "JnKvbEgs"))
            {
                uint[] uids = Client.Search(SearchCondition.SentSince(new DateTime(2020, 3, 20)));
                MailMessage[] messages = Client.GetMessages(uids);
                
                foreach (MailMessage msg in messages)
                {
                    Console.WriteLine(msg.Subject);
                    foreach (Attachment attachment in msg.Attachments)
                    {
                        byte[] allBytes = new byte[attachment.ContentStream.Length];
                        int bytesRead = attachment.ContentStream.Read(allBytes, 0, (int)attachment.ContentStream.Length);
                        string destinationFile = @"C:\Users\andreydruzhinin\Desktop\" + attachment.Name;
                        BinaryWriter writer = new BinaryWriter(new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
                        writer.Write(allBytes);
                        writer.Close();
                    }
                }
            }
        } 
    }
}


