using System;
using System.Net.Mail;

namespace KeeAnonAddy
{
    internal class AnonAddyEntry
    {
        public AnonAddyEntry(Guid? id, string address)
        {
            Id = id;
            MailAddress = new MailAddress(address);
        }

        public Guid? Id { get; }

        public MailAddress MailAddress { get; }
    }
}